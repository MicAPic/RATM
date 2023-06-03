using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;

public class RecordSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] 
    private GameObject markerPrefab;
    [SerializeField] 
    private int maxRecordingFrames = 9 * 60;
    [SerializeField] 
    private int replayFrameLength = 2;
    private int _currentFrame;
    
    [Header("Animation")]
    [SerializeField] 
    private float fadeTime = 2.0f;
    private Material _markerMaterial;
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");

    private List<GameObject> _instantiatedPrefabs;
    
    private Transform _playerTransform;
    
    private MemoryStream _memoryStream;
    private BinaryWriter _binaryWriter;
    private BinaryReader _binaryReader;

    private bool _recordingInitialized;
    private bool _recording;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameManager.Instance.player.transform;
        _markerMaterial = markerPrefab.GetComponent<Renderer>().sharedMaterial;
    }
    
    void FixedUpdate()
    {
        if (!_recording) return;
        
        if (_currentFrame > maxRecordingFrames)
        {
            StopRecording();
            _currentFrame = 0;
            return;
        }
        if (_currentFrame % replayFrameLength == 0)
        {
            SavePosition();
        }
        _currentFrame++;
    }
    
    public void StartRecording()
    {
        if (!_recordingInitialized)
        {
            InitializeRecording();
        }
        else
        {
            _memoryStream.SetLength(0);
        }
        
        ResetReplayFrame();
        _recording = true;
    }

    public void StopRecording()
    {
        _recording = false;
    }

    public void Replay()
    {
        ResetReplayFrame();
        
        while (_memoryStream.Position < _memoryStream.Length)
        {
            LoadPosition();
        }

        var normalizedPosition = (int)_memoryStream.Position / 12; // see LoadPosition()
        if (normalizedPosition >= _instantiatedPrefabs.Count) return; 
        for (var i = normalizedPosition; i < _instantiatedPrefabs.Count; i++)
        {
            _instantiatedPrefabs[i].SetActive(false);
        }
        
        _markerMaterial.SetFloat(Opacity, 0.0f);
        _markerMaterial.DOFloat(0.67f, Opacity, fadeTime);
    }

    private void InitializeRecording()
    {
        _memoryStream = new MemoryStream();
        _binaryWriter = new BinaryWriter(_memoryStream);
        _binaryReader = new BinaryReader(_memoryStream);

        _instantiatedPrefabs = new List<GameObject>();

        _recordingInitialized = true;
    }

    private void ResetReplayFrame()
    {
        _memoryStream.Seek(0, SeekOrigin.Begin);
        _binaryWriter.Seek(0, SeekOrigin.Begin);
    }

    private void SavePosition()
    {
        var currentPosition = _playerTransform.position;
        _binaryWriter.Write(currentPosition.x);
        _binaryWriter.Write(currentPosition.y);
        _binaryWriter.Write(currentPosition.z);
    }

    private void LoadPosition()
    {
        // this shifts the position by 3 * 4 bytes, this is why we work in increments of 12
        var markerPos = new Vector3(_binaryReader.ReadSingle(), 
                                    _binaryReader.ReadSingle(), 
                                    _binaryReader.ReadSingle());
        var normalizedPosition = (int)_memoryStream.Position / 12 - 1;
        
        if (normalizedPosition < _instantiatedPrefabs.Count)
        {
            _instantiatedPrefabs[normalizedPosition].transform.position = markerPos;
            _instantiatedPrefabs[normalizedPosition].SetActive(false);
            _instantiatedPrefabs[normalizedPosition].SetActive(true);
        }
        else
        {
            var newMarker = Instantiate(markerPrefab, markerPos, Quaternion.identity);
            newMarker.transform.parent = gameObject.transform;
            _instantiatedPrefabs.Add(newMarker);
        }
    }
}
