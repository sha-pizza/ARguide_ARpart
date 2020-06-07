using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inputscript : MonoBehaviour
{
    public InputField InputText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Test(Text text)
    {
        text.text = InputText.text;
    }

    public void Newvalue(Text text)
    {
        text.text = InputText.text;
    }
}
