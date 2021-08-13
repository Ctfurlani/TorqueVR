using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.IO;

public class Experiment2 : MonoBehaviour
{
    public Bar expandableBar;
    public GameObject refBar;
    public GameObject player;
    public Button button;

    private TextMesh limits;
    private TextMesh currentSize;

    private float previousMass;

    private const float DEFAULTRADIUS = 0.05f;
    private const float DEFAULTLENGTH = 0.5f;

    // Experiment answers
    private const int NUMBARS = 2;
    private const int NUMTRIALS = 10; // 2 model bar and 5 expandable bar changes

    public int ID = 0;
    public Pointer pointer;

    private string answerPath, filename, answer, densityFile;
    private int trial;
    private float[,] densities;

    private Vector3 iPosRefBar;
    private Quaternion iRotRefBar;
    private Vector3 iScaleRefBar;
    private Vector3 iPosExpBar;


    void Awake()
    {
        var textMeshs = GetComponentsInChildren<TextMesh>();
        limits = textMeshs[0];
        currentSize = textMeshs[1];
        SetText();

        // Save initial position, rotation and scale of each bar
        iPosRefBar = refBar.transform.position;
        iRotRefBar = refBar.transform.rotation;
        iScaleRefBar = refBar.transform.localScale;
        iPosExpBar = expandableBar.transform.position;


        // Prepare filename to write answer
        answerPath = @Application.dataPath + "/Answers/Exp2/";
        filename = answerPath + ID.ToString() + "-" + "exp2-answers.csv";
        answer = "";
        densityFile = @Application.dataPath + "/densities.txt";

        // First trial
        trial = 1;

        // Load all densities 
        densities = new float[NUMTRIALS, NUMBARS];
        string[] lines = File.ReadAllLines(densityFile);
        for (int i = 0; i < lines.Length; ++i)
        {
            string[] numberStrings = lines[i].Split(' ');
            for (int j = 0; j < numberStrings.Length; ++j)
            {
                densities[i, j] = float.Parse(numberStrings[j]);
            }
        }

        // Model bar
        refBar.GetComponent<Rigidbody>().mass = densities[0, 1] * Mathf.PI * DEFAULTLENGTH * Mathf.Pow(DEFAULTRADIUS, 2);
        expandableBar.SetDensity(densities[1, 1]);
        expandableBar.ChangeMass();
        previousMass = expandableBar.GetComponent<Rigidbody>().mass;

        limits.gameObject.SetActive(false);
        currentSize.gameObject.SetActive(false);

        Debug.Log("Trial 1: " + densities[0, 0] + " " + densities[0, 1]);
    }

    void Update()
    {
        if (button.IsPressed())
            CustomButtomPressed();
        if (previousMass != expandableBar.GetComponent<Rigidbody>().mass)
        {
            previousMass = expandableBar.GetComponent<Rigidbody>().mass;
            pointer.CalculateAngles(previousMass);

        }
        DrawText();
    }

    public void ShowText()
    {
        limits.gameObject.SetActive(true);
        currentSize.gameObject.SetActive(true);
    }
    public void CustomButtomPressed()
    {
        if (pointer.GetTimePassed() >= 1.0f)
        {
            Debug.Log("Finished trial: " + trial);
            GetAnswers();
            WriteAnswers();
            ResetScene();
        }
        
    }

    private void ResetScene()
    {
        // return bars to original position
        refBar.transform.position = iPosRefBar;
        refBar.transform.rotation = iRotRefBar;
        expandableBar.transform.position = iPosExpBar;
        expandableBar.transform.rotation = iRotRefBar;
        expandableBar.transform.localScale = iScaleRefBar;

        if (trial == NUMTRIALS)
        {
           // Finished experiment
           //TODO: END EXPERIMENT
        }
        else
        {
            // load new refBar density and expBar
            refBar.GetComponent<Rigidbody>().mass = densities[trial, 0] * Mathf.PI * DEFAULTLENGTH * Mathf.Pow(DEFAULTRADIUS, 2);
            expandableBar.SetDensity(densities[trial, 1]);
            expandableBar.ChangeMass();
            ++trial;
            Debug.Log("Trial " + trial + ": " + densities[0, 0] + " " + densities[0, 1]);
        }
    }

    private void GetAnswers()
    {
        // Ref bar mass and expBar mass + time
        answer += refBar.GetComponent<Rigidbody>().mass.ToString();
        answer += " " + expandableBar.GetMass().ToString();
        answer += " " + pointer.GetTimePassed().ToString();
        pointer.StopTimer();
    }
    private void WriteAnswers()
    {
        FileStream fileStream = new FileStream(filename, File.Exists(filename) ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write);
        Debug.Log("writing answers");

        using (StreamWriter fs = new StreamWriter(fileStream))
        {
            fs.WriteLine(answer);
        };
        fileStream.Close();
        answer = "";
    }

    private void DrawText()
    {
        currentSize.text = string.Format("Comprimento: {0:0.00}\nRaio: {1:0.00}", expandableBar.GetLength(), expandableBar.GetRadius());
        //limitsText.GetComponent<TextMesh>().text = string.Format("Comprimento: {0:0.00}\nRaio: {1:0.00}", expandableBar.GetLength(), expandableBar.GetRadius());
    }
    private void SetText()
    {
        limits.text = "Comprimento máximo: " + expandableBar.GetMaxLenght().ToString() +
                      "\nComprimento mínimo: " + expandableBar.GetMinLenght().ToString() +
                      "\nRaio máximo: " + expandableBar.GetMaxRadius().ToString() +
                      "\nRaio mínimo: " + expandableBar.GetMinRadius().ToString()
        ;


        currentSize.text = "Comprimento: " + expandableBar.GetLength().ToString() +
                           "\nRaio: " + expandableBar.GetRadius().ToString();
    }
}
