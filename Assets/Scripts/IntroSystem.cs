using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroSystem : MonoBehaviour
{
    [SerializeField] 
    private List<AudioClip> announcerClips;
    private PlayableDirector _director;
    [SerializeField] 
    private CarController _player;
    [SerializeField]
    private AudioSource _audioSource;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _director.Play();
        StartCoroutine(Countdown((float) _director.duration));
    }

    private IEnumerator Countdown(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var clip in announcerClips)
        {
            yield return new WaitForSeconds(1.0f);
            _audioSource.PlayOneShot(clip);
        }
        _player.canProcessInput = true;
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
