using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapZoomMgr : MonoBehaviour
{
    [Header("배경 지도의 줌인, 줌아웃, 이동 및 포함된 핀 사이즈 조절 스크립트")]
    public float nothing;


    RectTransform mapRect;
    public float displayWidth;
    public float displayHeight;
    public float mapWidth;
    public float mapHeight;


    [Header("배경 지도의 줌인, 줌아웃 속도")]
    [SerializeField] float zoomModifierSpeed = 0.002f;
    Vector2 firstTouchPrevPos, secondTouchPrevPos;
    float touchesPrevPosDifference;
    float touchesCurrPosDifference;
    float zoomModifier;
    public float zoomValue = 1.5f; // initial zoom value is 1.5f
    public float minZoomValue = 1f;
    public float maxZoomValue = 3f;


    [Header("배경 지도의 이동 속도")]
    [SerializeField] float dragModifierSpeed = 1f;
    public float dragXValue;
    public float dragYValue;

    Transform Pin_building;
    Transform Pin_entrance;
    Transform Route_point;
    Transform Route_line;


    

    // Start is called before the first frame update
    void Start()
    {
        // get rectTransform
        mapRect = GetComponent<RectTransform>();
    
        // 초기 줌 값 1.5
        zoomValue = 1.5f;
        // 초기 위치값 0,0
        dragXValue = mapRect.position.x;
        dragYValue = mapRect.position.y;

        // 맵 세로 사이즈를 화면 높이에 맞추어 조정 및 초기화
        // 기존 맵 사이즈 = 1365 * 2048
        displayWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        displayHeight = transform.parent.GetComponent<RectTransform>().rect.height;

        mapWidth = displayHeight*1800/2648;
        mapHeight = displayHeight;

        mapRect.sizeDelta = new Vector2(mapWidth, mapHeight);

        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //Debug.Log("ARGUIDE_mapZoom : parwidth:"+displayWidth+" / initial map size is "+mapRect.rect.width+"*"+mapHeight);

        // 핀 사이즈 조절을 위해 위치 받기
        Pin_building = transform.Find("Pins/Pin_building");
        Pin_entrance = transform.Find("Pins/Pin_entrance");
        Route_point = transform.Find("Routes/Route_point");
        Route_line = transform.Find("Routes/Route_line");

    }

    // Update is called once per frame 
    void Update()
    {
        if (Input.touchCount == 2) {
            ProcessZoombyTouch();
            LimitBorder();
            ApplyChange();
        } else if (Input.touchCount == 1) {
            ProcessDragbyTouch();
            LimitBorder();
            ApplyChange();
        }     

        if (Pin_building.childCount > 0){
            ProcessPinsize(Pin_building);
        }
        if (Pin_entrance.childCount > 0){
            ProcessPinsize(Pin_entrance);
        }
        if (Route_point.childCount > 0){
            ProcessPinsize(Route_point);
        }
      

    }

    // 터치로 줌 실행
    void ProcessZoombyTouch(){
        // get Touch
        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);

        // 이전 터치 및 이전 터치와의 거리 도출
        firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
        secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

        touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
        touchesCurrPosDifference = (firstTouch.position - secondTouch.position).magnitude;

        zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;

        // zoomValue 설정
        if (touchesPrevPosDifference > touchesCurrPosDifference){
            // 화면 축소, 줌아웃
            zoomValue -= zoomModifier;
        } else {
            // 화면 확장, 줌인
            zoomValue += zoomModifier; 
        }

        // 값이 범위 밖으로 나갈 경우 값 재조정
        if (zoomValue > maxZoomValue){
            zoomValue = maxZoomValue;
        } else if (zoomValue < minZoomValue){
            zoomValue = minZoomValue;
        }

        

    
    }

    

    // 터치로 드래그 실행
    void ProcessDragbyTouch(){
        // get touch
        Touch firstTouch = Input.GetTouch(0);

        // 이동 거리 구하기
        dragXValue += firstTouch.deltaPosition.x * dragModifierSpeed;
        dragYValue += firstTouch.deltaPosition.y * dragModifierSpeed;
    
    }

    // 계산된 줌과 이동값에 의해 지도가 화면 밖으로 나가지 않도록 값 조절
    void LimitBorder(){
        // 값이 범위 밖으로 나갈 경우 값 재조정
        // 확대 된 map 이미지(mapWidth*zoomValue)의 너비 높이를 w,h라고 했을 때
        // displayWidth - w/2 < posX < w/2
        // displayHeight = h/2 < posY < h/2
        if(displayWidth - mapWidth*zoomValue/2 > dragXValue){
            dragXValue = displayWidth - mapWidth*zoomValue/2;
        } else if (dragXValue > mapWidth*zoomValue/2){
            dragXValue = mapWidth*zoomValue/2;
        }
        if (displayHeight - mapHeight*zoomValue/2 > dragYValue){
            dragYValue = displayHeight - mapHeight*zoomValue/2;
        } else if (dragYValue > mapHeight*zoomValue/2){
            dragYValue = mapHeight*zoomValue/2;
        }
    }

    // 실제 값 적용
    void ApplyChange(){
        mapRect.position = new Vector2 (dragXValue, dragYValue);
        //Debug.Log("ARGUIDE_mapZoom : dragValue : "+dragXValue+","+dragYValue);    

        // 실제 지도 사이즈 조절
        mapRect.localScale =  new Vector3(zoomValue, zoomValue, zoomValue); 
        //Debug.Log("ARGUIDE_mapZoom : zoomValue : "+zoomValue);
        //Debug.Log("ARGUIDE_mapZoom : zoomModifier : "+zoomModifier);

    }

    // 매 프레임마다 줌값을 기반으로 포함된 핀사이즈 재조정
    void ProcessPinsize(Transform parentObj){

        for (int i = 0 ; i < parentObj.childCount ; i++){
            parentObj.GetChild(i).localScale = new Vector3(1/zoomValue, 1/zoomValue, 1/zoomValue);
        }  
    }

    public IEnumerator zoomToTarget(double centerlat, double centerlon, float targetZoomValue){
        // value zoom
        float currentZoomValue = zoomValue;
        float zoomdiff = targetZoomValue-currentZoomValue;

        // value move
        // localposition 필요해서 시작값을 따로 저장해둠
        float currentXValue = mapRect.localPosition.x;
        float currentYValue = mapRect.localPosition.y;

        double centerY = (centerlat-37.2945)*(mapHeight/21)*1000;
        double centerX = (centerlon-126.974)*(mapWidth/18)*1000;
        float targetY = (float) (centerY * targetZoomValue * -1);
        float targetX = (float) (centerX * targetZoomValue * -1);

        float Xdiff = targetX - (float)centerX;
        float Ydiff = targetY - (float)centerY;
                
        while (currentXValue-(float)centerX > Xdiff*0.9f){
            yield return new WaitForSeconds(0.02f);
            zoomValue = Mathf.Lerp(zoomValue, targetZoomValue, 0.1f);
            currentXValue = Mathf.Lerp(currentXValue, targetX, 0.1f);
            currentYValue = Mathf.Lerp(currentYValue, targetY, 0.1f);

            mapRect.localScale =  new Vector3(zoomValue, zoomValue, zoomValue);
            mapRect.localPosition = new Vector2 (currentXValue, currentYValue);

            dragXValue = mapRect.position.x;
            dragYValue = mapRect.position.y;
            Debug.Log("ARGUIDE_mapZoom : zoomToTarget : "+zoomValue);
        }

        
    }




}
