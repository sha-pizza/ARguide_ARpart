using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTourmodeMgr : MonoBehaviour
{
    MapMgr MapMgr;
    double[] basic_tourRoute;

    double[] gps_back = {37.296367, 126.970642};
    double[] gps_front = {37.290844, 126.974192};
    double[] gps_gate = {37.296291, 126.976485};


    double[] tourRoute_back = { // 후문 루트
                            37.296367, 126.970642, 37.296006, 126.972562,                           // 후문~신관
                            37.296044, 126.972787, 37.296437, 126.975134, 37.296147, 126.976121,    // 신관~산학
                            37.294615, 126.975824, 37.292387, 126.975447, 37.292248, 126.976652,    // 산학~반도체
                            37.292335, 126.975654, 37.292444, 126.974457, 37.292625, 126.973005,    // 반도체~의학관
                            37.292623, 126.973001, 37.293087, 126.973087, 37.293945, 126.973224,    // 의학관~학생회관
                            37.295229, 126.973451, 37.295888, 126.972853,                           // 학생회관~신관
                            37.295996, 126.972531, 37.296283, 126.970608                            // 신관~후문
    };
    double[] tourRoute_front = { // 정문 루트
                            37.290631, 126.974028, 37.292428, 126.974401,                           // 정문~농구장
                            37.292623, 126.973001, 37.293087, 126.973087, 37.293945, 126.973224,    // 의학관~학생회관
                            37.295229, 126.973451, 37.295888, 126.972853,                           // 학생회관~신관
                            37.296044, 126.972787, 37.296437, 126.975134, 37.296147, 126.976121,    // 신관~산학
                            37.294615, 126.975824, 37.292387, 126.975447, 37.292248, 126.976652,    // 산학~반도체
                            37.292335, 126.975654, 37.292444, 126.974457,                           // 반도체~농구장
                            37.292444, 126.974478, 37.290625, 126.974107                            // 농구장~정문
    };
    double[] tourRoute_gate = { // 교문 루트 (산학앞)
                            37.296261, 126.976526, 37.296009, 126.976113,                           // 교문~산학
                            37.294615, 126.975824, 37.292387, 126.975447, 37.292248, 126.976652,    // 산학~반도체
                            37.292335, 126.975654, 37.292444, 126.974457, 37.292625, 126.973005,    // 반도체~의학관
                            37.292623, 126.973001, 37.293087, 126.973087, 37.293945, 126.973224,    // 의학관~학생회관
                            37.295229, 126.973451, 37.295888, 126.972853,                           // 학생회관~신관
                            37.296044, 126.972787, 37.296437, 126.975134,                           // 신관~산학
                            37.296167, 126.976167, 37.296318, 126.976446                            // 산학~교문
    };

    // 세 리스트는 각각 시작 위치의 이름, gps, 해당하는 경로 정보를 가지고 있으며 길이가 같아야 함
    List<string> routeName_arr = new List<string>();
    List<double[]> gps_arr = new List<double[]>();
    List<double[]> tourRoute_arr = new List<double[]>();




    public GameObject tourRoute_point_obj;

    // Start is called before the first frame update
    void Start()
    {
        MapMgr = GameObject.Find("MapMgr").GetComponent<MapMgr>();
        double[] routeTmp = {
                            37.296044, 126.972787, 37.296437, 126.975134, 37.296147, 126.976121,    // 신관~산학
                            37.294615, 126.975824, 37.292387, 126.975447, 37.292248, 126.976652,    // 산학~반도체
                            37.292335, 126.975654, 37.292444, 126.974457, 37.292625, 126.973005,    // 반도체~의학관
                            37.292623, 126.973001, 37.293087, 126.973087, 37.293945, 126.973224,    // 의학관~학생회관
                            37.295229, 126.973451, 37.295888, 126.972853 };                         // 학생회관~신관

        routeName_arr.Add("후문");
        routeName_arr.Add("정문");
        routeName_arr.Add("교문");
        gps_arr.Add(gps_back);
        gps_arr.Add(gps_front);
        gps_arr.Add(gps_gate);
        tourRoute_arr.Add(tourRoute_back);
        tourRoute_arr.Add(tourRoute_front);
        tourRoute_arr.Add(tourRoute_gate);

        //basic_tourRoute = routeTmp;
    }

    // Update is called once per frame
    /*void Update()
    {
    }*/

    /* 이전 방식 */
    // 투어 경로 포인트들 중 현재 위치로부터 가장 가까운 거리에 있는 포인트 찾기
    // 루트 구성 : 현재위치 ---> 가장 가까웠던 포인트 ---> 시계방향으로 경로 포인트들 따라서 이동, 마지막 포인트까지
    // 구성한 루트 리턴, 위 작업 도중 실패할 경우 빈 루트 리턴

    /* 수정한 방식 */
    // 정문, 후문, 교문 중 현재 위치로부터 가장 가까운 거리에 있는 포인트 찾기
    // 가까운 포인트에 따라 정문루트, 후문루트, 교문루트중 하나 리턴

    public double[] create_Tourroute(){

        // 리턴용 경로 배열
        double[] route;

        route = basic_tourRoute;
        
        double nowLAT = GPSMgr.LAT;
        double nowLON = GPSMgr.LON;

        Vector2 p1 = new Vector2((float)GPSMgr.LAT, (float)GPSMgr.LON);
        //Vector2 p1 = new Vector2(37.295889f, 126.976873f); // 디버그용
        Vector2 p2;
        float minDist = 1000;
        string routeName = "";

        /* 수정한 방식 */

        for (int i=0 ; i<gps_arr.Count ; i++){
            p2 = new Vector2((float)gps_arr[i][0], (float)gps_arr[i][1]);
            float pDist = Vector2.Distance(p1, p2);

            if (pDist < minDist){
                minDist = pDist;
                route = tourRoute_arr[i];
                routeName = routeName_arr[i];
                Debug.Log("ARGUIDE_MapTourmode : set Tourroute : "+routeName);
            }
        }

        string message = "가장 가까운 "+routeName+" 루트로 투어모드를 진행합니다.";
        StartCoroutine(MapMgr.uiAlert(message, 3f, 1));
        

        /* 이전 방식 */
        /*
        // 경로 포인트 중 현재 위치랑 가장 가까운 것 찾기
        int startpointIndex = 0;


        for (int i=0 ; i<route.Length ; i=i+2){
            p2 = new Vector2((float)route[i], (float)route[i+1]);
            float thisDist = Vector2.Distance(p1, p2);
            Debug.Log("ARGUIDE_MapTourmode : "+i+"-> this : "+thisDist+" / min : "+minDist);

            if (thisDist < minDist){
                startpointIndex = i;
                minDist = thisDist;
            }
        }
        Debug.Log("ARGUIDE_MapTourmode : startIndex : "+startpointIndex);
        Debug.Log("ARGUIDE_MapTourmode : startPoint : "+route[startpointIndex]+","+route[startpointIndex+1]);

        string tester = "";
        double[] tmproute = new double[route.Length];

        // 경로 재배열
        for (int i=0 ; i<route.Length ; i++){
            if (i < route.Length - startpointIndex){
                tmproute[i] = basic_tourRoute[i+startpointIndex];
                tester += ">>"+route[i];
            } else {
                tmproute[i] = basic_tourRoute[i+startpointIndex-route.Length];
                tester += ">"+route[i];
            }
        }
        route = tmproute;

        Debug.Log("ARGUIDE_MapTourmode : route : "+tester);
        */

    

        return route;
    }

  

    public void draw_Tour_building(Transform parent){
        MapMgr.drawPin(37.296335, 126.972532, parent, tourRoute_point_obj, "신관" );
        MapMgr.drawPin(37.295935, 126.974142, parent, tourRoute_point_obj, "생명공학관" );
        MapMgr.drawPin(37.296885, 126.974664, parent, tourRoute_point_obj, "의관" );
        MapMgr.drawPin(37.296563, 126.975503, parent, tourRoute_point_obj, "예관" );
        MapMgr.drawPin(37.295963, 126.975765, parent, tourRoute_point_obj, "산학협력센터" );

        MapMgr.drawPin(37.295079, 126.976923, parent, tourRoute_point_obj, "제2공학관" );
        MapMgr.drawPin(37.293916, 126.976664, parent, tourRoute_point_obj, "제1공학관" );
        MapMgr.drawPin(37.293924, 126.974925, parent, tourRoute_point_obj, "삼성학술정보관" );
        MapMgr.drawPin(37.292909, 126975242, parent, tourRoute_point_obj, "잔디밭" );

        MapMgr.drawPin(37.291575, 126.977559, parent, tourRoute_point_obj, "반도체관" );
        MapMgr.drawPin(37.291490, 126.976712, parent, tourRoute_point_obj, "화학관" );
        MapMgr.drawPin(37.292119, 126.975521, parent, tourRoute_point_obj, "N센터" );
        MapMgr.drawPin(37.292168, 126.973456, parent, tourRoute_point_obj, "의학관" );

        MapMgr.drawPin(37.293118, 126.972714, parent, tourRoute_point_obj, "수성관" );
        MapMgr.drawPin(37.294021, 126.973566, parent, tourRoute_point_obj, "학생회관" );
        MapMgr.drawPin(37.293943, 126.972534, parent, tourRoute_point_obj, "복지회관" );
        MapMgr.drawPin(37.295323, 126.973805, parent, tourRoute_point_obj, "기초학문관" );
      
    }




}
