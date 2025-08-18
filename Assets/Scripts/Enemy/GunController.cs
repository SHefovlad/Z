using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : Enemy
{
    [Header("Attack")]
    public float attackTime = 3;
    float attackTimer = 0;
    GunAttack gunAttack;

    public int playerPos;
    public LayerMask playerMask;
    public Vector3 playerFindDistance;
    public override void Start()
    {
        base.Start();

        gunAttack = GetComponent<GunAttack>();
        gunAttack.playerCollider = player.GetComponent<Collider>();
        gunAttack.gunCollider = GetComponent<Collider>();
    }
    public override void OnLive()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        playerPos = 2;
        if (Physics.CheckSphere(transform.position, playerFindDistance.x, playerMask))
        {
            playerPos = 1;
            if (Physics.CheckSphere(transform.position, playerFindDistance.y, playerMask))
            {
                playerPos = 0;
                if (Physics.CheckSphere(transform.position, playerFindDistance.z, playerMask))
                {
                    playerPos = -1;
                }
            }
        }
        // ”правление цел€ми в зависимости от playerPos
        if (playerPos == 1)
        {
            animator.SetBool("isAttacking", false);
            targetIsPlayer = true;
            if (targetPoint != null) Destroy(targetPoint.gameObject);
            targetPoint = new GameObject("gunTarget").transform;
            targetPoint.position = player.transform.position;
            attackTimer = 0;
        }
        else if (playerPos == -1)
        {
            attackTimer = 0;
            animator.SetBool("isAttacking", false);
            targetIsPlayer = false;
            if (targetPoint != null) Destroy(targetPoint.gameObject);
            targetPoint = new GameObject("gunTarget").transform;

            // ¬ектор от пушки к игроку
            Vector3 awayDir = (transform.position - player.transform.position).normalized;
            float fleeDistance = 5f;
            targetPoint.position = transform.position + awayDir * fleeDistance;
        }
        else if (playerPos == 0)
        {
            animator.SetBool("isAttacking", true);
            attackTimer += Time.deltaTime;
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // чтобы не наклон€ть по вертикали

            if (direction.magnitude > destroyDistance)
            {
                Vector3 moveDir = direction.normalized;

                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
            if (attackTimer >= attackTime)
            {
                attackTimer = 0;
                gunAttack.FireAt(player.transform.position);
                animator.SetTrigger("Shoot");
            }
        }
        else
        {
            animator.SetBool("isAttacking", false);
            attackTimer = 0;
            if (targetPoint != null) Destroy(targetPoint.gameObject);
        }
        if (playerPos == 0 && targetPoint != null)
        {
            Destroy(targetPoint.gameObject);
            targetPoint = null;
            targetIsPlayer = false;
        }
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (targetPoint != null)
        {
            animator.SetBool("isWalking", true);
            nowSpeed = speed;

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
                targetPoint = null; // чтобы не повтор€ть
                targetIsPlayer = false;
            }
        }
        // ≈сли цели нет Ч враг стоит на месте
        if (targetPoint == null && isGrounded)
        {
            animator.SetBool("isWalking", false);
        }
    }
}
