package com.DefaultCompany.arguide;

import android.content.Context;
import android.os.Handler;
import android.os.Message;

import com.mapbox.api.directions.v5.DirectionsCriteria;
import com.mapbox.api.directions.v5.models.DirectionsResponse;
import com.mapbox.api.directions.v5.models.DirectionsRoute;
import com.mapbox.api.directions.v5.models.LegStep;
import com.mapbox.api.directions.v5.models.RouteLeg;
import com.mapbox.api.directions.v5.models.StepManeuver;
import com.mapbox.geojson.Point;
import com.mapbox.services.android.navigation.v5.navigation.NavigationRoute;

import org.jetbrains.annotations.NotNull;

import java.util.ArrayList;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import timber.log.Timber;


public class Waypoint extends Thread{

    private Handler wHandler;
    private Point origin;
    private Point destination;
    private Context context;

    private ArrayList<Destination> wlists = new ArrayList<>(10);

    private final Object lock = new Object();


    //생성자
    Waypoint(Handler handler, Context context, Point origin, Point destination) {
        this.wHandler = handler;
        this.context = context;
        this.origin= origin;
        this.destination = destination;

    }

    @Override
    public void run() {
        android.os.Process.setThreadPriority(android.os.Process.THREAD_PRIORITY_BACKGROUND);

	wlists.clear();
        

        String MAPBOX_ACCESS_TOKEN = "pk.eyJ1Ijoia2F0ZTk3MDgxOSIsImEiOiJjazllYXA4a20wMGJ5M3BxYm1kZHR5djF5In0.de_wnEWEQlNgLNSHM95dlg";
        NavigationRoute.builder(context)
                .accessToken(MAPBOX_ACCESS_TOKEN)
                .origin(origin).destination(destination)
                .profile(DirectionsCriteria.PROFILE_WALKING)
                .build()
                .getRoute(new Callback<DirectionsResponse>() {
                    @Override
                    public void onResponse(@NotNull Call<DirectionsResponse> call, @NotNull Response<DirectionsResponse> response) {
                        DirectionsResponse myBody = response.body();
                        assert myBody != null;
                        ArrayList<DirectionsRoute> myRouteList = (ArrayList<DirectionsRoute>) myBody.routes();
                        int n=0;
                        for (DirectionsRoute myRoute : myRouteList) {
                            ArrayList<RouteLeg> myLegList = (ArrayList<RouteLeg>) myRoute.legs();

                            assert myLegList != null;
                            for (RouteLeg myLeg : myLegList) {
                                ArrayList<LegStep> myStepList = (ArrayList<LegStep>) myLeg.steps();

                                assert myStepList != null;
                                for (LegStep myStep : myStepList) {
                                    StepManeuver maneuver = myStep.maneuver();

                                    double lat, lng;
                                    lat = maneuver.location().latitude();
                                    lng = maneuver.location().longitude();

                                    Destination subRoute = new Destination(null, 0, lat, lng);
                                    wlists.add(subRoute);

                                    //Timber.e(wlists[n].getLatitude() + " : " + wlists[n].getLongitude());


                                    n++;

                                }
                            }
                        }
                        synchronized (lock) { lock.notify();}
                    }

                    @Override
                    public void onFailure(@NotNull Call<DirectionsResponse> call, @NotNull Throwable t) {
                        //Timber.e("");
                    }

                });



        synchronized (lock) {
            try {
                lock.wait();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }


        //잘 들어갔는지 찍어주기
        for (Destination wlist : wlists) {
            //Timber.e(wlist.getLatitude() + " : " + wlist.getLongitude());
        }

        // 목적지가 wlists에 포함 안 되어 있으면 추가
        if (destination.latitude() != wlists.get(wlists.size() - 1).getLatitude() &&
        destination.longitude() != wlists.get(wlists.size() - 1).getLongitude()) {
            Destination subRoute = new Destination(null, 0, destination.latitude(), destination.longitude());
            wlists.add(subRoute);
        }



        //결과 출력
        Message m = new Message();
        m.obj = wlists;
        m.what = 0;
        wHandler.sendMessage(m);


    }



}
