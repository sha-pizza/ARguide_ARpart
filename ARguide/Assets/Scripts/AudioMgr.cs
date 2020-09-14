using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AudioMgr : MonoBehaviour
{

    public static AudioSource musicPlayer;
    public static AudioClip[] audios;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        audios = new AudioClip[40];
        audios[0] = Resources.Load<AudioClip>("Sound/말풍선눌러줘");
        audios[1] = Resources.Load<AudioClip>("Sound/출발해보자");
        audios[2] = Resources.Load<AudioClip>("Sound/멀어지면종료조심");
        audios[3] = Resources.Load<AudioClip>("Sound/어디보자");
        audios[4] = Resources.Load<AudioClip>("Sound/얼른와");
        audios[5] = Resources.Load<AudioClip>("Sound/설명UI참고해줘");
        audios[6] = Resources.Load<AudioClip>("Sound/건물자세한설명듣고싶니");

        audios[7] = Resources.Load<AudioClip>("Sound/생명공학관도착");
        audios[8] = Resources.Load<AudioClip>("Sound/제1공학관도착");
        audios[9] = Resources.Load<AudioClip>("Sound/제2공학관도착");
        audios[10] = Resources.Load<AudioClip>("Sound/제1과학관도착");
        audios[11] = Resources.Load<AudioClip>("Sound/제2과학관도착");
        audios[12] = Resources.Load<AudioClip>("Sound/기초학문관도착");
        audios[13] = Resources.Load<AudioClip>("Sound/화학관도착");
        audios[14] = Resources.Load<AudioClip>("Sound/반도체관도착");
        audios[15] = Resources.Load<AudioClip>("Sound/삼성학술정보관도착");
        audios[16] = Resources.Load<AudioClip>("Sound/산학협력센터도착");
        audios[17] = Resources.Load<AudioClip>("Sound/학생회관도착");
        audios[18] = Resources.Load<AudioClip>("Sound/복지회관도착");
        audios[19] = Resources.Load<AudioClip>("Sound/약학관도착");
        audios[20] = Resources.Load<AudioClip>("Sound/의학관도착");
        audios[21] = Resources.Load<AudioClip>("Sound/신관A동도착");
        audios[22] = Resources.Load<AudioClip>("Sound/신관B동도착");
        audios[23] = Resources.Load<AudioClip>("Sound/인관도착");
        audios[24] = Resources.Load<AudioClip>("Sound/의관도착");
        audios[25] = Resources.Load<AudioClip>("Sound/예관도착");
        audios[26] = Resources.Load<AudioClip>("Sound/정문도착");
        audios[27] = Resources.Load<AudioClip>("Sound/후문도착");

        audios[28] = Resources.Load<AudioClip>("Sound/생명공학관설명");
        audios[29] = Resources.Load<AudioClip>("Sound/제1공학관설명2");
        audios[30] = Resources.Load<AudioClip>("Sound/제2공학관설명3");
        audios[31] = Resources.Load<AudioClip>("Sound/제1과학관설명");
        audios[32] = Resources.Load<AudioClip>("Sound/제2과학관설명");
        audios[33] = Resources.Load<AudioClip>("Sound/기초학문관설명");
        audios[34] = Resources.Load<AudioClip>("Sound/화학관설명");
        audios[35] = Resources.Load<AudioClip>("Sound/반도체관설명");
        audios[36] = Resources.Load<AudioClip>("Sound/삼성학술정보관설명");
        audios[37] = Resources.Load<AudioClip>("Sound/산학협력센터설명");
        audios[38] = Resources.Load<AudioClip>("Sound/학생회관설명");
        audios[39] = Resources.Load<AudioClip>("Sound/복지회관설명");

        musicPlayer.clip = audios[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSound(int audioIndex)
    {
        musicPlayer.Stop();
        musicPlayer.clip = audios[audioIndex];
        musicPlayer.time = 0;
        musicPlayer.Play();
    }
}
