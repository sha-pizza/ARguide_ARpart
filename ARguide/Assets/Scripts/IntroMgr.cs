using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroMgr : MonoBehaviour
{
    Button introBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        introBtn = GameObject.Find("UICanvas/ui_Intro").GetComponent<Button>();
        introBtn.onClick.AddListener(TaskOnClick);
    }

	void TaskOnClick(){
		StateMgr.requestStateChange(StateMgr.state.INTRO, StateMgr.state.MAP);
	}
    
}
