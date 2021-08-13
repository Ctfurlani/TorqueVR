using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ButtonEffectExp1 : MonoBehaviour
{
    public void onButtonDown(Hand fromHand)
    {
        ColorSelf(Color.cyan);
    }

    public void onButtonUp(Hand fromHand)
    {
        ColorSelf(Color.white);
    }

    private void ColorSelf(Color newColor)
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        for (int rindex = 0; rindex < renderers.Length; ++rindex)
        {
            renderers[rindex].material.color = newColor;
        }
    }
}
