using System.Collections;
using DG.Tweening;
using UnityEngine;
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

        void Start()
        {
            _sceneLoadOperation = SceneManager.LoadSceneAsync("MainMenu");
            _sceneLoadOperation.allowSceneActivation = false;
            
            border.DOFillAmount(1.0f, introAnimationDuration)
                  .OnComplete(announcerSource.Play);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.anyKey)
            {
                StartCoroutine(PlayEffectAndTransition());
            }
        }

        private IEnumerator PlayEffectAndTransition()
        {
            sfxSource.Play();
            yield return new WaitForSeconds(sfxSource.clip.length);
            LoadSceneAsync(_sceneLoadOperation);
        }
    }
}
