using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Recording : MonoBehaviour
{

    public float recordingDelay = 0.1f; // how often output is written
    private StreamWriter outputFile;
    private GameObject player;
    private GameObject door;
    private double distanceFromDoor;
    private float timeEntered = -1;
    private float timeExited = -1;
    private Boolean insideDoor = false;
    public RevolvingDoorLogic script;
    // Start is called before the first frame update
    void Start()
    {
        string projectLocation = Application.dataPath;
        outputFile = new StreamWriter(Path.Combine(projectLocation, "../Output/outputdata.csv"));
        //outputFile = new StreamWriter(@"A:outputdata.txt");
        //outputFile = new StreamWriter(@"C:\Users\achen8\outputdata.txt");
        outputFile.WriteLine("time,playerXPos,playerZPos,playerAngle,door1Angle,door2Angle,door3Angle,printTimeEntered,printTimeExited,AngleDifference,TimeUntilHit");
        player = GameObject.Find("XR Rig/Camera Offset/Main Camera");
        door = GameObject.Find("Revolving Door/RevolvingDoor");
        StartCoroutine(RecordData());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator RecordData() {
        while (true)
        {
            string data = getData();
            outputFile.WriteLine(data);
            outputFile.Flush();
            yield return new WaitForSeconds(recordingDelay);
        }
    }

    private string getData(){
        distanceFromDoor = playerDistance();
        if (distanceFromDoor < 2 && !insideDoor) {
            timeEntered = Time.time;
            insideDoor = true;
        }
        if (distanceFromDoor > 2 && insideDoor) {
            timeExited = Time.time;
            insideDoor = false;
        }
  
        double time = Math.Round(Time.time, 2);
        float playerXPos = player.transform.position.x;
        float playerZPos = player.transform.position.z;
        double playerAngle = findPlayerAngle(playerXPos, playerZPos);
        float door1Angle = door.transform.eulerAngles.y;
        float door2Angle = getAngleOffset(door1Angle, 120);
        float door3Angle = getAngleOffset(door2Angle, 120);
        string printTimeEntered;
        if (timeEntered != -1) printTimeEntered = "" + timeEntered;
        else printTimeEntered = "enter undefined";
        string printTimeExited;
        if (timeExited != -1 && timeExited > timeEntered) printTimeExited = "" + timeExited;
        else printTimeExited = "exit undefined";
        double angleDiff = angleDifference(door1Angle, playerAngle);
        double timeUntilHit = findPlayerDoorTime(angleDiff);
        Debug.Log("player angle = " + playerAngle + "\tdoor1 = " + door1Angle + "\tangle diff = " + angleDifference(door1Angle, playerAngle) + "\ttimeuntilhit = " + timeUntilHit);
        return time + "," + playerXPos + "," + playerZPos + "," + playerAngle + "," + door1Angle
                + "," + door2Angle + "," + door3Angle + "," + printTimeEntered + "," + printTimeExited + "," + angleDiff + "," + timeUntilHit;
    }

    private float getAngleOffset(float original, float offset) { // handles wraparound
        return (original + offset) % 360f;
    }

    private double playerDistance() { // distance from center of door
        return Math.Sqrt(Math.Pow(player.transform.position.x, 2) + Math.Pow(player.transform.position.z, 2));
    }

    private double findPlayerAngle(float playerXPos, float playerZPos) { // player angle relative to door's center
        double playerDegreeAngle = (Math.Atan2(playerZPos, playerXPos)) * Mathf.Rad2Deg;
        if (playerDegreeAngle < 0) playerDegreeAngle *= -1;
        else playerDegreeAngle = 360 - playerDegreeAngle;
        return playerDegreeAngle;
    }

    private double findPlayerDoorTime(double angleDifference) { // time before hit from tail door
        return (angleDifference / script.rotationSpeed);
    }

    private double angleDifference(float d1Angle, double playerAngle) {
        double angleDifference;
        if (d1Angle >= playerAngle) angleDifference = (d1Angle - playerAngle) % 120;
        else angleDifference = (360 - (playerAngle - d1Angle)) % 120;
        return angleDifference;
    }

    void OnApplicationQuit() {
        outputFile.Close();
    }
}