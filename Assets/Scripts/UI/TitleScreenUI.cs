using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TitleScreenUI : UI
    {
        [SerializeField] 
        private float introAnimationDuration;
        [SerializeField] 
        private Image border;
        [SerializeField] 
        private AudioSource announcerSource;

        void Start()
        {
            border.DOFillAmount(1.0f, introAnimationDuration)
                  .OnComplete(announcerSource.Play);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.anyKey)
            {
                LoadScene("MainMenu");
            }
        }
    }
}
