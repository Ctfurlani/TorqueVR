using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using Valve.VR;

public class Pointer : MonoBehaviour
{
    public float pendulumLength = 0.25f; // 25 cm
    public float maxWeight = 1.0f; // 1 Kg

    public float phi = 0.0f;
    public float theta = 0.0f;

    public float corrPhi = 0.0f;
    public float corrTheta = 0.0f;

    [SerializeField]
    private GameObject pendulum;

    public bool truncate = false;

    private TextMesh thetaText;
    private TextMesh phiText;
    private TextMesh handThetaText;
    private TextMesh handPhiText;

    public PickupObject holder;

    public SteamVR_Input_Sources hand;
    public SteamVR_Action_Boolean grabGripPinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
    private bool handClosed = false;

    // Arduino Communication
    SerialPort stream = new SerialPort("COM4", 9600);

    // Start is called before the first frame update
    void Start()
    {
        var textMeshs = GetComponentsInChildren<TextMesh>();
        thetaText = textMeshs[0];
        phiText = textMeshs[1];
        handThetaText = textMeshs[2];
        handPhiText = textMeshs[3];

        thetaText.text = "Theta: 0";
        phiText.text = "Phi: 0";

        try
        {
            stream.Open();
            Debug.Log("Stream Open");
        }
        catch
        {
            Debug.Log("Arduino must be connected before running Unity");
        }
        //TestAngles();
    }

    private void OnApplicationQuit()
    {
        stream.Close();
    }


    // Update is called once per frame
    void Update()
    {
        if (grabGripPinch.GetState(hand))
        {
            handClosed = true;
        }
        else handClosed = false;
        RotatePendulum();
        AngleCorrection();
        AnglesToArduino();
    }

    private void RotatePendulum()
    {
        pendulum.transform.position = transform.position + new Vector3(0.0f, 0.06f, 0.0f);
        pendulum.transform.rotation = Quaternion.identity;
        Vector3 hinge = transform.position;

        pendulum.transform.RotateAround(hinge, Vector3.forward, phi);
        pendulum.transform.RotateAround(hinge, Vector3.up, theta + 90);
    }
    public void StartTimer()
    {
        holder.StartTimer();
    }

    public float GetTimePassed()
    {
        return holder.GetTimePassed();
    }
    public void StopTimer()
    {
        holder.StopTimer();
        
    }
    public void CalculateAngles(float mass)
    {
        GetAngles(holder.GetDistance(), mass);
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (handClosed)
        {
            if (collision.gameObject.CompareTag("Grabable"))
            {
                if (startTimer)
                {
                    StartTimer();
                }

                //Vector3 centerOfObjectCollided = collision.gameObject.transform.position;
                Vector3 centerOfObjectCollided = collision.gameObject.GetComponent<Collider>().bounds.center;
                float mass = collision.gameObject.GetComponent<Rigidbody>().mass;

                Vector3 pointOfCollision = collision.GetContact(0).point;
                Vector3 distance = centerOfObjectCollided - pointOfCollision;

                GetAngles(distance, mass);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (handClosed) {
            if (collision.gameObject.CompareTag("Grabable"))
            {
                if (startTimer)
                {
                    StartTimer();
                }

                Vector3 centerOfObjectCollided = collision.gameObject.GetComponent<Collider>().bounds.center;
                float mass = collision.gameObject.GetComponent<Rigidbody>().mass;

                Vector3 pointOfCollision = collision.GetContact(0).point;
                Vector3 distance = centerOfObjectCollided - pointOfCollision;

                GetAngles(distance, mass);
            }
        }   
    }
    */
    
    public void GetAngles(Vector3 distance, float mass)
    {
        float objectMass = mass;
        if (truncate)
            if (mass > maxWeight)
                objectMass = maxWeight;
        // Normalise because our pendulum has a fixed length and we cant move the mass 
        Vector3 r = distance;
        r = r.normalized * pendulumLength;
        r *= objectMass / maxWeight;
        // Angle theta is the XZ plane angle
        Vector3 from = new Vector3(r.x, 0, r.z);
        theta = Vector3.Angle(from, Vector3.forward);
        // Fix theta sign
        if (r.x < 0) theta *= -1;

        // We are interested in the projection of the distance in the XZ plane, so we don't use the original y value
        // Using the sphere equation we have the y value 
        float y_point = Mathf.Sqrt(Mathf.Pow(pendulumLength, 2.0f) - Mathf.Pow(r.x, 2.0f) - Mathf.Pow(r.z, 2.0f));
        if (float.IsNaN(y_point)) y_point = 0.0f;
        Vector3 new_r = new Vector3(r.x, y_point, r.z);
        phi = Vector3.Angle(new_r, Vector3.up);

        thetaText.text = "Theta: " + theta;
        phiText.text = "Phi: " + phi;
    }

    private void AngleCorrection()
    {
        Vector3 p = Vector3.up;     
        p = transform.worldToLocalMatrix * p;
        p = p.normalized;
        float pPhi = Mathf.Acos(p.y) * Mathf.Rad2Deg;
        float pTheta = Mathf.Atan(p.x / -p.z) * Mathf.Rad2Deg;
        //Debug.Log("Angle before correction theta: " + pTheta + " phi:" + pPhi);

        int iTheta = (int)Mathf.Ceil(pTheta);
        int iPhi = (int)Mathf.Ceil(pPhi);
        handThetaText.text = "Hand Theta: " + iTheta;
        handPhiText.text = "Hand Phi: " + iPhi;


        // Angle correction
        //Debug.Log(p);
        if (p.z > 0)
        {
            pTheta = 90 - pTheta;
            pPhi = 90 - pPhi;
        }
        else
        {
            pTheta = 90 - pTheta;
            pPhi = 90 + pPhi;
        }

        if (pPhi < 0)
            pPhi = 0;

        handThetaText.transform.rotation = Quaternion.identity;
        handPhiText.transform.rotation = Quaternion.identity;

        corrPhi = pPhi;
        corrTheta = pTheta;
        //Debug.Log("Angle after correction theta: " + corrTheta + " phi:" + corrPhi);

        // Draw Rays for debugging
        
        Vector3 worldAxis = new Vector3(1.0f, 1.0f, -2.0f);
        Debug.DrawRay(transform.position, Vector3.right, Color.red);
        Debug.DrawRay(transform.position, Vector3.up, Color.green);
        Debug.DrawRay(transform.position, Vector3.forward, Color.blue);

        Vector3 localX = transform.right;
        Vector3 localY = transform.up;
        Vector3 localZ = transform.forward;
        Debug.DrawRay(transform.position, localX, Color.magenta);
        Debug.DrawRay(transform.position, localY, Color.grey);
        Debug.DrawRay(transform.position, localZ, Color.cyan);

        Debug.DrawRay(transform.position, p, Color.black);

    }

    private void AnglesToArduino()
    {
        int _phi;
        int _theta;

        if (handClosed)
        {
            _phi = (int)Mathf.Ceil(phi);
            _theta = (int)Mathf.Ceil(theta);

            //Debug.Log("Theta: " + _theta + " Phi: " + _phi);
            bool sign = false;

            if (_theta <= 90 && _theta >= -90)
            {
                _theta += 90;
            }
            else if ( _theta >= -89 && _theta < -180)
            {
                _theta += 270;
                sign = true;
            }
            else
            {
                _theta -= 90;
                sign = true;
            }

            // Phi correction
            _phi = 90 - _phi;

            if (_phi < 0) _phi = 0;
            else if (_phi > 180) _phi = 180;


            if (sign)
            {
                _phi = 180 - _phi;
            }
        }
        else
        {
            _phi = (int)Mathf.Ceil(corrPhi);

            _theta = (int)Mathf.Ceil(corrTheta);
        }
        
       
        //Debug.Log("AFTER Arduino Theta: " + _theta + " Phi: " + _phi);
        try
        {
            stream.WriteLine(_phi.ToString() + "," + _theta.ToString() + "\n");
            //Debug.Log("Arduino angles: " + _theta.ToString() + "," + _phi.ToString());
        }
        catch
        {
            //Debug.Log("Could not write to stream");
            ;
        }
        
               
    }

    void TestAngles()
    {
        handClosed = true;
        // test all cube corners (8) + each face (6)
        Vector3 centre = new Vector3(2.0f, 2.0f, 0.0f);
        Vector3[] edges = new[] {
            new Vector3(1.5f, 1.5f, -0.5f), new Vector3(1.5f, 2.5f, -0.5f), new Vector3(2.5f, 1.5f, -0.5f), new Vector3(2.5f, 2.5f, -0.5f), // front bottom left, front upper left, front bottom right, front upper right
            new Vector3(1.5f, 1.5f, 0.5f), new Vector3(1.5f, 2.5f, 0.5f) , new Vector3(2.5f, 1.5f, 0.5f), new Vector3(2.5f, 2.5f, 0.5f) // back bottom left, back upper left, back bottom right, back upper right
        };
        Vector3[] faces = new[] {
            new Vector3(2.0f, 2.0f, -0.5f), // front face
            new Vector3(2.0f, 2.0f, 0.5f),  // back face
            new Vector3(2.5f, 2.0f, 0.0f),  // right face
            new Vector3(1.5f, 2.0f, 0.0f),  // left face
            new Vector3(2.0f, 2.5f, 0.0f),  // over face
            new Vector3(2.0f, 1.5f, 0.0f),  // under face
        };

        for (int i = 0; i < 8; ++i)
        {
            Debug.Log("EDGE: " + i);
            Vector3 distance = (centre - edges[i]);
            //Debug.Log(distance);
            GetAngles(distance, 1.0f);
            AnglesToArduino();
        }
        for (int i = 0; i < 6; ++i)
        {
            Debug.Log("FACE: " + i);
            Vector3 distance = (centre - faces[i]);
            //Debug.Log(distance);
            GetAngles(distance, 1.0f);
            AnglesToArduino();
        }
        handClosed = false;
    }

}