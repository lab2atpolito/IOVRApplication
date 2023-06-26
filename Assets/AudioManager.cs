using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _backgroundSound;
    private AudioSource[] _sounds;

    private bool _isBackgroundPlaying = true;
    private bool _isSoundActive = true;
    private float _currentVolume = 1f;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void TurnOffBackground()
    {
        _backgroundSound.volume = 0f;
    }

    private void TurnOnBackground()
    {
        _backgroundSound.volume = 0.2f;
    }

    public void ToggleBackgroundSound()
    {
        if (_isBackgroundPlaying)
        {
            _isBackgroundPlaying = false;
            TurnOffBackground();
        }
        else
        {
            _isBackgroundPlaying = true;
            TurnOnBackground();
        }
    }


    private void TurnOffSounds()
    {
        foreach (var audioSource in _sounds)
        {
            audioSource.volume = 0f;
        }
    }

    private void TurnOnSounds()
    {
        foreach (var audioSource in _sounds)
        {
            audioSource.volume = 1f;
        }
    }

    public void ToggleSounds()
    {
        if (_isSoundActive)
        {
            _isSoundActive = false;
            TurnOffSounds();
        }
        else
        {
            _isSoundActive = true;
            TurnOnSounds();
        }
    }

    public void UpdateSoundsVolume(float value)
    {
        _currentVolume = value; 
        foreach (var audioSource in _sounds)
        {
            audioSource.volume = _currentVolume;
        }
    }

    public void UpdateAudioManager()
    {
        var sounds = GameObject.FindGameObjectsWithTag("Sound");
        if (sounds.Length == 0)
        {
            Debug.Log("Nessun suono trovato!");
        }
        var background = GameObject.FindGameObjectWithTag("BackgroundSound");
        if (background != null)
        {
            Debug.Log("Background Music trovato!");
        }
        _sounds = new AudioSource[sounds.Length];
        for (int i = 0; i < _sounds.Length; i++)
        {
            _sounds[i] = sounds[i].GetComponent<AudioSource>();
        }
        _backgroundSound = background.GetComponent<AudioSource>();

        if (!_isSoundActive)
        {
            TurnOffSounds();
        }
        if (!_isBackgroundPlaying)
        {
            TurnOffBackground();
        }

        UpdateSoundsVolume(_currentVolume);
    }
}
