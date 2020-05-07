using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;


public class TouchMgr : MonoBehaviour
{

    private Camera ARCamera;
    private Text ARCamText;
    private IEnumerator ARCamchecker;

    private string statusText;
    public Transform ARCameraTransform;

    private int frameCount;


    [Header ("클릭시 생성되는 오브젝트")]
    public GameObject placeObject;
    
    [Header ("따라다니는 오브젝트")]
    public GameObject followObject;

    public GameObject followArrow;

    [Header ("GPS관리자")]
    [SerializeField] public GPSMgr GPSMgr;

    // 이 각도로 배치한 사물은 북쪽을 가르킵니다!
    // arcamera 각도 교정용
    private float Heading;


    // Start is called before the first frame update
    void Start() 
    {
            // arcore device 프리팹 하위의 카메라를 찾아 변수에 할당
            ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
            ARCameraTransform =  GameObject.Find("First Person Camera").transform;

            // 디버깅용 arcamera 위치 텍스트
            ARCamText = transform.Find("Canvas").Find("ARCamText").GetComponent<Text>();
            //ARCamchecker=checkARCamera(1.0f);
            //StartCoroutine(ARCamchecker);
            
            // followObject 생성
            //followArrow = Instantiate(followObject, ARCameraTransform.position, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;

        // ARCamera의 각도 - 나침반 각도 계산
        // 이 각도로 배치한 사물은 북쪽을 가리킵니다...
        Heading = ARCameraTransform.eulerAngles.y - GPSMgr.compass_trueHeading;

        // uitext 업데이트
        statusText = "...Camera fC: "+frameCount;
        statusText += "\nposX: "+ARCameraTransform.position.x;
        statusText += "\nposY: "+ARCameraTransform.position.y;
        statusText += "\nposZ: "+ARCameraTransform.position.z;
        statusText += "\n";
        statusText += "\nrotX: "+ARCameraTransform.eulerAngles.x;
        statusText += "\nrotY: "+ARCameraTransform.eulerAngles.y;
        statusText += "\nrotZ: "+ARCameraTransform.eulerAngles.z;
        statusText += "\n";
        ARCamText.text = statusText;

        // 일단 followArrow 위치 및 방향 부터 업데이트
        //followArrow.transform.position = ARCameraTransform.position;
        //followArrow.transform.rotation = Quaternion.Euler(0, Heading, 0);
        // 너무 뚝뚝끊겨서 lerp 넣어줌
        //followArrow.transform.position = Vector3.Lerp(followArrow.transform.position, ARCameraTransform.position, 0.5f);
        //followArrow.transform.rotation = Quaternion.Lerp(followArrow.transform.rotation, Quaternion.Euler(0, Heading, 0), 0.5f);
        

        // 터치 처리
        if(Input.touchCount==0){
            return;
        } else {
            // 첫번재 터치 정보 추출
            Touch touch = Input.GetTouch(0);

            // Arcore에서 제공하는 raycastHit와 유사한 구조체
            TrackableHit hit;

            // 검출 대상을 평면 또는 Feature Point 로 한정
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

            // 터치한 지점으로 ray 발사
            if (touch.phase == TouchPhase.Began
                && Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                {
                    // 객체를 고정할 앵커 생성
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    // 객체 생성
                    GameObject obj = Instantiate(placeObject, hit.Pose.position, Quaternion.identity, anchor.transform);
                    
                    // 생성한 객체가 사용자쪽을 바라보도록 회전값 계산
                    //var rot = Quaternion.LookRotation(ARCamera.transform.position - hit.Pose.position);

                    // 사용자 쪽 회전값 적용
                    //obj.transform.rotation = Quaternion.Euler(ARCamera.transform.position.x, rot.eulerAngles.y, ARCamera.transform.position.z);
                    obj.transform.rotation = Quaternion.Euler(0, Heading, 0);
                    
                }
        }
    }

    private IEnumerator checkARCamera(float waitTime){
        while (true){
            // 정해진 초마다 ARcamera 체크
            yield return new WaitForSeconds(waitTime);

            

            statusText = "...Camera";
            statusText += "\nposX: "+ARCameraTransform.position.x;
            statusText += "\nposY: "+ARCameraTransform.position.y;
            statusText += "\nposZ: "+ARCameraTransform.position.z;
            statusText += "\n";
            statusText += "\nrotX: "+ARCameraTransform.eulerAngles.x;
            statusText += "\nrotY: "+ARCameraTransform.eulerAngles.y;
            statusText += "\nrotZ: "+ARCameraTransform.eulerAngles.z;
            statusText += "\n";

            ARCamText.text = statusText;



        }
    }
}
