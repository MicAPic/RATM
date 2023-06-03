using System.Collections.Generic;
using System.Linq;
using Audio;
using Dan.Main;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Audio")] 
    public AudioClip mainTrack;
    
    [Header("Online")] 
    [SerializeField] 
    private string publicKey;

    [Header("Race Logic")]
    public int totalLaps = 3;
    public int currentLap = 1;
    [SerializeField] 
    private List<Checkpoint> checkpoints;
    private RecordSystem _recordSystem;
    
    [Space]
    
    public float totalBestTime;
    public float lapBestTime;
    public float lapStartTime;
    private float _raceStartTime;
    
    [Header("Scene Elements")] 
    [SerializeField]
    private InGameUI ui;
    public CarController player;
    [SerializeField]
    private GameObject topCamera;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
        // PlayerPrefs.DeleteKey($"{SceneManager.GetActiveScene().name}_bestTime");
    }

    // Start is called before the first frame update
    void Start()
    {
        lapBestTime = PlayerPrefs.GetFloat($"{SceneManager.GetActiveScene().name}_bestLapTime", float.MaxValue);
        totalBestTime = PlayerPrefs.GetFloat($"{SceneManager.GetActiveScene().name}_bestTime", float.MaxValue);
        if (lapBestTime < float.MaxValue)
        {
            ui.UpdateLapTime(lapBestTime);
        }
        ui.UpdateLapText(currentLap, totalLaps);

        _recordSystem = GetComponent<RecordSystem>();
    }
    
    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    public void StartRace()
    {
        // not all track are ready, so:
        if (mainTrack != null)
        {
            MusicManager.Instance.musicSource.clip = mainTrack;
            MusicManager.Instance.musicSource.GetComponent<AudioPlayer>().FadeIn(1.0f);
            MusicManager.Instance.musicSource.Play();
        }
        
        player.canProcessInput = true;
        _raceStartTime = Time.time;
        lapStartTime = _raceStartTime;
        ui.canPause = true;
        ui.isPaused = false;
        
        _recordSystem.StartRecording();
    }

    public void CheckForNewLap()
    {
        if (checkpoints.Any(checkpoint => !checkpoint.passed)) return;

        foreach (var checkpoint in checkpoints)
        {
            checkpoint.passed = false;
        }

        currentLap++;
        
        var lapTime = Time.time - lapStartTime;
        if (lapTime < lapBestTime)
        {
            lapBestTime = lapTime;
            ui.UpdateLapTime(lapTime);
            PlayerPrefs.SetFloat($"{SceneManager.GetActiveScene().name}_bestLapTime", lapTime);
        }
        lapStartTime = Time.time;
        
        if (currentLap > totalLaps)
        {
            ui.canPause = false;
            ui.isPaused = true;
            
            player.canProcessInput = false;
            player.currentSpeed = 0;
            player.currentYRotate = 0;
            player.currentZRotate = 0;
            topCamera.SetActive(true);

            var totalTime = Time.time - _raceStartTime;
            if (totalTime < totalBestTime)
            {
                totalBestTime = totalTime;
                
                PlayerPrefs.SetFloat($"{SceneManager.GetActiveScene().name}_bestTime", totalBestTime);
                ui.UpdateTotalTime(totalBestTime, true);
                ui.ShowEndScreen(true);
                UploadNewScore();
            }
            else
            {
                ui.UpdateTotalTime(totalTime, false);
                ui.ShowEndScreen(false);
                ui.EnableButtons();
            }
            return;
        }

        ui.UpdateLapText(currentLap, totalLaps);
        
        // Recording
        _recordSystem.StopRecording();
        _recordSystem.Replay();
        if (currentLap < totalLaps)
        {
            _recordSystem.StartRecording();
        }
    }
    
    private void UploadNewScore()
    {
        var nickname = PlayerPrefs.GetString("nickname", string.Empty);
        var score = (int)totalBestTime * 100;
        var boardEntry = $"{InGameUI.FormatTime(totalBestTime)}/{InGameUI.FormatTime(lapBestTime)}";
        
        LeaderboardCreator.Ping(isOnline => 
            { 
                if (isOnline && nickname != string.Empty) 
                {
                    ui.onlineIcons.SetActive(true);
                    LeaderboardCreator.UploadNewEntry(publicKey, nickname, score, boardEntry,
                        _ =>
                        {
                            StartCoroutine(ui.AnimateScoreUpload());
                            ui.EnableButtons();
                        },
                        error =>
                        {
                            if (error == null) return;
                            ui.EnableButtons();

                            ui.ShowErrorIcon();
                            Debug.Log(error);
                        });
                }
                else
                {
                    ui.ShowErrorIcon();
                    ui.EnableButtons();
                }
            });
    }
}
