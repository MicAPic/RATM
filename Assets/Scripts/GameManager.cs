using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPaused = true;

    [Header("Race Logic")]
    public int totalLaps = 3;
    public int currentLap = 1;
    [SerializeField] 
    private List<Checkpoint> checkpoints;

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
    }
    
    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    public void StartRace()
    {
        player.canProcessInput = true;
        _raceStartTime = Time.time;
        lapStartTime = _raceStartTime;
        isPaused = false;
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
            isPaused = true;
            
            player.canProcessInput = false;
            player.currentSpeed = 0;
            player.currentYRotate = 0;
            player.currentZRotate = 0;
            topCamera.SetActive(true);

            var totalTime = Time.time - _raceStartTime;
            if (totalTime < totalBestTime)
            {
                PlayerPrefs.SetFloat($"{SceneManager.GetActiveScene().name}_bestTime", totalTime);
                ui.UpdateTotalTime(totalTime, true);
            }
            else
            {
                ui.UpdateTotalTime(totalTime, false);
            }
            ui.ShowEndScreen();
            return;
        }
        ui.UpdateLapText(currentLap, totalLaps);
    }
}
