using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType
{
    public string name;
    public GameObject visualPrefab;
    public bool isWalkable;
    public float cost = 1;
}
