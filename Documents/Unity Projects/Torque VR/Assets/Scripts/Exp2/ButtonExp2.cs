using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class ButtonExp2 : MonoBehaviour
{
    public HoverButton hoverButton;
    public Experiment2 exp2;
    // Start is called before the first frame update
    void Start()
    {
        hoverButton.onButtonDown.AddListener(OnButtonDown);
    }

    private void OnButtonDown(Hand hand)
    {
        // End Scene
        StartCoroutine(EndTrial2());
    }

    // Update is called once per frame
    private IEnumerator EndTrial2()
    {
        Debug.Log("Button Pressed Exp2");
        
        yield return null;
    }
}
