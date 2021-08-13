using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    private const int CORRECTION = 3200;
    public GameObject plusRadius;
    public GameObject minusRadius;
    public GameObject plusLenght;
    public GameObject minusLenght;

    public GameObject modelBar;

    private float volume;
    public float maxLenght = 1.3f;
    public float minLenght = 0.1f;

    public float maxRadius = 0.07f;
    public float minRadius = 0.01f;

    public float density = 500.0f;
    public bool canChange;

    private Vector3 lengthChange, radiusChange, scaleChange;

    void Start()
    {
        radiusChange = new Vector3(0.01f, 0.0f, 0.01f);
        lengthChange = new Vector3(0.0f, 0.2f, 0.0f);
        scaleChange = new Vector3(0.0f, 0.0f, 0.0f);
        canChange = true;        
    }

    private void Update()
    {
        ChangeBar();
        CrampBar();
        ChangeMass();
    }
    public void IncreaseLenght()
    {
        scaleChange = lengthChange;
        transform.localScale += scaleChange;
    }
    public void DecreaseLenght()
    {
        scaleChange = -lengthChange;
        transform.localScale += scaleChange;
    }
    public void IncreaseRadius()
    {
        scaleChange = radiusChange;
        transform.localScale += scaleChange;
    }
    public void DecreaseRadius()
    {
        scaleChange = -radiusChange;
        transform.localScale += scaleChange;
    }

    public float GetLength()
    {
        return transform.localScale.y;
    }
    public float GetRadius()
    {
        return transform.localScale.x;
    }
    public float GetMinLenght()
    {
        return minLenght;
    }

    public float GetMaxLenght()
    {
        return maxLenght;
    }
    public float GetMinRadius()
    {
        return minRadius;
    }
    public float GetMaxRadius()
    {
        return maxRadius;
    }

    public void SetDensity(float d)
    {
        density = d;
    }

    public void ChangeBar()
    {
        bool inside = false;

        //Radius
        if (plusRadius.GetComponent<CubeInput>().IsInside())
        {
            inside = true;
            if (canChange)
            {
                IncreaseRadius();
                canChange = false;
            }
        }

        if (minusRadius.GetComponent<CubeInput>().IsInside())
        {
            inside = true;
            if (canChange)
            {
                DecreaseRadius();
                canChange = false;
            }
        }

        // Lenght
        if (plusLenght.GetComponent<CubeInput>().IsInside())
        {
            inside = true;
            if (canChange)
            {
                IncreaseLenght();
                canChange = false;
            }
        }
        if (minusLenght.GetComponent<CubeInput>().IsInside())
        {
            inside = true;
            if (canChange)
            {
                DecreaseLenght();
                canChange = false;
            }
        }

        if (!inside) canChange = true;
    }

    public void CrampBar()
    {
        float lenght = transform.localScale.y;
        float radius = transform.localScale.x;


        if (lenght < GetMinLenght())
        {
            transform.localScale = new Vector3(transform.localScale.x, GetMinLenght(), transform.localScale.z);
        }
        if (lenght > GetMaxLenght())
        {
            transform.localScale = new Vector3(transform.localScale.x, GetMaxLenght(), transform.localScale.z);
        }

        if (radius < GetMinRadius())
        {
            transform.localScale = new Vector3(GetMinRadius(), transform.localScale.y, GetMinRadius());
        }
        if (radius > GetMaxRadius())
        {
            transform.localScale = new Vector3(GetMaxRadius(), transform.localScale.y, GetMaxRadius());
        }
    }
    public void ChangeMass()
    {
        GetComponent<Rigidbody>().mass = Mathf.PI * Mathf.Pow(GetRadius(), 2) * GetLength() * density;
    }
    public float GetMass()
    {
        return GetComponent<Rigidbody>().mass;
    }

}
