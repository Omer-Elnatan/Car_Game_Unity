using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gem : MonoBehaviour
{
    public GameObject gem;
    public ParticleSystem takenParticales;
    public float floatPeriodTime, floatTimer, floatDirection, floatSpeed, rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        takenParticales.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        FloatManager();
        RotatationManager();
    }

    void FloatManager()
    {
        floatTimer += Time.deltaTime;
        transform.Translate(Vector3.up * Time.deltaTime * floatDirection * (1 - Math.Abs(0.5f - floatTimer)) * floatSpeed , Space.World);
        if(floatTimer >= floatPeriodTime)
        {
            floatTimer = 0;
            floatDirection *= -1;
        }
    }

    void RotatationManager()
    {
        transform.Rotate(0f,rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "car" && other.gameObject.GetComponent<Player>().iceWheelMode == false && other.gameObject.GetComponent<Player>().lightningWheelMode == false)
        {
            gem.SetActive(false);
            takenParticales.Play();
        }
        
    }
}
