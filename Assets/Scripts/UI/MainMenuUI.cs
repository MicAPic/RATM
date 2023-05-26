using System;
using System.Collections;
using Dan.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : UI
    {
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
        
        private static readonly int Active = Animator.StringToHash("Active");

        private void Awake()
        {
            // allow raycasts through transparent parts of the disc
            disc.GetComponent<Image>().alphaHitTestMinimumThreshold = 1.0f;
        }

        void Start()
        {
            CheckConnection();
        }

        // Update is called once per frame
        void Update()
        {
            var mouseVerticalPos = Input.GetAxis("Mouse Y");
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

        public void ToggleRadialGroup(Animator controller)
        {
            controller.SetBool(Active, !controller.GetBool(Active));
        }
    }
}
