using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    Player player;
    [SerializeField] GameObject playerObject;
    public GameObject clock;
    public Transform clockHand;
    public float clockTime;

    void Start()
    {
        player = playerObject.GetComponent<Player>();
        clockHand.eulerAngles = new Vector3(0, 0, 0);
        clock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(clock.tag == "ice clock")
        {
            if(player.iceWheelMode == true)
            {
                
                clockHand.eulerAngles = new Vector3(0, 0, player.abilityTimer * -360/clockTime);
            }
            else
            {
                clock.SetActive(false);
                
            }
        }
        
        if(clock.tag == "lightning clock")
        {
            if(player.lightningWheelMode == true)
            {
                
                clockHand.eulerAngles = new Vector3(0, 0, player.abilityTimer * -360/clockTime);
            }
            else
            {
                clock.SetActive(false);
            }
        }
        
    }
}
