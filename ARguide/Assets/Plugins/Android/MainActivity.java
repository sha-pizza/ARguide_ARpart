package com.DefaultCompany.arguide;

import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.annotation.SuppressLint;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.OrientationListener;
import android.widget.ArrayAdapter;
/*
import com.pedro.library.AutoPermissions;
import com.pedro.library.AutoPermissionsListener;
 */
import com.mapbox.geojson.Point;
import com.mapbox.mapboxsdk.Mapbox;
import com.unity3d.player.UnityPlayerActivity;

import java.util.ArrayList;

public class MainActivity extends UnityPlayerActivity /*implements AutoPermissionsListener*/ {
    final int REQUEST_CODE = 101;
    final long minTime = 100;
    final float minDistance = 0;

    double azimuth = 0;
    double latitude = 0;
    double longitude = 0;

    ArrayList<Destination> data = new ArrayList<>();
    ArrayList<Destination> route = new ArrayList<>();
    ArrayAdapter<String> adapter;

    String query;
    Destination dest;

    private double longi;
    private double lat;

    SQLiteDatabase database;
    final String DATABASE_NAME = "Database";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_main);

        //AutoPermissions.Companion.loadAllPermissions(this, REQUEST_CODE);

        OrientationListener orientationListener = new OrientationListener();
        SensorManager sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        Sensor orientationSensor = sensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION);
        sensorManager.registerListener(orientationListener, orientationSensor, SensorManager.SENSOR_DELAY_UI);

        GPSListener gpsListener = new GPSListener();
        LocationManager locationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);

        try {
            Location location = locationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
            if (location != null) {
                latitude = location.getLatitude();
                longitude = location.getLongitude();
            }
            locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, minTime, minDistance, gpsListener);
        } catch (SecurityException e) {
            e.printStackTrace();
        }


        // 여기부터 Map 부분
        Mapbox.getInstance(this, "MAPBOX_ACCESS_TOKEN");

        database = openOrCreateDatabase(DATABASE_NAME, MODE_PRIVATE, null);
        database.execSQL("create table if not exists DestinationTable (name text PRIMARY KEY, latitude real, longitude real)");
        database.execSQL("create table if not exists EndingMessageTable (building text PRIMARY KEY, message text)");
        Cursor cursor = database.rawQuery("select name, latitude, longitude from DestinationTable", null);
        if (cursor.getCount() == 0) {
            DB db = new DB();
            db.insertDataIntoTable(database, "DestinationTable");
        }

        Cursor cursor1 = database.rawQuery("select building, message from EndingMessageTable", null);
        if (cursor1.getCount() == 0) {
            DB db = new DB();
            db.insertDataIntoTable(database, "EndingMessageTable");
        }
    }

    public String getEndingMessage(String destination) {
        Cursor cursor1 = database.rawQuery("select building, message from EndingMessageTable where building = '" + destination +"'", null);
        if (cursor1.getCount() > 0) {
            cursor1.moveToNext();
            return cursor1.getString(1);
        } else {
            return "도착하였습니다 !";
        }
    }

    public void setDestination(String destination) {
        data.clear();
        Cursor cursor = database.rawQuery("select name, latitude, longitude from DestinationTable where name like '%" + destination + "%'", null);
        int recordCount = cursor.getCount();
        for (int i = 0 ; i < recordCount ; i++) {
            cursor.moveToNext();
            data.add(new Destination(cursor.getString(0), 61, cursor.getDouble(1), cursor.getDouble(2)));
        }

        cursor.close();
    }

    //검색용으로 추가된 함수
    public String[] getLocationsName(){
        String name[] = new String[data.size()];

        for(int i=0;i<data.size();i++){
            name[i] = data.get(i).getName();
        }
        return name;
    }

    public double[] getLocationsLat(){
        double lat[] = new double[data.size()];

        for(int i=0;i<data.size();i++){
            lat[i] = data.get(i).getLatitude();
        }
        return lat;
    }

    public double[] getLocationsLog(){
        double log[] = new double[data.size()];

        for(int i=0;i<data.size();i++){
            log[i] = data.get(i).getLongitude();
        }
        return log;
    }

    public void findRoute(int i) {
        //목적지를 고른다
        // TODO: 원래 리스트 UI 부분인데 지금은 임의로 가장 앞에 있는 것 가져옴. 수정필요
        dest = data.get(i);

        if (latitude != 0) {
            lat = latitude;
            longi = longitude;
        } else {
            lat = 37.48;
            longi = 127.49;
        }

        Context context = getApplicationContext();
        Point origin = Point.fromLngLat(longi,lat);
        Point destination1 = Point.fromLngLat(dest.getLongitude(), dest.getLatitude());
        Waypoint cthread = new Waypoint(wHandler,context,origin,destination1);
        cthread.setDaemon(true);
        cthread.start();
    }

    public double[] getRoute() {

        double[] route1 = new double[route.size() * 2];
        for (int i = 0 ; i < route.size() ; i++) {
            route1[i * 2 + 0] = route.get(i).getLatitude();
            route1[i * 2 + 1] = route.get(i).getLongitude();
        }

        return route1;
    }

    public double getAzimuth() {
        return azimuth;
    }

    public double[] getLocation() {
        double[] location = {latitude, longitude};
        return location;
    }

    class OrientationListener implements SensorEventListener {

        @Override
        public void onSensorChanged(SensorEvent sensorEvent) {
            if (sensorEvent.sensor.getType() == Sensor.TYPE_ORIENTATION) {
                double azimuth_temp = sensorEvent.values[0];
                azimuth = azimuth_temp;
            }
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int i) {

        }
    }

    class GPSListener implements LocationListener {

        @Override
        public void onLocationChanged(Location location) {
            double latitude_temp = location.getLatitude();
            double longitude_temp = location.getLongitude();

            latitude = latitude_temp;
            longitude = longitude_temp;
        }

        @Override
        public void onStatusChanged(String provider, int status, Bundle extras) {

        }

        @Override
        public void onProviderEnabled(String provider) {

        }

        @Override
        public void onProviderDisabled(String provider) {

        }
    }
/*
    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        AutoPermissions.Companion.parsePermissions(this, requestCode, permissions, this);
    }

    @Override
    public void onDenied(int i, String[] strings) {

    }

    @Override
    public void onGranted(int i, String[] strings) {

    }

 */

    // 핸들러


    @SuppressLint("HandlerLeak")
    Handler wHandler = new Handler() {
        public void handleMessage(Message m) {
            if (m.what == 0) {
                Log.d("asdf", "HI");
                route.clear();
                Destination[] list;
                list = (Destination[]) m.obj;
                Log.d("asdf", "BYE");
                for (Destination destination : list) {
                    if (destination != null) {
                        route.add(destination);
                    } else {
                        break;
                    }
                }
            }
        }
    };
}


