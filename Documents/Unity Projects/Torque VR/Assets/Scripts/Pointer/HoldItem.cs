using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HoldItem : MonoBehaviour
{
    GameObject heldItem = null;
    private bool grab = false;

    public SteamVR_Input_Sources hand;
    public SteamVR_Action_Boolean grabGripPinch= SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

    
  
    // Update is called once per frame
    void Update()
    {
        if (grabGripPinch.GetState(hand))
        {
 
            if (heldItem == null)
                grab = true;
        }
        else
        {
            grab = false; 
            if (heldItem != null)
            {
                heldItem.GetComponent<Rigidbody>().isKinematic = false;
                heldItem.transform.SetParent(null);
                heldItem = null;
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (grab)
        {
            if (heldItem == null)
            {
                heldItem = collision.gameObject;
                heldItem.GetComponent<Rigidbody>().isKinematic = true;
                heldItem.transform.SetParent(transform);

            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (grab)
        {
            if (heldItem == null)
            {
                heldItem = collision.gameObject;
                heldItem.GetComponent<Rigidbody>().isKinematic = true;
                heldItem.transform.SetParent(transform);

            }
        }
    }
    private void OnGUI()
    {
        string new_string = "Can grab: " + grab;
        GUI.Label(new Rect(20, 20, 300, 100), new_string);
    }
}
