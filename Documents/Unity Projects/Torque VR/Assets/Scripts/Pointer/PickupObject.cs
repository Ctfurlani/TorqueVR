using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickupObject : MonoBehaviour
{
    private bool hold = false;
    public Pointer pointer;
    public Vector3 distance;

    public SteamVR_Input_Sources hand;
    public SteamVR_Action_Boolean grabGripPinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

    // timer
    public float timePassed;
    private bool timerRunning;
    private bool startTimer;

    private void Start()
    {
        gameObject.AddComponent<FixedJoint>();
        //timer
        startTimer = true;
        timerRunning = false;
        timePassed = 0.0f;
    }

    void Update()
    {
        if (timerRunning)
            timePassed += Time.deltaTime;

        if (grabGripPinch.GetState(hand)) 
        {
            if (gameObject.GetComponent<FixedJoint>().connectedBody == null)
                hold = true;
        }
        else
        {
            hold = false;
            if ( gameObject.GetComponent<FixedJoint>().connectedBody != null)
            {
                gameObject.GetComponent<FixedJoint>().connectedBody = null;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (hold)
        {
            if (collision.gameObject.GetComponent<Rigidbody>() != null && collision.gameObject.CompareTag("Grabable"))
            {
                if (startTimer)
                    StartTimer();
                gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
                Vector3 centerOfObjectCollided = collision.gameObject.GetComponent<Collider>().bounds.center;

                float mass = collision.gameObject.GetComponent<Rigidbody>().mass;
                Vector3 pointOfCollision = collision.GetContact(0).point;
                distance = centerOfObjectCollided - pointOfCollision;
                pointer.GetAngles(distance, mass);
                hold = false;
            }
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (hold)
        {
            if (collision.gameObject.GetComponent<Rigidbody>() != null && collision.gameObject.CompareTag("Grabable"))
            {
                if (startTimer)
                    StartTimer();
                gameObject.GetComponent<FixedJoint>().connectedBody = collision.rigidbody;
                Vector3 centerOfObjectCollided = collision.gameObject.GetComponent<Collider>().bounds.center;
                
                float mass = collision.gameObject.GetComponent<Rigidbody>().mass;
                Vector3 pointOfCollision = collision.GetContact(0).point;
                distance = centerOfObjectCollided - pointOfCollision;

                pointer.GetAngles(distance, mass);
                hold = false;
            }
        }

    }
    public void StartTimer()
    {
        startTimer = false;
        timerRunning = true;
        timePassed = 0.0f;
    }
    public void StopTimer()
    {
        timerRunning = false;
        startTimer = true;
        timePassed = 0.0f;
    }
    public float GetTimePassed()
    {
        return timePassed;
    }

    public Vector3 GetDistance()
    {
        return distance;
    }
}
