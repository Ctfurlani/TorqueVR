using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class ButtonExp1 : MonoBehaviour
{
    public HoverButton hoverButton;
    public Experiment1 exp1;

    void Start()
    {
        hoverButton.onButtonDown.AddListener(OnButtonDown);
    }

    private void OnButtonDown(Hand hand)
    {
        // End Scene
        StartCoroutine(EndTrial());
    }

    private IEnumerator EndTrial()
    {
        Debug.Log("Button Pressed");
        exp1.CustomButtonPressed();
        yield return null;
    }
}
