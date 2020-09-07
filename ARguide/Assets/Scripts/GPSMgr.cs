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
    static public AndroidJavaObject m_JavaObject;

    //public InputField InputText;
    //검색된 목적지 리스트들의 위도 경도를 담는다
    public int searchcount = 0;
    public List<double> latslist = new List<double>();
    public List<double> longslist = new List<double>();

    // 경로
    public static double[] route;


    // 목적기 (가이드 후 안내용)
    public static string finalDestination = "";

    // 드롭다운 & 길 찾기 버튼
    public GameObject TapToStart;
    public Dropdown dropdown2;  // Dropdown2
    public Button findRouteBtn; // Button_Find_Route
    public GameObject Button2;  // button2
    public GameObject Button3;  // Button3
    public GameObject RawImage; // rawimage
    public GameObject Inputobj; // Input
    public GameObject MapPart; // map-backgroundimg and route object
    public MapRouteMgr RouteMaker; // active false 안해줘도댐 왜냐면 MapPart의 child이기 때문에

    // 백그라운드 이미지
    public Image backgroundImage;


    // 경로 찾았는지
    public static bool didFoundRoute = false;

    

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

    // 위치 못 가져오는 에러 처리
    private int previousLocationLoadedCount = 0;
    private int currentLocationLoadedCount = 0;
    public static int secsNotLoadedLocation = 0;
    private int secsNotLoadedTolerance = 10;
    public static bool overNsecsNotLoadedLocation = false;

    // tmp obj that help to set GPS text >> DEPRECATED
    // private string LOCtext;
 

    // VAR : ARCamera 관련 변수
    private Camera ARCamera;
    private Transform ARCameraTransform;

    public static float Heading;              // 이 각도로 배치한 사물은 북쪽을 가르킵니다!arcamera 각도 교정용



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ARGUIDE_gps : start gpsmgr");
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
        //backgroundImage = transform.Find("Canvas").Find("Image").GetComponent<Image>();
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

        // GPS 측정 시작
        Input.location.Start(0.01f, 0.01f); // Accuracy of 0.01 m
        Input.compass.enabled = true;
       
        
        // Checks if the GPS is enabled by the user (-> Allow location )
        int wait = 1000; // Per default
        if(!Input.location.isEnabledByUser){
            GPSstatus = "GPS not available";     
        } else {
            GPSstatus = "GPS available";
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

                // 위치 못 가져오는 에러 처리
                previousLocationLoadedCount = currentLocationLoadedCount;
                currentLocationLoadedCount = (int)locations[2];

                if (previousLocationLoadedCount == currentLocationLoadedCount) secsNotLoadedLocation++;
                else secsNotLoadedLocation = 0;

                if (secsNotLoadedLocation >= secsNotLoadedTolerance) overNsecsNotLoadedLocation = true;
                
                
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

    // 입력받은 목표지 저장
    public void Set_Destin(){

        finalDestination  = dropdown2.options[dropdown2.value].text;
        string editedDestin="";

        // '입구' 글자 자르기
        if (finalDestination != ""){
            for (int i = 0 ; i < finalDestination.Length ; i++){
                if (finalDestination[i] == '입'){
                    break;
                } else {
                    editedDestin += finalDestination[i];
                }
            }
        }

        finalDestination = editedDestin;
        //GPSText.text = finalDestination;
    }


    // 길 찾기 메소드
    public void Find_Route()
    {
        // 길 찾기
        //m_JavaObject.Call("findRoute");
         Debug.Log("setmap start: dropdown value in find route" + dropdown2.value);
        m_JavaObject.Call("findRoute", dropdown2.value);
    }

    // 목적지까지의 경로 표시 버튼
    public void Draw_Route()
    {
        // 경로 double 배열을 MaprouteMgr로 보냄
        
        //double[] routeToDraw = {37.2970, 126.9700, 37.2965, 126.9725, 37.2930, 126.9730, 37.2945, 126.9755, 37.295, 126.9745};
        //RouteMaker.drawRoute(routeToDraw);
        Debug.Log("GPSdropdown : "+dropdown2.options[dropdown2.value].text);

        // 가능할경우 받아오고, 없을 경우 exception 처리하여 길이 0 되도록
        double[] route;
        try{
            var routeTmp = m_JavaObject.Call<double[]>("getRoute");
            route = routeTmp;
        } catch (Exception e){
            double[] routeTmp = {};
            route = routeTmp;
            Debug.Log("ARGUIDE_gps : route : exception");
        }
        
        Debug.Log("ARGUIDE_gps : routelen : "+route.Length);
        Debug.Log("ARGUIDE_gps : findestin : "+finalDestination);
        Debug.Log("ARGUIDE_gps : route0 : "+route[0]);
        
        // 입력 없음 경로 없음 : 아무것도 하지 말기
        // 입력 있음 경로 40 이상 : 너무 먼 것 같으니 목적지만 표시
        // 입력 있음 경로 40 이하 : 경로 그리기
        // 0824SA : 테스팅 위해 경로 150이하일때 표시하는걸로 수정
        string dropdowntxt = dropdown2.options[dropdown2.value].text;

        if (route.Length < 1){
            if (dropdowntxt == "검색 결과가 없습니다. 다른 검색어로 다시 검색을 시도해주세요." || dropdowntxt == "검색어를 먼저 입력해주세요."){
                Debug.Log("ARGUIDE_gps : drawroute : no input");
            } else {
                Debug.Log("ARGUIDE_gps : drawroute : error");
            }
        } else if (route.Length >= 150){
            double lastlat = route[route.Length-2];
            double lastlon = route[route.Length-1];
            RouteMaker.drawDestin(lastlat, lastlon);

        } else {
            RouteMaker.drawRoute(route); // 실제 루트 그리기
        }

    }

    // 하단 안내 시작하기 버튼
    public void Get_Route()
    {
        // 목적지가 정해져있는지 확인
        Debug.Log("ARGUIDE_gps : getroute : check finaldestination");
        if ( finalDestination == ""){
            Debug.Log("ARGUIDE_gps : getroute : finaldestination does not exist");
            return;
        } else {
            Debug.Log("ARGUIDE_gps : getroute : finaldestination exist - start guide!!");
        }

        var routeTmp = m_JavaObject.Call<double[]>("getRoute");
        
        // route 전처리

        // route 길이 찾기
        int routeLen = 0;
        for (int i = 0 ; i < routeTmp.Length ; i++){
            Debug.Log("!!route!! : " + routeTmp[i]);
            routeLen = i;
            
        }
        route = new double[routeLen];

        // route 옮기기
        for (int i = 0 ; i < routeLen ; i++ ){
            route[i] = routeTmp[i];
        }

        Debug.Log("ARGUIDE_gps : getroute : success to get route");

    

        // 받은 경로 값도 옮김
        /*GPSText.text = "routeLen:"+routeLen;
        for (int i = 0; i < route.Length / 2; i++)
        {
            GPSText.text += "\n"+(i*2)+"lat: " + route[i * 2 + 0] + " lon: " + route[i * 2 + 1];
        }
        */
        

        // 카메라 화면으로 전환
        backgroundImage.enabled = false;
        findRouteBtn.enabled = false;
        dropdown2.enabled = false;

        backgroundImage.gameObject.SetActive(false);
        findRouteBtn.gameObject.SetActive(false);
        dropdown2.gameObject.SetActive(false);
        MapPart.gameObject.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);
        RawImage.SetActive(false);
        Inputobj.SetActive(false);

        didFoundRoute = true;

        Debug.Log("ARGUIDE_gps : getroute : camera on");
        /*
        if (didFoundRoute){
            //GPSText.text += "\ndidfoundroute !!";
        }
        */
        
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

        dropdown2.RefreshShownValue();

        /*
        double LAT = (float)locations[0];
        double LON = (float)locations[1];
        text.text = LAT.ToString() + LON.ToString();
        */
    }

    public void Erase_TapToStart(){
        TapToStart.gameObject.SetActive(false);
    }

    //언어 변수를 설정하는 메소드
    public string Language_Update() {
        var language = m_JavaObject.Call<string>("getLanguage");
        return language;
    }
}

