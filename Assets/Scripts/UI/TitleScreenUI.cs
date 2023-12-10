using System;
using System.Collections;
using Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
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
        [SerializeField] 
        private AudioSource sfxSource;

        private AsyncOperation _sceneLoadOperation;
        private bool _registerPresses = true;
        private IDisposable _eventListener;

        void Start()
        {
            _sceneLoadOperation = SceneManager.LoadSceneAsync("MainMenu");
            _sceneLoadOperation.allowSceneActivation = false;
            
            border.DOFillAmount(1.0f, introAnimationDuration)
                  .OnComplete(announcerSource.Play);
        }
        
        void OnEnable()
        {
            _eventListener = InputSystem.onAnyButtonPress.Call(_ =>
            {
                if (!_registerPresses) return;
                
                _registerPresses = false;
                StartCoroutine(PlayEffectAndTransition());
            });
        }

        void OnDisable()
        {
            _eventListener.Dispose();
        }

        private IEnumerator PlayEffectAndTransition()
        {
            sfxSource.Play();
            yield return new WaitForSeconds(sfxSource.clip.length);
            LoadSceneAsync(_sceneLoadOperation);
        }
    }
}
