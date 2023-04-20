using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class Replay : MonoBehaviour
{
    string[,] seperatedInput;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        // read entire output data file
        string projectLocation = Application.dataPath;
        string inputFile = Path.Combine(projectLocation, "../Output/outputdata1.csv");
        //string inputFile = @"A:outputdata1.txt";
        string[] lines = File.ReadAllLines(inputFile);
        seperatedInput = new string[lines.Length,lines[0].Length];
        for (int i=0; i<lines.Length; i++) {
            string[] data = lines[i].Split(",");
            for (int j=0; j<data.Length; j++) {
                seperatedInput[i,j] = data[j];
            }
        }
        index = 2;
    }

    // Update is called once per frame
    void Update()
    {   
        if (index < seperatedInput.GetLength(0)) {
            Debug.Log("time = " + Time.time);
            float nextTimeRecorded = float.Parse(seperatedInput[index,0]);
            if (nextTimeRecorded <= Time.time) {
                float xPos = float.Parse(seperatedInput[index,1]);
                float zPos = float.Parse(seperatedInput[index,2]);
                gameObject.transform.position = new Vector3(xPos, 1, zPos);
                index++;
            }
        }
    }
}