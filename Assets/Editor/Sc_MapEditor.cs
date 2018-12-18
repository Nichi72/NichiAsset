using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 유니티에서 에디터를 확장하기 위해 사용하는 api(기능)
using UnityEditor;
using System;
// 유니티가 시작되면
// 모노비헤이비어를 상속받는 애들을 먼저 찾아서 메모리에 올린다.
// 그 이후 에디터 폴더에 있는 스크립트를 찾아서 메모리에 올려서 먼저 올라온 스크립트에 참조를 하게 된다.


// Map 스크립트를 확장하여 Inspector창과 Scene 창에 표시되는 정보를 바꾸고싶다.

// 나 Map 객체를 확장하고 싶다.
// Attribute
[CustomEditor(typeof(Sc_TileMap))]// == target
public class Sc_MapEditor : Editor
{
    Sc_TileMap map;

    private void OnEnable()
    {
        map = (Sc_TileMap)target; 
    }
    // Inspector 찰을 그리는 역할을 하는 함수 
    public override void OnInspectorGUI()
    {
        // base => 나의 부모를 뜻함.
        // this => 나를 뜻한다.
        // 원래 부모가 쓰던 OnInspectorGUI를 쓰겠다/
        //base.OnInspectorGUI();

        // tileX라는 변수 이름을 가로 타일개수로 표시하고싶다
        // tileX는 Map 안에 있다.
        // map 을 가져와야 한다.

        // 타일 개수가 무조건 0 보다는 커야한다.
        // 최소 1

        // 맵 만들어줘 라는 버튼을 만든다. 
        // 버튼을 클릭하면 맵이 만들어지도록...
        Sc_TileMap map = (Sc_TileMap)target;
        map.tileX = Mathf.Clamp(map.tileX, 1, int.MaxValue);
        map.tileY = Mathf.Clamp(map.tileY, 1, int.MaxValue);
        map.tileX = EditorGUILayout.IntField("가로 타일 개수 ", map.tileX);
        map.tileY = EditorGUILayout.IntField("세로 타일 개수 ", map.tileY);

      



        // 에디트 상에서 한칸 띄우기! 
        EditorGUILayout.Space();
        map.floorTile = (GameObject)EditorGUILayout.ObjectField("바닥 타일 ", map.floorTile, typeof(GameObject), true);
        // 트루면 씬에 올라와 있는걸 넣을 수 있다.

        // 내가 선택한 타일 그리기 
        map.selectedTile = (GameObject)EditorGUILayout.ObjectField("그릴타일", map.selectedTile, typeof(GameObject), true);

        if (GUILayout.Button("맵생성"))
        {
            // 만약 TileMap 객체에 floorTile에 값이 안들어가 있으면 ?
            // 사용자한테 알려줘야해 
            if (map.floorTile == null)
            {

                if (EditorUtility.DisplayDialog("Error", "바닥타일 넣으세요 ~ ", " O K "))
                {
                    return;
                }
            }

            // 기존에 맵이 있으면 모든 맵에 올라간 모든 타일 제거 . 
            CheckTileExist();
            CreatMap();
        }
        // 꼭 찾아보기
        //ScriptableObject

    }

    private void CheckTileExist()
    {
        GameObject floorTile = GameObject.Find("FloorTile");
        // 만약 씬에 floorTile 이 있다면
        if (floorTile)
        {
            // 제거한다.
            DestroyImmediate(floorTile);
        }



    }

    // 씬 뷰에 바닥 타일을 이용해여 tile X , tile Y 크기만큼 그린다.
    private void CreatMap()
    {
        // 1. 어디서 부터 그릴지
        Vector3 center = Vector3.zero;
        // 2. 바닥타일을 생성 
        GameObject floor = (GameObject)PrefabUtility.InstantiatePrefab(map.floorTile);
        // 3. 생성된 타일의 크기를 맵 크기만큼 변경 
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileY);
        // 4. 위치 설정
        // 0,0,0으로 하면 정중앙으로 잡히니까! 
        floor.transform.position = center + Vector3.right * map.tileX * 0.5f + Vector3.forward * map.tileY * 0.5f;

    }


    // 
    // 씬에 타일 개수만큼 그리드를 그리고 싶다 .
    private void OnSceneGUI()
    {
        //씬에서 오브젝트를 클릭했을때 포커스가 다른 오브젝트로 변경되지 않도록 하자 
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        //map = (Sc_TileMap)target;

        DrowFloorTIle();
        
        // 만약에 SelectedTile에 값이 있다면
        if(map.selectedTile)
        {
            // 사용자 입력에 맞게 타일을 그려라
            DrowTiles();
        }

    }

    // 마우스 좌클릭 중일때만 그리게 
    // 우클릭 중이면 ?? 안되게 
    // alt , ctrl , 누르고 있으면 ?? 안되게 
    // 타일 위에 타일 그리기 
    // 쉬프트 누르고 좌클릭하면 지워지게 

    
    private void DrowTiles()
    {
        Debug.Log("111111111111111111111");

        // 1. 사용자의 마우스 입력을 가져와야 한다. 
        // -Event 처리를 통해 해야한다.
        // Event.current;현재의 모든 이벤트가 갱신된다.
        Event e = Event.current;

        if(e.alt || e.control)
        {
            return;
        }
        // 2. RAY를 쏜다. 
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hitInfo;
        int layer = 1 << LayerMask.NameToLayer("Tile");
        // 3. Ray위치에 타일을 그린다.
        if(Physics.Raycast(ray, out hitInfo, int.MaxValue ,layer))
        {
            Debug.Log("2222222222222222");
            // 1. 찾아낸 지점에 타일을 생성 
            GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(map.selectedTile);
            map.name = "Tile";
            // 2. 위치 설정 
            // - index 를 찾아야 한다.
            int x = Mathf.FloorToInt(hitInfo.point.x % map.tileX);
            int y = Mathf.FloorToInt(hitInfo.point.z % map.tileY);

            tile.transform.position = new Vector3(x + 0.5f, 0.5f, y + 0.5f);
            tile.transform.parent = GameObject.Find("FloorTile").transform;
        }
    }

    private void DrowFloorTIle()
    {
        //1.  화면의 (0,0,0)에 그리도록 하겠다.
        Vector3 center = Vector3.zero;
        //2. Grid 그릴 색상 지정하자
        // - Scene 에 무엇을 그리거나 버튼등을 배치하거나 하고 싶을때 HandleXXXX 를 사용한다.
        Handles.color = Color.green;
        //map의 tileY가 Z축 방향을 나타낸다.
        for (int i = 0; i <= map.tileY; i++)
        {
            for (int j = 0; j <= map.tileX; j++)
            {
                Handles.DrawLine(center + Vector3.right * j, center + Vector3.right * j + Vector3.forward * map.tileY);
            }
            Handles.DrawLine(center + Vector3.forward * i, center + Vector3.forward * i + Vector3.right * map.tileX);


        }
    }
}
