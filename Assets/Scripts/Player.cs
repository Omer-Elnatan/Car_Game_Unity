using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public GameObject playermesh;

    //gems clock scripts reference
    Clock iceClock, lightningClock;
    [SerializeField] GameObject iceClockObject, lightningClockObject;

    //audios sources
    public AudioSource engineSound, driftSound;

    //wheels collider
    public WheelCollider[] wheelColliders;
    private WheelFrictionCurve wheelFrictionForward,wheelFrictionSideways;


    //wheels meshes
    public GameObject[] wheelMeshes;
    public GameObject[] iceWheelMeshes;

    //Trail renderer list
    public TrailRenderer[] tireMarks;

    //Tire smoke list
    public ParticleSystem[] tireSmoke;
    public ParticleSystem splash;

    //fire particles for the lightning wheel
    public ParticleSystem[] wheelFire;

    //car stats
    public Rigidbody playerRigidbody;
    public float maxSteerAngle;
    public float motorForce;
    public float maxVToZeroForce;
    public float brakeForce;
    public float maxVelocityForward;
    public float maxVelocityBackward;


    //timers
    public float abilityTimer;

    //states flags of the car
    public bool isDrift, isOnTheGround, isTireSmoke, isDrawning, isOnIce, iceWheelMode, lightningWheelMode;
    
    

    //current stats(changes during debug)
    public float velocity, driftVelocity;
    public float currentSteerAngle;

    //input vars
    public float horizontalInput, verticalInput, DriftInput;

  
    
    //lower the center off mass for stability(just in the first frame)
    void Start()
    {
        wheelFrictionSideways = wheelColliders[0].GetComponent<WheelCollider>().sidewaysFriction;
        wheelFrictionForward = wheelColliders[0].GetComponent<WheelCollider>().forwardFriction;

        iceClock = iceClockObject.GetComponent<Clock>();
        lightningClock = lightningClockObject.GetComponent<Clock>();
        playerRigidbody.centerOfMass += new Vector3(0f,-1f,0);
        isDrift = false;
        isTireSmoke = false;
        isDrawning = false;
        isOnIce = false;
        iceWheelMode = false;
        lightningWheelMode = false; 
        splash.Stop();
        for(int i=0; i<wheelFire.Length; i++)
        {
            wheelFire[i].Stop();
        }
        
        
    }

    //calls the other functions to control the car
    void FixedUpdate()
    {
        velocity = transform.InverseTransformDirection(playerRigidbody.velocity).z;
        driftVelocity = transform.InverseTransformDirection(playerRigidbody.velocity).x;
        if(isDrawning == false)
        {
            Inputs();
        }
        CarOnGround();
        Drive();
        SteerCar();
    
        if(isOnIce == false || iceWheelMode)
        {
            Drift();
        }
        else
        {
            isDrift = false;
        }
        if(isOnIce)
        {
            OnIce();
        }
    }

    //calls pyrotechnic functions
    void Update()
    {
        if (iceWheelMode)
        {
            IceWheelTimer();
        }

        if(lightningWheelMode)
        {
            LightningWheelTimer();
        }
        DriftSound();
        DriftSmoke();
        TireMarks();
        EngineSound();
        for(int i=0; i < wheelColliders.Length; i++)
        {
            if(iceWheelMode == false)
            {
                WheelPosRot(wheelColliders[i], wheelMeshes[i].transform);
            }
            else
            {
                WheelPosRot(wheelColliders[i], iceWheelMeshes[i].transform);
            }
        }
    }
    
    //gets the inputs
    void Inputs()
    {
        DriftInput = Input.GetAxis("Drift");
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    //controls the motor affect on the wheels
    void Drive()
    { 
        foreach(WheelCollider WC in wheelColliders)
        {
            WC.brakeTorque = 0;
            WC.motorTorque = 0;
        }
        if (verticalInput != 0)
        {
            if ((velocity / Math.Abs(velocity) != verticalInput / Math.Abs(verticalInput)) && Math.Abs(velocity) > 0.5)
            {
                if(isOnIce && iceWheelMode == false)
                {
                    foreach(WheelCollider WC in wheelColliders)
                    {
                        WC.brakeTorque = maxVToZeroForce;
                    }
                }
                else
                {
                    if(isOnTheGround)
                    {
                        playerRigidbody.AddForce(transform.forward * verticalInput * 100000, ForceMode.Force);
                    }
                    foreach(WheelCollider WC in wheelColliders)
                    {
                        WC.brakeTorque = brakeForce;
                    }
                } 
            }
            else
            {
                if (isDrift == true)
                {
                    int i = 0;
                    foreach(WheelCollider WC in wheelColliders)
                    {
                        
                        if (i == (3 + velocity/Math.Abs(velocity)*2 + currentSteerAngle/Math.Abs(currentSteerAngle))/2)
                        {
                            WC.brakeTorque = brakeForce;
                            WC.motorTorque = 0;
                        }
                        else if((verticalInput < 0 && velocity > maxVelocityBackward) || (verticalInput > 0 && velocity < maxVelocityForward))
                        {
                            WC.brakeTorque = 0;
                            WC.motorTorque = motorForce * verticalInput;
                        }
                        i++;
                    }
                    if (Math.Abs(velocity) < 3)
                    {
                        playerRigidbody.AddForce(transform.forward * 50000 * verticalInput, ForceMode.Force);
                    }
                }
                else
                {
                    if((verticalInput < 0 && velocity > maxVelocityBackward) || (verticalInput > 0 && velocity < maxVelocityForward))
                    {
                        foreach(WheelCollider WC in wheelColliders)
                        {
                            WC.motorTorque = motorForce * verticalInput;
                        }
                    }
                }
                
            }
        }
        else
        {
            if(isOnTheGround == true)
            {
                if(velocity > maxVelocityForward)
                {
                    playerRigidbody.AddForce(transform.forward * -10000, ForceMode.Force);
                }
                if(velocity < maxVelocityBackward)
                {
                    playerRigidbody.AddForce(transform.forward * 10000, ForceMode.Force);
                }
            }
            
            if(Math.Abs(velocity) > 0)
            {
                foreach(WheelCollider WC in wheelColliders)
                {
                    WC.brakeTorque = maxVToZeroForce;
                }
            }
            else
            {
                foreach(WheelCollider WC in wheelColliders)
                {
                    WC.brakeTorque = 0;
                }
            }
            
        }
    }

    //controls the angle of the wheels
    void SteerCar()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        wheelColliders[2].steerAngle = currentSteerAngle;
        wheelColliders[3].steerAngle = currentSteerAngle;
    }

    //makes the wheel mesh at the same position and rotation of his wheel collider
    void WheelPosRot(WheelCollider wheelCol, Transform wheelMesh)
    {
        Vector3 pos = wheelMesh.position;
        Quaternion rot = wheelMesh.rotation;

        wheelCol.GetWorldPose(out pos,out rot);
        wheelMesh.position = pos;
        wheelMesh.rotation = rot;
        if(iceWheelMode)
        {
            wheelMesh.Rotate(Vector3.forward ,90);
        }
    }


    //if press x button make the friction value low for drift
    void Drift()
    {
        
        WheelFrictionCurve BWS = wheelColliders[0].GetComponent<WheelCollider>().sidewaysFriction;
        WheelFrictionCurve FWS = wheelColliders[2].GetComponent<WheelCollider>().sidewaysFriction;
        WheelFrictionCurve BWF = wheelColliders[0].GetComponent<WheelCollider>().forwardFriction;
        WheelFrictionCurve FWF = wheelColliders[2].GetComponent<WheelCollider>().forwardFriction;
        
        if(DriftInput == 1)
        {
            isDrift = true;
            BWS.extremumSlip = 5f;
            BWS.extremumValue = 0.5f;

            FWS.extremumSlip = 2.5f;
            FWS.extremumValue = 1.5f;
        }
        else
        {
            if(isDrift == true)
            {
                BWF.extremumSlip = 0.1f;
                BWF.extremumValue = 8f;

                if(BWS.extremumSlip <= 0.3f || BWS.extremumValue >= 7f)
                {
                    BWS.extremumSlip = 0.4f;
                    BWS.extremumValue = 7f;
                    isDrift = false;
                    
                }
                else
                {
                    BWS.extremumSlip -= 4f * Time.deltaTime;
                    BWS.extremumValue += 5f * Time.deltaTime;
                }
                FWS = BWS;
            }
            else
            {
                BWF.extremumSlip = 0.4f;
                BWF.extremumValue = 2f;
    
            }
        }
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if(i <= 1)
            {
                wheelColliders[i].GetComponent<WheelCollider>().sidewaysFriction = BWS;
                wheelColliders[i].GetComponent<WheelCollider>().forwardFriction = BWF;
            }
            else
            {
                wheelColliders[i].GetComponent<WheelCollider>().sidewaysFriction = FWS;
            }
        }
    }

    //make the tire marks when drifting
    void TireMarks()
    {
        if(isDrift && isOnTheGround && Math.Abs(driftVelocity) > 0.5) 
        {
            foreach(TrailRenderer T in tireMarks)
            {
                T.emitting = true;
            } 
        }
        else
        {
            foreach(TrailRenderer T in tireMarks)
            {
                T.emitting = false;
            } 
        }
    }

    //checks if the car on the isGrounded
    void CarOnGround()
    {
        isOnTheGround = false;
        foreach(WheelCollider WC in wheelColliders)
        {
            if(WC.isGrounded)
            {
                isOnTheGround = true;
            }
        }
    }

    //makes the engine sound(depends on vehicle velocity)
    void EngineSound()
    {
        engineSound.pitch = 1 + Math.Abs(velocity) * 0.015f;
        if(isDrawning)
        {
            engineSound.volume -= Time.deltaTime * 0.3f;
        }
    }

    //makes the drift sound(depends on the velocity on the local x axis)
    void DriftSound()
    {
        if(isDrift == true && isOnTheGround)
        {
            driftSound.volume = Math.Abs(driftVelocity * 0.1f);
        }
        else
        {
            driftSound.volume = 0f;
        }
    }

    //makes the drift smoke with particle system
    void DriftSmoke()
    {
        if(isDrift == true && isTireSmoke == false && Math.Abs(driftVelocity) > 0.5 && isOnTheGround == true)
        {
            isTireSmoke = true;
            foreach(ParticleSystem P in tireSmoke)
            {
                P.Play();
            }
        }
        if(isDrift == false || isOnTheGround == false)
        {
            isTireSmoke = false;
            foreach(ParticleSystem P in tireSmoke)
            {
                P.Stop();
            }
        }
        
    }

    //controls the friction of the wheels on ice
    void OnIce()
    {
        WheelFrictionCurve WS = wheelColliders[2].GetComponent<WheelCollider>().sidewaysFriction;
        WheelFrictionCurve WF = wheelColliders[0].GetComponent<WheelCollider>().forwardFriction;
        if(iceWheelMode && isDrift == false)
        {
            WS.extremumSlip = 0.2f;
            WS.extremumValue = 7f;
        }
        else if(iceWheelMode == false)
        {
            WS.extremumSlip = 7f;
            WS.extremumValue = 7f;
        }
        if (isDrift == false)
        {
            for (int i = 0; i < wheelColliders.Length; i++)
            {
                    wheelColliders[i].GetComponent<WheelCollider>().sidewaysFriction = WS;
            }
        }
    }

    //awake ice cloak object, runs the time and awakes the ice wheels
    void IceWheelTimer()
    {
        abilityTimer += Time.deltaTime;
        if(abilityTimer >= iceClock.clockTime)
        {
            iceWheelMode = false;
            abilityTimer = 0;
            for(int i=0; i<wheelMeshes.Length; i++)
            {
                iceWheelMeshes[i].SetActive(false);
                wheelMeshes[i].SetActive(true);
            }
        }
    }

    void LightningWheelTimer()
    {
        abilityTimer += Time.deltaTime;
        if(abilityTimer >= lightningClock.clockTime)
        {
            lightningWheelMode = false;
            abilityTimer = 0;
            maxVelocityForward = 30;
            maxVelocityBackward = -20;
            for(int i=0; i<wheelFire.Length; i++)
            {
                wheelFire[i].Stop();
            }
        }
    }

    //collider enter manager  
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "water")
        {
            isDrawning = true;
            playerRigidbody.AddForce(transform.up * 10000);
            playerRigidbody.drag += 7f;
            splash.Play();
        }

        if(other.tag == "snow")
        {
            isOnIce = true;
        }
        if (other.tag == "ice gem"  && abilityTimer == 0)
        {
            iceClockObject.SetActive(true);
            iceWheelMode = true;
            for(int i=0; i<wheelMeshes.Length; i++)
            {
                iceWheelMeshes[i].SetActive(true);
                wheelMeshes[i].SetActive(false);
                
            }
        }

        if (other.tag == "lightning gem"  && abilityTimer == 0)
        {
            lightningClockObject.SetActive(true);
            lightningWheelMode = true;
            maxVelocityForward = 45;
            maxVelocityBackward = -30;
            for(int i=0; i<wheelFire.Length; i++)
            {
                wheelFire[i].Play();
            }
        }
    }

    //collider exit manager
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "snow")
        {
            isOnIce = false;
            if(isDrift == false)
            {
                for (int i = 0; i < wheelColliders.Length; i++)
                {
                    wheelColliders[i].GetComponent<WheelCollider>().sidewaysFriction = wheelFrictionSideways;
                    wheelColliders[i].GetComponent<WheelCollider>().forwardFriction = wheelFrictionForward;
                }
            }
        }
    }
    
    
}
