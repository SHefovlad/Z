using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrShlyap : Interactable
{
    public bool isPressed = false;
    public override void Interact()
    {
        isPressed = !isPressed;
        HatsManager.Instance.hatPanel.SetActive(isPressed);
        HatsManager.Instance.FillHats();
    }
    public override void IsClosestValueChanged(bool value)
    {
        base.IsClosestValueChanged(value);
        if (!value)
        {
            isPressed = false;
            HatsManager.Instance.hatPanel.SetActive(false);
        }
    }
}
