using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PiramidController : Enemy
{
    [Header("Idle Animation Settings")]
    public float checkInterval = 3f;       // как часто проверять (в секундах)
    public float animationChance = 0.3f;   // шанс проиграть анимацию (0–1)

    private float idleTimer = 0f;

    public GameObject plTarget;

    public override void OnLive()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (targetPoint != null)
        {
            if (targetIsPlayer)
            {
                animator.SetBool("isWalking", false);
                nowSpeed = speed * 2;
            }
            else
            {
                animator.SetBool("isWalking", true);
                nowSpeed = speed;
            }
            Vector3 direction = targetPoint.position - transform.position;
            direction.y = 0;

            if (direction.magnitude > destroyDistance)
            {
                Vector3 moveDir = direction.normalized;
                controller.Move(moveDir * nowSpeed * Time.deltaTime);

                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            else
            {
                Destroy(targetPoint.gameObject);
                targetPoint = null; // чтобы не повторять
                targetIsPlayer = false;
            }
        }
        // Если цели нет — враг стоит на месте
        if (targetPoint == null && isGrounded)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            idleTimer += Time.deltaTime;
            if (idleTimer >= checkInterval)
            {
                idleTimer = 0f;
                if (Random.value < animationChance)
                {
                    animator.SetTrigger("Peak");
                }
            }
        }
        else
        {
            idleTimer = 0f; // сброс, если цель есть
        }
    }
    public void PlayerInFinder()
    {
        if (!targetIsPlayer)
        {
            if (targetPoint != null) Destroy(targetPoint.gameObject);
            animator.SetBool("isRunning", true);
            animator.SetBool("isStopping", false);
            targetIsPlayer = true;
            targetPoint = Instantiate(plTarget).transform;
            targetPoint.position = player.transform.position;
            Target target = targetPoint.transform.GetChild(0).gameObject.GetComponent<Target>();
            target.animator = animator;
            animator.Play("Run");
        }
    }
}
