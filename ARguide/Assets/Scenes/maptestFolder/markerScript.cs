using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class markerScript : MonoBehaviour
{
    public Button debugBtn;
    public double[] coord = new double[2];

    // Start is called before the first frame update
    void Start()
    {
        debugBtn = transform.GetComponent<Button>();
		debugBtn.onClick.AddListener(makeLog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void makeLog(){
        Debug.Log("this marker : "+coord[0]+" , "+coord[1]+" !");
    }
}
