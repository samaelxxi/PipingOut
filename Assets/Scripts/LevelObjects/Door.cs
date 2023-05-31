using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[SelectionBase]
public class Door : MonoBehaviour
{
    [SerializeField] int _keyValue;
    [SerializeField] Rigidbody _leftPart;
    [SerializeField] Rigidbody _rightPart;

    [SerializeField] Transform _keyStartPlace;
    [SerializeField] Transform _keyHole;
    [SerializeField] Transform _lock;


    [SerializeField] AudioSource _doorOpenSound;
    [SerializeField] AudioSource _doorFallSound;

    Transform _key;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player.HasKey(_keyValue))
            {
                _key = player.TakeKeyAway().transform;
                StartCoroutine(OpenDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        _key.SetParent(_keyStartPlace);
        _key.DOLocalMove(Vector3.zero, 2f);
        _key.DOScale(_key.transform.localScale * 2, 2f);
        yield return _key.DOLocalRotate(Vector3.zero, 2f);
        yield return new WaitForSeconds(2f);
        yield return _key.DOMove(_keyHole.position, 0.6f).SetEase(Ease.InQuint);
        yield return new WaitForSeconds(0.3f);
        _doorOpenSound.Play();
        yield return new WaitForSeconds(0.3f);
        yield return _key.DOLocalRotate(_key.transform.localEulerAngles + new Vector3(0, -90, 0), 0.7f).SetEase(Ease.InQuint);
        yield return new WaitForSeconds(1.5f);
        _leftPart.isKinematic = false;
        _rightPart.isKinematic = false;
        _lock.DOScale(Vector3.zero, 1f).OnComplete(() => Destroy(_keyHole.gameObject));
        _key.DOScale(Vector3.zero, 0.5f).OnComplete(() => Destroy(_key.gameObject));
        _doorFallSound.Play();
        _leftPart.AddForce(transform.forward * 0.3f, ForceMode.Impulse);
        _rightPart.AddForce(transform.forward * -0.3f, ForceMode.Impulse);
        Destroy(gameObject, 10);
    }
}
