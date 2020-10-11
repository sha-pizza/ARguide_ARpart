using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour
{
    [Header("StateMgr : State 정의 및 getter, setter / state에 따라 ui 수정 및 값 설정 스크립트")]
    public Transform nothing;

    // state
    public enum state {
        INTRO,              // 인트로 페이지 로드
                            // -> map : 확인 버튼 누를 시 전환
        MAP,                // 지도 ui 로드, 포함된 건물들 핀 로드
                            // -> map_searchByText : 텍스트로 검색할 시 전환
                            // -> map_searchByTap : 건물들 핀 중 하나를 눌렀을 시 전환
                            // 이후 투어모드와 세팅 추가
        GUIDE

        //tourMode,
        //setting,
    }

    [Header("ARGUIDE_STATE")]
    [SerializeField] private static state arguide_state;
    private state previous_state;

    // active, inactive 설정을 위한 변수들
    private static GameObject UI_Intro;
    private static GameObject UI_Map;
    private static GameObject UI_AR;

    // 각 Mgr 찾기
    private static IntroMgr Mgr_Intro;
    private static MapMgr Mgr_Map;
    private static GuideMgr Mgr_Guide;


    // Start is called before the first frame update
    void Start()
    {
        arguide_state = state.INTRO;
        previous_state = arguide_state;

        // 조절할 ui들의 위치와, 요청 보내야 할 Mgr들 받아두기
        UI_Intro = GameObject.Find("UICanvas/ui_Intro");
        UI_Map = GameObject.Find("UICanvas/ui_Map");
        UI_AR = GameObject.Find("UICanvas/ui_AR");
        
        UI_AR.SetActive(false);

        // 매니저들 받아두기
        Mgr_Intro = GameObject.Find("IntroMgr").GetComponent<IntroMgr>();
        Mgr_Map = GameObject.Find("MapMgr").GetComponent<MapMgr>();
        Mgr_Guide = GameObject.Find("GuideMgr").GetComponent<GuideMgr>();

        Mgr_Map.gameObject.SetActive(false);
        Mgr_Guide.gameObject.SetActive(false);

    }

    // Update is called once per frame
    /*    void Update()
    {
        if (previous_state != arguide_state){
            ProcessUI(previous_state, arguide_state);
            previous_state = arguide_state;
        }
    }
    */
   
    // getter & setter of ARGUIDE_STATE
    public static state getState(){
        return arguide_state;
    }

    // 외부 Mgr에서 온 스테이트 변경 리퀘스트를 받아 처리
    public static bool requestStateChange(state prevS, state nextS){
        // check request
        // req(현상태, 변경요청상태)에서 현상태가 stateMgr에 저장된것과 같은지 확인
        if (prevS != arguide_state){
            Debug.Log("ARGUIDE_stateMgr : wrong request : prev state is "+arguide_state+" but "+prevS+" -> "+nextS+" requested");
            return false;
        }

        // INTRO --> MAP
        if (arguide_state == state.INTRO && nextS == state.MAP){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;

            // ui 전환
            UI_Intro.SetActive(false);

            // mgr 수정
            Mgr_Map.gameObject.SetActive(true);
               

            return true;
        } 

        // prohibit else cases
        else {
            Debug.Log("ARGUIDE_stateMgr : prohibit change : "+arguide_state+" -> "+nextS);
            return false;
        }

        /*
        // from MAP
        else if (arguide_state == state.MAP && nextS == state.MAP_SEARCH_BY_TEXT){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } else if (arguide_state == state.MAP && nextS == state.MAP_SEARCH_BY_TAP){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } 
        // from MAP_SEARCH_BY_TEXT
        else if (arguide_state == state.MAP_SEARCH_BY_TEXT && nextS == state.MAP){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } else if (arguide_state == state.MAP_SEARCH_BY_TEXT && nextS == state.MAP_SET_DESTIN){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } 
        // from MAP_SEARCH_BY_TAP
        else if (arguide_state == state.MAP_SEARCH_BY_TAP && nextS == state.MAP){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } else if (arguide_state == state.MAP_SEARCH_BY_TAP && nextS == state.MAP_SET_DESTIN){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        }
        // from MAP_SET_DESTIN
        else if (arguide_state == state.MAP_SET_DESTIN && nextS == state.MAP_SEARCH_BY_TEXT){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } else if (arguide_state == state.MAP_SET_DESTIN && nextS == state.MAP_SEARCH_BY_TAP){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } else if (arguide_state == state.MAP_SET_DESTIN && nextS == state.GUIDE_START){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } 
        // from GUIDE_START
        else if (arguide_state == state.GUIDE_START && nextS == state.GUIDE_PART){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } 
        // from GUIDE_PART
        else if (arguide_state == state.GUIDE_PART && nextS == state.GUIDE_END){
            Debug.Log("ARGUIDE_stateMgr : allow change : "+arguide_state+" -> "+nextS);
            arguide_state = nextS;
            return true;
        } 
        
        // prohibit else cases
        else {
            Debug.Log("ARGUIDE_stateMgr : prohibit change : "+arguide_state+" -> "+nextS);
            return false;
        }
        */
    }


    
}
