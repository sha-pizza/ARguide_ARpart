using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testscript : MonoBehaviour
{
    public Text t;
    public InputField latinput;
    // Start is called before the first frame update
    void Start()
    {
        latinput = GameObject.Find("/LATInput").GetComponent<InputField>();
        t = this.gameObject.GetComponent<Text>();
        
        string test1 = latinput.text;
        Debug.Log(test1);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
