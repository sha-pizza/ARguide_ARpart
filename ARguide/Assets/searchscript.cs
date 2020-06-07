using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class searchscript : MonoBehaviour
{
    public GPSMgr temp;
    public Text text;
    public InputField InputText;

    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void searchLocation()
    {
        temp = GameObject.Find("/GPSMgr").GetComponent("GPSMgr") as GPSMgr;
        if (temp)
        {
            temp.Search(text,InputText);
        }
        else
        {
            Debug.Log("No game object called GPSMgr found:jinryeong");
        }
    }
    */
    

}
