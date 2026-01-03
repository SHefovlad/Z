using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public PlayerController playerController;
    bool _isClosest = false;
    public bool IsClosest
    {
        get { return _isClosest; }
        set { IsClosestValueChanged(value); }
    }
    public virtual void IsClosestValueChanged(bool value)
    {
        _isClosest = value;
        foreach (Outline o in outlines)
        {
            o.enabled = value;
        }
    }
    public List<Outline> outlines;
    public abstract void Interact();
}
