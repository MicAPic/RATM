using System;
using UnityEngine;

namespace Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] 
        private float fadeDuration;
        [SerializeField] 
        private string exposedVolumeName;
        [SerializeField] 
        private string prefsVolumeName;

        private float _maxVolume;

        void Awake()
        {
            // TODO: Cross-fade, perhaps?
            _maxVolume = PlayerPrefs.GetFloat(prefsVolumeName, 0.994f);
        }

        // Start is called before the first frame update
        void Start()
        {
            MusicManager.Instance.audioMixer.SetFloat(exposedVolumeName, Mathf.Log10(0.0001f) * 20);
            FadeIn(fadeDuration);
        }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }

        public void FadeOut(float duration)
        {
            StartCoroutine(FadeMixerGroup.StartFade(
                MusicManager.Instance.audioMixer, 
                exposedVolumeName, 
                duration, 
                0.0001f
            ));
        }

        public void FadeIn(float duration)
        {
            StartCoroutine(FadeMixerGroup.StartFade(
                MusicManager.Instance.audioMixer, 
                exposedVolumeName, 
                duration, 
                _maxVolume
            ));
        }
    }
}
