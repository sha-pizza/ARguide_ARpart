using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class googlemapscript : MonoBehaviour
{
    public RawImage img;
    public Dropdown dropdown;
    public GPSMgr temp;

    public string url;
    string center;
    string zoom;
    string size;
    string key;
    string markers;

    

    private void Awake()
    {
        img = this.gameObject.GetComponent<RawImage>();
    }
    // Start is called before the first frame update
    void Start()
    {
        url = "https://maps.googleapis.com/maps/api/staticmap?";
        center = "center=37.294429%2c%20126.974268";
        zoom = "zoom=16";
        size = "size=1200x1200";
        key = "key=AIzaSyCkT-dh4GaQ4uPTCsWva84lg1D84BJemd0";
        markers = "markers=color:blue";
        StartCoroutine(GetTexture());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GetTexture()
    {
        /*
        url = "https://maps.googleapis.com/maps/api/staticmap?";
        center = "center=37.294429%2c%20126.974268";
        zoom = "zoom=16";
        size = "size=700x700";
        key = "key=AIzaSyCkT-dh4GaQ4uPTCsWva84lg1D84BJemd0";
        markers = "";*/
        //markers = "markers=color:blue|37.29421496%2c%20126.9750168|37.29364566%2c%20126.9748691|37.296032%2c%20126.975092";
        url = url + center + "&" + zoom + "&" + size + "&" + markers + "&" + key;



        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);


        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            img.texture = myTexture;
            //img.SetNativeSize();
            img.rectTransform.sizeDelta = new Vector2(1000, 1000);
        }


    }

    public void SetMap()
    {
        Debug.Log("setmap start");
        markers = "markers=color:blue";
        url = "https://maps.googleapis.com/maps/api/staticmap?";
        //temp = GameObject.Find("/GPSMgr").GetComponent("GPSMgr") as GPSMgr;
        //dropdown = GameObject.Find("/Dropdown2").GetComponent("Dropdown") as Dropdown;
        Debug.Log("setmap start: dropdown value" + dropdown.value);
        Debug.Log("setmap start: lat" + temp.latslist[0]);
        Debug.Log("setmap start: lat" + temp.latslist[dropdown.value]);
        Debug.Log("setmap start: long" + temp.longslist[dropdown.value]);
        markers = "markers=color:blue|" + temp.latslist[dropdown.value] + "%2c%20" + temp.longslist[dropdown.value];
        
        
        StartCoroutine(GetTexture());
    }
}
