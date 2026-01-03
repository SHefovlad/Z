using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : Enemy
{
    public override void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        cameraHolder = FindAnyObjectByType<CameraHolder>();
        controller = GetComponent<CharacterController>();
        Collider enemyCol = GetComponent<CharacterController>();
        Collider playerCol = player.GetComponent<CharacterController>();

        Physics.IgnoreCollision(playerCol, enemyCol);
    }
    public override void OnLive() { }
}
