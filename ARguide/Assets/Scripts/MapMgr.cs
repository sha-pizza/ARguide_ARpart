using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMgr : MonoBehaviour
{
    [Header("배경 지도위의 Pin 오브젝트, Route오브젝트와 포함된 point, line 오브젝트 스크립트")]
    public float nothing;

    /*
    ** map ui 1 : map_show_buildings
    ** map ui 2 : map_searched_entrance
    ** map ui 3 : map_show_route
    ** 1->2 검색창 또는 빌딩 버튼 눌러 입구 검색 
    ** 2->3 검색된 입구 중 하나를 선택하면 경로 띄워줌
    ** 2->1 3->1 뒤로가기 버튼
    */

    // 자바 인스턴스
    static public AndroidJavaObject m_JavaObject;

    // stateMgr에서 받아오는 state
    StateMgr.state arguide_state;
    // state가 intro->map으로 전환될때 1회 수정
    bool isChangedToMapState;

    // 지도 이미지 관련 값
    MapZoomMgr uiMapZoomMgr;
    public static float mapWidth;
    public static float mapHeight;

    // 투어모드 관련
    MapTourmodeMgr uiMapTouremodeMgr;


    [Header("Pins")]
    public Transform Pin_building;
    public Transform Pin_entrance; 
    public GameObject Pin_building_obj;
    public GameObject Pin_entrance_obj;

    [Header("Routes")]
    public Transform Route_point;
    public Transform Route_line;

    [Header("입구 아이콘")]
    public Sprite entrance_normal;
    public Sprite entrance_selected;
    public GameObject entrance_loading;

    [Header("경로 그리기 프리팹")]
    public GameObject rStart_obj;
    public GameObject rMid_obj;
    public GameObject rLine_obj;



    // ui 받아올 오브젝트
    InputField SearchInput;
    Button SearchBtn;
    public Button BackBtn;
    
    public Button TourmodeBtn;
    RectTransform TourmodeBtnRect;

    Button StartARBtn;
    RectTransform StartARBtnRect;

    Button StartARtourBtn;
    RectTransform StartARtourBtnRect;

    RectTransform AlertRect;
    Text AlertText;


    // 검색 결과 리스트
    List<SearchResult> resultlist = new List<SearchResult>();
    // 경로 검색 중인지 표시
    bool isSearchingRoute = false;
    // 경로 저장용
    public static double[] route;
    public static string finalDestination;

    // 하단 버튼 위치
    Vector2 btn_inactivePos = new Vector2(0, -400);
    Vector2 btn_activePos = new Vector2(0, 0);


    
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ARGUIDE_Map : start()"); 
        // find objects
        Pin_building = GameObject.Find("UICanvas/ui_Map/elem_Map/Pins/Pin_building").transform;
        Pin_entrance = GameObject.Find("UICanvas/ui_Map/elem_Map/Pins/Pin_entrance").transform;
        Route_point = GameObject.Find("UICanvas/ui_Map/elem_Map/Routes/Route_point").transform;
        Route_line = GameObject.Find("UICanvas/ui_Map/elem_Map/Routes/Route_line").transform;

        SearchInput = GameObject.Find("UICanvas/ui_Map/elem_Search/SearchInput").GetComponent<InputField>();
        SearchBtn = GameObject.Find("UICanvas/ui_Map/elem_Search/SearchBtn").GetComponent<Button>();
        BackBtn = GameObject.Find("UICanvas/ui_Map/elem_Search/BackBtn").GetComponent<Button>();    // 시작 시 active false

        TourmodeBtn = GameObject.Find("UICanvas/ui_Map/elem_Tourmode").GetComponent<Button>();            
        TourmodeBtnRect = GameObject.Find("UICanvas/ui_Map/elem_Tourmode/Button").GetComponent<RectTransform>();

        StartARtourBtn = GameObject.Find("UICanvas/ui_Map/elem_StartARtour").GetComponent<Button>();// 시작 시 active false
        StartARtourBtnRect = GameObject.Find("UICanvas/ui_Map/elem_StartARtour/Button").GetComponent<RectTransform>();

        StartARBtn = GameObject.Find("UICanvas/ui_Map/elem_StartAR").GetComponent<Button>();        // 시작 시 active false
        StartARBtnRect = GameObject.Find("UICanvas/ui_Map/elem_StartAR/Button").GetComponent<RectTransform>();
        
        AlertRect = GameObject.Find("UICanvas/ui_Map/elem_Alert").GetComponent<RectTransform>();
        AlertText = AlertRect.transform.Find("Text").GetComponent<Text>();
        


        // Mgr 가져오기
        uiMapZoomMgr = GameObject.Find("UICanvas/ui_Map/elem_Map").GetComponent<MapZoomMgr>();
        uiMapTouremodeMgr = GameObject.Find("UICanvas/ui_Map/elem_Tourmode").GetComponent<MapTourmodeMgr>();

        // androidjavaobject 가져오기
        var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        try {
            m_JavaObject = jc.GetStatic<AndroidJavaObject>("currentActivity");
        } catch {
            Debug.Log("ARGUIDE_Map : fail get javaobject"); 
        }
        

        // Pin Route 초기화
        foreach (Transform child in Pin_building) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Pin_entrance) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Route_point) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Route_line) {
            Destroy(child.gameObject);
        }

        // 초기 지도 사이즈에 맞추어 Pins, Routes의 Rect크기 조절      
        mapWidth = GameObject.Find("UICanvas/ui_Map/elem_Map").GetComponent<RectTransform>().rect.height*1800/2648;
        mapHeight = GameObject.Find("UICanvas/ui_Map/elem_Map").GetComponent<RectTransform>().rect.height;

        isChangedToMapState = false;

        // onclick 설정
        SearchBtn.onClick.AddListener(TaskOnClick_SearchBtn);
        BackBtn.onClick.AddListener(TaskOnClick_BackBtn);
        TourmodeBtn.onClick.AddListener(TaskOnClick_TourmodeBtn);
        StartARtourBtn.onClick.AddListener(TackOnClick_StartARtourBtn);
        StartARBtn.onClick.AddListener(TaskOnClick_StartARBtn);

        // 사전 세팅
        AlertRect.gameObject.SetActive(false);
        BackBtn.gameObject.SetActive(false);
        StartARtourBtnRect.localPosition = btn_inactivePos;   //StartARtourBtn.gameObject.SetActive(false);  
        StartARBtnRect.localPosition = btn_inactivePos;   //StartARBtn.gameObject.SetActive(false);  



    }

    // Update is called once per frame
    void Update()
    {
        // state가 MAP으로 전환되었을 때 빌딩 핀 표시하고 onclick 설정
        // TourmodeBtn이 등장하도록 설정
        arguide_state = StateMgr.getState();
        if (!isChangedToMapState && arguide_state == StateMgr.state.MAP){
            
            StartCoroutine ( uiLerp(TourmodeBtnRect, btn_inactivePos, btn_activePos, 0.15f, 1f) );
            draw_Pin_building();
            isChangedToMapState = true;
        }     
    }

    /*
    ** map ui 1 : map_show_buildings
    */

    // 빌딩 핀 표시하고 onclick 설정
    public void draw_Pin_building(){
        Debug.Log("ARGUIDE_Map : drawy builing pins"); 

        GameObject newPin01 = drawPin(37.296335, 126.972532, Pin_building, Pin_building_obj, "신관" );
        GameObject newPin02 = drawPin(37.296897, 126.973854, Pin_building, Pin_building_obj, "인관" );
        GameObject newPin03 = drawPin(37.296885, 126.974664, Pin_building, Pin_building_obj, "의관" );
        GameObject newPin04 = drawPin(37.296563, 126.975503, Pin_building, Pin_building_obj, "예관" );
        //GameObject newPin05 = drawPin(37.296350, 126.977606, Pin_building, Pin_building_obj, "지관" );

        GameObject newPin06 = drawPin(37.295963, 126.975765, Pin_building, Pin_building_obj, "산학협력센터" );
        GameObject newPin07 = drawPin(37.295935, 126.974142, Pin_building, Pin_building_obj, "생명공학관" );
        GameObject newPin08 = drawPin(37.295533, 126.974588, Pin_building, Pin_building_obj, "기초학문관" );
        GameObject newPin09 = drawPin(37.295069, 126.975018, Pin_building, Pin_building_obj, "제2과학관" );
        GameObject newPin10 = drawPin(37.294539, 126.975245, Pin_building, Pin_building_obj, "제1과학관" );
        GameObject newPin11 = drawPin(37.295079, 126.976923, Pin_building, Pin_building_obj, "제2공학관" );
        GameObject newPin12 = drawPin(37.293916, 126.976664, Pin_building, Pin_building_obj, "제1공학관" );

        GameObject newPin13 = drawPin(37.293943, 126.972534, Pin_building, Pin_building_obj, "복지회관" );
        GameObject newPin14 = drawPin(37.294021, 126.973566, Pin_building, Pin_building_obj, "학생회관" );
        GameObject newPin15 = drawPin(37.293924, 126.974925, Pin_building, Pin_building_obj, "삼성학술정보관" );

        GameObject newPin16 = drawPin(37.291575, 126.977559, Pin_building, Pin_building_obj, "반도체관" );
        GameObject newPin17 = drawPin(37.291490, 126.976712, Pin_building, Pin_building_obj, "화학관" );
        GameObject newPin18 = drawPin(37.291805, 126.976359, Pin_building, Pin_building_obj, "약학관" );
        GameObject newPin19 = drawPin(37.292168, 126.973456, Pin_building, Pin_building_obj, "의학관" );

        GameObject newPin20 = drawPin(37.296310, 126.970578, Pin_building, Pin_building_obj, "후문" );
        GameObject newPin21 = drawPin(37.296291, 126.976485, Pin_building, Pin_building_obj, "교문" );
        GameObject newPin22 = drawPin(37.290751, 126.974085, Pin_building, Pin_building_obj, "정문" );

        //drawPin(37.29, 126.97, Pin_building_obj, "" );

        setPin_building( newPin01 ); 
        setPin_building( newPin02 ); 
        setPin_building( newPin03 ); 
        setPin_building( newPin04 ); 
        //setPin_building( newPin05 ); 
        setPin_building( newPin06 ); 
        setPin_building( newPin07 ); 
        setPin_building( newPin08 ); 
        setPin_building( newPin09 ); 
        setPin_building( newPin10 ); 
        setPin_building( newPin11 ); 
        setPin_building( newPin12 ); 
        setPin_building( newPin13 ); 
        setPin_building( newPin14 ); 
        setPin_building( newPin15 ); 
        setPin_building( newPin16 ); 
        setPin_building( newPin17 ); 
        setPin_building( newPin18 ); 
        setPin_building( newPin19 ); 
        setPin_building( newPin20 ); 
        setPin_building( newPin21 ); 
        setPin_building( newPin22 );         

    }
    

    // 개별 핀을 그리고, 이름을 붙여줌
    // 이름이 없는 핀의 경우 이름 없이 그려짐 (catch)
    public static GameObject drawPin(double LAT, double LON, Transform Pinparent, GameObject Pin_obj, string Pinname){
        double calculedLAT = (LAT-37.2945)*(mapHeight/21)*1000;
        double calculedLON = (LON-126.974)*(mapWidth/18)*1000;
        //Debug.Log("ARGUIDE_Map : draw pin  : "+(float)calculedLAT+","+(float)calculedLON); 

        GameObject newPin = Instantiate(Pin_obj, new Vector3(0,0,0), Quaternion.identity);
        newPin.transform.SetParent(Pinparent);
        newPin.transform.localPosition = new Vector3((float)calculedLON, (float)calculedLAT, 0);

        try {
            newPin.transform.GetChild(0).GetComponent<Text>().text = Pinname;
            //Debug.Log("ARGUIDE_Map : draw pin ! Pinname is : "+Pinname+", "+(float)calculedLON+", "+(float)calculedLAT);
        } catch {
            //Debug.Log("ARGUIDE_Map : draw pin without Pinname : "+Pinname); 
        }

        return newPin;
    }

    // 빌딩 핀에 온클릭 설정, 누를 시 건물이름으로 searchDestin 실행되도록 함
    public GameObject setPin_building(GameObject Pin){
        Button Pin_btn = Pin.GetComponent<Button>();
        string Pin_name = Pin.transform.GetChild(0).GetComponent<Text>().text;
        Pin_btn.onClick.AddListener(delegate {TaskOnClick_Pin_building(Pin_name);});
        //Debug.Log("ARGUIDE_Map : set pin onclick ! Pinname is : "+Pin_name); 
        return Pin;

    }

    // onClick methods
    void TaskOnClick_Pin_building(string destin){
        Debug.Log("ARGUIDE_Map : search by click : "+destin); 
        resultlist.Clear();
        resultlist = searchEntrance(destin);
        if (resultlist.Count != 0 && resultlist[0].lat!=0 && resultlist[0].lon!=0){
            drawSearchedEntrance(resultlist);           // 검색된 입구 그리기
            SearchInput.text = destin;                  // 검색창 문구 수정
            Pin_building.gameObject.SetActive(false);   // 빌딩들 끄기

            StartCoroutine ( uiLerp(TourmodeBtnRect, btn_activePos, btn_inactivePos, 0.15f, 1f) ); //TourmodeBtn.gameObject.SetActive(false);  // 투어모드 버튼 숨기기
            BackBtn_SetActive(true);                    // 뒤로가기버튼 활성화 및 ui 정리
            moveToSearchedEntrance();                   // 검색들 결과 위치로 지도 이동
        } else {
            SearchInput.text = "검색 결과가 없습니다.";
        }
    }
    void TaskOnClick_SearchBtn(){
        string destin = SearchInput.text;
        Debug.Log("ARGUIDE_Map : search by text : "+destin); 
        resultlist.Clear();                             // 검색결과 리스트 초기화
        resultlist = searchEntrance(destin);            // 검색결과 받아오기
        if (resultlist.Count != 0 && resultlist[0].lat!=0 && resultlist[0].lon!=0){
            drawSearchedEntrance(resultlist);           // 검색된 입구들 그리기
            Pin_building.gameObject.SetActive(false);   // 빌딩들 끄기
            
            StartCoroutine ( uiLerp(TourmodeBtnRect, btn_activePos, btn_inactivePos, 0.15f, 1f) ); //TourmodeBtn.gameObject.SetActive(false);  // 투어모드 버튼 숨기기
            BackBtn_SetActive(true);                    // 뒤로가기버튼 활성화 및 ui 정리
            moveToSearchedEntrance();                   // 검색들 결과 위치로 지도 이동
        } else {
            SearchInput.text = "검색 결과가 없습니다.";
        }
    }

    void TaskOnClick_BackBtn(){
        // map ui 1 : map_show_buildings의 초기 상태로 되돌림
        Debug.Log("ARGUIDE_Map : backbtn"); 
        resultlist.Clear();

        BackBtn_SetActive(false);
        SearchInput.text = "";
        Pin_building.gameObject.SetActive(true);
        StartCoroutine ( uiLerp(TourmodeBtnRect, btn_inactivePos, btn_activePos, 0.15f, 1f) ); //TourmodeBtn.gameObject.SetActive(true);
        StartARBtnRect.localPosition = btn_inactivePos;   //StartARBtn.gameObject.SetActive(false);  
        StartARtourBtnRect.localPosition = btn_inactivePos; 

        // Pin Route 초기화
        foreach (Transform child in Pin_entrance) {     Destroy(child.gameObject);  }
        foreach (Transform child in Route_point) {      Destroy(child.gameObject);  }
        foreach (Transform child in Route_line) {       Destroy(child.gameObject);  }
    }

    void TaskOnClick_TourmodeBtn(){
        Debug.Log("ARGUIDE_Map : ToumodeBtn clicked"); 
        
        // 빌딩 끄기, 뒤로가기버튼 활성화, 검색창 '투어 모드', 안내시작버튼 활성화
        BackBtn_SetActive(true);
        SearchInput.text = "투어 모드";
        Pin_building.gameObject.SetActive(false);

        StartCoroutine ( uiLerp(TourmodeBtnRect, btn_activePos, btn_inactivePos, 0.15f, 1f) );
        StartCoroutine ( uiLerp(StartARtourBtnRect, btn_inactivePos, btn_activePos, 0.15f, 1f) );
        
        //StartARBtn.gameObject.SetActive(true);

        // 투어모드 경로 포인트들 중 가장 가까운 것을 찾아 전체 경로 만들기
        double[] tourRoute = uiMapTouremodeMgr.create_Tourroute();

        // 경로가 성공적으로 구성되었을 경우 지도에 해당 루트 표시
        if (tourRoute.Length != 0){
            // 루트 초기화
            foreach (Transform child in Route_point) {      Destroy(child.gameObject);  }
            foreach (Transform child in Route_line) {       Destroy(child.gameObject);  }
            // 투어에서 지나치는 빌딩그리기
            uiMapTouremodeMgr.draw_Tour_building(Pin_entrance);
            drawRoute(tourRoute, Route_point, Route_line);
            route = tourRoute; // 경로 연결

        } else {
            Debug.Log("ARGUIDE_Map : ToumodeBtn : fail to get route");
            SearchInput.text = "투어 모드 로드에 실패하였습니다.";
            return; 
        }

    }

    void TackOnClick_StartARtourBtn(){
        Debug.Log("ARGUIDE_Map : StartARtourBtn clicked"); 

        // TODO : 시작점에 가까운지 확인하고, 가깝지 않은 경우 리턴 / 가까울 경우 현재위치 경로에 추가후 시작

        double nowLAT = GPSMgr.LAT;
        double nowLON = GPSMgr.LON;
        /*
        if (route.Length != 0){
            StateMgr.requestStateChange(StateMgr.state.MAP, StateMgr.state.GUIDE);
            Debug.Log("ARGUIDE_Map : start AR :  "); 
        } else {
            Debug.Log("ARGUIDE_Map : cant start AR : routelen is 0 "); 
        }*/

        if (route.Length == 0){
            Debug.Log("ARGUIDE_Map : StartARtourBtn : cant start tourmode : routelen is 0 "); 
            return;
        } else if (Mathf.Abs((float)nowLAT-(float)route[0])> 0.0001f || Mathf.Abs((float)nowLON-(float)route[1])> 0.0001f){
            Debug.Log("ARGUIDE_Map : StartARtourBtn : cant start tourmode : too far from startpoint");
            StartCoroutine(uiAlert("시작 지점과 너무 멀리 떨어져 있습니다.\n시작 지점에 가까이 이동한 후 다시 시도해 주세요.", 3f, 2)); 
            return;
        }

        // 경로에 현재 위치 추가
        double[] routeedit = new double[route.Length+2];
        routeedit[0] = nowLAT;
        routeedit[1] = nowLON;
        for (int i=2 ; i<routeedit.Length ; i++){
            routeedit[i] = route[i-2];
        }
        route = routeedit;
        Debug.Log("ARGUIDE_Map : StartARtourBtn : route :"+route[0]+", "+route[1]+", "+route[2]+", "+route[3]); 
        

        StateMgr.requestStateChange(StateMgr.state.MAP, StateMgr.state.GUIDE);
        Debug.Log("ARGUIDE_Map : start AR tour :  "); 

    }

    // 뒤로가기버튼을 활성화하고 inputfield 위치를 오른쪽으로 밀어 정리 / 뒤로가기버튼을 비활성화하고 inputfield 원위치로
    void BackBtn_SetActive(bool B){
        RectTransform RT = SearchInput.gameObject.GetComponent<RectTransform>();
        if (B){
            // 활성화
            BackBtn.gameObject.SetActive(true);
            RT.offsetMin = new Vector2 (130, RT.offsetMin.y);
        } else {
            // 비활성화
            BackBtn.gameObject.SetActive(false);
            RT.offsetMin = new Vector2 (50, RT.offsetMin.y);
        }
    }

    /*
    ** map ui 2 : map_searched_entrance
    */

    // 해당 이름으로 목적지 검색하기
    public List<SearchResult> searchEntrance(string destin){
        // 결과 리스트
        List<SearchResult> result = new List<SearchResult>();

        // 이름 검색하기
        m_JavaObject.Call("setDestination", destin);

        // 검색한 이름 결과 받아오기
        var locations = m_JavaObject.Call<string[]>("getLocationsName");
        var locations_lat = m_JavaObject.Call<double[]>("getLocationsLat");
        var locations_lon = m_JavaObject.Call<double[]>("getLocationsLog");
        Debug.Log("ARGUIDE_Map : loc/lat/lon size: " + locations.Length+", "+locations_lat.Length+", "+locations_lon.Length);

        // searchresult 오브젝트 리스트로 저장
        if (locations.Length == locations_lat.Length && locations.Length == locations_lon.Length && locations.Length!=0){
            for (int i=0 ; i< locations.Length ; i++){
                SearchResult newResult = new SearchResult();
                newResult.name = locations[i];
                newResult.lat = locations_lat[i];
                newResult.lon = locations_lon[i];
                newResult.index = i;
                result.Add(newResult);
                //Debug.Log("ARGUIDE_Map : "+locations[i]+", "+locations_lat[i]+", "+locations_lon[i]);
            }
        } else if (locations.Length == 0){
            Debug.Log("ARGUIDE_Map : search - no result !");
        } else {
            Debug.Log("ARGUIDE_Map : search - error");
        }
        return result;
    }

    void drawSearchedEntrance(List<SearchResult> result){
        Debug.Log("ARGUIDE_Map : draw "+result.Count+" searched results : ");
        for (int i = 0; i<result.Count ; i++){
            GameObject newPin = drawPin(result[i].lat, result[i].lon, Pin_entrance, Pin_entrance_obj, result[i].name);
            setPin_entrance(newPin);
            //Debug.Log("ARGUIDE_Map : draw"+result[i].name+" - "+result[i].lat);
        }
    }

    // 검색된 결과 근처로 화면을 이동시키고 확대하여 사용자가 검색된 결과에 주목할 수 있도록 함
    // resultlist 기준으로 이동
    void moveToSearchedEntrance(){
        if (resultlist.Count == 0 || Pin_entrance.childCount == 0){
            Debug.Log("ARGUIDE_Map : moveToSearchedResult : doesnt have any result! return");
            return;
        }

        double minLAT = resultlist[0].lat , maxLAT = resultlist[0].lat;
        double minLON = resultlist[0].lon , maxLON = resultlist[0].lon;

        for (int i=0 ; i<resultlist.Count ; i++){
            if (resultlist[i].lat < minLAT){
                minLAT = resultlist[i].lat;
            }
            if (resultlist[i].lat > maxLAT){
                maxLAT = resultlist[i].lat;
            }
            if (resultlist[i].lon < minLON){
                minLON = resultlist[i].lon;
            }
            if (resultlist[i].lon > maxLON){
                maxLON = resultlist[i].lon;
            }
        }

        Debug.Log("ARGUIDE_Map : moveToSearchResult : LAT : "+minLAT+"~"+maxLAT+" / LON : "+minLON+"~"+maxLON);

        IEnumerator zoomToTarget = uiMapZoomMgr.zoomToTarget( (minLAT+maxLAT)/2, (minLON+maxLON)/2, 3 );
        StartCoroutine( zoomToTarget );

    }



    
    /*
    ** map ui 3 : map_show_route
    */

    // 입구 핀에 온클릭 설정, 누를 시 입구이름으로 TaskOnClick 실행되도록 함
    public GameObject setPin_entrance(GameObject Pin){
        Button Pin_btn = Pin.GetComponent<Button>();
        string Pin_name = Pin.transform.GetChild(0).GetComponent<Text>().text;
        
        Pin_btn.onClick.AddListener(delegate {TaskOnClick_Pin_entrance(Pin_name);});
        //Debug.Log("ARGUIDE_Map : set pin onclick ! Pinname is : "+Pin_name); 
        return Pin;

    }

    // 입구 핀 눌렀을 때 작동
    void TaskOnClick_Pin_entrance(string Pin_name){
        Debug.Log("ARGUIDE_Map : click entrance : "+Pin_name); 

        // 하나 루트 찾을동안 또 진입하지 못하도록 막음
        if (isSearchingRoute){
            Debug.Log("ARGUIDE_Map : onclick entrance : is now searching other route"); 
            return;
        } 

        // 핀 이름으로 인덱스 찾기
        // 선택한 입구 강조, 다른 입구들은 현재 사이즈 유지
        int Pin_Index = -1;
        Transform Pin_Obj = Pin_entrance ;

        for (int i=0 ; i<Pin_entrance.childCount ; i++) {
            Transform child = Pin_entrance.GetChild(i);
            if (child.transform.GetChild(0).GetComponent<Text>().text == Pin_name){
                child.GetChild(1).GetComponent<Image>().sprite = entrance_selected;
                Pin_Index = i;
                Pin_Obj = child;
            } else {
                child.GetChild(1).GetComponent<Image>().sprite = entrance_normal;
            }
        }

        if (Pin_Index != -1){
            Debug.Log("ARGUIDE_Map : onclick entrance : find index of selected entrance : "+Pin_Index); 
        } else {
            Debug.Log("ARGUIDE_Map : onclick entrance : cant find index of selected entrance : "+Pin_Index); 
            return;
        }   

        // finaldestination 설정  
        finalDestination = Pin_Obj.transform.GetChild(0).GetComponent<Text>().text;

        // 경로 요청후 3초 대기, 경로 가져와서 보여주기
        // 안내 시작하기 버튼 활성화
        IEnumerator getroute = Get_Route(Pin_Obj, Pin_Index);
        StartCoroutine(getroute);
    }

    // 경로 요청 및 가져온 경로 그리기
    IEnumerator Get_Route (Transform Pin_Obj, int Pin_Index){
        isSearchingRoute = true;
        // 경로 요청
        try{
            m_JavaObject.Call("findRoute", Pin_Index);
        } catch {
            Debug.Log("ARGUIDE_Map : onclick entrance : findRoute : exception");
        }
        
        // 기존 아이콘 잠시 끄고 로딩아이콘 추가        
        GameObject loadingobj = Instantiate(entrance_loading, new Vector3(0,0,0), Quaternion.identity);
        loadingobj.transform.SetParent(Pin_Obj);
        loadingobj.transform.localPosition = new Vector3(0, 20, 0);
        Pin_Obj.GetChild(1).gameObject.SetActive(false);

        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 기존 아이콘 다시 켜고 로딩아이콘 삭제
        Pin_Obj.GetChild(1).gameObject.SetActive(true);
        Destroy(loadingobj);

        // 경로 받아오기
        try{
            var routeTmp = m_JavaObject.Call<double[]>("getRoute");
            route = routeTmp;
        } catch {
            double[] routeTmp = {};
            route = routeTmp;
            Debug.Log("ARGUIDE_Map : getRoute : exception");
        }

        
        Debug.Log("ARGUIDE_Map : findestin : "+Pin_Obj.GetChild(0).GetComponent<Text>().text+" / routelen : "+route.Length);
        Debug.Log("ARGUIDE_Map : route_start : "+route[0]+","+route[1]+" / route_end : "+route[route.Length-2]+","+route[route.Length-1]);

        // 경로 초기화
        // route line, point 의 모든 자식 삭제
        foreach (Transform child in Route_line) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Route_point) {
            Destroy(child.gameObject);
        }
        // 경로 그리기
        drawRoute(route, Route_point, Route_line);
       

        // 경로가 있을 경우
        // 안내 시작 버튼 활성화 및 기타 값 설정
        StartCoroutine ( uiLerp(StartARBtnRect, btn_inactivePos, btn_activePos, 0.15f, 1f) ); //StartARBtn.gameObject.SetActive(true);      

        isSearchingRoute = false;        
    }

    // 경로 그리기
    public void drawRoute(double[] route, Transform pointParent, Transform lineParent){
        // 처음부터 마지막 포인트까지 그리기
        for (int i=0 ; i<route.Length ; i=i+2) { 
            if (i==0){
                // 경로의 첫 번째 노드
                drawPin(route[i], route[i+1], Route_point, rStart_obj, "");
            } else {
                // 경로의 중간~마지막 노드
                drawPin(route[i], route[i+1], Route_point, rMid_obj, "");
            }  
        }

        // 받은 좌표 배열 연산
        double[] calculed_route = new double[route.Length]; // 조건에 따라 연산된 루트
        for (int i=0 ; i<route.Length ; i=i+2){
            calculed_route[i] = (route[i] - 37.2945) * (mapHeight/21) * 1000;   // 조건
            calculed_route[i+1] = (route[i+1] - 126.974) * (mapWidth/18) * 1000;
        }
        //Debug.Log("ARGUIDE_Map : calculed route : "+calculed_route.ToString());
        //Debug.Log("ARGUIDE_Map : original route : "+route.ToString());

        // 라인 그리기
        for (int i=0 ; i<route.Length-2 ; i=i+2) { 
            // startPoint 부터 endPoint까지 라인 그리기
            Vector3 s_point = new Vector3((float)calculed_route[i+1], (float)calculed_route[i], 0);
            Vector3 e_point = new Vector3((float)calculed_route[i+3], (float)calculed_route[i+2], 0);
            // 개별 라인 생성
            GameObject newline = Instantiate(rLine_obj, new Vector3(0,0,0), Quaternion.identity);
            newline.transform.parent = Route_line.transform;
            RectTransform lineRT = newline.GetComponent<RectTransform>();
            // 라인 위치를 startPoint위치로 설정
            lineRT.localPosition = s_point;
            lineRT.localScale = new Vector3(1,1,1);
            // 라인 길이 설정
            float distance = Vector3.Distance(s_point, e_point);
            lineRT.sizeDelta = new Vector2(lineRT.sizeDelta.x, distance);
            // 라인 방향설정
            float angle = Mathf.Atan2(e_point.y-s_point.y , e_point.x-s_point.x) * Mathf.Rad2Deg;
            lineRT.localRotation = Quaternion.Euler(0,0,angle);
            
        }

    }



    void TaskOnClick_StartARBtn(){
        Debug.Log("ARGUIDE_Map : start AR btn clicked : "); 

        if (route.Length != 0){
            StateMgr.requestStateChange(StateMgr.state.MAP, StateMgr.state.GUIDE);
            Debug.Log("ARGUIDE_Map : start AR :  "); 
        } else {
            Debug.Log("ARGUIDE_Map : cant start AR : routelen is 0 "); 
        }
    

    }

    
    

    public class SearchResult{
        public string name;
        public double lat;
        public double lon;
        public int index;
    }

    // functions !


    // ui 이동
    IEnumerator uiLerp(RectTransform rect, Vector2 startpos, Vector2 endpos, float lerpvalue, float duration){
        int movecount = (int)(duration/0.03f);
        //Debug.Log("ARGUIDE_Map : moveCount : "+movecount);


        rect.localPosition = startpos;
        for (int i=0 ; i<movecount ; i++){
            yield return new WaitForSeconds(0.03f);
            rect.localPosition = Vector2.Lerp(rect.localPosition, endpos, lerpvalue);
            //Debug.Log("ARGUIDE_Map : moveOn : "+rect.localPosition.y);
        }
        rect.localPosition = endpos;
    }
    

    // ui 안내 alert
    public IEnumerator uiAlert(string message, float duration, int linenum){
        Debug.Log("ARGUIDE_Map : ui alert : "+message);

        // 텍스트 세팅
        AlertText.text = message;

        // 사이즈 세팅
        float alertWidth = AlertText.preferredWidth + 100;
        float alertHeight = 60 + linenum*60;
        Debug.Log("ARGUIDE_Map : ui alert : alertw&h : "+alertWidth+", "+alertHeight );
        AlertRect.sizeDelta = new Vector2(alertWidth, alertHeight); 

        // duration 만큼 보여주고 사라지기
        AlertRect.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        AlertRect.gameObject.SetActive(false);
        
    }


}
