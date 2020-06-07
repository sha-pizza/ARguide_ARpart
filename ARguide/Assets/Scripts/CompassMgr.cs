using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassMgr : MonoBehaviour
{

    [Header ("CompassMgr : 카메라 하단의 나침반 오브젝트와 방향안내 화살표 담당")]
    public Transform Nothing;
    private Camera ARCamera;
    

    private Transform compassObject;
    private Transform arrowObject;

    private Transform UI_SmallMap;

    // Start is called before the first frame update
    void Start()
    {
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
      
        compassObject = transform.Find("Arrows_compass").transform;
        arrowObject = transform.Find("Arrows_guide").transform;
        //UI_SmallMap = GameObject.Find("UICanvas/SmallMap").transform;

        IEnumerator compM = compassMover();
        StartCoroutine(compM);
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator compassMover(){
        while (true){
            yield return new WaitForSeconds(0.05f);
            // compassmgr의 위치를 카메라 위치로 이동
            transform.position = Vector3.Lerp(transform.position, ARCamera.transform.position, 0.5f);
            // 나침반 오브젝트가 북쪽을 가르키도록 회전
            compassObject.rotation = Quaternion.Lerp(compassObject.rotation , Quaternion.Euler(0, GPSMgr.Heading, 0), 0.5f);
            // 화살표 오브젝트도 현재 목표점 따라 회전시키기
            Vector3 targetPos = new Vector3(DestinationMgr.destination.position.x, 0, DestinationMgr.destination.position.z);
            arrowObject.LookAt(targetPos);    
            // ui smallmap도 회전
            //UI_SmallMap.rotation = Quaternion.Lerp(UI_SmallMap.rotation, Quaternion.Euler(0, 0, GPSMgr.Heading), 0.5f);

        }
    }
}
