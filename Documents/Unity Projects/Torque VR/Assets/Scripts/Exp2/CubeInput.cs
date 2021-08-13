using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInput : MonoBehaviour
{
    private bool inside;
    void Awake()
    {
        inside = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Pointer")
        {
            inside = true;
            //Debug.Log("Hand is inside");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Pointer")
        {
            inside = false;
            //Debug.Log("Hand is not inside");
        }
    }

    public bool IsInside()
    { 
        return inside;
    }
}
