package com.DefaultCompany.arguide;

import android.os.Handler;
import android.os.Message;

public class Location1 extends Thread {

    private Destination[] lists = new Destination[100];
    private Handler mHandler;
    private String query;

    // 생성자
    public Location1(Handler handler, String query) {
        this.query = query;
        this.mHandler = handler;
    }


    @Override
    public void run() {

        android.os.Process.setThreadPriority(android.os.Process.THREAD_PRIORITY_BACKGROUND);
        //건물정보 저장
        // 변수 이름은 엑셀에 저장된 번호
        Destination nine = new Destination("생명공학관61동 입구 1",61,37.296032, 126.975092);
        Destination thirteen = new Destination("제2공학관 26동 입구 1",26,37.2955584, 126.9774348);
        Destination fourteen = new Destination("제1공학관 23동 입구 1",23,37.2941397, 126.977208);
        Destination fifteen = new Destination("제1공학관 22동 입구 1", 22, 37.29409879, 126.97723441);
        Destination sixteen = new Destination("제1공학관 23동 입구 2", 23, 37.2942313, 126.9764795);
        Destination seventeen = new Destination("제1공학관 23동 입구 3", 23,37.2941511, 126.9762426);
        Destination eighteen = new Destination("제1공학관 21동 입구 1", 21, 37.2937529, 126.9762151);
        Destination nineteen = new Destination("제1공학관 21동 입구 2", 21,37.2935472, 126.9767259);
        Destination twenty = new Destination("제1공학관 22동 입구 2", 22, 37.29356306, 126.97692735);
        Destination twentyone = new Destination("반도체관 입구", 40, 37.29173541, 126.97755157);
        Destination twentytwo = new Destination("화학관 입구", 33, 37.29173541, 126.97755157);
        Destination twentythree = new Destination("약학관 입구", 53, 37.29196334, 126.97662929);
        Destination twentyfive = new Destination("의학관 입구", 71, 37.29232035, 126.97337907);
        Destination thirtyone = new Destination("제1과학관 31동 입구 1", 31, 37.2943467, 126.9749003);
        Destination thirtytwo = new Destination("제1과학관 31동 입구 2", 31, 37.2945249, 126.9745645);
        Destination thirtythree = new Destination("제2과학관 32동 입구 1", 32, 37.29476503, 126.97466907);
        Destination thirtyfour = new Destination("제2과학관 32동 입구 2", 32, 37.2949351, 126.9750574);
        Destination thirtyfive = new Destination("제2과학관 32동 입구 3", 32, 37.2950882, 126.9752141);
        Destination thirtysix = new Destination("제1과학관 31동 입구 3", 31, 37.2947687, 126.9755243);
        Destination thirtyseven = new Destination("제1공학관 23동 입구 4", 23, 37.2944154, 126.9762365);
        Destination thirtyeight = new Destination("제1공학관 23동 입구 5", 23, 37.2942944, 126.9760368);
        Destination thirtynine = new Destination("제2공학관 25동 입구 1", 25, 37.2948579, 126.9765595);
        Destination fourty = new Destination("제2공학관 26동 입구 2", 26, 37.2948295, 126.9772252);
        Destination fourtyone = new Destination("제2공학관 27 입구 1", 27, 37.2954567, 126.9766503);
        Destination fourtytwo = new Destination("제2공학관 27 입구 2", 27, 37.2953564, 126.9762973);
        Destination fourtyfour = new Destination("기초학문관 입구 1", 51, 37.2953845, 126.9745237);
        Destination fourtyfive = new Destination("기초학문관 입구 2", 51, 37.2955701, 126.9747193);
        Destination fourtysix = new Destination("기초학문관 입구 3", 51, 37.29550025, 126.9737743);
        Destination fourtyseven = new Destination("생명공학관61동 입구 2", 61, 37.29570906, 126.97382614);

        //건물번호 없는 건물 및 교문은 건물번호 0번
        Destination one = new Destination("후문", 0, 37.29636733, 126.97064236);
        Destination two = new Destination("신관 A동 입구", 0, 37.29643383, 126.971905);
        Destination three = new Destination("신관 B동 입구 1", 0, 37.2964, 126.9723661);
        Destination four = new Destination("신관 B동 입구 2", 0, 37.2961856, 126.9728776);
        Destination five = new Destination("인관 입구", 0, 37.2968223, 126.9738586);
        Destination six = new Destination("의관 입구", 0, 37.2968831, 126.9745931);
        Destination seven = new Destination("예관 입구 1", 0, 37.2966323, 126.975143);
        Destination eight = new Destination("예관 입구 2", 0, 37.2965323, 126.9755221);
        Destination ten = new Destination("산학협력센터 입구 1", 0, 37.2958812, 126.9755917);
        Destination eleven = new Destination("산학협력센터 입구 2", 0, 37.2960426, 126.9757491);
        Destination twelve = new Destination(" 교문", 0, 37.29634503, 126.97647007);
        Destination twentyfour = new Destination("정문", 0, 37.29084496, 126.97419212);
        Destination twentysix = new Destination("복지회관",0,37.29398199, 126.97265598);
        Destination twentyseven = new Destination("학생회관 입구 1", 0, 37.29428292, 126.97382114);
        Destination twentyeight = new Destination("학생회관 입구 2", 0, 37.29369471, 126.9737552);
        Destination twentynine = new Destination("삼성학술정보관 입구 1", 0, 37.29364566, 126.97486913);
        Destination thirty = new Destination("삼성학술정보관 입구 2", 0, 37.29421496, 126.97501682);
        Destination fourtythree = new Destination("산학협력센터 입구 3", 0, 37.2954208, 126.9759323);

        //ATM은 건물번호 1번
        Destination ATM1 = new Destination("ATM1", 1, 37.2963872, 126.971013); // 후문쪽 ATM
        Destination ATM2 = new Destination("ATM2" ,1, 37.29420377, 126.97619748); // 공학관 ATM
        Destination ATM3 = new Destination("ATM3", 1, 37.29373614, 126.97380615); // 학생회관 ATM


        Destination[] buildings = new Destination[47];
        //buildings 배열 초기화
        for(int i=0;i<47;i++){
            buildings[i] = new Destination("",0,0,0);
        }


        buildings[0] = one;
        buildings[1] = two;
        buildings[2] = three;
        buildings[3] = four;
        buildings[4] = five;
        buildings[5] = six;
        buildings[6] = seven;
        buildings[7] = eight;
        buildings[8] = nine;
        buildings[9] = ten;
        buildings[10] = eleven;
        buildings[11] = twelve;
        buildings[12] = thirteen;
        buildings[13] = fourteen;
        buildings[14] = fifteen;
        buildings[15] = sixteen;
        buildings[16] = seventeen;
        buildings[17] = eighteen;
        buildings[18] = nineteen;
        buildings[19] = twenty;
        buildings[20] = twentyone;
        buildings[21] = twentytwo;
        buildings[22] = twentythree;
        buildings[23] = twentyfour;
        buildings[24] = twentyfive;
        buildings[25] = twentysix;
        buildings[26] = twentyseven;
        buildings[27] = twentyeight;
        buildings[28] = twentynine;
        buildings[29] = thirty;
        buildings[30] = thirtyone;
        buildings[31] = thirtytwo;
        buildings[32] = thirtythree;
        buildings[33] = thirtyfour;
        buildings[34] = thirtyfive;
        buildings[35] = thirtysix;
        buildings[36] = thirtyseven;
        buildings[37] = thirtyeight;
        buildings[38] = thirtynine;
        buildings[39] = fourty;
        buildings[40] = fourtyone;
        buildings[41] = fourtytwo;
        buildings[42] = fourtythree;
        buildings[43] = fourtyfour;
        buildings[44] = fourtyfive;
        buildings[45] = fourtysix;
        buildings[46] = fourtyseven;

        Destination[] ATMs = new Destination[3];

        ATMs[0] = ATM1;
        ATMs[1] = ATM2;
        ATMs[2] = ATM3;



        //마커 띄우기 써서 마커 뿌려주기, main에서 해야함 settag 써서 findPOIItemByTag(int)로 검색가능 혹은 이름으로 검색가능


        //검색쿼리

        String temp;

        //lists 초기화
        for(int i=0;i<10;i++){
            lists[i] = new Destination("",0,0,0);
        }

        boolean flag = false;
        //건물번호 검색인지 아닌지 판별하기
        for(int i=0;i<query.length();i++){
	if(Character.isDigit(query.charAt(i)) == false){
		flag = true;
		break;
	}
        }

	int count= 0;

        //이름으로 검색

        if(flag == true){
        
        for (Destination building : buildings) {
            temp = building.getName();
            if (temp.contains(query)) {
                //찾은 목록에 추가
                lists[count] = building;
                count++;
            			}
        		}
	}

        //건물번호로 검색
	//query 길이 예외처리 필요

	if(flag == false){
	int temper;
        for (Destination building : buildings) {
             temper = building.getNumber();
	temp = Integer.toString(temper);
            query = query.substring(0,2);
            if (query.equals(temp)) {
                //찾은 목록에 추가
                lists[count] = building;
                count++;
            }
        }
}
        //결과 출력
        Message m = new Message();
        m.what = 0;
        m.obj = lists;
        mHandler.sendMessage(m);




    }





}
