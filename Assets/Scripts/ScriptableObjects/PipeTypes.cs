using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PipeTypes", menuName = "ScriptableObjects/PipeTypes", order = 1)]
public class PipeTypes : ScriptableObject
{
    public GameObject _openPipePrefab;
    public List<Material> _openPipeMaterials;
    public GameObject _bigCornerPipePrefab;
    public List<Material> _bigCornerPipeMaterials;
    public GameObject _smallCornerPipePrefab;
    public List<Material> _smallCornerPipeMaterials;
}
