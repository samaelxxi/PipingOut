using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[SelectionBase]
public class Key : MonoBehaviour
{
    [field: SerializeField] public int KeyValue { get; private set; }

    [SerializeField] AudioSource _pickUpSound;

    Tween _rotateTween;

    Transform _parent;
    Vector3 _startPos;
    Vector3 _startScale;
    Quaternion _startRot;

    void Start()
    {
        _parent = transform.parent;
        _startPos = transform.position;
        _startScale = transform.localScale;
        _startRot = transform.rotation;
        StartRotation();
    }

    void StartRotation()
    {
        var rotation = transform.rotation.eulerAngles;
        rotation.y += 360;
        _rotateTween = transform.DORotate(rotation, 10f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"Triggered by {other.name}");
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().HasAnyKey())
                return;
            GetComponent<Collider>().enabled = false;
            _rotateTween.Kill();
            _pickUpSound.Play();
            other.GetComponent<Player>().PickUpKey(this);
        }
    }

    public void DropKey()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
        transform.SetParent(_parent);
        transform.localScale = _startScale;
        GetComponent<Collider>().enabled = true;
        StartRotation();
    }
}
