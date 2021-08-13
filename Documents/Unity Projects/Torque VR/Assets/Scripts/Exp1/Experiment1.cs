using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.IO;

public class Experiment1 : MonoBehaviour
{
    
    private const int NUMCUBES = 4;
    private const int NUMTRIALS = 10;
    private const int NUMWEIGHTS = 70; // Total de combinações
    public int ID = 0;

    public GameObject[] cubes = new GameObject[NUMCUBES];
    public GameObject[] slots = new GameObject[NUMCUBES];
    public GameObject endText;
    private float[] answers;

    private string answerPath, filename, answer, weigthfile;
    private int trial;
    private float[,] weights;

    public Pointer pointer;
    public Button button;
    public GameObject player;

    private Vector3 initialPos = new Vector3(0.0f, 0.0f, 2.5f);

    void Awake()
    {
        // Prepare filename to write answer
        answerPath = @Application.dataPath + "/Answers/Exp1/";
        filename = answerPath + ID + "-" + "exp1-answers.csv";
        answer = "";
        weigthfile = @Application.dataPath + "/weights.txt";
        answers = new float[NUMCUBES];

        //first trial
        trial = 1;

        // Load all weights in array of arrays
        float[,] allweights = new float[NUMWEIGHTS, NUMCUBES];
        weights = new float[NUMTRIALS, NUMCUBES];
        string[] lines = File.ReadAllLines(weigthfile);
        for (int i = 0; i < lines.Length; ++i)
        {
            string[] numberStrings = lines[i].Split(' ');
            for(int j = 0; j < numberStrings.Length; ++j)
            {
                allweights[i, j] = float.Parse(numberStrings[j]);
            }
        }

        // Separate the weights that this user will test with
        for(int i = 0; i < NUMTRIALS; ++i)
        {
            for (int j = 0; j < NUMCUBES; ++j)
            {
                weights[i, j] = allweights[i + (NUMTRIALS * ID), j];
                Debug.Log(weights[i, j]);
            }
        }

        // Load first weights in cubes from file
        // Weigths range from 1-8, meaning 0.5-4.0
        for (int i = 0; i < cubes.Length; ++i)
        {
            cubes[i].GetComponent<Rigidbody>().mass = weights[0, i] * 0.5f;
            Debug.Log(cubes[i].GetComponent<Rigidbody>().mass);
        }

        // End of trials text
        endText.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (button.IsPressed())
            CustomButtonPressed();
    }

    public void CustomButtonPressed()
    {
        // All cubes are placed on top of a platform
        if (Finished())
        {
            Debug.Log("Finished trial: " + trial);
            GetAnswers();
            WriteAnswers();
            ResetScene();
        }     
    }

    // Reset cube placements to initial positions and load new weights
    public void ResetScene()
    {
        if (trial == NUMTRIALS)
        {
            
            Debug.Log("Finished");
            // return cubes to initial position
            for (int i = 0; i < NUMCUBES; ++i)
            {
                cubes[i].transform.position = new Vector3(-0.6f + 0.4f * i, 0.95f, -2.3f);
                cubes[i].transform.rotation = Quaternion.identity;
            }
            //Move player to new experiment
            player.transform.position = initialPos;
            pointer.StopTimer();
        }
        else
        {
            for (int i = 0; i < NUMCUBES; ++i)
            {
                cubes[i].transform.position = new Vector3(-0.6f + 0.4f * i, 0.95f, -2.3f);
                cubes[i].transform.rotation = Quaternion.identity;
                cubes[i].GetComponent<Rigidbody>().mass = weights[trial, i] * 0.5f;
                Debug.Log(cubes[i].GetComponent<Rigidbody>().mass);
            }
            ++trial;
        }
        
    }
    public bool Finished()
    {
        for (int i = 0; i < NUMCUBES; ++i)
        {
            if (slots[i].GetComponent<DetectCollision>().isPlaced() == false)
                return false;
        }
        return true;
    }

    public void GetAnswers()
    {
        for (int i = 0; i < NUMCUBES; ++i)
        {
            string objname = slots[i].GetComponent<DetectCollision>().getObjectName();
            answers[i] = GameObject.Find(objname).GetComponent<Rigidbody>().mass * 2.0f;
            answer += answers[i].ToString() + " ";
        }
        answer += " " + pointer.GetTimePassed().ToString();
        pointer.StopTimer();
    }

    public void WriteAnswers()
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

}
