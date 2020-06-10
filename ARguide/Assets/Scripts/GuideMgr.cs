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
    
    //public static double[] route = {37.295600, 126.976000, 37.295400, 126.976000, 37.295400, 126.976200}; 
    public static double[] route = {37.295400, 126.976000, 37.295400, 126.976200}; 
    //public static double[] route = {37.296363, 126.975296, 37.296363, 126.975296};       
    public static int nowPointNum = 0;  // 현재 향하는 좌표! nowPoint0일때 rount0,1지점으로 향한다
    int lastPointNum;                   // 마지막 좌표! 전체 좌표들의 개수와도 같다
    
    [Header ("안내 관련 값")]
    private double minD = 0.000100;              // 목표 좌표의 +=minD 거리에 도달하면 도착한것으로!

    public double dist_wait = 2.4;             // 마스코트가 유저와 이만큼 떨어져 있을 경우 기다림
    public double dist_warning = 7.0;          // 마스코트가 유저와 이만큼 떨어져 있을 경우 종료 방지 위해 안내
    public double dist_disable = 10.0;         // 마스코트가 유저와 이만큼 떨어져 있을 경우 비활성화 및 컨텐츠 종료





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
    private GameObject guideBack;



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
            
            //routeInfo = GameObject.Find("DebugCanvas/Routeinfo").GetComponent<Text>();
            guideInfo = GameObject.Find("DebugCanvas/Guideinfo").GetComponent<Text>();
            guideUI = GameObject.Find("DebugCanvas/GuideUI/GuideUIText").GetComponent<Text>();
            guideBack = GameObject.Find("DebugCanvas/GuideUI");
            //guideUI.gameObject.SetActive(false);
            guideBack.gameObject.SetActive(false);
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

            // 경로 가져오기
            route = GPSMgr.route;

            // 전체 좌표 수 계산
            lastPointNum = route.Length  ;

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

        guideInfo.text = "start guide coroutine1";

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

        Mascot_anim.SetBool("isStartGuide", true);

        /* oldscript !
        spchText.text = "좋아,출발해보자!";
        StartCoroutine(SpchBubble_Fadein(0.2f, 1.0f, 0.03f));
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(SpchBubble_Fadeout(1.0f, 0.2f, 0.03f));
        yield return new WaitForSeconds(0.4f);*/
        
        
        

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

        /*routeInfo.text = "guide to : "+nowPointNum+"->"+lastPointNum;
        for (int i = nowPointNum ; i < lastPointNum ; i = i+2 ){
            routeInfo.text += "\n"+i+" : "+(float)route[i]+"/"+(float)route[i+1];
        }*/
       
        // 파트 이동 코루틴 호출
        //guideInfo.text = "start guide part "+nowPointNum+"/"+lastPointNum;
        

        guideInfo.text = "start guide coroutine2";
        nowPointNum = 0;
        int nowPointNumSaver=0;
        //StartCoroutine(Guide_Part((float)GPSMgr.LAT, (float)GPSMgr.LON, route[0], route[1]));
        
        // 0->2 안내 np=0 nps=0
        // 0->2 안내 코루틴 끝날 때 np+2 / np=2 nps=0
        // while 돌리면서, if (np > nps) 이면 nps+2 하고
        // 2->4 안내 코루틴 시작
        
        for (int i = 0 ; i<lastPointNum ; i = i+2){
            while (nowPointNum != i){
                yield return new WaitForSeconds(0.5f);
            }
            guideInfo.text = "np:"+nowPointNum+"/ nps:"+nowPointNumSaver+"/ lp:"+lastPointNum;
            // nowPointnum은 Guide_Part 코루틴 안에서 바꿔줄 예정
            StartCoroutine(Guide_Part(GPSMgr.LAT, GPSMgr.LON, route[i], route[i+1]));
            
        }
        
        
        /*
        StartCoroutine(Guide_Part(GPSMgr.LAT, GPSMgr.LON, route[0], route[1]));
        

        while (nowPointNum != lastPointNum){
            yield return new WaitForSeconds(1.0f); 
            guideInfo.text = "np:"+nowPointNum+"/ nps:"+nowPointNumSaver+"/ lp:"+lastPointNum;
            if (nowPointNum == lastPointNum){
                guideInfo.text += "!@!@!@!@";
                break;
            }

            if (nowPointNum > nowPointNumSaver){
                
                nowPointNumSaver = nowPointNum;
                StartCoroutine(Guide_Part(GPSMgr.LAT, GPSMgr.LON, route[nowPointNum], route[nowPointNum+1]));
            }
        }
        
        guideInfo.text = "~np:"+nowPointNum+"/ nps:"+nowPointNumSaver;
        */
        
        while (nowPointNum != lastPointNum){
            yield return new WaitForSeconds(5f);
        }

        guideInfo.text = "end guiding";
        StartCoroutine(Guide_End());
        
          
    }

    private IEnumerator Guide_Part(double sLAT, double sLON, double eLAT, double eLON){
        // start LAT LON , end LAT LON value of part
        //guideInfo.text = "\nkeep guide";
        yield return new WaitForSeconds(1.0f); 


        Mascot_anim.SetBool("isMove", true);
        /*guideInfo.text = "start guide part "+nowPointNum+"/"+lastPointNum;
        guideInfo.text += "\nlat abs : "+Mathf.Abs((float)(GPSMgr.LAT - eLAT));
        guideInfo.text += "\nlon abs : "+Mathf.Abs((float)(GPSMgr.LON - eLON));*/


                                                 

        // 마스코트가 while 타겟오브젝트와 충분히 가까이 있지 않은 동안
        // DestinationMgr 의 Destination 오브젝트를 향해
        // Lerp회전 하면서 MoveToward한다
        while (  ( Mathf.Abs((float)(GPSMgr.LAT - eLAT)) > minD ) && ( Mathf.Abs((float)(GPSMgr.LON - eLON)) > minD ) ){
            yield return new WaitForSeconds(0.05f);
            //guideInfo.text = "start guide part "+nowPointNum+"/"+lastPointNum;
            //guideInfo.text += "\nlat abs : "+Mathf.Abs((float)(GPSMgr.LAT - eLAT));
            //guideInfo.text += "\nlon abs : "+Mathf.Abs((float)(GPSMgr.LON - eLON));
            //guideInfo.text += "\n"+(float)GPSMgr.LAT+" - "+eLAT;

      
            guideInfo.text = "np:"+nowPointNum+"/ lp:"+lastPointNum;
            // 사용자와의 거리 확인
            float dist = Vector3.Distance( ARCamera.transform.position, Mascot_MR.transform.position );
            guideInfo.text += "\ndistance :"+dist;

            if (dist > dist_disable) {
                // dist_disable 보다 멀리 떨어질 경우 : 모든 코루틴 종료 및 ui 안내
                //guideInfo.text += "\n dist_disable";
                //guideUI.gameObject.SetActive(true);
                guideBack.gameObject.SetActive(true);
                guideUI.text = "마스코트 캐릭터와\n너무 멀리 떨어졌습니다!\n서비스가 종료됩니다.";
                spchBubble.gameObject.SetActive(false);
                // 실제 서비스 종료 추가

            } else if (dist > dist_warning ) {
                // if dist_warning 보다 멀리 떨어질 경우 : 일단 ui로 안내
                //guideInfo.text += "\n dist_warning";
                //guideUI.gameObject.SetActive(true);
                guideBack.gameObject.SetActive(true);
                guideUI.text = "마스코트 캐릭터\n가까이로 이동해 주세요.\n더 멀어질 경우\n서비스가 종료될 수 있습니다.";
                spchBubble.gameObject.SetActive(false);
                // 이후 추가!!

            } else if (dist > dist_wait) {
                
                // dist_wait 보다 멀리 떨어져 있고 마스코트가 앞서갈 경우( 나 > 마스코트 > 목표지점 ) : 기다림
                // dist_wait 보다 멀리 떨어져 있고 마스코트가 뒤에 있을 경우 ( 마스코트 > 나 > 목표지점 ) : 뛴다
                if ( Vector3.Distance(ARCamera.transform.position, DestinationMgr.destination.position) >
                      Vector3.Distance(Mascot_MR.transform.position, DestinationMgr.destination.position)  ){
                    //guideInfo.text += "\n dist_wait";
                    guideUI.text = "";
                    //guideUI.gameObject.SetActive(false);
                    guideBack.gameObject.SetActive(false);
                    
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
                    //guideInfo.text += "\n dist_run";
                    guideUI.text = "";
                    //guideUI.gameObject.SetActive(false);
                    guideBack.gameObject.SetActive(false);

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
                //guideInfo.text += "\n dist_walk";
                guideUI.text = "";
                //guideUI.gameObject.SetActive(false);
                guideBack.gameObject.SetActive(false);

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
        nowPointNum += 2;
        //guideInfo.text += "\n while - end! nP+2";

    }


    private IEnumerator Guide_End(){
        yield return new WaitForSeconds(0.5f);
        //guideInfo.text = "end guide and give info about "+GPSMgr.finalDestination;

        string endSpch = GPSMgr.finalDestination;
        string endInfo = "";

        if (endSpch == "학생회관 "){
            endInfo = "학생회관은 종합행정실, 학생회관식당, 각종 동아리방과 성대 신문사, 성균 Times 등의 언론반 등이 위치해 있는 곳입니다. 만약 동아리에 관심이 있다면 학생회관에 들어가보세요!";
        } else if (endSpch == "복지회관 "){
            endInfo = "복지회관은 교직원식당, 카운슬링센터, 건강센터, 우체국, 은행, 등 각종 교내 편의시설이 위치해 있는 곳입니다.";
        } else if (endSpch == "제1공학관 21동 "){
            endInfo = "제1공학관은 21동부터 23동까지로 나뉘며, ‘ㄷ’자 형태로 구분되어있습니다. 21동에는 정보통신/소프트웨어/공과대학행정실을 비롯한 행정실, CAD 연구실 등 다양한 연구실과 스마트라운지, 스마트갤러리와 같은 시설을 갖추고 있습니다.";
        } else if (endSpch == "제1공학관 22동 "){
            endInfo = "제1공학관은 21동부터 23동까지로 나뉘며, ‘ㄷ’자 형태로 구분되어있습니다. 22동에는 다양한 연구실과 ADIC센터, 프레젠테이션룸, 설계실, 첨단강의실 및 세미나실 등의 시설을 갖추고 있습니다.";
        } else if (endSpch == "제1공학관 23동 "){
            endInfo = "제1공학관은 21동부터 23동까지로 나뉘며, ‘ㄷ’자 형태로 구분되어있습니다. 23동에는 각종 연구시설과 교수 연구실, 캠퍼스관리팀과 세미나실 등을 갖추고 있습니다.";
        } else if (endSpch == "제2공학관 25동 "){
            endInfo = "제2공학관은 25동부터 27동까지로 나뉘며 ‘ㄷ’자 형태로 연결되어 있습니다. 25동에는 다양한 연구실과 실험실, 회의실 등의 시설을 갖추고 있습니다.";
        } else if (endSpch == "제2공학관 26동 "){
            endInfo = "제2공학관은 25동부터 27동까지로 나뉘며 ‘ㄷ’자 형태로 연결되어 있습니다. 26동에는 공대식당을 비롯한 휴게실과 매점, 열람실 등의 편의시설과 첨단강의실, 연구공간이 마련되어 있습니다.";
        } else if (endSpch == "제2공학관 27 "){
            endInfo = "제2공학관은 25동부터 27동까지로 나뉘며 ‘ㄷ’자 형태로 연결되어 있습니다. 27동에는 공학교육혁신센터와 성균어학원, 우주과학기술연구소, 창업기업 사무실 등이 자리하며, 다양한 세미나실, 연구실, 강의실을 갖추고 있습니다.";
        } else if (endSpch == "제1과학관 31동 "){
            endInfo = "제1과학관은 자연과학대학이 주로 사용하는 공간입니다. 제1과학관부터 제2과학관, 기초학문관, 생명공학관까지 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.";
        } else if (endSpch == "제2과학관 32동 "){
            endInfo = "제2과학관은 자연과학대학이 주로 사용하는 공간입니다. 제1과학관과 기초학문관, 생명공학관이 하나로 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.";
        } else if (endSpch == "화학관 "){
            endInfo = "화학관은 약학관과 반도체관을 잇는 종합강의동입니다. 이곳은 첨단강의실과 연구실, 라운지 등의 공간과 슈퍼컴퓨터실, 동위원소실험실, 세포배양실 등 연구에 필요한 최신식 시설을 갖추고 있습니다.";
        } else if (endSpch == "반도체관 "){
            endInfo = "반도체관은 약학관과 반도체관을 잇는 종합강의동으로, 첨단강의실과 연구실 및 실습실 등의 학업 및 연구공간과 워크스테이션실, 디지털콘텐츠스튜디오, SW 스튜디오 등의 최신식 시설을 갖추고 있습니다.";
        } else if (endSpch == "삼성학술정보관 "){
            endInfo = "삼성학술정보관은 국내서 약 42만권, 국외서 약 20만권으로 62만권에 가까운 도서를 소장 중이며, 기본적인 도서관의 기능 뿐 아니라 정보화사회에 걸맞는 다기능 도서관의 역할을 수행하고 있습니다.";
        } else if (endSpch == "기초학문관 "){
            endInfo = "기초학문관은 학부/사범대학행정실 등의 행정공간과 강의실, 연구실 등이 있습니다. 제1과학관과 제2과학관, 생명공학관이 하나로 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.";
        } else if (endSpch == "생명공학관 "){
            endInfo = "생명공학관은 생명공학대학 학생들이 주로 이용하는 공간입니다. 생명공학관은 제1과학관과, 제2과학관, 기초학문관이 하나로 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.";
        } else if (endSpch == "산학협력센터 "){
            endInfo = "산학협력센터는 산학협력단과 연구실, 세미나실이 있으며, 40여개의 창업보육기업 및 실습실 등이 위치해 있습니다.";
        } else {
            endInfo = "도착하였습니다 !";
        }

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

        spchText.text = endSpch + "에 도착했어!";
        spchBubble.gameObject.SetActive(true);
        Invoke("spchBubbleFadein", 0f);
        yield return new WaitForSeconds (2.4f);
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.36f);
        spchBubble.gameObject.SetActive(false);
        yield return new WaitForSeconds (0.1f);

        spchText.text = "이 장소에 대한 설명은 \n아래 ui를 참고해 줘 !";
        spchBubble.gameObject.SetActive(true);
        Invoke("spchBubbleFadein", 0f);
        /*
        yield return new WaitForSeconds (2.4f);
        Invoke("spchBubbleFadeout", 0f);
        yield return new WaitForSeconds (0.36f);
        spchBubble.gameObject.SetActive(false);
        yield return new WaitForSeconds (0.1f);*/
        

     
        
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
