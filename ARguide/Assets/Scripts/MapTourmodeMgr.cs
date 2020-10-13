using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTourmodeMgr : MonoBehaviour
{
    //List<double> basic_tourRoute;
    double[] basic_tourRoute;
    /*
    public Transform tourRoute_point;
    public Transform tourRoute_line;
    public Transform tourPin;*/

    public GameObject tourRoute_point_obj;

    // Start is called before the first frame update
    void Start()
    {
        // find objects
        /*tourRoute_point = transform.Find("Routes/Route_point");
        tourRoute_line = transform.Find("Routes/Route_line");
        tourPin = transform.Find("Routes/Pin");*/

        /*basic_tourRoute.Add(37.296115);
        basic_tourRoute.Add(126.972257); */ // 신관앞

        double[] routeTmp = {37.296044, 126.972787, 37.296437, 126.975134, 37.296147, 126.976121,   // 신관~산학
                            37.294615, 126.975824, 37.292387, 126.975447, 37.292248, 126.976652,    // 산학~반도체
                            37.292335, 126.975654, 37.292444, 126.974457, 37.292625, 126.973005,    // 반도체~의학관
                            37.292623, 126.973001, 37.293087, 126.973087, 37.293945, 126.973224,    // 의학관~학생회관
                            37.295229, 126.973451, 37.295888, 126.972853 };                         // 학생회관~신관
        basic_tourRoute = routeTmp;

      
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    // 투어 경로 포인트들 중 현재 위치로부터 가장 가까운 거리에 있는 포인트 찾기
    // 루트 구성 : 현재위치 ---> 가장 가까웠던 포인트 ---> 시계방향으로 경로 포인트들 따라서 이동, 마지막 포인트까지
    // 구성한 루트 리턴, 위 작업 도중 실패할 경우 빈 루트 리턴

    public double[] create_Tourroute(){

        // 리턴용 경로 배열
        double[] route ;

        route = basic_tourRoute;
        // TODO : 현재위치랑 가장 가까운 곳 찾기
        // TODO : 현위치~가장가까운 곳의 경로 찾아서 기존경로 앞에 붙이기

        double nowLAT = GPSMgr.LAT;
        double nowLON = GPSMgr.LON;

        // 경로 포인트 중 현재 위치랑 가장 가까운 것 찾기
        int startpointIndex = 0;
        //Vector2 p1 = new Vector2((float)GPSMgr.LAT, (float)GPSMgr.LON);
        Vector2 p1 = new Vector2(37.294180f, 126.977964f); // 디버그용
        Vector2 p2;
        float minDist = 10;

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
