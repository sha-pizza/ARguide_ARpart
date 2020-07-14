using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_facemaker_v2 : MonoBehaviour
{
    [Header("material")]
    [SerializeField] Material mascot_facemat;
    [SerializeField] Material mascot_mousemat;

    [Header("idle facetex")]
    [SerializeField] Texture idle1;
    [SerializeField] Texture idle2;
    [SerializeField] Texture idle3;
    [SerializeField] Texture idle4;

    [Header("bling facetex")]
    [SerializeField] Texture bling1;
    [SerializeField] Texture bling2;
    [SerializeField] Texture bling3;
    [SerializeField] Texture bling4;
    [SerializeField] Texture bling5;

    [Header("smile facetex")]
    [SerializeField] Texture smile1;

    [Header("close facetex")]
    [SerializeField] Texture close1;


    [Header("idle mousetex")]
    [SerializeField] Texture idleM1;


    [Header("talking mousetex")]
    [SerializeField] Texture talk1;
    [SerializeField] Texture talk2;
    [SerializeField] Texture talk3;
    [SerializeField] Texture talk4;
    [SerializeField] Texture talk5;
    [SerializeField] Texture talk6;

    [Header("close mousetex")]
    [SerializeField] Texture closeM1;

    


    
    private Animator mascot_animator;

    private AnimatorClipInfo[] clipInfo;
    private string clipName = "";
    private string clipName_crnt = "";

    // 얼굴&입 상태
    public enum faceState { idle, bling, smile, close };
    public faceState fState;
    public enum mouseState { idle, talking, close };
    public mouseState mState;

    // parti
    private ParticleSystem parti_flower;
    private ParticleSystem parti_spark;

    

    int Fcount;

    // Start is called before the first frame update
    void Start()
    {
        // find objects
        mascot_animator = transform.gameObject.GetComponent<Animator>();
        parti_flower = transform.Find("parti_flower").GetComponent<ParticleSystem>();
        parti_spark = transform.Find("parti_spark").GetComponent<ParticleSystem>();

        IEnumerator tCorout = TexCtrler();
        StartCoroutine(tCorout);

      
    }


    // Update : clip 확인하고 state 수정
    // IEnumerator TexCtrler : 0.03초마다 state 확인하고 코루틴 실행
    // IEnumerator ~Ctrler : 각 얼굴/입 표정 담당

    // Update is called once per frame
    void Update()
    {   
        
        // check current animationclip
        clipInfo = mascot_animator.GetCurrentAnimatorClipInfo(0);
        clipName = clipInfo[0].clip.name;
        Fcount++;
        
        // 애니메이션클립이 바뀌었을때 한번만 실행
        if (clipName == "m_starthi"){
            if (clipName_crnt != clipName){
                clipName_crnt = clipName;
                fState = faceState.smile;
                mState = mouseState.talking;

                parti_flower.gameObject.SetActive(true);
                parti_flower.Play();
                parti_spark.gameObject.SetActive(false);
            }
        } else if (clipName == "m_startwait"){
            if (clipName_crnt != clipName){
                clipName_crnt = clipName;
                fState = faceState.idle;
                mState = mouseState.close;

                parti_flower.gameObject.SetActive(false);
                parti_spark.gameObject.SetActive(false);
            }
        } else if (clipName == "m_idlecute"){
            if (clipName_crnt != clipName){
                clipName_crnt = clipName;
                fState = faceState.bling;
                mState = mouseState.idle;

                parti_flower.gameObject.SetActive(false);
                parti_spark.gameObject.SetActive(true);
                parti_spark.Play();
            }
        } else if (clipName == "m_run"){
            if (clipName_crnt != clipName){
                clipName_crnt = clipName;
                fState = faceState.smile;
                mState = mouseState.idle;

                parti_flower.gameObject.SetActive(false);
                parti_spark.gameObject.SetActive(false);
            }
        } else if (clipName == "m_talk"){
            if (clipName_crnt != clipName){
                clipName_crnt = clipName;
                fState = faceState.close;
                mState = mouseState.talking;

                parti_flower.gameObject.SetActive(false);
                parti_spark.gameObject.SetActive(false);
            }
        } else {
            if (clipName_crnt != clipName){
                clipName_crnt = clipName;
                fState = faceState.idle;
                mState = mouseState.idle;

                parti_flower.gameObject.SetActive(false);
                parti_spark.gameObject.SetActive(false);              
            }
        }

        
    }

    IEnumerator TexCtrler (){
        faceState fState_crnt = faceState.idle;
        mouseState mState_crnt = mouseState.idle;

        while (true){
            yield return new WaitForSeconds(0.03f);

            // 얼굴 변경시 얼굴 코루틴 실행
            if (fState == faceState.idle && fState_crnt != fState){
                fState_crnt = fState;
                IEnumerator fCorout = idleFaceCtrler();
                StartCoroutine(fCorout);
            } else if (fState == faceState.bling && fState_crnt != fState){
                fState_crnt = fState;
                IEnumerator fCorout = blingFaceCtrler();
                StartCoroutine(fCorout);
            } else if (fState == faceState.smile && fState_crnt != fState){
                fState_crnt = fState;
                IEnumerator fCorout = smileFaceCtrler();
                StartCoroutine(fCorout);
            } else if (fState == faceState.close && fState_crnt != fState){
                fState_crnt = fState;
                IEnumerator fCorout = closeFaceCtrler();
                StartCoroutine(fCorout);
            }

            // 입 변경시 입 코루틴 실행
            if (mState == mouseState.idle && mState_crnt != mState) {
                mState_crnt = mState;
                IEnumerator mCorout = idleMouseCtrler();
                StartCoroutine(mCorout);
            } else if (mState == mouseState.talking && mState_crnt != mState) {
                mState_crnt = mState;
                IEnumerator mCorout = talkingMouseCtrler();
                StartCoroutine(mCorout);
            } else if (mState == mouseState.close && mState_crnt != mState) {
                mState_crnt = mState;
                IEnumerator mCorout = closeMouseCtrler();
                StartCoroutine(mCorout);
            }
        }
    }


    // face Coroutine 
    IEnumerator idleFaceCtrler (){
        while (fState == faceState.idle){
            Debug.Log(Fcount+" : FACE : idle ");
            yield return new WaitForSeconds(1.2f);

            mascot_facemat.SetTexture("_MainTex",idle2);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",idle3);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",idle4);
            yield return new WaitForSeconds(0.5f);

            mascot_facemat.SetTexture("_MainTex",idle3);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",idle2);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",idle1);
        }
    }

    IEnumerator blingFaceCtrler (){
        while (fState == faceState.bling){
            Debug.Log(Fcount+" : FACE : bling ");
            mascot_facemat.SetTexture("_MainTex",bling1);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",bling2);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",bling3);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",bling4);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",bling5);
            yield return new WaitForSeconds(0.03f);

            mascot_facemat.SetTexture("_MainTex",bling4);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",bling3);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",bling2);
            yield return new WaitForSeconds(0.03f);
            mascot_facemat.SetTexture("_MainTex",bling1);
        }
    }

    IEnumerator smileFaceCtrler (){
        while (fState == faceState.smile){
            Debug.Log(Fcount+" : FACE : smile ");
            mascot_facemat.SetTexture("_MainTex",smile1);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator closeFaceCtrler (){
        while (fState == faceState.close){
            Debug.Log(Fcount+" : FACE : close ");
            mascot_facemat.SetTexture("_MainTex",close1);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // mouse Coroutine 
    IEnumerator idleMouseCtrler (){
        while (mState == mouseState.idle){
            Debug.Log(Fcount+" : MOUSE : idle ");
            mascot_mousemat.SetTexture("_MainTex",talk1);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator talkingMouseCtrler (){
        while (mState == mouseState.talking){
            Debug.Log(Fcount+" : MOUSE : talking ");
            yield return new WaitForSeconds(1.0f);
            
            mascot_mousemat.SetTexture("_MainTex",talk1);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk2);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk3);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk4);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk5);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk6);
            yield return new WaitForSeconds(0.1f);

            mascot_mousemat.SetTexture("_MainTex",talk5);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk4);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk3);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk2);
            yield return new WaitForSeconds(0.03f);
            mascot_mousemat.SetTexture("_MainTex",talk1);
        }
    }

    IEnumerator closeMouseCtrler (){
        while (mState == mouseState.close){
            Debug.Log(Fcount+" : MOUSE : close ");
            mascot_mousemat.SetTexture("_MainTex",closeM1);
            yield return new WaitForSeconds(0.1f);
        }
    }


    // olds
    /*
    IEnumerator idleBlinker(float waittime, float eachtime, float blinktime){
        while(isIdleFace){
            Debug.Log(Fcount+" : BLINKER : idleBlinker");
            yield return new WaitForSeconds(waittime);
            
            mascot_facemat.SetTexture("_MainTex",idle2);
            yield return new WaitForSeconds(eachtime);

            mascot_facemat.SetTexture("_MainTex",idle3);
            yield return new WaitForSeconds(eachtime);

            mascot_facemat.SetTexture("_MainTex",idle4);
            yield return new WaitForSeconds(blinktime);

            mascot_facemat.SetTexture("_MainTex",idle3);
            yield return new WaitForSeconds(eachtime);

            mascot_facemat.SetTexture("_MainTex",idle2);
            yield return new WaitForSeconds(eachtime);

            mascot_facemat.SetTexture("_MainTex",idle1);
           
        }
    }

    IEnumerator talkBlinker(float waittime, float eachtime ,float blinktime){
        while(isTalkingFace){
            Debug.Log(Fcount+" : BLINKER : talkBlinker");
            yield return new WaitForSeconds(waittime);
            
            mascot_facemat.SetTexture("_MainTex",talk1);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk2);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk3);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk4);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk5);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk6);
            yield return new WaitForSeconds(blinktime);

            mascot_facemat.SetTexture("_MainTex",talk5);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk4);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk3);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk2);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",talk1);
          
        }
    }

    IEnumerator blingBlinker(float waittime, float eachtime ,float blinktime) {
        while(isIdleCuteFace){
            Debug.Log(Fcount+" : BLINKER : cuteBlinker");
            mascot_facemat.SetTexture("_MainTex",bling1);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",bling2);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",bling3);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",bling4);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",bling5);
            yield return new WaitForSeconds(eachtime);

            mascot_facemat.SetTexture("_MainTex",bling4);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",bling3);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",bling2);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex",bling1);
        }
    }

    
    IEnumerator talkOnBlinker(float waittime, float eachtime ,float blinktime) {
        while(isStartHiFace){
            Debug.Log(Fcount+" : BLINKER : talkOnBlinker");
            mascot_facemat.SetTexture("_MainTex", talkOn01);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn02);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn03);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn04);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn05);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn06);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn07);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn08);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn09);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn10);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn11);
            yield return new WaitForSeconds(eachtime);
            mascot_facemat.SetTexture("_MainTex", talkOn12);
            yield return new WaitForSeconds(eachtime);

        }
    }
    */
}
