using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    //player that the camera follows
    public GameObject player;

    //position camera stats
    public double zCamera;
    public double xCamera;
    public double yCamera;

    //angle camera stats
    public float xAngle;
    public float yAngle;
    public float zAngle;


    // Update is called once per frame
    void Update()
    {   
        //Camera position calculation
        zCamera = player.transform.position[2] - 15*Math.Cos((player.transform.rotation.eulerAngles.y * Math.PI)/180);
        xCamera = player.transform.position[0] - 15*Math.Sin((player.transform.rotation.eulerAngles.y * Math.PI)/180);
        yCamera = player.transform.position[1] + 4;

        //Camera angle calculation
        yAngle = player.transform.rotation.eulerAngles.y;
        xAngle = 9f;
        zAngle = 0f;

        transform.position = new Vector3((float)xCamera, (float)yCamera , (float)zCamera);
        
        //Camera rotation
        transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle);
    }
}
