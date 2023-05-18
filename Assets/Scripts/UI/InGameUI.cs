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
        
        [Header("UI Elements")]
        [SerializeField]
        private Image speedometer;
        [SerializeField]
        private TMP_Text lapText;
        
        [Header("Outro")]
        [SerializeField]
        private Image strikeLine;
        [SerializeField]
        private float lineAnimationDuration;
        [SerializeField]
        private GameObject endScreen;

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
            speedometer.fillAmount = Mathf.Abs(Mathf.Round(player.currentSpeed) / topSpeed);
            markerTransform.anchoredPosition = PlayerPos2MinimapPos(player.transform.position);
        }

        public void ShowEndScreen()
        {
            strikeLine.DOFillAmount(1.0f, lineAnimationDuration)
                      .OnComplete(() => endScreen.SetActive(true));
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
    }
}
