using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour {

    public Rigidbody[] cars;
    public float respawnDelay = 5f;
    public float distanceToCover = 1f;
    private CarController1[] scripts;
    private float[] respawnTimes;
    private float[] distanceLeftToTravel;
    private Transform[] waypoint;

    private int[] laps;

    public static RaceManager Instance { get { return Instance;  } }
    private static RaceManager instance = null;

    public Texture2D startRaceImage;
    public Texture2D digital1Image;
    public Texture2D digital2Image;
    public Texture2D digital3Image;
    private int counterTimerDelay;
    private int countdownTimerStartTime;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = null;
        }
        countdownTimerReset(1);
    }

    private void OnGUI()
    {
        GUILayout.Label(CountdownTimerImage());
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(CountdownTimerImage());
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();
    }

    Texture2D CountdownTimerImage()
    {
        switch (coutdownTimerSecondsRemaining())
        {
            case 3:
                return digital3Image;
            case 2:
                return digital2Image;
            case 1:
                return digital1Image;
            case 0:
                return startRaceImage;
            default:
                return null;
        }
    }

    int coutdownTimerSecondsRemaining()
    {
        int elapsedSeconds = (int) (Time.time - countdownTimerStartTime);
        int secondsLeft = (counterTimerDelay - elapsedSeconds);
        return secondsLeft;
    }

    void countdownTimerReset(int delayInSeconds)
    {
        counterTimerDelay = delayInSeconds;
        //countdownTimerStartTime = Time.time;
    }

    public void LapFinishedByAI(CarController1 script)
    {
        //search through and find the car that comunicated with us.
        for (int i = 0; i < respawnTimes.Length; ++i)
        {
            //increment ints lap counter
            laps[i]++;
            break;
        }
    }

    // Use this for initialization
    void Start () {
        respawnTimes = new float[cars.Length];
        distanceLeftToTravel = new float[cars.Length];
        scripts = new CarController1[cars.Length];
        waypoint = new Transform[cars.Length];

        laps = new int[cars.Length];

        //initialize the array with starting values 
        for(int i=0; i < respawnTimes.Length; ++i)
        {
            scripts[i] = cars[i].gameObject.GetComponent<CarController1>();
            respawnTimes[i] = respawnDelay;
            distanceLeftToTravel[i] = respawnDelay;
            distanceLeftToTravel[i] = float.MaxValue;
            laps[i] = 0;
        }
	}

    // Update is called once per frame
    void Update ()
    {
        //check if cars need a respawn
        for(int i = 0; i < cars.Length; i++)
        {
            Transform nextWaypoint = scripts[i].GetCurrentWaypoint();
            float distanceCovered = (nextWaypoint.position - cars[i].position).magnitude;
            //if the car has moved far enough or is moving to a new waypoint reset its values.
            if(distanceLeftToTravel[i] - distanceToCover > distanceCovered || waypoint[i] != nextWaypoint)
            {
                waypoint[i] = nextWaypoint;
                respawnTimes[i] = respawnDelay;
                distanceLeftToTravel[i] = distanceCovered;
            }

            //otherwise tick down time before we respawn it.
            else
            {
                respawnTimes[i] -= Time.deltaTime;
            }
            if(respawnTimes[i] <= 0)
            {
                //reset its respawn tracking variables
                respawnTimes[i] = respawnDelay;
                distanceLeftToTravel[i] = float.MaxValue;
                cars[i].velocity = Vector3.zero;

                //and spawn it at its last waypoint facing the next waypoint.
                Transform lastWaypoint = scripts[i].GetLastWaypoint();
                cars[i].position = lastWaypoint.position;
                cars[i].rotation = Quaternion.LookRotation(nextWaypoint.position - lastWaypoint.position);
            }

            //testing the lap counter works. first car to complete 3  laps triggers a level restart
            if(laps[i] >= 3)
            {
                Application.LoadLevel("LevelOne");
            }
        }
	}
}
