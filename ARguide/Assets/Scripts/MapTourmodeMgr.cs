using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTourmodeMgr : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 투어 경로 포인트들 중 현재 위치로부터 가장 가까운 거리에 있는 포인트 찾기
    // 루트 구성 : 현재위치 ---> 가장 가까웠던 포인트 ---> 시계방향으로 경로 포인트들 따라서 이동, 마지막 포인트까지
    // 지도에 해당 루트 표시
    // 구성한 루트 리턴, 위 작업 도중 실패할 경우 빈 루트 리턴

    public double[] create_Tourroute(){
        double[] route = {};

        return route;
    }
}
