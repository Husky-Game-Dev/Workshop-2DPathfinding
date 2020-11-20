using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnTile : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public TileMap map;

    void OnMouseUp()
    {
        Debug.Log("Tile Clicked!");
        //map.moveUnit(tileX, tileY);
        //map.moveBFS(tileX, tileY);
        StartCoroutine(colorFlash());
        //map.moveA(tileX, tileY);
    }

    IEnumerator colorFlash() {
        SpriteRenderer c = GetComponent<Transform>().GetComponent<SpriteRenderer>();
        Color defaultColor = c.color;
        c.color = Color.blue;
        yield return new WaitForSeconds(0.5f);
        c.color = defaultColor;
    }
}
