using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RayMgr : MonoBehaviour
{

    [Header ("rayMgr : raycast로 말풍선 오브젝트에 대한 클릭 감지")]
    public Transform Nothing;

    public static bool isBubbleClicked = false;
    public static bool isSampleMRClicked = false;

  

    // Start is called before the first frame update
    void Start()
    {      
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            // get touch
            Touch touch = Input.GetTouch(0); 
            Vector3 TouchPosition = new Vector3(touch.position.x, touch.position.y, 100);
            // cast a ray
            Ray ray = Camera.main.ScreenPointToRay(TouchPosition);
            RaycastHit hit;

            
            // check collided object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
                if (hit.collider.tag == "speechBubble"){
                    isBubbleClicked = true;
                } 
            } 
        }        
    }
}
