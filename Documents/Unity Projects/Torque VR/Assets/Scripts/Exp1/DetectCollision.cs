using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    // Start is called before the first frame update

    private string objName;
    private bool inSlot;

    void Start()
    {
        objName = "";
        inSlot = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "cube1" || other.name == "cube2" || other.name == "cube3" || other.name == "cube4")
        {
            objName = other.name;
            inSlot = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "cube1" || other.name == "cube2" || other.name == "cube3" || other.name == "cube4")
        {
            objName = null;
            inSlot = false;
        }
    }

    public bool isPlaced()
    {
        return inSlot;
    }

    public string getObjectName()
    {
        return objName;
    }
}
