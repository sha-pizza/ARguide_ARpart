using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class GPSMgr : MonoBehaviour
{
    [Header ("GPSMgr : GPS랑 방위 관리 및 androidjavaobject관리")]
    public Transform Nothing;

    // 자바 인스턴스
    private AndroidJavaObject m_JavaObject;

    //public InputField InputText;
    //검색된 목적지 리스트들의 위도 경도를 담는다
    public int searchcount = 0;
    public List<double> latslist = new List<double>();
    public List<double> longslist = new List<double>();

    // 경로
    double[] route;

    // 드롭다운 & 길 찾기 버튼
    public Dropdown dropdown2;
    public Button findRouteBtn;

    // 경로 찾았는지
    // 0530SA : guide시작할 때 접근해야해서 프라이빗 -> 퍼블릿스태틱으로 변경했습니다
    public static bool didFoundRoute = false;

    // 백그라운드 이미지
    private Image backgroundImage;

    // VAR : GPS및 방위 관련 
    public static string GPSstatus = "";
    private Text GPSText;
    private bool GPSinit;

    // location 
    public static LocationInfo LOC;
    public static double LAT;
    public static double LON;
    public static float compass_headingAccu;
    public static float compass_trueHeading;
    public static int validCount;
    // tmp obj that help to set GPS text >> DEPRECATED
    // private string LOCtext;
 

    // VAR : ARCamera 관련 변수
    private Camera ARCamera;
    private Transform ARCameraTransform;

    public static float Heading;              // 이 각도로 배치한 사물은 북쪽을 가르킵니다!arcamera 각도 교정용
    /* >> DEPRECATED
    public GameObject obj_Compass;      // 나침반
    public GameObject obj_GuideArrow;   // 방향안내용

    private IEnumerator GPSloader;

    [Header ("나침반 오브젝트")]
    [SerializeField] public GameObject compassObj;

    [Header ("안내용 화살표")]
    [SerializeField] public GameObject guideArrow;
    */


    // Start is called before the first frame update
    void Start()
    {
        // 자바 클래스, 인스턴스 생성
        // 0601SA : Exception처리함
        var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        m_JavaObject = jc.GetStatic<AndroidJavaObject>("currentActivity");
        /*
        try {
            var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_JavaObject = jc.GetStatic<AndroidJavaObject>("currentActivity");
        } catch (Exception e){
            Debug.LogException(e);
        }
        */
        
        
        // UI
        backgroundImage = transform.Find("Canvas").Find("Image").GetComponent<Image>();
        // Dropdown part
        /*
        dropdown = transform.Find("Canvas").Find("Dropdown").GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate

        {
            DropdownValueChangedHandler(dropdown);

        }); 
        findRouteBtn = transform.Find("Canvas").Find("Button_Find_Route").GetComponent<Button>();
        findRouteBtn.onClick.AddListener(Find_Route);
        */

       
        // 카메라 받기
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        ARCameraTransform = ARCamera.transform;
        // Debug.Log("camera : "+ARCameraTransform.eulerAngles.y);

        // 받아온 gps 방향정보 띄워주는 텍스트
        GPSText = transform.Find("Canvas").Find("GPSText").GetComponent<Text>();

        // targetLATLON 받아오는 InputField 및 버튼과 디버깅용 텍스트오브젝트 불러오기
        /* >> DEPRECATED
        enteredLAT = transform.Find("Canvas").Find("LATInput").GetComponent<InputField>();
        enteredLON = transform.Find("Canvas").Find("LONInput").GetComponent<InputField>();
        enterBtn = transform.Find("Canvas").Find("Button").GetComponent<Button>();
        enterBtn.onClick.AddListener(Check_enteredValue);
        enterStatus = transform.Find("Canvas").Find("Enterstatus").GetComponent<Text>();
        guideStatus = transform.Find("Canvas").Find("Guidestatus").GetComponent<Text>();
        */

        // GPS 측정 시작
        Input.location.Start(0.01f, 0.01f); // Accuracy of 0.01 m
        Input.compass.enabled = true;
       
        
        // Checks if the GPS is enabled by the user (-> Allow location )
        int wait = 1000; // Per default
        if(!Input.location.isEnabledByUser){
            GPSstatus = "GPS not available !!";     
        } else {
            GPSstatus = "";
            while(Input.location.status == LocationServiceStatus.Initializing && wait>0){
                wait--;
            }
            if (Input.location.status == LocationServiceStatus.Failed) {
                GPSstatus = "GPS get FAILED";
            } else {
                GPSinit = true;
                IEnumerator loadgps = LoadGPS(1.0f);
                StartCoroutine(loadgps);
            }
        }
        // 나침반을 카메라의 child 로 생성 >> DEPRECATED!
        // obj_Compass = Instantiate(compassObj, ARCameraTransform.position, Quaternion.identity, ARCameraTransform);
    }

    public void SelectButton()// SelectButton을 누름으로써 값 테스트.
    {
       // Debug.Log("Dropdown Value: " + dropdown.value + ", List Selected: " + (dropdown.value + 1));
        Debug.Log("Dropdown Value: " + dropdown2.value + ", List Selected: " + (dropdown2.value + 1));
    }

    // Update is called once per frame
    void Update()
    {
        // ARCamera의 각도 - 나침반 각도 계산
        // 이 각도로 배치한 사물은 북쪽을 가리킵니다!
        Heading = ARCamera.transform.eulerAngles.y - compass_trueHeading;

        /* >> DEPRECATED
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
        */
        
    }

    // GPS 좌표와 각도를 계속 로드해 ui에 표시해주는 코루틴
    private IEnumerator LoadGPS(float waitTime){
        while (Input.location.isEnabledByUser){
            // 정해진 초마다 gps 체크
            yield return new WaitForSeconds(waitTime);

            if(Input.location.isEnabledByUser){

                validCount++;
                
                // 좌표 및 방향 확인
                // 유니티 기본 리소스로
                /*
                LOC = Input.location.lastData;
                LAT = LOC.latitude;
                LON = LOC.longitude;
                compass_headingAccu = Input.compass.headingAccuracy;
                compass_trueHeading = Input.compass.trueHeading;
                */
                

                // android java object 사용해서
                
                compass_headingAccu = Input.compass.headingAccuracy;
                compass_trueHeading = (float)m_JavaObject.Call<double>("getAzimuth");
                var locations = m_JavaObject.Call<double[]>("getLocation");
                LAT = (float)locations[0];
                LON = (float)locations[1];
                
                // --> 텍스트박스 넘쳐서 잠시 비활성화했습니다
                
                if (didFoundRoute)
                {
                    // 루트 받아오기
                    //string destination = "신관";
                    route = m_JavaObject.Call<double[]>("getRoute");

                    for (int i = 0; i < route.Length / 2; i++)
                    {
                        GPSText.text += "\nroute - lat: " + route[i * 2 + 0] + " lon: " + route[i * 2 + 1];
                    }
                }
                
            } else {
                GPSstatus = "GPS not available !"+"\nLAT: "+"\nLON:";
            }
        }
    }


    // 소수점 둘째자리 이하 버리는 메서드
    float sosu2 (double value){
        return (float)Math.Truncate(value*100)/100;
    }

    // 드롭다운 메뉴 선택했을 때 콜백
    /*
    private void DropdownValueChangedHandler(Dropdown target)

    {
        switch (target.value)
        {
            case 0:
                m_JavaObject.Call("setDestination", "생명공학관61동 입구 1");
                break;
            case 1:
                m_JavaObject.Call("setDestination", "제2공학관 26동 입구 1");
                break;
            case 2:
                m_JavaObject.Call("setDestination", "제1공학관 23동 입구 1");
                break;
            case 3:
                m_JavaObject.Call("setDestination", "제1공학관 22동 입구 1");
                break;
            case 4:
                m_JavaObject.Call("setDestination", "제1공학관 23동 입구 2");
                break;
            default:
                break;
        }
    }
    */

    // 길 찾기 메소드
    public void Find_Route()
    {
        // 길 찾기
        //m_JavaObject.Call("findRoute");
         Debug.Log("setmap start: dropdown value in find route" + dropdown2.value);
        m_JavaObject.Call("findRoute", dropdown2.value);
    }

    public void Get_Route()
    {
        var route = m_JavaObject.Call<double[]>("getRoute");
        for (int i = 0; i < route.Length; i++)
        {
            if (route[i] != 0)
            {
                Debug.Log("route " + i + " " + route[i]);
            }

        }

        // 카메라 화면으로 전환
        backgroundImage.enabled = false;
        findRouteBtn.enabled = false;
        dropdown2.enabled = false;
        findRouteBtn.gameObject.SetActive(false);
        dropdown2.gameObject.SetActive(false);

        didFoundRoute = true;
    }

     public void Searchdropdown(Dropdown dropdown, InputField InputText)
    {
        //검색하기
        string query;
        query = InputText.text;
        //Debug.Log("passed:search ");
        //배열 초기화
        latslist.Clear();
        longslist.Clear();
        searchcount = 0;
        //이름 검색하기
        m_JavaObject.Call("setDestination", query);
        //Debug.Log("passed3:search " + query);
        //싱크맞추기
        Thread.Sleep(3000);
        //검색한 이름 결과 받아오기
        var locations = m_JavaObject.Call<string[]>("getLocationsName");
        var locations2 = m_JavaObject.Call<double[]>("getLocationsLat");
        var locations3 = m_JavaObject.Call<double[]>("getLocationsLog");
        Debug.Log("location size: " + locations.Length);
        Debug.Log("location2 size: " + locations2.Length);
        Debug.Log("location3 size: " + locations3.Length);
        for (int i = 0; i < locations.Length; i++)
        {
            if (locations[i] != null & locations[i] != "")
            {
                //Debug.Log("succeed location: " + i);
                //Debug.Log("succeed location: " + locations[i]);
                searchcount++;
                dropdown.options.Add(new Dropdown.OptionData(locations[i]));
                Debug.Log("location name: " + locations[i]);


            }

        }

        for (int i = 0; i < locations2.Length; i++)
        {
            Debug.Log("lat i number: " + i);
            Debug.Log("lat: " + locations2[i]);
            
            if (locations2[i] != 0)
            {
                //Debug.Log("succeed location: " + i);
                //Debug.Log("succeed location: " + locations[i]);
                latslist.Add(locations2[i]);
                Debug.Log("lat succeed: " + latslist[i]);
            }

        }

        for (int i = 0; i < locations3.Length; i++)
        {
            if (locations3[i] != 0)
            {
                //Debug.Log("succeed location: " + i);
                //Debug.Log("succeed location: " + locations[i]);
                
                longslist.Add(locations3[i]);
                Debug.Log("long succeed: " + longslist[i]);
            }

        }
        /*
        double LAT = (float)locations[0];
        double LON = (float)locations[1];
        text.text = LAT.ToString() + LON.ToString();
        */
    }
}

