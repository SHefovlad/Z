using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammurController : Enemy
{
    [Header("Attack")]
    public float playerTimer;
    public float cooldown;
    public int damage = 10;

    public override void OnLive()
    {
        playerTimer += Time.deltaTime;
        if (targetIsPlayer && playerTimer >= cooldown)
        {
            if (targetPoint != null) Destroy(targetPoint.gameObject);
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
            targetPoint = new GameObject("hammurTarget").transform;
            targetPoint.transform.position = player.transform.position;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (targetPoint != null)
        {
            if (!targetIsPlayer)
            {
                nowSpeed = speed;
                animator.SetBool("isWalking", true);
            }
            else
            {
                nowSpeed = speed * 4;
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
                if (!targetIsPlayer)
                {
                    Destroy(targetPoint.gameObject);
                    targetPoint = null; // чтобы не повторять
                }
                else
                {
                    if (isGrounded)
                    {
                        animator.Play("Hit");
                        Destroy(targetPoint.gameObject);
                        playerTimer = 0;
                        PlayerController playerController = FindAnyObjectByType<PlayerController>();
                        if (!playerController.isRolling)
                        {
                            playerController.velocity.y = Mathf.Sqrt(playerController.jumpHeight * -2f * playerController.gravity / 1.5f);
                            playerController.healthManager.GetDamage(damage);
                        }
                    }
                }
            }
        }
        // Если цели нет — враг стоит на месте
        if (targetPoint == null && isGrounded)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }
    public override void OnDied()
    {
        animator.SetBool("isDead", true);
        deadTimer += Time.deltaTime;
        if (targetPoint != null) Destroy(targetPoint.gameObject);
        if (deadTimer >= deadTime)
        {
            if (!vbSwit)
            {
                healthManager.healthGO.SetActive(false);
                vbSwit = true;
                foreach (Outline o in outlines)
                {
                    VoxelBreaker vb = o.gameObject.AddComponent<VoxelBreaker>();
                    vb.cellSize = 0.1f;
                    vb.BreakIntoCubes();
                    if (o.gameObject.name == "body") o.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            if (deadTimer >= deadTime + 15) Destroy(this.gameObject);
        }
    }
}
