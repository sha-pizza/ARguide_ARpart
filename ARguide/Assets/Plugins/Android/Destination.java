package com.DefaultCompany.arguide;



public class Destination {

    //생성자

    private double latitude;
    private double longitude;
    private int number; //건물번호
    private String name; //건물이름


    Destination(String v,int v3, double v1, double v2) {
        this.name = v;
        this.latitude=v1;
        this.longitude=v2;
        this.number=v3;
    }

    public int getNumber() {
        return number;
    }

    public void setNumber(int number) {
        this.number = number;
    }

    double getLongitude() {
        return longitude;
    }

    public void setLongitude(double longitude) {
        this.longitude = longitude;
    }

    double getLatitude() {
        return latitude;
    }

    public void setLatitude(double latitude) {
        this.latitude = latitude;
    }

    String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
}
