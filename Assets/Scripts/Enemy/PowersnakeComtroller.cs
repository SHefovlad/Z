using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowersnakeController : Enemy
{
    [Header("Attack")]
    public float playerTimer;
    public float cooldown;
    public LayerMask playerLayer;
    public float playerFindDist = 10;
    public int damage = 10;
    public float stun = 1;

    public List<Rigidbody> rigidbodies;
    public List<Collider> colliders;

    public LayerMask playerMask;
    public override void OnLive()
    {
        if (targetPoint != null && targetPoint.gameObject.name != "powersnakeTargetAP" && Physics.CheckSphere(transform.position, playerFindDist, playerLayer)) targetIsPlayer = true;
        else targetIsPlayer = false;

        playerTimer += Time.deltaTime;
        if (targetIsPlayer && playerTimer >= cooldown)
        {
            if (targetPoint != null) Destroy(targetPoint.gameObject);
            targetPoint = new GameObject("powersnakeTarget").transform;
            targetPoint.transform.position = player.transform.position;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        animator.SetBool("isCrowling", targetPoint != null);
        if (targetPoint != null)
        {
            if (!targetIsPlayer && (targetPoint == null && targetPoint.gameObject.name == "powersnakeTargetAP"))
            {
                nowSpeed = speed;
            }
            else
            {
                nowSpeed = speed * 6;
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
                        Destroy(targetPoint.gameObject);
                        playerTimer = 0;
                        PlayerController playerController = FindAnyObjectByType<PlayerController>();
                        if (!playerController.isRolling)
                        {
                            //playerController.velocity.y = Mathf.Sqrt(playerController.jumpHeight * -2f * playerController.gravity / 1.5f);
                            playerController.healthManager.GetDamage(damage);
                            playerController.stun += stun;
                            targetIsPlayer = false;
                            targetPoint = new GameObject("powersnakeTargetAP").transform;
                            targetPoint.position = new Vector3(transform.position.x + ((Random.value * 2 - 1) * 10), transform.position.y, transform.position.z + ((Random.value * 2 - 1) * 10));
                        }
                    }
                }
            }
        }
        else
        {
            if (Physics.CheckSphere(transform.position, 4, playerMask))
            {
                Vector3 awayDir = (transform.position - player.transform.position).normalized;
                targetPoint = new GameObject("powersnakeTargetAP").transform;
                targetPoint.position = transform.position + awayDir * 5f;
                targetIsPlayer = false;
            }
        }
    }
    public override void OnDied()
    {
        if (!odi) { onDeath?.Invoke(); odi = true; }
        animator.enabled = false;
        controller.enabled = false;
        foreach (var r in rigidbodies) { r.useGravity = true; r.interpolation = RigidbodyInterpolation.Interpolate; }
        foreach (var c in colliders) { c.enabled = true; }
        deadTimer += Time.deltaTime;
        if (targetPoint != null) Destroy(targetPoint.gameObject);
        if (deadTimer >= deadTime)
        {
            foreach (var r in rigidbodies) { r.useGravity = false; r.interpolation = RigidbodyInterpolation.None; }
            if (!vbSwit)
            {
                healthManager.healthGO.SetActive(false);
                vbSwit = true;
                foreach (Outline o in outlines)
                {
                    onVoxelling?.Invoke();
                    VoxelBreaker vb = o.gameObject.AddComponent<VoxelBreaker>();
                    vb.cellSize = 0.1f;
                    vb.BreakIntoCubes();
                }
            }
            if (deadTimer >= deadTime + 15) Destroy(this.gameObject);
            foreach (var c in colliders) { c.enabled = false; }
        }
    }
    public override IEnumerator CreateTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            if (Random.value < 0.65f && targetPoint == null && !targetIsPlayer)
            {
                targetPoint = new GameObject("powersnakeTarget").transform;
                targetPoint.position = new Vector3(transform.position.x + (Random.value * 8 - 4), transform.position.y, transform.position.z + (Random.value * 8 - 4));
            }
        }
    }
}
