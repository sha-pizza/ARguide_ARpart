using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_sample : MonoBehaviour
{
    private Renderer Mascot_samplemat;
    public static bool isOnPlane = false;

    // Start is called before the first frame update
    void Start()
    {
        Mascot_samplemat = transform.GetComponent<Renderer>();
        //Mascot_samplemat = transform.Find("SampleMR/MASCOT_MR_sample/root_1").GetComponent<Renderer>();
        //Mascot_samplecollider = transform.Find("SampleMR/MASCOT_MR_sample/root_1").GetComponent<Collider>();

        StartCoroutine(changeEmission());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col){
        // detectedPlane과 충돌하면 true
        // 현재 horizontal 평면만 인식하도록 되어 있음
        if (col.gameObject.tag == "detectedPlane"){
            isOnPlane = true;
        }
        
    }

    void OnTriggerExit(Collider col){
        isOnPlane = false;
    }

    IEnumerator changeEmission(){
        while(true){
            yield return new WaitForSeconds (0.2f);

            if (isOnPlane == true){
                Mascot_samplemat.material.SetColor("_EmissionColor", new Color(0, 1f, 0.7f, 0.5f));
            } else {
                //Mascot_samplemat.material.SetColor("_EmissionColor", new Color(0.9f, 0, 0.1f, 0.5f));
                Mascot_samplemat.material.SetColor("_EmissionColor", new Color(0, 1f, 0.7f, 0.1f));
            }
        }
    }
}
