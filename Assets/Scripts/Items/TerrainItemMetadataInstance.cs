using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainItemMetadataInstance
{
    public TerrainItemMetadataInstance()
    {

    }

    public TerrainItemMetadataInstance(Vector2Int newPos, GameObject newInstance)
    {
        itemPosition = newPos;
        itemInstance = newInstance;
    }

    public Vector2Int itemPosition;
    public GameObject itemInstance;
}
