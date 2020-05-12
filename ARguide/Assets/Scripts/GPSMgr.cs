using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSMgr : MonoBehaviour
{
    // 자바 인스턴스
    private AndroidJavaObject m_JavaObject;

    // VAR : GET loation info
    private Text GPSText;
    private bool gpsInit;
    // location 
    private static LocationInfo LOC;
    private double LAT;
    private double LON;
    private float compass_headingAccu;
    public float compass_trueHeading;
    private int validCount;
    // tmp obj that help to set GPS text
    private string LOCtext;


    // VAR : GET lat and lon from INPUTFIELD
    private InputField enteredLAT;
    private InputField enteredLON;
    private Button enterBtn;
    private Text enterStatus;
    private Text guideStatus;

    // VAR : 화살표로 안내할 타겟위치의 위도경도
    public double targetLAT = 0;
    public double targetLON = 0;
    private bool isGuiding = false;
    private int guideCount = 0;

    // VAR : ARCamera 관련 변수
    private Camera ARCamera;
    public Transform ARCameraTransform;

    private float Heading;              // 이 각도로 배치한 사물은 북쪽을 가르킵니다!arcamera 각도 교정용
    public GameObject obj_Compass;      // 나침반
    public GameObject obj_GuideArrow;   // 방향안내용

    private IEnumerator GPSloader;


    [Header ("나침반 오브젝트")]
    [SerializeField] public GameObject compassObj;

    [Header ("안내용 화살표")]
    [SerializeField] public GameObject guideArrow;


    // Start is called before the first frame update
    void Start()
    {
        // 자바 클래스, 인스턴스 생성
        var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        m_JavaObject = jc.GetStatic<AndroidJavaObject>("currentActivity");

        // 카메라 받기
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        ARCameraTransform = GameObject.Find("First Person Camera").transform;

        // 받아온 gps 방향정보 띄워주는 텍스트
        GPSText = transform.Find("Canvas").Find("GPSText").GetComponent<Text>();

        // targetLATLON 받아오는 InputField 및 버튼과 디버깅용 텍스트오브젝트 불러오기
        enteredLAT = transform.Find("Canvas").Find("LATInput").GetComponent<InputField>();
        enteredLON = transform.Find("Canvas").Find("LONInput").GetComponent<InputField>();
        enterBtn = transform.Find("Canvas").Find("Button").GetComponent<Button>();
        enterBtn.onClick.AddListener(Check_enteredValue);
        enterStatus = transform.Find("Canvas").Find("Enterstatus").GetComponent<Text>();
        guideStatus = transform.Find("Canvas").Find("Guidestatus").GetComponent<Text>();


        // GPS 측정 시작
        Input.location.Start(0.01f, 0.01f); // Accuracy of 0.01 m
        Input.compass.enabled = true;
       
        int wait = 1000; // Per default
       
        // Checks if the GPS is enabled by the user (-> Allow location )
        if(!Input.location.isEnabledByUser){
            GPSText.text = "GPS not available !!";
            
        } else {
            GPSText.text = "GPS is available !!";
            while(Input.location.status == LocationServiceStatus.Initializing && wait>0){
                wait--;
            }
            if (Input.location.status == LocationServiceStatus.Failed) {
                GPSText.text = "GPS get FAILED";
            } else {
                gpsInit = true;
                GPSloader=LoadGPS(1f);
                StartCoroutine(GPSloader);
            }
        }
        // 나침반을 카메라의 child 로 생성
        obj_Compass = Instantiate(compassObj, ARCameraTransform.position, Quaternion.identity, ARCameraTransform);
    }

    // Update is called once per frame
    void Update()
    {
        // ARCamera의 각도 - 나침반 각도 계산
        // 이 각도로 배치한 사물은 북쪽을 가리킵니다!
        Heading = ARCameraTransform.eulerAngles.y - compass_trueHeading;
        // 나침반 각도 업데이트 (lerp 추가)
        //obj_Compass.transform.rotation = Quaternion.Euler(0, Heading, 0);
        obj_Compass.transform.rotation = Quaternion.Lerp(obj_Compass.transform.rotation, Quaternion.Euler(0, Heading, 0), 0.5f);

        // 안내중이지 않고, targetLATLON 값이 있을 때 코루틴 시작!
        if(isGuiding == false){
            if(targetLAT != 0 && targetLON !=0 ){
                isGuiding = true;
                StartCoroutine(GuideToTarget());
            }
        }
        
    }

    // GPS 좌표와 각도를 계속 로드해 ui에 표시해주는 코루틴
    private IEnumerator LoadGPS(float waitTime){
        while (Input.location.isEnabledByUser){
            // 정해진 초마다 gps 체크
            yield return new WaitForSeconds(waitTime);

            if(Input.location.isEnabledByUser){
                // 좌표 및 방향 확인
                /*
                LOC = Input.location.lastData;
                LAT = LOC.latitude;
                LON = LOC.longitude;
                compass_headingAccu = Input.compass.headingAccuracy;
                compass_trueHeading = Input.compass.trueHeading;
                */
                compass_headingAccu = Input.compass.headingAccuracy;
                validCount++;

                compass_trueHeading = (float)m_JavaObject.Call<double>("getAzimuth");
                var locations = m_JavaObject.Call<double[]>("getLocation");
                LAT = (float)locations[0];
                LON = (float)locations[1];

                LOCtext = "GPS is available ! vC:"+validCount;
                LOCtext += "\nstatus: "+Input.location.status;
                LOCtext += "\nLAT: "+LAT;
                LOCtext += "\nLON: "+LON;
                LOCtext += "\ncompAccu: "+compass_headingAccu;
                LOCtext += "\ncompHead: "+compass_trueHeading;
                GPSText.text = LOCtext;

            } else {
                GPSText.text = "GPS not available !"+"\nLAT: "+"\nLON:";
            }
        }
    }

    // 버튼 눌릴 시 실행되는 메서드
    // InputField로 받은 위도경도값이 적절하면 targetLATLON 값으로 설정
    // 적절하지 않을 경우 0으로 설정
    private void Check_enteredValue(){
        string tmpText = "";
        //string tmpLAT = enteredLAT.text;
        //string tmpLON = enteredLON.text;
        double tmpLAT;
        double tmpLON;

        try{
            tmpLAT = double.Parse(enteredLAT.text);
            tmpLON = double.Parse(enteredLON.text);

            if (37<tmpLAT && tmpLAT<38 && 126<tmpLON && tmpLON<128){
                tmpText += "valid point";
                tmpText += "\nNOW GUIDING";
                targetLAT = tmpLAT;
                targetLON = tmpLON;
            } else {
                tmpText += "\n invalid point";
                tmpText += "\n LAT37~38 LON126~128 needed";
                targetLAT = 0;
                targetLON = 0;
            }

        } catch (Exception e) {
            tmpText = "ERR during parse str -> double";
        }

        // set status text
        enterStatus.text = tmpText;

    }

    // 현재 설정된 targetLATLON 의 방향으로 화살표를 띄워줌
    // targetLANLON 값이 있으면 이 코루틴이 시작되고. 화살표 생성하여 안내
    // targetLANLON 값이 없으면 화살표 삭제, 안내 중지
    private IEnumerator GuideToTarget(){

        // 안내용 arrow를 카메라의 child 로 생성
        obj_GuideArrow = Instantiate(guideArrow, ARCameraTransform.position, Quaternion.identity, ARCameraTransform);

        

        // create guiding arrow
        while (targetLAT != 0 && targetLON != 0){
            yield return new WaitForSeconds(0.1f);
            guideCount++;
            guideStatus.text = "\ngC="+guideCount;


            // 나침반상에서 targetPoint가 어느 방향에 있는지 구하기
            float targetAngle = Mathf.Atan2((float)targetLON-(float)LON, (float)targetLAT-(float)LAT) * Mathf.Rad2Deg;

            obj_GuideArrow.transform.rotation = Quaternion.Lerp(obj_GuideArrow.transform.rotation, Quaternion.Euler(0, Heading+targetAngle, 0), 0.5f);
            //obj_GuideArrow.transform.LookAt(targetPoint);

            // 로그 찍기
            guideStatus.text += "\ntAngle: "+targetAngle;
            guideStatus.text += "\n\nnow guide \nFROM LAT "+sosu2(LAT)+" LON "+sosu2(LON);
            guideStatus.text += "\nTO LAT "+sosu2(targetLAT)+" LON "+sosu2(targetLON);
            //guideStatus.text += "\nx: "+obj_GuideArrow.transform.eulerAngles.x;
            //guideStatus.text += "\ny: "+obj_GuideArrow.transform.eulerAngles.y;
            //guideStatus.text += "\nz: "+obj_GuideArrow.transform.eulerAngles.z;

            

            

        }
        // targetLATLON의 값이 0으로 변경될 경우 guidingarrow 삭제하고 안내 중지
        Destroy(obj_GuideArrow);
        guideStatus.text = "stop guide";
        isGuiding = false;
    }

    // 소수점 둘째자리 이하 버리는 메서드
    float sosu2 (double value){
        return (float)Math.Truncate(value*100)/100;
    }


}

