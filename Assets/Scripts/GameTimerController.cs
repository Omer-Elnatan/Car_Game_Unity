using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float gameTime;
    public float timeCounter;


    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        float minute, seconds;
        timeCounter -= Time.deltaTime;
        minute = (float)Math.Floor(timeCounter / 60f);
        seconds = (float)Math.Floor(timeCounter % 60f);

        if( seconds < 10)
        {
            timerText.text = minute.ToString() + ":0" + seconds.ToString();
        }
        else
        {
            timerText.text = minute.ToString() + ":" + seconds.ToString();
        }
    }
}
