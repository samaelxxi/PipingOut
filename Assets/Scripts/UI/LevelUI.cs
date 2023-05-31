using System.Collections;
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelUI : MonoBehaviour
{
    [SerializeField] Image _overlay;
    [SerializeField] DialogWindow _dialogWindow;

    [SerializeField] GameObject _hintWindow;
    [SerializeField] TMPro.TMP_Text _hintText;


    public void Init()
    {
        gameObject.SetActive(true);
        _overlay.gameObject.SetActive(true);
        _dialogWindow.gameObject.SetActive(false);
        _hintWindow.SetActive(false);
    }

    public void FadeOverlay(float alpha, float time, Action onComplete = null)
    {
        _overlay.DOFade(alpha, time).OnComplete(() => onComplete?.Invoke());
    }

    public void HideScreen(float time, Action OnComplete = null)
    {
        FadeOverlay(1, time, OnComplete);
    }

    public void RevealScreen(float time, Action OnComplete = null)
    {
        FadeOverlay(0, time, OnComplete);
    }

    public IEnumerator ShowDialogCoroutine(string text, bool force = false)
    {
        return _dialogWindow.Show(text, force);
    }

    public IEnumerator ShowDialogCoroutine(List<string> text, bool force = false)
    {
        return _dialogWindow.Show(text, force);
    }

    public void ShowDialog(string text, bool force = false, Action onComplete = null)
    {
        _dialogWindow.Show(text, force, onComplete);
    }

    public void ShowDialog(List<string> texts, bool force = false, Action onComplete = null)
    {
        _dialogWindow.Show(texts, force, onComplete);
    }

    public void ShowHint(string text, float duration = 0)
    {
        _hintText.text = text;
        _hintWindow.SetActive(true);
        if (duration > 0)
            Invoke(nameof(HideHint), duration);
    }

    public void HideHint()
    {
        _hintWindow.SetActive(false);
    }

    public void CompleteLevel()  // cheat
    {
        FindObjectOfType<GameLevel>().CompleteLevel();
    }
}
