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
        if (Input.GetButtonDown("Hit") && playerController.isGrounded && !playerController.isRolling && !playerController.isSeat)
        {
            timer = 0;
            animator.Play("Hit");
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
            enemy.healthManager.GetDamage(damage);
            if (enemy.healthManager.HEALTH > 0) enemy.velocity.y += 11;
            //particles.Play();
        }
        /*
        if (other.CompareTag("Piramid"))
        {
            PiramidController pc = other.GetComponent<PiramidController>();
            pc.healthManager.GetDamage(damage);
            if (pc.healthManager.HEALTH > 0) pc.velocity.y += 11;
            particles.Play();
        }
        if (other.CompareTag("Hammur"))
        {
            HammurController hc = other.GetComponent<HammurController>();
            hc.healthManager.GetDamage(damage);
            if (hc.healthManager.HEALTH > 0) hc.velocity.y += 11;
            particles.Play();
        }
        if (other.CompareTag("Gun"))
        {
            GunController gc = other.GetComponent<GunController>();
            gc.healthManager.GetDamage(damage);
            if (gc.healthManager.HEALTH > 0) gc.velocity.y += 11;
            particles.Play();
        }
        if (other.CompareTag("Powersnake"))
        {
            PowersnakeController psc = other.GetComponent<PowersnakeController>();
            psc.healthManager.GetDamage(damage);
            if (psc.healthManager.HEALTH > 0) psc.velocity.y += 11;
            particles.Play();
        }
        */
    }
}
