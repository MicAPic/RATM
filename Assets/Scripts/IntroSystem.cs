using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroSystem : MonoBehaviour
{
    [SerializeField] 
    private List<AudioClip> announcerClips;
    [SerializeField]
    private GameObject inGameUI;
    private PlayableDirector _director;
    [SerializeField] 
    private CarController player;
    [SerializeField]
    private AudioSource audioSource;

    private bool _canSkip = true;
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

    void Update()
    {
        if (_canSkip && Input.anyKey)
        {
            _canSkip = false;
            _director.time = _director.duration;
            StopCoroutine(_delayCoroutine);
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator DelayCountdown(float delay)
    {
        yield return new WaitForSeconds(delay);
        _canSkip = false;
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        inGameUI.SetActive(true);
        foreach (var clip in announcerClips)
        {
            yield return new WaitForSeconds(1.0f);
            audioSource.PlayOneShot(clip);
        }
        player.canProcessInput = true;
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}