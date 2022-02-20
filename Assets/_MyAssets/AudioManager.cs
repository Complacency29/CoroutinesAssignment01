using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField, Range(1, 15)] private int _maxSfxTracks = 5;
    [SerializeField] private GameObject _audioObject;

    private AudioSource[] _sfxTracks;
    private int _curSfxIndex = 0;

    private AudioSource _bgm;

    private void Awake()
    {
        if (AudioManager.instance == null)
        {
            Debug.Log("audio manager instance is null, we'll set this one to be it.");
            AudioManager.instance = this;
            Debug.Log("Audio manager instance: " + gameObject.name);
        }
        else if (AudioManager.instance != this)
        {
            Debug.Log("there already is an audio manager, we'll delete this one.");
            Destroy(this);
        }
        InitAudioSources();
    }

    private void Start()
    {
        //InitAudioSources();
    }

    private void InitAudioSources()
    {
        _sfxTracks = new AudioSource[_maxSfxTracks];

        for (int i = 0; i < _maxSfxTracks; i++)
        {
            _sfxTracks[i] = gameObject.AddComponent<AudioSource>();
        }

        _bgm = gameObject.AddComponent<AudioSource>();
    }

    public void PlayBGM(AudioClip musicToPlay, float fadeDuration, bool isLooping = true, float volume = 1)
    {
        //start coroutine
        StartCoroutine(PlayBGMCo(musicToPlay, fadeDuration, isLooping));
    }

    private IEnumerator PlayBGMCo(AudioClip musicToPlay, float fadeDuration, bool isLooping = true, float volume = 1)
    {
        AudioSource newBGM = gameObject.AddComponent<AudioSource>();
        newBGM.clip = musicToPlay;
        newBGM.loop = isLooping;
        newBGM.volume = 0;
        newBGM.Play();

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float percent = t / fadeDuration;
            _bgm.volume = Mathf.Lerp(1, 0, percent);
            newBGM.volume = Mathf.Lerp(0, 1, percent);

            yield return null;
        }

        Destroy(_bgm);
        _bgm = newBGM;

    }

    /// <summary>
    /// Set the SFX index clip to the desired clip
    /// Plays that clip
    /// Increases index by 1
    /// Resets the index when it is over the maximum
    /// </summary>
    /// <param name="clipToPlay">The clip you want to play</param>
    public void PlaySFX(AudioClip clipToPlay)
    {
        AudioSource _source = _sfxTracks[_curSfxIndex];
        _source.clip = clipToPlay;
        _source.Play();

        _curSfxIndex++;
        if (_curSfxIndex > _sfxTracks.Length - 1)
            _curSfxIndex = 0;
    }

    /// <summary>
    ///Make an audio object at the desired position
    /// Check if that object has an audio source
    /// if not, add one
    /// set the clip and the spatial blend
    /// </summary>
    /// <param name="clipToPlay">Desired clip</param>
    /// <param name="position">Desired location</param>
    /// <param name="volume">Volume, defaults to 1</param>
    /// <param name="spatialBlend">Spatial blend, defaults to 1 (3D sound)</param>
    public void PlaySFX(AudioClip clipToPlay, Vector3 position, float volume = 1, float spatialBlend = 1)
    {
        GameObject go = GameObject.Instantiate(_audioObject, position, Quaternion.identity);
        if(go.GetComponent<AudioSource>() == null)
        {
            go.AddComponent<AudioSource>();
        }

        AudioSource temp = go.GetComponent<AudioSource>();
        temp.clip = clipToPlay;
        temp.spatialBlend = spatialBlend;
        temp.Play();

        StartCoroutine(CleanUp(go, clipToPlay.length));
    }

    private IEnumerator CleanUp(GameObject go, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(go);
    }
}
