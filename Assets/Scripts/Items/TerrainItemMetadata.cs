using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainItemMetadata 
{
    [SerializeField] private GameObject _item;
    [SerializeField] private int _amount;
    [SerializeField] private bool _fixedPosition;
    [SerializeField] private List<TerrainItemMetadataInstance> _instances;

    public GameObject Item => _item;
    public int Amount => _amount;
    public bool FixedPosition => _fixedPosition;
    public List<TerrainItemMetadataInstance> Instances => _instances;
    
} 
