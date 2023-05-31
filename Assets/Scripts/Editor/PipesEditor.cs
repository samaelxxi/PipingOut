using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PipesEditor : MonoBehaviour
{
    [SerializeField] PipeTypes _pipeType;
    [SerializeField] PipeColor _materialNum;
    [SerializeField] List<Transform> _points;
    // Start is called before the first frame update

    [SerializeField] bool _place = false;
    [SerializeField] bool _clear = false;
    [SerializeField] Transform _pipeParent;


    void OnValidate()
    {
        if (_place == true)
        {
            _place = false;
            TryPlacePipes();
        }
        else if (_clear)
        {
            for (int i = 0; i < _pipeParent.childCount; i++)
            {
                StartCoroutine(Destroy(_pipeParent.GetChild(i).gameObject));
            }
            _clear = false;
        }
    }

    void TryPlacePipes()
    {
        if (_points.Count < 2)
        {
            Debug.Log($"Not enough points for {_pipeType} pipe type");
            return;
        }
        
        if (!_points.TrueForAll(p => p != null))
        {
            Debug.Log($"Some points are null for {_pipeType} pipe type");
            return;
        }

        var p1 = _points[0].position;
        var p2 = _points[1].position;
        var dist = p2 - p1;
        Debug.Log($"{p1} {p2} {dist}");
        int x = Mathf.RoundToInt(dist.x);
        int y = Mathf.RoundToInt(dist.y);
        int z = Mathf.RoundToInt(dist.z);

        bool toX, toY, toZ = false;
        toX = x != 0;
        toY = y != 0;
        toZ = z != 0;

        if ((toX && toY) || (toX && toZ) || (toY && toZ))
        {
            Debug.Log($"Pipe {_pipeType} can't be placed on diagonal");
            return;
        }

        while ((p2 - p1).magnitude > 1)
        {
            var pipe = Instantiate(_pipeType._openPipePrefab, _pipeParent);
            pipe.GetComponent<MeshRenderer>().material = _pipeType._openPipeMaterials[(int)_materialNum];
            pipe.transform.position = p1;
            if (toX)
            {
                pipe.transform.Rotate(0, 90, 0);
                if (x < 0)
                {
                    pipe.transform.position -= new Vector3(1, 0, 0);
                }
                p1 += new Vector3(1, 0, 0) * Mathf.Sign(x);
            }

            else if (toZ)
            {
                if (z < 0)
                {
                    pipe.transform.position -= new Vector3(0, 0, 1);
                }
                p1 += new Vector3(0, 0, 1) * Mathf.Sign(z);
            }

            else if (toY)
            {
                pipe.transform.Rotate(90, 0, 0);
                pipe.transform.position -= new Vector3(0, 1, 0);
                if (y < 0)
                {
                    pipe.transform.position -= new Vector3(0, 1, 0);
                }
                p1 += new Vector3(0, 1, 0) * Mathf.Sign(y);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_points.Count < 2 || !_points.TrueForAll(p => p != null))
            return;

        for (int i = 0; i < _points.Count - 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
        }
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(go);
    }

}
