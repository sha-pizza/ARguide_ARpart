package com.DefaultCompany.arguide;

import android.database.sqlite.SQLiteDatabase;

public class DB {

    public void insertDataIntoTable(SQLiteDatabase destinationDatabase, String table) {
        if (table.equals("DestinationTable")) {
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

            for (int i = 0 ; i < buildings.length ; i++) {
                destinationDatabase.execSQL("insert into " + table + "(name, latitude, longitude) values ('"
                        + buildings[i].getName() + "', " + buildings[i].getLatitude() + ", " + buildings[i].getLongitude() + ")");
            }

            for (int i = 0 ; i < ATMs.length ; i++) {
                destinationDatabase.execSQL("insert into " + table + "(name, latitude, longitude) values ('"
                        + ATMs[i].getName() + "', " + ATMs[i].getLatitude() + ", " + ATMs[i].getLongitude() + ")");
            }
        } else if (table.equals("EndingMessageTable")) {
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('학생회관 ', '학생회관은 종합행정실, 학생회관식당, 각종 동아리방과 성대 신문사, 성균 Times 등의 언론반 등이 위치해 있는 곳입니다. 만약 동아리에 관심이 있다면 학생회관에 들어가보세요!')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('복지회관 ', '복지회관은 교직원식당, 카운슬링센터, 건강센터, 우체국, 은행, 등 각종 교내 편의시설이 위치해 있는 곳입니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제1공학관 21동 ', '제1공학관은 21동부터 23동까지로 나뉘며, ‘ㄷ’자 형태로 구분되어있습니다. 21동에는 정보통신/소프트웨어/공과대학행정실을 비롯한 행정실, CAD 연구실 등 다양한 연구실과 스마트라운지, 스마트갤러리와 같은 시설을 갖추고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제1공학관 22동 ', '제1공학관은 21동부터 23동까지로 나뉘며, ‘ㄷ’자 형태로 구분되어있습니다. 22동에는 다양한 연구실과 ADIC센터, 프레젠테이션룸, 설계실, 첨단강의실 및 세미나실 등의 시설을 갖추고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제1공학관 23동 ', '제1공학관은 21동부터 23동까지로 나뉘며, ‘ㄷ’자 형태로 구분되어있습니다. 23동에는 각종 연구시설과 교수 연구실, 캠퍼스관리팀과 세미나실 등을 갖추고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제2공학관 25동 ', '제2공학관은 25동부터 27동까지로 나뉘며 ‘ㄷ’자 형태로 연결되어 있습니다. 25동에는 다양한 연구실과 실험실, 회의실 등의 시설을 갖추고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제2공학관 26동 ', '제2공학관은 25동부터 27동까지로 나뉘며 ‘ㄷ’자 형태로 연결되어 있습니다. 26동에는 공대식당을 비롯한 휴게실과 매점, 열람실 등의 편의시설과 첨단강의실, 연구공간이 마련되어 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제2공학관 27 ', '제2공학관은 25동부터 27동까지로 나뉘며 ‘ㄷ’자 형태로 연결되어 있습니다. 27동에는 공학교육혁신센터와 성균어학원, 우주과학기술연구소, 창업기업 사무실 등이 자리하며, 다양한 세미나실, 연구실, 강의실을 갖추고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제1과학관은 자연과학대학이 주로 사용하는 공간입니다. 제1과학관부터 제2과학관, 기초학문관, 생명공학관까지 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('제2과학관은 자연과학대학이 주로 사용하는 공간입니다. 제1과학관과 기초학문관, 생명공학관이 하나로 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('화학관 ', '화학관은 약학관과 반도체관을 잇는 종합강의동입니다. 이곳은 첨단강의실과 연구실, 라운지 등의 공간과 슈퍼컴퓨터실, 동위원소실험실, 세포배양실 등 연구에 필요한 최신식 시설을 갖추고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('반도체관 ', '반도체관은 약학관과 반도체관을 잇는 종합강의동으로, 첨단강의실과 연구실 및 실습실 등의 학업 및 연구공간과 워크스테이션실, 디지털콘텐츠스튜디오, SW 스튜디오 등의 최신식 시설을 갖추고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('삼성학술정보관 ', '삼성학술정보관은 국내서 약 42만권, 국외서 약 20만권으로 62만권에 가까운 도서를 소장 중이며, 기본적인 도서관의 기능 뿐 아니라 정보화사회에 걸맞는 다기능 도서관의 역할을 수행하고 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('기초학문관 ', '기초학문관은 학부/사범대학행정실 등의 행정공간과 강의실, 연구실 등이 있습니다. 제1과학관과 제2과학관, 생명공학관이 하나로 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('생명공학관 ', '생명공학관은 생명공학대학 학생들이 주로 이용하는 공간입니다. 생명공학관은 제1과학관과, 제2과학관, 기초학문관이 하나로 연결되어 있어 건물 내에서 자유롭게 이동할 수 있습니다.')");
            destinationDatabase.execSQL("insert into " + table +
                    "(building, message) values ('산학협력센터 ', '산학협력센터는 산학협력단과 연구실, 세미나실이 있으며, 40여개의 창업보육기업 및 실습실 등이 위치해 있습니다.')");
        }


    }
}
