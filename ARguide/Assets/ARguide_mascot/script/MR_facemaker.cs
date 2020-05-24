﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_facemaker : MonoBehaviour
{

    [Header("idle facetex")]
    [SerializeField] Texture idle1;
    [SerializeField] Texture idle2;
    [SerializeField] Texture idle3;
    [SerializeField] Texture idle4;

    [Header("talking facetex")]
    [SerializeField] Texture talk1;
    [SerializeField] Texture talk2;
    [SerializeField] Texture talk3;
    [SerializeField] Texture talk4;
    [SerializeField] Texture talk5;
    [SerializeField] Texture talk6;

    [Header("bling facetex")]
    [SerializeField] Texture bling1;
    [SerializeField] Texture bling2;
    [SerializeField] Texture bling3;
    [SerializeField] Texture bling4;
    [SerializeField] Texture bling5;


    private Material mascot_facemat;
    private Animator mascot_animator;

    private AnimatorClipInfo[] clipInfo;
    private string clipName;

    // is~~Face == true : 해당하는 코루틴이 실행중임을 의미합니다.
    // 개별 코루틴은 is~~Face가 false로 설정되면 중지됩니다.
    private bool isIdleFace;
    private bool isIdleCuteFace;
    private bool isTalkingFace;

    

    int Fcount;

    // Start is called before the first frame update
    void Start()
    {
        // find objects
        mascot_facemat = GameObject.Find("face/face_2").GetComponent<Renderer>().material;
        mascot_animator = GetComponent<Animator>();

        // start coroutine
        //IEnumerator idleBlink = idleBlinker(1.0f, 0.03f, 0.3f);
        //StartCoroutine(idleBlink);
        //IEnumerator talkBlink = talkBlinker(0.1f, 0.03f, 0.1f);
        //StartCoroutine(talkBlink);
        //IEnumerator blingBlink = blingBlinker(0.1f, 0.03f, 0.1f);
        //StartCoroutine(blingBlink);
    }

    // Update is called once per frame
    void Update()
    {   
        
        // check current animationclip
        clipInfo = mascot_animator.GetCurrentAnimatorClipInfo(0);
        clipName = clipInfo[0].clip.name;
        Fcount++;
        //Debug.Log(Fcount+":"+clipName);
        
        

        
        if (clipName == "m_talk"){
            if (isTalkingFace == false){
                isIdleFace = false;
                isIdleCuteFace = false;
                isTalkingFace = true;
                Debug.Log(Fcount+" : FACE : talking");
                IEnumerator talkBlink = talkBlinker(0.1f, 0.03f, 0.1f);
                StartCoroutine(talkBlink);
            }
        } else if (clipName == "m_idlecute"){
            if (isIdleCuteFace == false){
                isIdleFace = false;
                isIdleCuteFace = true;
                isTalkingFace = false;
                Debug.Log(Fcount+" : FACE : cute");
                IEnumerator blingBlink = blingBlinker(0.1f, 0.03f, 0.1f);
                StartCoroutine(blingBlink);
            }
        } else {
            if (isIdleFace == false){
                isIdleFace = true;
                isIdleCuteFace = false;
                isTalkingFace = false;
                Debug.Log(Fcount+" : FACE : idle");
                IEnumerator idleBlink = idleBlinker(1.0f, 0.03f, 0.3f);
                StartCoroutine(idleBlink);
            }
        }

        
    }

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
}
