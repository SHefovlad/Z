using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool isPlayerIn = true;
    public Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
        }
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<PiramidController>() != null) {
                PlayerController playerController = FindAnyObjectByType<PlayerController>();
                if (isPlayerIn && !playerController.isRolling)
                {
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isStopping", false);
                    playerController.velocity.y = Mathf.Sqrt(playerController.jumpHeight * -2f * playerController.gravity / 1.5f);
                    playerController.healthManager.GetDamage(10);
                }
                else
                {
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isStopping", true);
                } 
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }
}
