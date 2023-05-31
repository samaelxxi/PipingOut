using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PlaneMusic { BG, X, Y, Z}

public class GameOST : MonoBehaviour
{
    [SerializeField] AudioSource _xMusic;
    [SerializeField] AudioSource _yMusic;
    [SerializeField] AudioSource _zMusic;
    [SerializeField] AudioSource _bgMusic;


    [SerializeField] float _musicVolume;
    [SerializeField] float _fadeTime;
    public void StartLevelMusic()
    {
        _bgMusic.Play();
        _xMusic.Play();
        _yMusic.Play();
        _zMusic.Play();
        _bgMusic.volume = _musicVolume;
        _xMusic.volume = _musicVolume;
        _yMusic.volume = _musicVolume;
        _zMusic.volume = _musicVolume;
    }

    public void SetPlaneMusic(PlaneMusic type, float loudness = -1, float duration = -1)
    {
        if (loudness < 0)
            loudness = _musicVolume;
        if (duration < 0)
            duration = _fadeTime;
        switch (type)
        {
            case PlaneMusic.BG:
                _bgMusic.DOFade(loudness, duration);
                break;
            case PlaneMusic.X:
                _xMusic.DOFade(loudness, duration);
                break;
            case PlaneMusic.Y:
                _yMusic.DOFade(loudness, duration);
                break;
            case PlaneMusic.Z:
                _zMusic.DOFade(loudness*1.3f, duration);
                break;
        }
    }

    public void StopPlanesMusic(float duration)
    {
        _xMusic.DOFade(0, duration);
        _yMusic.DOFade(0, duration);
        _zMusic.DOFade(0, duration);
    }

    public void StopLevelMusic()
    {
        _bgMusic.DOFade(0, 1);
        _xMusic.DOFade(0, 1);
        _yMusic.DOFade(0, 1);
        _zMusic.DOFade(0, 1);
    }
}
