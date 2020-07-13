using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class markerVisualizer : MonoBehaviour
{

    [Header("마커 오브젝트")]
    [SerializeField] public GameObject marker;

    [Header("예시 좌표")]
    public double[] coord1 = {37.290900, 126.979797};
    public double[] coord2 = {37.292929, 126.978787};
    public double[] coord3 = {37.293636, 126.979000};
    public double[] coord4 = {37.294500, 126.976565};
    public double[] coord5 = {37.296868, 126.973456};

    [Header("좌표리스트 / 생성된 마커 리스트")]
    public List<double[]> array_coords = new List<double[]>();
    public List<GameObject> array_markers = new List<GameObject>();

    public LineRenderer routeMaker;

    // Start is called before the first frame update
    void Start()
    {
        routeMaker = transform.Find("routeMaker").GetComponent<LineRenderer>();

        setMarker(coord1[0], coord1[1]);
        setMarker(coord2[0], coord2[1]);
        setMarker(coord3[0], coord3[1]);
        setMarker(coord4[0], coord4[1]);
        setMarker(coord5[0], coord5[1]);

        setRoute();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 적당히 값을 수정해서, 좌표에 해당하는 위치에 marker 생성하는 메서드
    // 순서대로 값을 array_coords에 저장하고, 치환된 값은 array_markers에 같은 인덱스로 저장해둠
    void setMarker(double lat, double lon){
        // 값 치환
        double calculed_lat = (lat - 37.298) * 1000;
        double calculed_lon = (lon - 126.969) * 1000;

        // 오브젝트 생성하고 route 게임오브젝트 자식으로 설정
        GameObject newmarker = Instantiate(marker, new Vector3(0,0,0), Quaternion.identity);
        newmarker.transform.parent = gameObject.transform;

        // 적당한 위치로 이동 : localposition 수정
        RectTransform markerRT = newmarker.GetComponent<RectTransform>();
        markerRT.localPosition = new Vector3((float)calculed_lat, (float)calculed_lon, 0);
        markerRT.localRotation = Quaternion.identity;

        // marker오브젝트 내부의 markerScript에 좌표값 저장 (좌표 클릭 이벤트용)
        markerScript mS = newmarker.transform.GetComponent<markerScript>();
        mS.coord[0] = lat;
        mS.coord[1] = lon;

        // array_coord에 좌표 저장
        // array_markers에 마커 오브젝트 저장
        double[] tmpD = {lat,lon};
        array_coords.Add( tmpD );
        array_markers.Add( newmarker );

    }

    // list 순서대로 경로 그리기
    void setRoute(){
        routeMaker.positionCount = 5;

        routeMaker.SetPosition(0, array_markers[0].transform.GetComponent<RectTransform>().localPosition);
        routeMaker.SetPosition(1, array_markers[1].transform.GetComponent<RectTransform>().localPosition);
        routeMaker.SetPosition(2, array_markers[2].transform.GetComponent<RectTransform>().localPosition);
        routeMaker.SetPosition(3, array_markers[3].transform.GetComponent<RectTransform>().localPosition);
        routeMaker.SetPosition(4, array_markers[4].transform.GetComponent<RectTransform>().localPosition);

    }
}
