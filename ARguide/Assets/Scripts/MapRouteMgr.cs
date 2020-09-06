using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRouteMgr : MonoBehaviour
{
    [Header("경로의 각 포인트들")]
    public Transform routePoint;
    public RectTransform routePointRect;

    [Header("경로의 각 라인들")]
    public Transform routeLine;
    public RectTransform routeLineRect;

    [Header("경로 그리기 프리팹")]
    public GameObject rStart_obj;
    public GameObject rMid_obj;
    public GameObject rEnd_obj;
    public GameObject rLine_obj;
    public GameObject rEnd_failfindobj;


    // Start is called before the first frame update
    void Start()
    {
        // MapPart > RoutePart > routemaker
        // routemaker : 1200px * 800px
        // RoutePart의 크기는 화면 크기에 따라 유동적
        // Start()에서 routemaker의 부모인 RoutePart 의 사이즈 확인하고 scale 설정

        // 오브젝트들 찾기
        routePoint = transform.Find("routepoint").transform;
        routePointRect = routePoint.GetComponent<RectTransform>();
        routeLine = transform.Find("routeline").transform;
        routeLineRect = routeLine.GetComponent<RectTransform>();

        float parentWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        Debug.Log("ARGUIDE_maproute : parwidth:"+parentWidth);

        // 화면 사이즈에 맞게 set scale
        routePointRect.localScale = new Vector3(parentWidth/1200, (float)(parentWidth/1200*1.26), parentWidth/1200);
        routeLineRect.localScale = new Vector3(parentWidth/1200, parentWidth/1200, parentWidth/1200);

        //double[] routeToDraw = {37.2970, 126.9700, 37.2965, 126.9725, 37.2930, 126.9730, 37.2945, 126.9755, 37.295, 126.9745};
        //drawRoute(routeToDraw);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 받은 더블 배열의 루트 그리기
    public void drawRoute(double[] route){
        Debug.Log("ARGUIDE_maproute : drawroute");
        Debug.Log("ARGUIDE_maproute : routelen : "+route.Length);
        int routeLen = route.Length;

        // routemaker / liner 의 모든 자식 삭제
        for (int i=0 ; i<routePoint.childCount ; i++) { 
            Destroy(routePoint.GetChild(i).gameObject); 
        }
        for (int i=0 ; i<routeLine.childCount ; i++) {
            Destroy(routeLine.GetChild(i).gameObject); 
        }

        // 받은 좌표 배열 연산
        double[] calculed_route = new double[routeLen]; // 조건에 따라 연산된 루트
        string tester = "calculed route : ";
        for (int i=0 ; i<routeLen ; i=i+2){
            calculed_route[i] = (route[i] - 37.298) * 100000;   // 조건
            calculed_route[i+1] = (route[i+1] - 126.968) * 100000;
            tester = tester + (int)calculed_route[i] + "," + (int)calculed_route[i+1] + " / "; 
        }
        Debug.Log("ARGUIDE_maproute : "+tester);

        // 라인 생성
        /*LineRenderer newline = Instantiate(rLine_obj, new Vector3(0,0,0), Quaternion.identity);
        newline.transform.parent = gameObject.transform;*/

        // 처음부터 마지막 노드까지 그리기
        for (int i=0 ; i<routeLen ; i=i+2) { 
            if (i==0){
                // 경로의 첫 번째 노드
                setRoutePoint(calculed_route[i], calculed_route[i+1], rStart_obj);
            } else if (i==routeLen-2){
                // 경로의 마지막 노드
                setRoutePoint(calculed_route[i], calculed_route[i+1], rEnd_obj);
            } else {
                // 경로의 중간 노드
                setRoutePoint(calculed_route[i], calculed_route[i+1], rMid_obj);
            }  
        }

        // 라인 그려서 routeliner에 넣기
        for (int i=0 ; i<routeLen-2 ; i=i+2) { 
            setRouteLine(calculed_route[i], calculed_route[i+1], calculed_route[i+2], calculed_route[i+3]);   
        }

    }

    // 경로의 각 노드 포인트 찍기 (연산된 lat, 연산된 lon, 설치할 게임오브젝트)
    public void setRoutePoint(double c_lat, double c_lon, GameObject obj){
        Debug.Log("ARGUIDE_maproute : setRoutePoint - calculedP("+c_lat+","+c_lon+") - "+obj);

        // 원래의 위도경도 테스트출력
        float realLat = (float)(c_lat/100000+37.298);
        float realLon = (float)(c_lon/100000+126.968);
        Debug.Log("ARGUIDE_maproute : setRoutePoint - realP("+realLat+","+realLon+")");
        
        // 오브젝트 생성하고 route 게임오브젝트 자식으로 설정
        GameObject newpoint = Instantiate(obj, new Vector3(0,0,0), Quaternion.identity);
        newpoint.transform.parent = routePoint.transform;

        // 적당한 위치로 이동 : localposition 수정
        RectTransform pointRT = newpoint.GetComponent<RectTransform>();
        pointRT.localPosition = new Vector3((float)c_lon, (float)c_lat, 0);
        pointRT.localRotation = Quaternion.identity;
        //Debug.Log("point1 localpos : "+pointRT.localPosition.x+" / "+pointRT.localPosition.y);
    }

    // 경로의 각 포인트 사이에 선 긋기 (시작lat, 시작lon, 끝lat, 끝lon)
    public void setRouteLine(double s_lat, double s_lon, double e_lat, double e_lon){
        // startPoint 부터 endPoint까지 라인 그리기
        Vector3 s_point = new Vector3((float)s_lon, (float)(s_lat*1.26), 0);
        Vector3 e_point = new Vector3((float)e_lon, (float)(e_lat*1.26), 0);
        // 개별 라인 생성
        GameObject newline = Instantiate(rLine_obj, new Vector3(0,0,0), Quaternion.identity);
        newline.transform.parent = routeLine.transform;
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

    // 경로를 찾지 못할경우 목적지만 띄워줌
    public void drawDestin(double d_lat, double d_lon){
        // routemaker / liner 의 모든 자식 삭제
        for (int i=0 ; i<routePoint.childCount ; i++) { 
            Destroy(routePoint.GetChild(i).gameObject); 
        }
        for (int i=0 ; i<routeLine.childCount ; i++) {
            Destroy(routeLine.GetChild(i).gameObject); 
        }
        
        // 계산
        double c_lat = (d_lat - 37.298) * 100000;
        double c_lon = (d_lon - 126.968) * 100000;

        // 오브젝트 놓기
        setRoutePoint(c_lat, c_lon, rEnd_failfindobj);
        
    }
}
