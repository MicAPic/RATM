using System;
using System.Collections;
using Audio;
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
        [SerializeField]
        private AudioClip finishClip;
        [SerializeField]
        private AudioClip recordClip;

        [Header("UI Elements")]
        [SerializeField]
        private CanvasGroup countdown;
        [SerializeField]
        private Image countdownCircle;
        [SerializeField]
        private TMP_Text[] countdownText;
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

        private readonly Color _deepSaffron = new(1.0f, 0.6196079f, 0.2392157f);
        
        [Header("Outro / Pause")]
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
        public GameObject onlineIcons;
        [SerializeField] 
        private Sprite offlineIcon;
        [SerializeField] 
        private Button[] buttons;

        public bool canPause;
        public bool isPaused = true;

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

            if (!Input.GetKeyDown(KeyCode.Escape) || !canPause) return;

            if (isPaused)
            {
                // Unpause
                Time.timeScale = 1.0f;
                MusicManager.Instance.normalSnapshot.TransitionTo(0.5f);
                strikeLine.DOFillAmount(0.0f, lineAnimationDuration);
                endScreen.SetActive(false);

                MusicManager.Instance.sfxSource.GetComponent<AudioPlayer>().FadeIn(0.001f);
                EnableButtons(false);
            }
            else
            {
                // Pause
                Time.timeScale = 0.0f;
                MusicManager.Instance.muffledSnapshot.TransitionTo(0.5f);
                strikeLine.DOFillAmount(1.0f, lineAnimationDuration).SetUpdate(true);
                endScreen.SetActive(true);
                
                MusicManager.Instance.sfxSource.GetComponent<AudioPlayer>().FadeOut(0.001f);
                EnableButtons();
            }

            isPaused = !isPaused;
        }

        void FixedUpdate()
        {
            speedometer.fillAmount = _currentSpeedometerFill;
            if (!isPaused)
            {
                lapTime.text = "<size=50%><font=\"Audiowide SDF\">C. </font></size>" +
                               FormatTime(Time.time - GameManager.Instance.lapStartTime);
            }
        }

        public IEnumerator VisualizeCountdown()
        {
            countdown.DOFade(1.0f, 1.0f);
            for (var i = 3; i >= 0; i--)
            {
                yield return new WaitForSeconds(1.0f);
                foreach (var tmpText in countdownText)
                {
                    tmpText.text = i.ToString();
                }
                countdownCircle.fillAmount = 1.0f;
                countdownCircle.DOFillAmount(0.0f, 1.0f);
            }
            
            countdownCircle.fillAmount = 0.0f;
            countdownText[0].color = _deepSaffron;
            
            foreach (var tmpText in countdownText)
            {
                tmpText.text = "GO";
                tmpText.DOColor(Color.white, 1.0f);
            }
            yield return new WaitForSeconds(1.0f);
            foreach (var tmpText in countdownText)
            {
                tmpText.DOColor(Color.clear, 1.0f);
            }
        }

        public void ShowEndScreen(bool newRecord)
        {
            MusicManager.Instance.announcerSource.PlayOneShot(finishClip);
            if (newRecord)
            {
                MusicManager.Instance.announcerSource.clip = recordClip;
                MusicManager.Instance.announcerSource.PlayDelayed(finishClip.length + 1.0f);
            }
            
            strikeLine.DOFillAmount(1.0f, lineAnimationDuration)
                      .OnComplete(() => endScreen.SetActive(true));
            MusicManager.Instance.sfxSource.GetComponent<AudioPlayer>().FadeOut(0.001f);
            MusicManager.Instance.muffledSnapshot.TransitionTo(1.0f);
        }
        
        public void UpdateLapTime(float newBest)
        {
            lapBestTime.text = $"<size=50%><font=\"Audiowide SDF\">H. </font></size>{FormatTime(newBest)}";
            lapBestTime.GetComponent<Animator>().SetTrigger("NewLap");
        }
        
        public void UpdateTotalTime(float time, bool newRecord)
        {
            totalTime.gameObject.SetActive(true);
            totalTime.text = FormatTime(time);
            if (newRecord)
            {
                title.text = "new record";
                title.color = _deepSaffron;
            }
            else
            {
                title.text = "total time";
            }
        }

        public void UpdateLapText(int current, int total)
        {
            if (current == total)
            {
                lapText.color = _deepSaffron;
            }
            lapText.text = current.ToString();
            lapText.GetComponent<Animator>().SetTrigger("NewLap");
        }

        public void EnableButtons(bool value=true)
        {
            foreach (var button in buttons)
            {
                button.interactable = value;
            }
        }
        
        public IEnumerator AnimateScoreUpload()
        {
            onlineIcons.SetActive(true);
            var arrows = onlineIcons.transform.GetChild(0).GetComponent<TMP_Text>();
            arrows.maxVisibleCharacters = 0;
            yield return new WaitForSeconds(1.0f);
            while (arrows.maxVisibleCharacters < 3)
            {
                arrows.maxVisibleCharacters += 1;
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void ShowErrorIcon()
        {
            var globe = onlineIcons.transform.GetChild(1).GetComponent<Image>();
            globe.sprite = offlineIcon;
        }

        private Vector2 PlayerPos2MinimapPos(Vector3 playerPos)
        {
            var x = minimapPosConstraints[0].x + (playerPos.x - worldPosConstraints[0].x) * _minimapPosRange.x /
                        (worldPosConstraints[1].x - worldPosConstraints[0].x);
            var y = minimapPosConstraints[0].y + (playerPos.z - worldPosConstraints[0].y) * _minimapPosRange.y /
                (worldPosConstraints[1].y - worldPosConstraints[0].y);

            return new Vector2(x, y);
        }

        public static string FormatTime(float time)
        {
            var minutes = (int)(time / 60);
            var seconds = Math.Round(time % 60, 2);
            var milliseconds = (int)(seconds % 1 * 100);

            return $"{minutes}:{(int)seconds:00}<size=50%>{milliseconds:00}";
        }
    }
}
