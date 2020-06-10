using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestinationMgr : MonoBehaviour
{

    [Header ("DestinationMgr : 경로의 각 파트별 안내를 위한 가상의 목적지 생성 담당")]
    public Transform Nothing;

    public static Transform destination;

    private Transform ARCameraTransform;     // AR 카메라의 transform 컴포넌트

    //private Text camtext;
    public int Bumper = 12;



    // Start is called before the first frame update
    void Start()
    {
        destination = transform.Find("Destination").transform;
        ARCameraTransform = GameObject.Find("First Person Camera").transform;
        

        //camtext = GameObject.Find("DebugCanvas/Routeinfo").GetComponent<Text>();

        IEnumerator destinM = destinationMover();
        StartCoroutine(destinM);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator destinationMover(){
        while (true){
            yield return new WaitForSeconds(0.05f);
            //destinationMgr 오브젝트가 북쪽을 바라보도록 수정
            //compassObject.rotation = Quaternion.Lerp(compassObject.rotation , Quaternion.Euler(0, GPSMgr.Heading, 0), 0.5f);
            transform.position = new Vector3(ARCameraTransform.position.x, 0, ARCameraTransform.position.z);
            transform.rotation = Quaternion.Euler(0, GPSMgr.Heading, 0);
            //현재 파트의 목표지점으로 destination 오브젝트 이동
            double targetLAT = GuideMgr.route[GuideMgr.nowPointNum] - (float)GPSMgr.LAT;
            double targetLON = GuideMgr.route[(GuideMgr.nowPointNum)+1] - (float)GPSMgr.LON;

            double cosValue = targetLON / Mathf.Sqrt((float)(targetLAT*targetLAT + targetLON*targetLON));
            double sinValue = targetLAT / Mathf.Sqrt((float)(targetLAT*targetLAT + targetLON*targetLON));
            
            
            destination.transform.localPosition = new Vector3(Bumper*(float)cosValue, -1.2f, Bumper*(float)sinValue);
            
            //destination.transform.localPosition = new Vector3 (targetLON*10000,0,targetLAT*10000);

            /*
            camtext.text = "\nLAT :"+ GPSMgr.LAT + "\n->" + GuideMgr.route[GuideMgr.nowPointNum] ;
            camtext.text += "\nLON :"+ GPSMgr.LON + "\n->" + GuideMgr.route[(GuideMgr.nowPointNum)+1] ;
            camtext.text += "\ntLAT: "+destination.transform.localPosition.z;
            camtext.text += "\ntLON: "+destination.transform.localPosition.x;
            */
            
        }
    }
}
