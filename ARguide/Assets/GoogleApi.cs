using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GoogleApi : MonoBehaviour
{
    public RawImage img;

    private void Awake()
    {
        img = this.gameObject.GetComponent<RawImage>();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetTexture());
    }

    IEnumerator GetTexture()
    {
        string url;
        url = "https://maps.googleapis.com/maps/api/staticmap?";
        string center;
        string zoom;
        string size;
        string key;
        string markers;

        center = "center=37.294429%2c%20126.974268";
        zoom = "zoom=16";
        size = "size=300x300";
        key = "key=AIzaSyCkT-dh4GaQ4uPTCsWva84lg1D84BJemd0";
        markers = "markers=color:blue|37.29421496%2c%20126.9750168|37.29364566%2c%20126.9748691|37.296032%2c%20126.975092";
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
            img.SetNativeSize();
        }

    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
