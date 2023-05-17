using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Race Logic")]
    public int totalLaps = 3;
    public int currentLap = 1;
    [SerializeField] 
    private List<Checkpoint> checkpoints;
    
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
        ui.UpdateLapText(currentLap, totalLaps);
    }
    
    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    public void CheckForNewLap()
    {
        if (checkpoints.Any(checkpoint => !checkpoint.passed)) return;

        foreach (var checkpoint in checkpoints)
        {
            checkpoint.passed = false;
        }

        currentLap++;
        if (currentLap > totalLaps)
        {
            player.canProcessInput = false;
            player.currentSpeed = 0;
            player.currentYRotate = 0;
            player.currentZRotate = 0;
            topCamera.SetActive(true);
            return;
        }
        ui.UpdateLapText(currentLap, totalLaps);
    }
}
