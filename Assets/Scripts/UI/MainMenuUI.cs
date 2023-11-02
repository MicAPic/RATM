using Dan.Main;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : UI
    {
        [Header("Animation")]
        [SerializeField] 
        private RectTransform disc;
        [Header("Online-related")]
        [SerializeField] 
        private GameObject offlineIcon;
        [SerializeField] 
        private GameObject onlineIcon;
        [SerializeField] 
        private GameObject ranksButton;
        [SerializeField] 
        private GameObject nameEntryPopup;
        [SerializeField] 
        private TMP_InputField nameInputField;

        private void Awake()
        {
            // allow raycasts through transparent parts of the disc
            disc.GetComponent<Image>().alphaHitTestMinimumThreshold = 1.0f;
            Cursor.visible = true;
        }

        void Start()
        {
            CheckConnection();
        }

        // Update is called once per frame
        void Update()
        {
            //TODO: add controller support
            // var mouseVerticalPos = Input.GetAxis("Mouse Y");
            var mouseVerticalPos = Mouse.current.delta.ReadValue().y;
            mouseVerticalPos = Mathf.Clamp(mouseVerticalPos, -1.0f, 1.0f);
            disc.Rotate(0, 0, -mouseVerticalPos);
        }

        private void CheckConnection()
        {
            LeaderboardCreator.Ping(isOnline =>
            {
                if (isOnline)
                {
                    onlineIcon.SetActive(true);
                    if (PlayerPrefs.GetString("nickname", string.Empty) == string.Empty)
                    {
                        nameEntryPopup.SetActive(true);
                    }
                }
                else
                {
                    offlineIcon.SetActive(true);
                    Destroy(ranksButton);
                }
            });
        }

        public void SetNickname()
        {
            if (nameInputField.text == string.Empty) return;
            PlayerPrefs.SetString("nickname", nameInputField.text);
            nameEntryPopup.GetComponent<Animator>().SetTrigger("NameIsSet");
        }

        public void ToggleRadialGroup(RadialLayout radialLayout)
        {
            float endValue;
            endValue = Mathf.Abs(radialLayout.fDistance - 910.0f) < Mathf.Abs(radialLayout.fDistance - 450.0f) ? 
                450.0f : 910.0f;
            
            DOTween.To(
                () => radialLayout.fDistance,
                x =>
                {
                    radialLayout.fDistance = x;
                    radialLayout.CalculateLayoutInputHorizontal();
                }, 
                endValue, 
                0.416f);
        }

        public void SetActiveButton(Button button)
        {
            button.Select();
        }
    }
}
