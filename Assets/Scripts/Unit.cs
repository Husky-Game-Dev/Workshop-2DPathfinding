﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public TileMap map;
    public List<TileMap.Node> currentPath = null;

    void Update()
    {
        if (currentPath != null) {
            //Debug.Log("Current path: " + currentPath.Count);
            int currNode = 0;
            while (currNode < currentPath.Count - 1) {
                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode+1].x, currentPath[currNode+1].y);
                Debug.DrawLine(start, end, Color.red);
                currNode++;
            }
        }
    }
}
