using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Transform movingPart;

    public Vector3 localMoveDistance = new Vector3(0, -0.1f, 0);

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 releasePosition;

    private bool buttonPressed;

    // Start is called before the first frame update
    void Start()
    {
        if (movingPart == null && this.transform.childCount > 0)
            movingPart = this.transform.GetChild(0);

        startPosition = movingPart.localPosition;
        endPosition = startPosition + 2 * localMoveDistance;
        releasePosition = startPosition + localMoveDistance;

        ColorSelf(Color.white);
    }

    // Update is called once per frame
    void Update()
    {
        if (movingPart.localPosition.y > startPosition.y)
            movingPart.localPosition = startPosition;

        if (movingPart.localPosition.y < endPosition.y)
            movingPart.localPosition = endPosition;

        if (movingPart.localPosition.y == endPosition.y)
            ButtonPressed();
        else if (movingPart.localPosition.y >= releasePosition.y)
            ButtonReleased();
    }

    public void ButtonPressed()
    {
        ColorSelf(Color.green);
        buttonPressed = true;
    }
    public void ButtonReleased()
    {
        ColorSelf(Color.white);
        buttonPressed = false;
    }

    public bool IsPressed()
    {
        return buttonPressed;
    }

    private void ColorSelf(Color newColor)
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
        {
            renderers[rendererIndex].material.color = newColor;
        }
    }

}
