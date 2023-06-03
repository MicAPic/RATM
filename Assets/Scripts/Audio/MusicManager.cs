using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;
        
        public AudioMixer audioMixer;

        [Header("Snapshots")] 
        public AudioMixerSnapshot normalSnapshot;
        public AudioMixerSnapshot muffledSnapshot;

        [Header("Audio Players")] 
        public AudioSource announcerSource;
        public AudioSource sfxSource;
        public AudioSource musicSource;

        void Awake()
        {
            if (Instance != null)
            {
                if (SceneManager.GetActiveScene().name != "MainMenu")
                {
                    Destroy(Instance.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    
        // Start is called before the first frame update
        void Start()
        {
            announcerSource = transform.GetChild(0).GetComponent<AudioSource>();
            sfxSource = transform.GetChild(1).GetComponent<AudioSource>();
            musicSource = transform.GetChild(2).GetComponent<AudioSource>();
        }

        public void Clear()
        {
            Instance = null;
            Destroy(gameObject);
        }
        
    }
}
