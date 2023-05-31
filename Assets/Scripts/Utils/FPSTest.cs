using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTest : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text _text;
    [SerializeField] GameObject _prefab;
    [SerializeField] Transform _parent;


    int _totalCount = 0;
    int _count = 0;
    float t = 0;
    float timer = 0;
    int _left = 0;

    float frameRate;

    // Update is called once per frame
    void Update()
    {
        if (timer < 1)
        {
            timer += Time.deltaTime;
            frameRate += 1;
        }
        else
        {
            _text.text = $"{frameRate.ToString()}\n{_totalCount}";
            timer = 0;
            frameRate = 0;
        }


        t += Time.deltaTime;
        if (t > 0.2f)
        {
            t = 0;
            Instantiate(_prefab, _parent.position + 0.5f * Vector3.up * _count++ + Vector3.right * _left, 
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), _parent);
            _totalCount++;
            if (_count > 20)
            {
                _count = 0;
                _left++;
            }
        }
    }
}
