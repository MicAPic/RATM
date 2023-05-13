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
        // TODO: move the material reference to a GameManager
        [SerializeField] 
        private Material _transitionMaterial;

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
                StartCoroutine(PrepareTransition());
            }
        }

        private IEnumerator PrepareTransition()
        {
            yield return new WaitForEndOfFrame();
            
            var screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            var screenRegion = new Rect(0, 0, Screen.width, Screen.height);
            screenTexture.ReadPixels(screenRegion, 0, 0, false);
            screenTexture.Apply(); // render the texture on GPU
            _transitionMaterial.SetTexture("_FadeTex", screenTexture);
                
            LoadScene("MainMenu");
        }
    }
}
