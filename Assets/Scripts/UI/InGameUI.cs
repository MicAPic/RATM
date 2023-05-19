using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : UI
    {
        [SerializeField]
        private float topSpeed;  // as of now, has to be determined empirically 
        [SerializeField] 
        private CarController player;

        [Header("Audio")] 
        public AudioSource announcerAudioSource;
        [SerializeField]
        private AudioClip finishClip;
        [SerializeField]
        private AudioClip recordClip;

        [Header("UI Elements")]
        [SerializeField]
        private TMP_Text lapText;
        [SerializeField]
        private TMP_Text lapTime;
        [SerializeField]
        private TMP_Text lapBestTime;
        [SerializeField]
        private Image speedometer;

        private float _speedometerFill;
        private float _currentSpeedometerFill;
        
        [Header("Outro")]
        [SerializeField]
        private Image strikeLine;
        [SerializeField]
        private float lineAnimationDuration;
        [SerializeField]
        private GameObject endScreen;
        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private TMP_Text totalTime;

        [Header("Minimap")]
        [SerializeField] 
        private Vector2[] worldPosConstraints;
        [SerializeField] 
        private Vector2[] minimapPosConstraints;
        [SerializeField] 
        private RectTransform markerTransform;

        private Vector2 _minimapPosRange;

        void Awake()
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            if (worldPosConstraints.Length != 2 && worldPosConstraints.Length != minimapPosConstraints.Length)
            {
                Debug.LogError("Minimap constraints are not set up correctly.");
                return;
            }

            _minimapPosRange = new Vector2(minimapPosConstraints[1].x - minimapPosConstraints[0].x,
                                           minimapPosConstraints[1].y - minimapPosConstraints[0].y);
        }

        // Update is called once per frame
        void Update()
        {
            _speedometerFill = Mathf.Abs(Mathf.Round(player.sphere.velocity.magnitude) / topSpeed);
            _currentSpeedometerFill = Mathf.SmoothStep(_currentSpeedometerFill, _speedometerFill,
                                                       Time.deltaTime * 12.0f);
            _speedometerFill = 0.0f;
            
            markerTransform.anchoredPosition = PlayerPos2MinimapPos(player.transform.position);
        }

        void FixedUpdate()
        {
            speedometer.fillAmount = _currentSpeedometerFill;
            if (!GameManager.Instance.isPaused)
            {
                lapTime.text = "<size=50%><font=\"Audiowide SDF\">C. </font></size>" +
                               FormatTime(Time.time - GameManager.Instance.lapStartTime);
            }
        }

        public void ShowEndScreen(bool newRecord)
        {
            announcerAudioSource.PlayOneShot(finishClip);
            if (newRecord)
            {
                announcerAudioSource.clip = recordClip;
                announcerAudioSource.PlayDelayed(finishClip.length + 1.0f);
            }
            
            strikeLine.DOFillAmount(1.0f, lineAnimationDuration)
                      .OnComplete(() => endScreen.SetActive(true));
        }
        
        public void UpdateLapTime(float newBest)
        {
            lapBestTime.text = $"<size=50%><font=\"Audiowide SDF\">H. </font></size>{FormatTime(newBest)}";
            lapBestTime.GetComponent<Animator>().SetTrigger("NewLap");
        }
        
        public void UpdateTotalTime(float time, bool newRecord)
        {
            totalTime.text = FormatTime(time);
            if (newRecord)
            {
                title.text = "new record";
                title.color = new Color(1.0f, 0.6196079f, 0.2392157f);
            }
        }

        public void UpdateLapText(int current, int total)
        {
            if (current == total)
            {
                lapText.color = new Color(1.0f, 0.6196079f, 0.2392157f);
            }
            lapText.text = current.ToString();
            lapText.GetComponent<Animator>().SetTrigger("NewLap");
        }

        private Vector2 PlayerPos2MinimapPos(Vector3 playerPos)
        {
            var x = minimapPosConstraints[0].x + (playerPos.x - worldPosConstraints[0].x) * _minimapPosRange.x /
                        (worldPosConstraints[1].x - worldPosConstraints[0].x);
            var y = minimapPosConstraints[0].y + (playerPos.z - worldPosConstraints[0].y) * _minimapPosRange.y /
                (worldPosConstraints[1].y - worldPosConstraints[0].y);

            return new Vector2(x, y);
        }

        private string FormatTime(float time)
        {
            var minutes = (int)(time / 60);
            var seconds = Math.Round(time % 60, 2);
            var milliseconds = (int)(seconds % 1 * 100);

            return $"{minutes}:{seconds:00}<size=50%>{milliseconds:00}";
        }
    }
}
