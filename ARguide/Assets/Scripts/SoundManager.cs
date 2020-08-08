using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip SoundClick;
    AudioSource myAudio;

    public static SoundManager instance;

    void Awake() {
        if(SoundManager.instance == null) {
            SoundManager.instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    public void PlaySound() {
        myAudio.PlayOneShot(SoundClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
