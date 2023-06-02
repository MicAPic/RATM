using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;
        
        public AudioMixer audioMixer;

        [Header("Audio Players")] 
        public AudioSource announcerSource;
        public AudioSource sfxSource;
        public AudioSource musicSource;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
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

        // Update is called once per frame
        // void Update()
        // {
        //     
        // }
    }
}
