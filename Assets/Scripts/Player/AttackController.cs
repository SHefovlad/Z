using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public PlayerController playerController;
    CapsuleCollider col;
    public Animator animator;
    public bool isHitting = false;
    public float hittingTime;
    public Vector2 colliderEnable;
    float timer;
    public float damage;
    public ParticleSystem particles;
    private void Start()
    {
        timer = colliderEnable.y + 1;
        col = GetComponent<CapsuleCollider>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        isHitting = timer < hittingTime;
        if (Input.GetButtonDown("Hit") && !isHitting && playerController.isGrounded && !playerController.isRolling && !playerController.isSeat)
        {
            timer = 0;
            animator.Play(playerController.moving ? "MoveHit" : "Hit");
        }
        if (timer >= colliderEnable.x && timer <= colliderEnable.y)
        {
            col.enabled = true;
        }
        else
        {
            col.enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.healthManager?.GetDamage(damage);
            if (enemy.healthManager?.HEALTH > 0 && enemy.healthManager?.damageJump == true) enemy.velocity.y += 11;
            //particles.Play();
        }
    }
}
