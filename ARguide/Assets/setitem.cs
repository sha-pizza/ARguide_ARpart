using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setitem : MonoBehaviour
{
    public Dropdown dropdown;
    public GPSMgr temp;
    public InputField InputText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setitem()
    {
        temp = GameObject.Find("/GPSMgr").GetComponent("GPSMgr") as GPSMgr;
        if (temp)
        {
            dropdown.ClearOptions();
            temp.Searchdropdown(dropdown, InputText);
        }
        else
        {
            Debug.Log("No game object called GPSMgr found:jinryeong");
        }
    }

}
