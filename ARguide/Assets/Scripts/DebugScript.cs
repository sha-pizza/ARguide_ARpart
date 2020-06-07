using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
    [Header("DebugScript : GPS 및 ARcamera정보 로그찍기")]
    public Transform Nothing;


    private Text cameraInfo;
    private Text GPSInfo;
    private Camera ARCamera;




    // Start is called before the first frame update
    void Start()
    {
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        cameraInfo = transform.Find("Camerainfo").GetComponent<Text>();
        GPSInfo = transform.Find("GPSinfo").GetComponent<Text>();

        StartCoroutine(Debugchecker());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Debugchecker () {
        while(true){
            yield return new WaitForSeconds(0.2f);
            // set gps info
            if (GPSMgr.GPSstatus.Length == 0){
                GPSInfo.text = "GPS is available ! vC:"+GPSMgr.validCount;
                GPSInfo.text += "\nstatus: "+Input.location.status;
                GPSInfo.text += "\nLAT: "+GPSMgr.LAT;
                GPSInfo.text += "\nLON: "+GPSMgr.LON;
                GPSInfo.text += "\ncompAccu: "+GPSMgr.compass_headingAccu;
                GPSInfo.text += "\ncompHead: "+GPSMgr.compass_trueHeading;
            } else {
                GPSInfo.text = GPSMgr.GPSstatus;
            }
            




            // set camera info
            cameraInfo.text = "posX: "+ARCamera.transform.position.x;
            cameraInfo.text += "\nposY: "+ARCamera.transform.position.y;
            cameraInfo.text += "\nposZ: "+ARCamera.transform.position.z;
            cameraInfo.text += "\nrotX: "+ARCamera.transform.eulerAngles.x;
            cameraInfo.text += "\nrotY: "+ARCamera.transform.eulerAngles.y;
            cameraInfo.text += "\nrotZ: "+ARCamera.transform.eulerAngles.z;
        }
    }
}
