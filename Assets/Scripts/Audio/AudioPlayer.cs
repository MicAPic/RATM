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

        void Awake()
        {
            // TODO: Cross-fade, perhaps?
        }

        // Start is called before the first frame update
        void Start()
        {
            MusicManager.Instance.audioMixer.SetFloat(exposedVolumeName, Mathf.Log10(0.0001f) * 20); 
            
            var maxVolume = PlayerPrefs.GetFloat(prefsVolumeName, 0.994f);
            StartCoroutine(FadeMixerGroup.StartFade(
                MusicManager.Instance.audioMixer, 
                exposedVolumeName, 
                fadeDuration, 
                maxVolume
                ));
        }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }
    }
}
