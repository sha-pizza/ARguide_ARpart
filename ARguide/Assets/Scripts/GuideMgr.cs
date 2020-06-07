using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GuideMgr : MonoBehaviour
{
    [Header ("GuideMgr : 안내 코루틴 담당 및 루트 관리")]
    public Transform Nothing;

    // 경로!!
    // 테스트용으로 설정해둠
    //double[] route;     
    public static float[] route = {37.600000f, 126.905700f, 37.600000f, 126.905500f, 37.600200f, 126.905300f };        
    public static int nowPointNum = 0;  // 현재 향하는 좌표! nowPoint0일때 rount0,1지점으로 향한다
    int lastPointNum;                   // 마지막 좌표! 전체 좌표들의 개수와도 같다
    
    [Header ("안내 관련 값")]
    public float minD = 0.00008f;              // 목표 좌표의 +=minD 거리에 도달하면 도착한것으로!

    public float dist_wait = 2.4f;             // 마스코트가 유저와 이만큼 떨어져 있을 경우 기다림
    public float dist_warning = 7.0f;          // 마스코트가 유저와 이만큼 떨어져 있을 경우 종료 방지 위해 안내
    public float dist_disable = 10.0f;         // 마스코트가 유저와 이만큼 떨어져 있을 경우 비활성화 및 컨텐츠 종료





    private Camera ARCamera;                // AR 카메라
    private Transform ARCameraTransform;     // AR 카메라의 transform 컴포넌트

    private bool didGuideStart = false;

    [Header("명륜이 프리팹")]
    [SerializeField] public GameObject Mascot_MRPrefab;

    [Header("마스코트 설정")]
    [SerializeField] public float Mascot_walkSpeed;
    [SerializeField] public float Mascot_runSpeed;

    private GameObject Mascot_MR;
    private Animator Mascot_anim;
    private Canvas spchCanvas;
    private Transform spchBubble;
    private Text spchText;

    private Transform Mascot_sample;
    private Renderer Mascot_samplemat;
    private Collider Mascot_samplecollider;




    // 디버그용 로그
    private Text routeInfo;
    private Text guideInfo;

    private Text guideUI;



    // Start is called before the first frame update
    void Start()
    {
        // 카메라 받기
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        ARCameraTransform = GameObject.Find("First Person Camera").transform;

        // 평면찾기용 샘플마스코트 받기
        Mascot_sample = transform.Find("SampleMR").transform;
        Mascot_samplemat = transform.Find("SampleMR/MASCOT_MR_sample/root_1").GetComponent<Renderer>();
        Mascot_samplecollider = transform.Find("SampleMR/MASCOT_MR_sample/root_1").GetComponent<Collider>();

        // 디버깅용 텍스트 찾기 - 디버깅용이므로 exception 처리해둠
        try{
            routeInfo = GameObject.Find("DebugCanvas/Routeinfo").GetComponent<Text>();
            guideInfo = GameObject.Find("DebugCanvas/Guideinfo").GetComponent<Text>();
            guideUI = GameObject.Find("DebugCanvas/GuideUI").GetComponent<Text>();
        } catch(Exception e){
            Debug.Log(e);
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        // 경로가 찾아진 경우 안내 시작!
        if (GPSMgr.didFoundRoute && !didGuideStart){
        //if (!didGuideStart){
            didGuideStart = true;

            // 전체 좌표 수 계산
            lastPointNum = ( route.Length / 2 ) - 1;

            // 가이드 코루틴 시작
            IEnumerator guide_findplane = Guide_FindPlane();
            StartCoroutine(guide_findplane);

            
        }
    }

    private IEnumerator Guide_FindPlane(){
        guideInfo.text = "find plane ...";

        while (!MR_sample.isOnPlane){
            yield return new WaitForSeconds(0.05f);
            Mascot_sample.rotation = Quaternion.Lerp(Mascot_sample.rotation,
                                                        Quaternion.Euler(0, ARCameraTransform.eulerAngles.y, 0), 0.3f);
            Mascot_sample.position = Vector3.Lerp(Mascot_sample.position,
                                                        ARCameraTransform.position, 0.3f);

            // UI 안내 - find horizontal plane
            
        }

        guideInfo.text = "init guide";

        // 마스코트 생성후 방향설정
        Vector3 mascotpos = Mascot_samplemat.gameObject.transform.position;
        mascotpos.y = -1.2f;
        var mascotrottmp = ARCameraTransform.position - mascotpos;
        mascotrottmp.y = 0;
        Quaternion mascotrot = Quaternion.LookRotation(mascotrottmp);

        Mascot_MR = Instantiate(Mascot_MRPrefab, mascotpos, Quaternion.identity);
        Mascot_MR.transform.rotation = mascotrot;

        // 안내 오브젝트 삭제
        Destroy(Mascot_sample.gameObject);

        // 말풍선 오브젝트가 담긴 canvas 받아두기
        spchCanvas = Mascot_MR.transform.Find("root/speechCanvas").GetComponent<Canvas>();
        spchBubble = Mascot_MR.transform.Find("root/speechCanvas/speechBubble");
        spchText = Mascot_MR.transform.Find("root/speechCanvas/speechBubble/speechText").GetComponent<Text>();

        // 말풍선 캔버스 꺼두기
        spchCanvas.gameObject.SetActive(false);            

        // 마스코트 애님컨트롤러 설정
        Mascot_anim = Mascot_MR.GetComponent<Animator>();

        // 가이드 코루틴 시작
        IEnumerator guide_start = Guide_Start();
        StartCoroutine(guide_start);
        

    }

    // 가이드 코루틴
    private IEnumerator Guide_Start(){

        guideInfo.text = "start guide coroutine";

        yield return new WaitForSeconds(2.0f);

        spchCanvas.gameObject.SetActive(true);
       
        spchText.text = "안녕!\n안내를 시작하려면\n말풍선을 눌러줘!";
        spchBubble.gameObject.SetActive(true);
        Invoke("spchBubbleFadein", 0f);

        
        while (!RayMgr.isBubbleClicked){
            yield return new WaitForSeconds(0.5f);
        }
        RayMgr.isBubbleClicked = false;
        
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.36f);
        spchBubble.gameObject.SetActive(false);
        yield return new WaitForSeconds (1.0f);

        spchText.text = "좋아,출발해보자!";
        spchBubble.gameObject.SetActive(true);
        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds (2.4f); // between in-out
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.36f);
        spchBubble.gameObject.SetActive(false);
        yield return new WaitForSeconds (0.1f); // between active false-true

        spchText.text = "너무 멀어지면\n종료될 수도 있으니까\n조심해야해!";
        spchBubble.gameObject.SetActive(true);
        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds (2.4f);
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.36f);
        spchBubble.gameObject.SetActive(false);
        yield return new WaitForSeconds (2.0f);


        /* oldscript !
        spchText.text = "좋아,출발해보자!";
        StartCoroutine(SpchBubble_Fadein(0.2f, 1.0f, 0.03f));
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(SpchBubble_Fadeout(1.0f, 0.2f, 0.03f));
        yield return new WaitForSeconds(0.4f);
        */
        
        Mascot_anim.SetBool("isStartGuide", true);

        spchText.text = "어디보자...";
        spchBubble.gameObject.SetActive(true);
        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds (2.4f);
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.36f);
        spchBubble.gameObject.SetActive(false);
        yield return new WaitForSeconds (0.1f);

        /*
        spchText.text = "이쪽이야 !";
        spchBubble.gameObject.SetActive(true);
        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds (1.9f);
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.36f);
        spchBubble.gameObject.SetActive(false);
        yield return new WaitForSeconds (0.1f);
        */

        routeInfo.text = "now guide to : "+nowPointNum;
        for (int i = nowPointNum ; i <= lastPointNum ; i++ ){
            routeInfo.text += "\n"+(i*2)+" : "+route[(i*2)]+"/"+route[(i*2)+1];
        }
       
        // 파트 이동 코루틴 호출
        guideInfo.text = "start guide part "+nowPointNum+"/"+lastPointNum;
        //StartCoroutine(Guide_Part((float)GPSMgr.LAT, (float)GPSMgr.LON, route[0], route[1]));

        for (int i = 0 ; i<=lastPointNum ; i++){
            if (i == 0){
                StartCoroutine(Guide_Part((float)GPSMgr.LAT, (float)GPSMgr.LON, route[0], route[1]));
            } else {
                StartCoroutine(Guide_Part((float)GPSMgr.LAT, (float)GPSMgr.LON, route[i], route[i+1]));
            }
        }
        
          
    }

    private IEnumerator Guide_Part(float sLAT, float sLON, float eLAT, float eLON){
        // start LAT LON , end LAT LON value of part
        guideInfo.text += "\nkeep guide";
        yield return new WaitForSeconds(1.0f); 


        Mascot_anim.SetBool("isMove", true);
                                                 

        // 마스코트가 while 타겟오브젝트와 충분히 가까이 있지 않은 동안
        // DestinationMgr 의 Destination 오브젝트를 향해
        // Lerp회전 하면서 MoveToward한다
        while ( Mathf.Abs((float)GPSMgr.LAT - eLAT) > minD && Mathf.Abs((float)GPSMgr.LON - eLON) > minD ){
            yield return new WaitForSeconds(0.05f);
            guideInfo.text = "start guide part "+nowPointNum+"/"+lastPointNum;

      
            // 사용자와의 거리 확인
            float dist = Vector3.Distance( ARCamera.transform.position, Mascot_MR.transform.position );
            guideInfo.text += "\ndistance :"+dist;

            if (dist > dist_disable) {
                // dist_disable 보다 멀리 떨어질 경우 : 모든 코루틴 종료 및 ui 안내
                guideInfo.text += "\n dist_disable";
                guideUI.text = "마스코트 캐릭터와\n너무 멀리 떨어졌습니다!\n서비스가 종료됩니다.";
                spchBubble.gameObject.SetActive(false);
                // 실제 서비스 종료 추가

            } else if (dist > dist_warning ) {
                // if dist_warning 보다 멀리 떨어질 경우 : 일단 ui로 안내
                guideInfo.text += "\n dist_warning";
                guideUI.text = "마스코트 캐릭터\n가까이로 이동해 주세요.\n더 멀어질 경우\n서비스가 종료될 수 있습니다.";
                spchBubble.gameObject.SetActive(false);
                // 이후 추가!!

            } else if (dist > dist_wait) {
                
                // dist_wait 보다 멀리 떨어져 있고 마스코트가 앞서갈 경우( 나 > 마스코트 > 목표지점 ) : 기다림
                // dist_wait 보다 멀리 떨어져 있고 마스코트가 뒤에 있을 경우 ( 마스코트 > 나 > 목표지점 ) : 뛴다
                if ( Vector3.Distance(ARCamera.transform.position, DestinationMgr.destination.position) >
                      Vector3.Distance(Mascot_MR.transform.position, DestinationMgr.destination.position)  ){
                    guideInfo.text += "\n dist_wait";
                    guideUI.text = "";
                    Mascot_anim.SetBool("isMove", false);
                    Mascot_anim.SetBool("isMoveFast", false);
                    Mascot_anim.SetBool("isCute", true);

                    // 각도수정 및 이동
                    var lookpos = ARCameraTransform.position - Mascot_MR.transform.position;
                    lookpos.y = 0;
                    var rotation = Quaternion.LookRotation(lookpos);
                    Mascot_MR.transform.rotation = Quaternion.Lerp(Mascot_MR.transform.rotation, rotation, 0.3f);                                                            
                    
                    
                    spchText.text = "얼른와 !";
                    spchBubble.gameObject.SetActive(true);
                    spchBubble.transform.localScale = new Vector3(1.0f, 1.0f, 0);
                    spchBubble.transform.localPosition = new Vector3(0, 0, 0);

                } else {
                    guideInfo.text += "\n dist_run";
                    guideUI.text = "";
                    Mascot_anim.SetBool("isMove", true);
                    Mascot_anim.SetBool("isMoveFast", true);
                    Mascot_anim.SetBool("isCute", false);
                    spchBubble.gameObject.SetActive(false);

                    // 각도수정 및 이동
                    //Mascot_MR.transform.LookAt(DestinationMgr.destination.position);  
                    var lookpos = DestinationMgr.destination.position - Mascot_MR.transform.position;
                    lookpos.y = 0;
                    var rotation = Quaternion.LookRotation(lookpos);
                    Mascot_MR.transform.rotation = Quaternion.Lerp(Mascot_MR.transform.rotation, rotation, 0.3f);                                                          
                    Mascot_MR.transform.position = Vector3.MoveTowards (Mascot_MR.transform.position, 
                                                                        DestinationMgr.destination.position,
                                                                        Mascot_runSpeed * Time.deltaTime);
                }

            } else {
                // 가까이에 있을 경우 : 목표지점으로 걷는다
                guideInfo.text += "\n dist_walk";
                guideUI.text = "";
                Mascot_anim.SetBool("isMove", true);
                Mascot_anim.SetBool("isMoveFast", false);
                Mascot_anim.SetBool("isCute", false);
                spchBubble.gameObject.SetActive(false);

                // 각도수정 및 이동
                //Mascot_MR.transform.LookAt(DestinationMgr.destination.position);  
                var lookpos = DestinationMgr.destination.position - Mascot_MR.transform.position;
                lookpos.y = 0;
                var rotation = Quaternion.LookRotation(lookpos);
                Mascot_MR.transform.rotation = Quaternion.Lerp(Mascot_MR.transform.rotation, rotation, 0.3f);                                                            
                Mascot_MR.transform.position = Vector3.MoveTowards (Mascot_MR.transform.position, 
                                                                    DestinationMgr.destination.position,
                                                                    Mascot_walkSpeed * Time.deltaTime);
                
            }   

        }

        guideInfo.text += "\n while - end!";



  
        
        
        


    }

    // 말풍선 등장 퇴장 포함해서 말하기 메서드
    /*
    private IEnumerator SpchBubble_Talk(string sentence, float duration){
        spchText.text = sentence;
        StartCoroutine(SpchBubble_Fadein(0.2f, 1.0f, 0.03f));
        yield return new WaitForSeconds(duration);
        StartCoroutine(SpchBubble_Fadeout(1.0f, 0.2f, 0.03f));
        yield return new WaitForSeconds(0.4f);
    }

    // 말풍선 active true 하고 등장 연출
    private IEnumerator SpchBubble_Fadein(float startSize, float endSize, float waittime){
        spchBubble.transform.localScale = new Vector3(startSize, startSize, startSize);
        spchCanvas.gameObject.SetActive(true);
        for (int i = 0 ; i < 10 ; i++){
            spchBubble.transform.localScale = Vector3.Lerp(spchBubble.transform.localScale, 
                                                            new Vector3(endSize, endSize, endSize), 0.3f);
            yield return new WaitForSeconds(waittime);
        }
        spchBubble.transform.localScale = new Vector3(endSize, endSize, endSize);
    }

    // 말풍선 active false 하고 퇴장 연출
    private IEnumerator SpchBubble_Fadeout(float startSize, float endSize, float waittime){
        spchBubble.transform.localScale = new Vector3(startSize, startSize, startSize);
        for (int i = 0 ; i < 10 ; i++){
            spchBubble.transform.localScale = Vector3.Lerp(spchBubble.transform.localScale, 
                                                            new Vector3(endSize, endSize, endSize), 0.3f);
            yield return new WaitForSeconds(waittime);
        }
        spchBubble.transform.localScale = new Vector3(endSize, endSize, endSize);
        spchCanvas.gameObject.SetActive(false);
    }
    */



    void spchBubbleFadeout(){
        spchBubble.transform.localScale = new Vector3(1.0f, 1.0f, 0);
        spchBubble.transform.localPosition = new Vector3(0, 0, 0);
        for (int i=1 ; i <= 10 ; i ++){
            Invoke("spchBubbleSmaller", 0.04f*i);
        }  
    }
    void spchBubbleSmaller(){        
        spchBubble.transform.localScale = Vector3.Lerp( spchBubble.transform.localScale, new Vector3(0.2f, 0.2f, 0), 0.3f );
        spchBubble.transform.localPosition = Vector3.Lerp( spchBubble.transform.localPosition, new Vector3(0, -0.2f, 0), 0.3f);
    }

    void spchBubbleFadein(){
        spchBubble.transform.localScale = new Vector3(0.2f, 0.2f, 0);
        spchBubble.transform.localPosition = new Vector3(0, -0.2f, 0);
        for (int i=1 ; i <= 10 ; i ++){
            Invoke("spchBubbleBigger", 0.04f*i);
        }  
    }
    void spchBubbleBigger(){
        spchBubble.transform.localScale = Vector3.Lerp( spchBubble.transform.localScale, new Vector3(1.0f, 1.0f, 0), 0.3f );
        spchBubble.transform.localPosition = Vector3.Lerp( spchBubble.transform.localPosition, new Vector3(0, 0, 0), 0.3f);
    }

    




}
