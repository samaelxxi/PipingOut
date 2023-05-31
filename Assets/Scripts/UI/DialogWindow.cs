using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialogWindow : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text _text;


    bool _isTalking = false;

    [SerializeField] float _waitBeforeStart = 0.5f;
    [SerializeField] float _letterTime = 0.07f;
    [SerializeField] float _spaceTime = 0.03f;
    [SerializeField] float _dotTime = 0.3f;
    [SerializeField] float _commaTime = 0.15f;
    [SerializeField] float _endTime = 1;


    Coroutine _coroutine;

    Coroutine _talkCor;

    [SerializeField] Transform _hat;


    public void Init()
    {
        // TODO make unity dont lag goddamnit
        // gameObject.SetActive(true);
        // _text.text = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~DELÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜø£Ø×ƒáíóúñÑªº¿®¬½¼¡«»░▒▓│┤ÁÂÀ©╣║╗╝¢¥┐└┴┬├─┼ãÃ╚╔╩╦╠═╬¤ðÐÊËÈıÍÎÏ┘┌█▄¦Ì▀ÓßÔÒõÕµþÞÚÛÙýÝ¯´­±‗¾¶§÷¸°¨·¹³²■";
        // _text.text = "";
        // gameObject.SetActive(false);
    }

    // IEnumerator ShowCoroutine(string text, bool force = false)
    // {
    //     if (_isTalking && !force)
    //         yield break;

    //     Prepare(force);
    //     yield return StartCoroutine(ShowText(text));
    // }


    IEnumerator MakeFaceTalking()
    {
        while (true)
        {
            yield return _hat.DOShakeRotation(0.5f, -30, 5, 0, true, ShakeRandomnessMode.Full).WaitForCompletion();
        }
    }

    void StopTalking(float duration = 0)
    {
        if (_talkCor != null)
            StopCoroutine(_talkCor);
        _talkCor = null;
        _hat.DOLocalRotate(Vector3.zero, duration);
    }

    public IEnumerator Show(string text, bool force)
    {
        if (_isTalking && !force)
            yield break;

        Prepare(force);
        _coroutine = StartCoroutine(ShowText(text));
        yield return _coroutine;
    }

    public IEnumerator Show(List<string> texts, bool force)
    {
        if (_isTalking && !force)
            yield break;

        Prepare(force);
        _coroutine = StartCoroutine(ShowTexts(texts));
        yield return _coroutine;
    }

    public void Show(string text, bool force, Action onComplete)
    {
        if (_isTalking && !force)
            return;

        Prepare(force);
        _coroutine = StartCoroutine(ShowText(text, onComplete));
    }

    public void Show(List<string> texts, bool force, Action onComplete)
    {
        if (texts.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        if (_isTalking && !force)
            return;

        Prepare(force);
        _coroutine = StartCoroutine(ShowTexts(texts, onComplete));
    }

    void Prepare(bool force)
    {
        if (force && _coroutine != null)
            StopCoroutine(_coroutine);

        StopTalking();
        _text.text = "";
        _isTalking = true;
        // Debug.Log($"DialogWindow Prepare {gameObject == null} ");
        gameObject.SetActive(true);
    }

    IEnumerator ShowTexts(List<string> texts, Action onComplete=null)
    {
        yield return new WaitForSeconds(_waitBeforeStart);
        
        _talkCor = StartCoroutine(MakeFaceTalking());
        foreach (var text in texts)
        {
            yield return StartCoroutine(ShowTextInternal(text));
            _text.text = "";
        }
        StopTalking();
        yield return new WaitForSeconds(_endTime);
        onComplete?.Invoke();
        Clean();
    }

    IEnumerator ShowText(string text, Action onComplete=null)
    {
        yield return new WaitForSeconds(_waitBeforeStart);
        _talkCor = StartCoroutine(MakeFaceTalking());
        yield return StartCoroutine(ShowTextInternal(text));
        StopTalking(_endTime / 2);
        yield return new WaitForSeconds(_endTime);
        onComplete?.Invoke();
        Clean();
    }

    void Clean()
    {
        _text.text = "";
        _isTalking = false;
        gameObject.SetActive(false);
    }

    IEnumerator ShowTextInternal(string text)
    {
        foreach (var letter in text)
            yield return StartCoroutine(ShowSymbol(letter));
        float waitTime = Mathf.Clamp(text.Length * 0.07f, 0.5f, 1.2f);
        yield return new WaitForSeconds(waitTime);
    }

    IEnumerator ShowSymbol(char symbol)
    {
        _text.text += symbol;
        if (symbol == '.')
        {
            yield return new WaitForSeconds(_dotTime);
        }
        else if (symbol == ',')
        {
            yield return new WaitForSeconds(_commaTime);
        }
        else if (symbol == ' ')
        {
            yield return new WaitForSeconds(_spaceTime);
        }
        else
            yield return new WaitForSeconds(_letterTime);
    }
}
