using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] 
        private string prefsVolumeName;

        void Awake()
        {
            GetComponent<Slider>().value = PlayerPrefs.GetFloat(prefsVolumeName, 0.994f);
        }

        public void SetVolume(float volume)
        {
            PlayerPrefs.SetFloat(prefsVolumeName, volume);
            MusicManager.Instance.audioMixer.SetFloat(prefsVolumeName, Mathf.Log10(volume) * 20);
        }
    }
}
