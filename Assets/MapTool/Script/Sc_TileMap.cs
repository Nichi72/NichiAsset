using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 맵 속성만을 기억하는 객체 
// 타일의 가로 세로 개수
public class Sc_TileMap : MonoBehaviour {
    // 타일의 가로 세로 개수
    // 
    public int tileX;
    public int tileY;

    public GameObject floorTile;
    // 내가 선택한 타일 
    public GameObject selectedTile;
}
