using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
    [Header("DebugScript : GPS, 가이드상태, 경로 등의 정보 로그찍기")]
    public Transform Nothing;

    private Text DebugText;
    


    private Text cameraInfo;
    private Text GPSInfo;
    private Camera ARCamera;




    // Start is called before the first frame update
    void Start()
    {
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        DebugText = transform.Find("DebugText").GetComponent<Text>();
        StartCoroutine(Debugchecker());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Debugchecker () {
        string DebugStr;
        
        while(true){
            yield return new WaitForSeconds(0.5f);
            // set debugText
            // GPSstatus : ~ / LatLon : ~, ~
            DebugStr = "";
            DebugStr += "GPSstatus : " + GPSMgr.GPSstatus + "/ LatLon : "+ (float)GPSMgr.LAT + ", " + (float)GPSMgr.LON ;
            DebugStr += "\nGuidestatus : " + GuideMgr.Guidestatus;

            DebugText.text = DebugStr;

            /*
            // set gps info
            if (GPSMgr.GPSstatus.Length == 0){
                //GPSInfo.text = "GPS is available ! vC:"+GPSMgr.validCount;
                //GPSInfo.text += "\nstatus: "+Input.location.status;
                //GPSInfo.text += "\nLAT: "+GPSMgr.LAT;
                //GPSInfo.text = "LAT: "+GPSMgr.LAT;
                //GPSInfo.text += "\nLON: "+GPSMgr.LON;
                //GPSInfo.text += "\ncompAccu: "+GPSMgr.compass_headingAccu;
                //GPSInfo.text += "\ncompHead: "+GPSMgr.compass_trueHeading;
            } else {
                GPSInfo.text = GPSMgr.GPSstatus;
            }*/
            



        }
    }
}
