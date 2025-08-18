using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public PlayerController playerController;
    public abstract void Interact();
}
