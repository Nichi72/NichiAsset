using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class Sc_Bezier_Curves : MonoBehaviour {
    public enum MoveState
    {
        None,
        Straight_movePos,
        Straight_moveLerp,
        Curves
    }

    // - 위치들

    public GameObject moveTargetObj;
    public GameObject moveObj;
    public Transform startTr;
    public Transform senterTr;
    public Transform endTr;
    //곡선을 그릴 점의 갯수
    public float KSecCount = 50;
    // 등속도 운동 속도

    
    public float gravitSpeed = 3f;
    public float speed = 3f;
    public float moveUpLength=10;
    // 처음에 위로 등속도 운동 할 때 처음위치에서 얼마나 위로 올라갈지
    public Vector3 moveUpPos;

    Vector3 prePos;
    Vector3 pos;
    Rigidbody moveObjRig;
    // 현재 도달한 점
    int nowSec = 1;

    float KSecCountPower = 0.7f;
    public MoveState state = MoveState.None;
    // Use this for initialization
    void Start () {
        pos = Bezier2(startTr.position, senterTr.position, endTr.position, nowSec / KSecCount);
        moveObj.transform.position = startTr.position;
        moveTargetObj.transform.position = startTr.position;
        moveObjRig = moveObj.GetComponent<Rigidbody>();

        moveUpPos = startTr.position + Vector3.up* moveUpLength;

        // 점들마다 Lerp를 사용해서 이동하고 싶다.
        // 오브젝트 -> 베지어중 점 하나 거리가 가까워지면 다음 베지어 점 -....->
        StartCoroutine("IE_Update");

    }

    // Update is called once per frame
    void Update () {

        


    }
    IEnumerator IE_Update()
    {
        while(true)
        {
            switch(state)
            {
                case MoveState.None:
                    None();
                    break;
                case MoveState.Straight_movePos:
                    Straight_movePos();
                    break;
                    
                case MoveState.Straight_moveLerp:
                    Straight_moveLerp();
                    break;
                case MoveState.Curves:
                    Curves();
                    break;

            }

            yield return null;
        }
    }

    private IEnumerator Straight_moveLerp()
    {
        speed += 0.4f;
        speed = Mathf.Clamp(speed, 0, 20f);

        moveObj.transform.position = Vector3.Lerp(moveObj.transform.position, moveUpPos, Time.deltaTime * speed/10);
        if ((moveUpPos.y - moveObj.transform.position.y) < 0.3f)
        {
            startTr.position = moveUpPos;
            pos = Bezier2(startTr.position, senterTr.position, endTr.position, nowSec / KSecCount);
            moveObj.transform.position = startTr.position;
            moveTargetObj.transform.position = startTr.position;
            //KSecCountPower = speed;
            state = MoveState.Curves;

        }

        return null;
    }

    private IEnumerator None()
    {
        return null;
    }
    private IEnumerator Straight_movePos()
    {

        speed += 0.4f;
        speed = Mathf.Clamp(speed, 0, 13f);

        moveObj.transform.position += Vector3.up * speed * Time.deltaTime;
        if((moveUpPos.y -moveObj.transform.position.y )<0.1f )
        {
            startTr.position = moveUpPos;
            pos = Bezier2(startTr.position, senterTr.position, endTr.position, nowSec / KSecCount);
            moveObj.transform.position = startTr.position;
            moveTargetObj.transform.position = startTr.position;
            //KSecCountPower = speed;
            state = MoveState.Curves;

        }

        return null;
    }

    private IEnumerator Curves()
    {
        if (Vector3.Distance(pos, moveTargetObj.transform.position) < 1f)
        {
            nowSec++;
            float Temp = nowSec / (float)KSecCount;
            if (Temp > 0.3f)
            {
                KSecCountPower += 0.1f;
                KSecCountPower = Mathf.Clamp(KSecCountPower, 0f, 3f);
                //Debug.Log("KSecCountPower : " + KSecCountPower);
            }
            //Debug.Log("Temp : " + Temp);
            pos = Bezier2(startTr.position, senterTr.position, endTr.position, Temp);
        }
        moveTargetObj.transform.position = Vector3.Lerp(moveTargetObj.transform.position, pos, Time.deltaTime * speed);
        moveObj.transform.position = Vector3.Lerp(moveObj.transform.position, moveTargetObj.transform.position, Time.deltaTime * speed*1);//5f * KSecCountPower
        return null;
    }


 

    private void OnDrawGizmos()
    {
        
        Runner_Bezier(KSecCount);
    }

    List<Vector3> Runner_Bezier(float KSecCount)
    {
        List<Vector3> bezierPos = new List<Vector3>();
        Vector3 prePos = Vector3.zero;
        for (int i = 0; i < KSecCount; i++)
        {
            // 점이 많으면 많을 수록 촘촘해져서 더욱 부드러운 곡선이 된다.
            // 점이 곧 가중치이다.
            // 점이 3개라면 1/3씩 점을 찍으며 곡선이 된다.
            float t = i / (float)KSecCount;
            Vector3 pos = Bezier2(startTr.position, senterTr.position, endTr.position, t);
            bezierPos.Add(pos);
            Gizmos.color = Color.red;
            if (i > 0)
            {
                Gizmos.DrawLine(prePos, pos);
                Gizmos.DrawSphere(pos, 0.3f);
            }
            prePos = pos;
        }
        return bezierPos;
    }
    void Draw_Bezier(float KSecCount)
    {
        Vector3 prePos = Vector3.zero;
        for (int i = 0; i <= KSecCount; i++)
        {
            // 점이 많으면 많을 수록 촘촘해져서 더욱 부드러운 곡선이 된다.
            // 점이 곧 가중치이다.
            // 점이 3개라면 1/3씩 점을 찍으며 곡선이 된다.
            float t = i / (float)KSecCount;
            Vector3 pos = Bezier2(startTr.position, senterTr.position, endTr.position, t);
            Gizmos.color = Color.red;
            if (i > 0&&i<KSecCount)
            {
                Gizmos.DrawLine(prePos, pos);
                Gizmos.DrawSphere(pos, 0.3f);
            }
            else if(i==KSecCount)
            {
                Gizmos.DrawLine(pos, endTr.position);
            }
            prePos = pos;
        }
    }

    /// <summary>
    /// 배지어 곡선의 로직 부분이다.
    /// 스타트 , 센터 , 엔드 서로의 선분중 t(0~1)의 가중치로 배지어곡선의 점들이 생긴다. 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="center"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    Vector3 Bezier2(Vector3 start , Vector3 center , Vector3 end , float t)
    {
        // 1. start~center 가중치 위치
        Vector3 s_c = Vector3.Lerp(start, center, t);
        // 2. center~end 가중치 위치
        Vector3 c_e = Vector3.Lerp(center, end, t);
        // 1번위치와 2번 위치의 가중치 위치를 반환
        return Vector3.Lerp(s_c, c_e, t);
    }
}
