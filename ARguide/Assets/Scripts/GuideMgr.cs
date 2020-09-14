using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GuideMgr : MonoBehaviour
{
    [Header ("GuideMgr : 안내 코루틴 담당 및 루트 관리")]
    public Transform Nothing;

    // 경로!!
    // 테스트용으로 설정해둠
    //double[] route;        
    //public static double[] route = {37.295400, 126.976000, 37.295400, 126.976200}; 
    public static double[] route;
           
    public static int nowPointNum = 0;  // 현재 향하는 좌표! nowPoint0일때 rount0,1지점으로 향한다
    int lastPointNum;                   // 마지막 좌표! 전체 좌표들의 개수와도 같다
    
    [Header ("안내 관련 값")]
    private double minD = 0.0001;              // 목표 좌표의 +=minD 거리에 도달하면 도착한것으로!

    public double dist_wait = 2.4;             // 마스코트가 유저와 이만큼 떨어져 있을 경우 기다림
    public double dist_warning = 7.0;          // 마스코트가 유저와 이만큼 떨어져 있을 경우 종료 방지 위해 안내
    public double dist_disable = 10.0;         // 마스코트가 유저와 이만큼 떨어져 있을 경우 비활성화 및 컨텐츠 종료

    // 가이드 상태
    public static string Guidestatus = "";




    private Camera ARCamera;                // AR 카메라
    private Transform ARCameraTransform;     // AR 카메라의 transform 컴포넌트

    public static bool didGuideStart = false;

    [Header("명륜이 프리팹")]
    [SerializeField] public GameObject Mascot_MRPrefab;

    [Header("마스코트 설정")]
    [SerializeField] public float Mascot_walkSpeed;
    [SerializeField] public float Mascot_runSpeed;

    private GameObject Mascot_MR;
    private Animator Mascot_anim;
    private Canvas spchCanvas;
    private Transform spchBubble;

    private string spchBubbleText;
    private Text spchText;

    private Transform Mascot_sample;
    private Renderer Mascot_samplemat;
    private Collider Mascot_samplecollider;

    String language = "korean";


    // 하단 ui
    private Text guideUI;
    private GameObject guideBack;

    


    // 학교와 너무 멀리 떨어져 있을 때 에러처리를 위한 상수
    private const double COLLEGE_LAT = 37.293889;
    private const double COLLEGE_LON = 126.974904;
    //private const double DISTANCE_LIMIT = 0.009; // 성균관대역보다 약간 먼 거리 (경우에 따라 수정)
    private const double DISTANCE_LIMIT = 0.75; // 베타버전 원거리 허용


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ARGUIDE_guide : start()");
        Guidestatus = "Start()";

        // 카메라 받기
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        ARCameraTransform = GameObject.Find("First Person Camera").transform;

        // 평면찾기용 샘플마스코트 받기
        Mascot_sample = transform.Find("SampleMR").transform;
        Mascot_samplemat = transform.Find("SampleMR/MASCOT_MR_sample/root_1").GetComponent<Renderer>();
        Mascot_samplecollider = transform.Find("SampleMR/MASCOT_MR_sample/root_1").GetComponent<Collider>();

        // 하단 ui 오브젝트 받기
        guideUI = GameObject.Find("UICanvas/GuideUI/GuideUIText").GetComponent<Text>();
        guideBack = GameObject.Find("UICanvas/GuideUI");

        guideUI.gameObject.SetActive(false);
        guideBack.gameObject.SetActive(false);

        // 디버깅용 텍스트 찾기 - 
        try{
            //guideInfo = GameObject.Find("DebugCanvas/Guideinfo").GetComponent<Text>(); 
        } catch(Exception e){
            Debug.Log("ARGUIDE_guide : "+e);
        }
        Debug.Log("ARGUIDE_guide : start()_end");
    }

    // Update is called once per frame
    void Update()
    {
        // 경로가 찾아진 경우 안내 시작!
        if (GPSMgr.didFoundRoute && !didGuideStart){    
        //if (!didGuideStart){
            Debug.Log("ARGUIDE_guide : didFoundRoute & didGuideStart");
            didGuideStart = true;
        
            // 경로 가져오기
            route = GPSMgr.route;            

            // 전체 좌표 수 계산
            lastPointNum = route.Length;
            Debug.Log("ARGUIDE_guide : route length is ... "+lastPointNum);
            Debug.Log("ARGUIDE_guide : route is ~ from "+route[0]+","+route[1]+" to "+route[route.Length-2]+","+route[route.Length-1]);

            // 가이드 코루틴 시작 (바닥면 찾기부터!)
            IEnumerator guide_findplane = Guide_FindPlane();
            StartCoroutine(guide_findplane);
        }
    }

    // Coroutine to find guide
    // 평면 찾아 마스코트 설치
    private IEnumerator Guide_FindPlane(){
        Debug.Log("ARGUIDE_guide : start coroutine : findplane");
        Guidestatus = "Guide_FindPlane()";

        while (!MR_sample.isOnPlane){
            yield return new WaitForSeconds(0.05f);
            Mascot_sample.rotation = Quaternion.Lerp(Mascot_sample.rotation,
                                                        Quaternion.Euler(0, ARCameraTransform.eulerAngles.y, 0), 0.3f);
            Mascot_sample.position = Vector3.Lerp(Mascot_sample.position,
                                                        ARCameraTransform.position, 0.3f);            
        }

        

        Debug.Log("ARGUIDE_guide : instantiate mascot object");

        // 마스코트 생성후 방향설정
        Vector3 mascotpos = Mascot_samplemat.gameObject.transform.position;
        //mascotpos.y = -1.2f;
        mascotpos.y = ARCameraTransform.position.y - 1.2f;
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

    // Coroutine to start guide
    private IEnumerator Guide_Start(){
        Debug.Log("ARGUIDE_guide : start coroutine : guide_start");
        Guidestatus = "Guide_Start()";
        
        /*
        // Mascot : 대사 작성 시 아래 복붙
        spchText.text = "대사\n5줄이내";
        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds(2.4f); // 대사 지속 시간
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds(1.0f);
        */

        yield return new WaitForSeconds(2.0f);


        spchCanvas.gameObject.SetActive(true); // 이줄 없애면 말풍선 안뜨니까 그대로 두기

        language = GameObject.Find("GPSMgr").GetComponent<GPSMgr>().Language_Update();

        //AudioMgr.playSound(AudioMgr.music, AudioMgr.musicPlayer);
        //AudioMgr.playSound1();

        playSound(0);
        if (language == "korean")
        {
            spchText.text = "안녕!\n안내를 시작하려면\n말풍선을 눌러줘!";
        } else if (language == "chinese")
        {
            spchText.text = "你好!\n 点按气泡以开始指导";
        } else if (language == "japanese")
        {
            spchText.text = "おはよう！\n　案内を始めるために\nフキダシを押して";
        }
        else {
            spchText.text = "Hello!\nTo start the guide\nTap the message!";
        }
        
        spchBubble.gameObject.SetActive(true);

        Invoke("spchBubbleFadein", 0f);
        
        while (!RayMgr.isBubbleClicked){
            yield return new WaitForSeconds(0.5f);
        }
        RayMgr.isBubbleClicked = false;

        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds(1.0f);
        

        // 0706 학교에서 너무 멀리 떨어져 있는 지 확인 / 0713 위치 정보가 업데이트 되지 않고 있으면 종료
        if (!isWithinCollegeArea() || GPSMgr.overNsecsNotLoadedLocation)
        {

            // 너무 멀리 떨어져 있으면 메인화면으로 돌아감
            GPSMgr.didFoundRoute = false;
            GPSMgr.route = null;
            GPSMgr.overNsecsNotLoadedLocation = false;
            GPSMgr.secsNotLoadedLocation = 0;


            if (!isWithinCollegeArea())
            {
                if (language == "korean")
                {
                    spchText.text = "학교와 너무 멀리\n떨어져 있어서\n가이드를\n진행할 수 없어 !\n다시 시도해 줘 !";
                }
                else if (language == "chinese")
                {
                    spchText.text = "Because it is\ntoo far from \nUniversity,\nWe can't guide !\nTry Again !";
                }
                else if (language == "japanese")
                {
                    spchText.text = "Because it is\ntoo far from \nUniversity,\nWe can't guide !\nTry Again !";
                }
                else
                {
                    spchText.text = "Because it is\ntoo far from \nUniversity,\nWe can't guide !\nTry Again !";
                }
            }

            else
            {
                if (language == "korean")
                {
                    spchText.text = "위치 정보가\n업데이트 되지 않아\n가이드를\n진행할 수 없어 !\n건물 밖에서\n다시 시도해 줘 !";
                }
                else if (language == "chinese")
                {
                    spchText.text = "Because Location info\nis not updated,\nTry again Outside of buliding!";
                }
                else if (language == "japanese")
                {
                    spchText.text = "Because Location info\nis not updated,\nTry again Outside of buliding!";
                }
                else
                {
                    spchText.text = "Because Location info\nis not updated,\nTry again Outside of buliding!";
                }
                
            }
            spchBubble.gameObject.SetActive(true);

            Invoke("spchBubbleFadein", 0f);

            // 안내문 설정
            guideUI.gameObject.SetActive(true);
            guideBack.gameObject.SetActive(true);
            guideUI.fontSize = 40;


            
            if (language == "korean")
            {
                guideUI.text = "곧 메인화면으로 돌아갑니다.";
            }
            else if (language == "chinese")
            {
                guideUI.text = "Back main page soon.";
            }
            else if (language == "japanese")
            {
                guideUI.text = "Back main page soon.";
            }
            else
            {
                guideUI.text = "Back main page soon.";
            }


            yield return new WaitForSeconds(2.4f);

            Invoke("spchBubbleFadeout", 0f);
            yield return new WaitForSeconds(2.0f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else
        {
            // 기존 정상작동 코드



            playSound(1);
            if (language == "korean")
            {
                spchText.text = "좋아,출발해보자!";
            }
            else if (language == "chinese")
            {
                spchText.text = "好吧走吧";
            }
            else if (language == "japanese")
            {
                spchText.text = "よし、出発！";
            }
            else
            {
                spchText.text = "Okay, Let's go!";
            }
            spchBubble.gameObject.SetActive(true);

            Invoke("spchBubbleFadein", 0f);
            yield return new WaitForSeconds(2.4f); // between in-out
            Invoke("spchBubbleFadeout", 0f);
            yield return new WaitForSeconds(0.6f); // between active false-true

            playSound(2);
            if (language == "korean")
            {
                spchText.text = "너무 멀어지면\n종료될 수도 있으니까\n조심해야해!";
            }
            else if (language == "chinese")
            {
                spchText.text = "小心，\n因为如果距离太远，\n它可能会结束！";
            }
            else if (language == "japanese")
            {
                spchText.text = "私と離れたら\n案内が終わるかもしれないから\n気お付けてね！";
            }
            else
            {
                spchText.text = "If you far away from me,\n it can be quit,\n so be careful!";
            }
            
            spchBubble.gameObject.SetActive(true);

            Invoke("spchBubbleFadein", 0f);
            yield return new WaitForSeconds(2.4f);
            Invoke("spchBubbleFadeout", 0f);
            yield return new WaitForSeconds(1.0f);

            Mascot_anim.SetBool("isStartGuide", true);

            playSound(3);
            if (language == "korean")
            {
                spchText.text = "어디보자...";
            }
            else if (language == "chinese")
            {
                spchText.text = "让我们来看看...";
            }
            else if (language == "japanese")
            {
                spchText.text = "どれどれ…";
            }
            else
            {
                spchText.text = "Let me see...";
            }
            
            spchBubble.gameObject.SetActive(true);

            Invoke("spchBubbleFadein", 0f);
            yield return new WaitForSeconds(2.4f);
            Invoke("spchBubbleFadeout", 0f);
            yield return new WaitForSeconds(0.6f);


        
            nowPointNum = 0;
        

            // 0->2 안내 np=0 nps=0
            // 0->2 안내 코루틴 끝날 때 np+2 / np=2 nps=0
            // while 돌리면서, if (np > nps) 이면 nps+2 하고
            // 2->4 안내 코루틴 시작

            for (int i = 0; i < lastPointNum; i = i + 2)
            {
                while (nowPointNum != i)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                // nowPointnum은 Guide_Part 코루틴 안에서 바꿔줄 예정
                Guidestatus = "Guide_Part() : now on" + nowPointNum + " / " + lastPointNum + "routepoint \nMove to latlon : " + route[i] + ", " + route[i + 1];
                StartCoroutine(Guide_Part(GPSMgr.LAT, GPSMgr.LON, route[i], route[i + 1]));

            }



            while (nowPointNum != lastPointNum)
            {
                yield return new WaitForSeconds(1f);
            }

            StartCoroutine(Guide_End());
        }
        
        
        
          
    }

    // Coroutine to guide each part of route
    private IEnumerator Guide_Part(double sLAT, double sLON, double eLAT, double eLON){
        // start LAT LON , end LAT LON value of part
        yield return new WaitForSeconds(1.0f); 


        Mascot_anim.SetBool("isMove", true);
                    

        // 마스코트가 while 타겟오브젝트와 충분히 가까이 있지 않은 동안
        // DestinationMgr 의 Destination 오브젝트를 향해
        // Lerp회전 하면서 MoveToward한다
        while (  ( Mathf.Abs((float)(GPSMgr.LAT - eLAT)) > minD ) || ( Mathf.Abs((float)(GPSMgr.LON - eLON)) > minD ) ){
            yield return new WaitForSeconds(0.05f);

            // 사용자와의 거리 확인
            float dist = Vector3.Distance( ARCamera.transform.position, Mascot_MR.transform.position );
            //guideInfo.text += "\ndistance :"+dist;

            if (dist > dist_disable) {
                // dist_disable 보다 멀리 떨어질 경우 : 모든 코루틴 종료 및 ui 안내
                //guideInfo.text += "\n dist_disable";
                guideUI.gameObject.SetActive(true);
                guideBack.gameObject.SetActive(true);
                if (language == "korean")
                {
                    guideUI.text = "마스코트 캐릭터와\n너무 멀리 떨어졌습니다!\n곧 서비스가 종료됩니다.";
                }
                else if (language == "chinese")
                {
                    guideUI.text = "It is too far \nfrom mascot!\nService will quit.";
                }
                else if (language == "japanese")
                {
                    guideUI.text = "It is too far \nfrom mascot!\nService will quit.";
                }
                else
                {
                    guideUI.text = "It is too far \nfrom mascot!\nService will quit.";
                }
                
                spchBubble.gameObject.SetActive(false);

                // 실제 서비스 종료 추가
                Destroy(Mascot_MR.gameObject);
                for (int i = 10 ; i > 0 ; i--){
                    yield return new WaitForSeconds(0.9f);
                    if (language == "korean")
                    {
                        guideUI.text = "마스코트 캐릭터와\n너무 멀리 떨어졌습니다!\n" + i + "초후 서비스가 종료됩니다.";
                    }
                    else if (language == "chinese")
                    {
                        guideUI.text = "It is too far \nfrom mascot!\nService will quit after " + i + "seconds";
                    }
                    else if (language == "japanese")
                    {
                        guideUI.text = "It is too far \nfrom mascot!\nService will quit after " + i + "seconds";
                    }
                    else
                    {
                        guideUI.text = "It is too far \nfrom mascot!\nService will quit after " + i + "seconds";
                    }
                    
                }

                // 현재 씬 리로드
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            } else if (dist > dist_warning ) {
                // if dist_warning 보다 멀리 떨어질 경우 : 일단 ui로 안내
                //guideInfo.text += "\n dist_warning";
                guideUI.gameObject.SetActive(true);
                guideBack.gameObject.SetActive(true);
                if (language == "korean")
                {
                    guideUI.text = "마스코트 캐릭터\n가까이로 이동해 주세요.\n더 멀어질 경우\n서비스가 종료될 수 있습니다.";
                }
                else if (language == "chinese")
                {
                    guideUI.text = "Move close to \nthe mascot character.\nif you far more,\nservice can be quit.";
                }
                else if (language == "japanese")
                {
                    guideUI.text = "Move close to \nthe mascot character.\nif you far more,\nservice can be quit.";
                }
                else
                {
                    guideUI.text = "Move close to \nthe mascot character.\nif you far more,\nservice can be quit.";
                }
                
                spchBubble.gameObject.SetActive(false);

            } else if (dist > dist_wait) {
                
                // dist_wait 보다 멀리 떨어져 있고 마스코트가 앞서갈 경우( 나 > 마스코트 > 목표지점 ) : 기다림
                // dist_wait 보다 멀리 떨어져 있고 마스코트가 뒤에 있을 경우 ( 마스코트 > 나 > 목표지점 ) : 뛴다
                if ( Vector3.Distance(ARCamera.transform.position, DestinationMgr.destination.position) >
                      Vector3.Distance(Mascot_MR.transform.position, DestinationMgr.destination.position)  ){
                    //guideInfo.text += "\n dist_wait";
                    guideUI.text = "";
                    guideUI.gameObject.SetActive(false);
                    guideBack.gameObject.SetActive(false);
                    
                    Mascot_anim.SetBool("isMove", false);
                    Mascot_anim.SetBool("isMoveFast", false);
                    Mascot_anim.SetBool("isCute", true);

                    // 각도수정 및 이동
                    var lookpos = ARCameraTransform.position - Mascot_MR.transform.position;
                    lookpos.y = 0;
                    var rotation = Quaternion.LookRotation(lookpos);
                    Mascot_MR.transform.rotation = Quaternion.Lerp(Mascot_MR.transform.rotation, rotation, 0.3f);

                    playSound(4);
                    if (language == "korean")
                    {
                        spchText.text = "얼른와 !";
                    }
                    else if (language == "chinese")
                    {
                        spchText.text = "来吧！";
                    }
                    else if (language == "japanese")
                    {
                        spchText.text = "速くついてきて!";
                    }
                    else
                    {
                        spchText.text = "Come fast !";
                    }
                    
                    spchBubble.gameObject.SetActive(true);
                    spchBubble.transform.localScale = new Vector3(1.0f, 1.0f, 0);
                    spchBubble.transform.localPosition = new Vector3(0, 0, 0);

                } else {
                    //guideInfo.text += "\n dist_run";
                    guideUI.text = "";
                    guideUI.gameObject.SetActive(false);
                    guideBack.gameObject.SetActive(false);

                    Mascot_anim.SetBool("isMove", true);
                    Mascot_anim.SetBool("isMoveFast", true);
                    Mascot_anim.SetBool("isCute", false);
                    spchBubble.gameObject.SetActive(false);

                    // 각도수정 및 이동
                    // 유저쪽으로! 
                    /*
                    var lookpos = DestinationMgr.destination.position - Mascot_MR.transform.position;
                    lookpos.y = 0;
                    var rotation = Quaternion.LookRotation(lookpos);
                    Mascot_MR.transform.rotation = Quaternion.Lerp(Mascot_MR.transform.rotation, rotation, 0.3f);                                                          
                    Mascot_MR.transform.position = Vector3.MoveTowards (Mascot_MR.transform.position, 
                                                                        DestinationMgr.destination.position,
                                                                        Mascot_runSpeed * Time.deltaTime);
                                                                        */
                    var lookpos = ARCameraTransform.position - Mascot_MR.transform.position;
                    lookpos.y = 0;
                    var rotation = Quaternion.LookRotation(lookpos);
                    Mascot_MR.transform.rotation = Quaternion.Lerp(Mascot_MR.transform.rotation, rotation, 0.3f);

                    // 0715 y축 보정
                    Mascot_MR.transform.position = new Vector3(Mascot_MR.transform.position.x, ARCameraTransform.position.y - 1.2f, Mascot_MR.transform.position.z);

                    var movepos = new Vector3(ARCameraTransform.position.x, ARCameraTransform.position.y-1.2f, ARCameraTransform.position.z);                                                         
                    Mascot_MR.transform.position = Vector3.MoveTowards (Mascot_MR.transform.position, 
                                                                        movepos,
                                                                        Mascot_runSpeed * Time.deltaTime);
                }

            } else {
                // 가까이에 있을 경우 : 목표지점으로 걷는다
                //guideInfo.text += "\n dist_walk";
                guideUI.text = "";
                guideUI.gameObject.SetActive(false);
                guideBack.gameObject.SetActive(false);

                Mascot_anim.SetBool("isMove", true);
                Mascot_anim.SetBool("isMoveFast", false);
                Mascot_anim.SetBool("isCute", false);
                spchBubble.gameObject.SetActive(false);

                // 0715 y축 보정
                Mascot_MR.transform.position = new Vector3(Mascot_MR.transform.position.x, ARCameraTransform.position.y - 1.2f, Mascot_MR.transform.position.z);

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
        nowPointNum += 2;
        //guideInfo.text += "\n while - end! nP+2";

    }

    // Coroutine to end guide process
    private IEnumerator Guide_End(){
        yield return new WaitForSeconds(0.5f);
        Guidestatus = "Guide_End()";


        string endSpch = GPSMgr.finalDestination;
        string endInfo = "";

        // 엔딩메세지 받아오기
        endInfo = GPSMgr.m_JavaObject.Call<String>("getEndingMessage", endSpch);

        // 안내문 설정
        guideBack.gameObject.SetActive(true);
        guideUI.fontSize = 40;
        
        guideUI.text = endInfo;

        // 유저 바라보기
        var lookpos = ARCameraTransform.position - Mascot_MR.transform.position;
        lookpos.y = 0;
        var rotation = Quaternion.LookRotation(lookpos);
        Mascot_MR.transform.rotation = rotation; 

        // 말풍선 설정

        Mascot_anim.SetBool("isTalk", true);

        if (language == "korean")
        {
            spchText.text = endSpch + "에 도착했어!";
        }
        else if (language == "chinese")
        {
            spchText.text = "您已经到达" + endSpch;
        }
        else if (language == "japanese")
        {
            spchText.text = endSpch + "に到着したよ！";
        }
        else
        {
            spchText.text = "We arrived to " + endSpch;
        }
        
        spchBubble.gameObject.SetActive(true);

        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds (2.4f);
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.4f);

        playSound(5);
        if (language == "korean")
        {
            spchText.text = "이 장소에 대한 설명은 \n아래 ui를 참고해 줘 !";
        }
        else if (language == "chinese")
        {
            spchText.text = "To know explanation of here \nSee below UI !";
        }
        else if (language == "japanese")
        {
            spchText.text = "To know explanation of here \nSee below UI !";
        }
        else
        {
            spchText.text = "To know explanation of here \nSee below UI !";
        }
        
        spchBubble.gameObject.SetActive(true);

        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds (0.4f);

        // 종료후 메인으로
        for (int i = 10 ; i > 0 ; i--){
            yield return new WaitForSeconds(0.9f);
            if (language == "korean")
            {
                guideUI.text = endInfo + "\n" + i + "초후 안내를 종료합니다.";
            }
            else if (language == "chinese")
            {
                guideUI.text = endInfo + "\n" + "After" + i + "seconds, guide will quit.";
            }
            else if (language == "japanese")
            {
                guideUI.text = endInfo + "\n" + "After" + i + "seconds, guide will quit.";
            }
            else
            {
                guideUI.text = endInfo + "\n" + "After" + i + "seconds, guide will quit.";
            }
            
        }

        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.4f);

        // 현재 씬 리로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        
        

     
        
    }

    // 0706 학교에서 멀리 떨어져 있는 지 확인하는 메서드
    private bool isWithinCollegeArea()
    {
        double distanceFromCollege = Math.Sqrt(Math.Pow(GPSMgr.LAT - COLLEGE_LAT, 2.0) + Math.Pow(GPSMgr.LON - COLLEGE_LON, 2.0));

        if (distanceFromCollege < DISTANCE_LIMIT)
        {
            return true;
        } else
        {
            return false;
        }
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
        Debug.Log("ARGUIDE_guide : invoke spchbubblefadeout");
        spchBubble.transform.localScale = new Vector3(1.0f, 1.0f, 0);
        spchBubble.transform.localPosition = new Vector3(0, 0, 0);
        for (int i=1 ; i <= 10 ; i ++){
            Invoke("spchBubbleSmaller", 0.04f*i);
        }  
        Invoke("spchBubbleTurnOff", 0.4f);
    }
    void spchBubbleSmaller(){        
        spchBubble.transform.localScale = Vector3.Lerp( spchBubble.transform.localScale, new Vector3(0.2f, 0.2f, 0), 0.3f );
        spchBubble.transform.localPosition = Vector3.Lerp( spchBubble.transform.localPosition, new Vector3(0, -0.2f, 0), 0.3f);
    }

    void spchBubbleTurnOff() {
        spchBubble.gameObject.SetActive(false);
    }

    void spchBubbleFadein(){
        Debug.Log("ARGUIDE_guide : invoke spchbubblefadein");
        spchBubble.gameObject.SetActive(true);
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

    void spchBubbleTurnOn(){
        Debug.Log("ARGUIDE_guide : spchbubble setactive");
        spchBubble.gameObject.SetActive(true);
    }

    void playSound(int audioIndex)
    {
        GameObject.Find("Audio Source").GetComponent<AudioMgr>().playSound(audioIndex);
    }




}
