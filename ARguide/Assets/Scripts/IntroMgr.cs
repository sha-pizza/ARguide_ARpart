using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroMgr : MonoBehaviour
{
    enum introState {
        page1,
        page2,
        page3
    }
    introState state;

    Transform ui_Intro;
    float screenWidth;


    Button prevbtn;
    Button nextbtn;
    Button startbtn;

    RectTransform page1img;
    RectTransform page2img;
    RectTransform page3img;

    RectTransform indicator;

    Button introBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        //introBtn = GameObject.Find("UICanvas/ui_Intro").GetComponent<Button>();
        //introBtn.onClick.AddListener(TaskOnClick);

        // manage state
        state = introState.page1;

        // find objects
        ui_Intro = GameObject.Find("UICanvas/ui_Intro").transform;
        screenWidth = ui_Intro.GetComponent<RectTransform>().rect.width;

        prevbtn = ui_Intro.Find("prevbtn").GetComponent<Button>();
        nextbtn = ui_Intro.Find("nextbtn").GetComponent<Button>();
        startbtn = ui_Intro.Find("startbtn").GetComponent<Button>();

        page1img = ui_Intro.Find("imgholder/img1").GetComponent<RectTransform>();
        page2img = ui_Intro.Find("imgholder/img2").GetComponent<RectTransform>();
        page3img = ui_Intro.Find("imgholder/img3").GetComponent<RectTransform>();

        indicator = ui_Intro.Find("indicatorholder/indicator").GetComponent<RectTransform>();


        // set objects

        prevbtn.onClick.AddListener(TaskOnClickPrevbtn);
        nextbtn.onClick.AddListener(TaskOnClickNextbtn);
        startbtn.onClick.AddListener(TaskOnClickStartbtn);
        prevbtn.gameObject.SetActive(false);
        startbtn.gameObject.SetActive(false);

        page2img.localPosition = new Vector2(screenWidth, 0);
        page3img.localPosition = new Vector2(screenWidth, 0);

        indicator.localPosition = new Vector2(-70, 0);

    


    }

    // 이전 버튼
    void TaskOnClickPrevbtn(){
    }

    // 다음 버튼
    void TaskOnClickNextbtn(){
        // page 1 -> 2
        if (state == introState.page1){
            state = introState.page2;

            // page1 out, page2 in
            StartCoroutine(uiLerp(page1img, page1img.localPosition, new Vector2(-1*screenWidth, 0), 0.2f, 1f));
            StartCoroutine(uiLerp(page2img, page2img.localPosition, new Vector2(0, 0), 0.2f, 1f));
            // indicator
            StartCoroutine( uiLerp(indicator, indicator.localPosition, new Vector2(0, 0), 0.3f, 1f) );

        } else if (state == introState.page2){
            state = introState.page3;

            // page2 out, page3 in
            StartCoroutine(uiLerp(page2img, page1img.localPosition, new Vector2(-1*screenWidth, 0), 0.2f, 1f));
            StartCoroutine(uiLerp(page3img, page2img.localPosition, new Vector2(0, 0), 0.2f, 1f));
            // indicator
            StartCoroutine( uiLerp(indicator, indicator.localPosition, new Vector2(70, 0), 0.3f, 1f) );

            prevbtn.gameObject.SetActive(false);
            nextbtn.gameObject.SetActive(false);
            startbtn.gameObject.SetActive(true);
        }

    }

    // 시작하기 버튼
    void TaskOnClickStartbtn(){
        StateMgr.requestStateChange(StateMgr.state.INTRO, StateMgr.state.MAP);
    }

	void TaskOnClick(){
		StateMgr.requestStateChange(StateMgr.state.INTRO, StateMgr.state.MAP);
	}

    IEnumerator uiLerp(RectTransform rect, Vector2 startpos, Vector2 endpos, float lerpvalue, float duration){
        int movecount = (int)(duration/0.03f);
        Debug.Log("ARGUIDE_Intro : moveCount : "+movecount);

        for (int i=0 ; i<movecount ; i++){
            yield return new WaitForSeconds(0.03f);
            rect.localPosition = Vector2.Lerp(rect.localPosition, endpos, lerpvalue);
        }
        rect.localPosition = endpos;
    }
    
}
