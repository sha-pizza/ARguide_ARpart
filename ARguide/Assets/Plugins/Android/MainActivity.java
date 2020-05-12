package com.DefaultCompany.arguide;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
/*
import com.pedro.library.AutoPermissions;
import com.pedro.library.AutoPermissionsListener;
 */
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity /*implements AutoPermissionsListener*/ {
    final int REQUEST_CODE = 101;
    final long minTime = 100;
    final float minDistance = 0;

    double azimuth = 0;
    double latitude = 0;
    double longitude = 0;

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
}
