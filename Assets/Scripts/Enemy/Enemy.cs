using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Enemy : MonoBehaviour
{
    public Transform targetPoint;
    public float speed = 3f;
    public float nowSpeed;
    public float gravity = -9.81f;
    public float destroyDistance = 1f;

    public CharacterController controller;
    public Vector3 velocity;
    public bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform modelTransform;
    public float rotationSpeed = 10f;

    public Animator animator;

    //Attack
    public GameObject player;
    public bool targetIsPlayer = false;

    //Health
    public EnemyHealthManager healthManager;
    public float deadTime;
    public float deadTimer;

    public CameraHolder cameraHolder;
    public List<Outline> outlines;
    public bool vbSwit = false;

    public virtual void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        cameraHolder = FindAnyObjectByType<CameraHolder>();
        controller = GetComponent<CharacterController>();
        Collider enemyCol = GetComponent<CharacterController>();
        Collider playerCol = player.GetComponent<CharacterController>();

        Physics.IgnoreCollision(playerCol, enemyCol);

        StartCoroutine(CreateTarget());
    }

    public virtual void Update()
    {
        if (cameraHolder.target == this.gameObject)
        {
            foreach (var o in outlines)
            {
                o.enabled = true;
            }
        }
        else
        {
            foreach (var o in outlines)
            {
                o.enabled = false;
            }
        }
        if (healthManager?.HEALTH <= 0) OnDied();
        else OnLive();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public virtual void OnDied()
    {
        animator?.SetBool("isDead", true);
        deadTimer += Time.deltaTime;
        if (targetPoint != null) Destroy(targetPoint.gameObject);
        if (deadTimer >= deadTime)
        {
            if (!vbSwit)
            {
                healthManager?.healthGO.SetActive(false);
                vbSwit = true;
                foreach (Outline o in outlines)
                {
                    VoxelBreaker vb = o.gameObject.AddComponent<VoxelBreaker>();
                    vb.cellSize = 0.1f;
                    vb.BreakIntoCubes();
                }
            }
            if (deadTimer >= deadTime + 15) Destroy(this.gameObject);
        }
    }

    public abstract void OnLive();

    public virtual IEnumerator CreateTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            if (Random.value < 0.65f && !targetIsPlayer && targetPoint == null)
            {
                animator?.Play("Walk");
                targetPoint = new GameObject("enemyTarget").transform;
                targetPoint.position = new Vector3(transform.position.x + (Random.value * 8 - 4), transform.position.y, transform.position.z + (Random.value * 8 - 4));
            }
        }
    }
}
