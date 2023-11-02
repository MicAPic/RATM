using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class IntroSystem : MonoBehaviour
{
    [SerializeField] 
    private List<AudioClip> announcerClips;
    [SerializeField]
    private GameObject inGameUI;
    private PlayableDirector _director;

    private bool _canSkip = true;
    private IDisposable _eventListener;
    private Coroutine _delayCoroutine;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _director.Play();
        _delayCoroutine = StartCoroutine(DelayCountdown((float) _director.duration));
    }
    
    void OnEnable()
    {
        _eventListener = InputSystem.onAnyButtonPress.Call(_ =>
        {
            if (!_canSkip) return;
            
            _canSkip = false;
            _director.time = _director.duration;
            MusicManager.Instance.musicSource.GetComponent<AudioPlayer>().FadeOut(0.01f);
            StopCoroutine(_delayCoroutine);
            StartCoroutine(Countdown());
        });
    }

    private void OnDisable()
    {
        _eventListener.Dispose();
    }

    private IEnumerator DelayCountdown(float delay)
    {
        yield return new WaitForSeconds(delay - 3.0f);
        MusicManager.Instance.musicSource.GetComponent<AudioPlayer>().FadeOut(3.0f);
        
        yield return new WaitForSeconds(3.0f);
        _canSkip = false;
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        inGameUI.SetActive(true);
        var ui = inGameUI.GetComponentInParent<InGameUI>();
        StartCoroutine(ui.VisualizeCountdown());
        foreach (var clip in announcerClips)
        {
            yield return new WaitForSeconds(1.0f);
            MusicManager.Instance.announcerSource.PlayOneShot(clip);
        }
        
        GameManager.Instance.StartRace();
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
