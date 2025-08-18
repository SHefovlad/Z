using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactable
{
    public Transform seatPlace;
    public override void Interact()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        playerController.isSeat = !playerController.isSeat;
        playerController.modelTransform.rotation = seatPlace.rotation;
        playerController.gameObject.transform.position = seatPlace.position;
    }
}
