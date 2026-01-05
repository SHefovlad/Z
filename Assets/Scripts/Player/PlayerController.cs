using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    float x;
    float z;

    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    public Vector3 velocity;
    public bool isGrounded;
    public bool isRoofed;

    public Transform groundCheck;
    public Transform roofCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask roofMask;

    public Transform modelTransform;
    public Transform hatPlaceTransform;
    public float rotationSpeed = 10f;

    // Перекат
    public float rollSpeed = 10f;
    public float rollDuration = 0.4f;
    public bool isRolling = false;
    private Vector3 rollDirection;
    private float rollTimer;
    private float totalRollDuration;

    public Animator animator;

    public AttackController attackController;
    public PlayerHealthManager healthManager;
    public CameraHolder cameraHolder;

    Vector3 flatMove;

    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    Vector3 oldPosition = Vector3.zero;
    float treeTimer = 0;
    public bool onTree = false;

    public bool isSeat = false;

    public float stun;

    float hitX = 0;
    float hitZ = 0;
    public bool moving = false;

    Hat _currentHat = null;
    public Hat CurrentHat
    {
        get { return _currentHat; }
        set
        {
            foreach (Transform child in hatPlaceTransform) Destroy(child.gameObject);
            Instantiate(value.prefab, hatPlaceTransform);
            _currentHat = value;
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        animator.SetBool("isSeating", isSeat);
        if (stun > 0) stun -= Time.deltaTime;
        else stun = 0;
        animator.SetBool("isStunned", stun > 0);
        if (!isSeat && stun == 0)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            isRoofed = Physics.CheckSphere(roofCheck.position, groundDistance, roofMask);
            if (isGrounded) treeTimer = 0;
            animator.SetBool("isJumping", !isGrounded);

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            if (isRoofed && velocity.y > 0) velocity.y = 0;

            animator.SetBool("isRolling", isRolling);
            if (isRolling)
            {
                controller.Move(rollDirection * rollSpeed * Time.deltaTime);

                // Вращаем модель по оси Y
                //float rollProgress = 1f - (rollTimer / totalRollDuration); // от 0 до 1
                //float spinAngle = 360f * Time.deltaTime / totalRollDuration;
                //modelTransform.Rotate(Vector3.right, spinAngle);

                rollTimer -= Time.deltaTime;
                if (rollTimer <= 0f)
                    isRolling = false;

                return;
            }
            if (!attackController.isHitting)
            {
                x = Input.GetAxis("Horizontal");
                z = Input.GetAxis("Vertical");

                moving = Mathf.Abs(x) + Mathf.Abs(z) >= 0.05f;

                hitX = x;
                hitZ = z;
            }
            else
            {
                x = hitX; z = hitZ;
            }

            Vector3 move = transform.right * x + transform.forward * z;
            move = move.normalized;
            if (move != Vector3.zero && Physics.CheckSphere(groundCheck.position, 1, groundMask))
            {
                onTree = false;
                animator.SetBool("isFalling", false);
                rb.useGravity = false;
                controller.enabled = true;
                capsuleCollider.enabled = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
            if (!onTree)
            {
                if (isGrounded)
                {
                    float attackSpeed = attackController.isHitting ? 0.3f : 1;
                    controller.Move(move * speed * attackSpeed * Time.deltaTime);

                    animator.SetBool("isWalking", Mathf.Abs(move.x) + Mathf.Abs(move.z) >= 0.05f);

                    flatMove = new Vector3(move.x, 0, move.z);
                }
                else if (treeTimer <= 0.5f)
                {
                    float attackSpeed = attackController.isHitting ? 0.3f : 1;
                    controller.Move(move * speed * attackSpeed * Time.deltaTime);

                    animator.SetBool("isWalking", Mathf.Abs(move.x) + Mathf.Abs(move.z) >= 0.05f);

                    flatMove = new Vector3(move.x, 0, move.z);
                }


                // Если задана цель, вращаемся на неё
                if (cameraHolder.target != null && cameraHolder.targeting && !attackController.isHitting)
                {
                    Vector3 directionToTarget = cameraHolder.target.transform.position - modelTransform.position;
                    directionToTarget.y = 0; // Игнорируем высоту
                    if (directionToTarget.magnitude > 0.1f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                        modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    }
                }
                // Иначе — вращаемся по направлению движения
                else if (flatMove.magnitude > 0.1f && !attackController.isHitting)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(flatMove);
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                if (Input.GetButtonDown("Jump") && isGrounded && !attackController.isHitting)
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                velocity.y += gravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);

                if (Input.GetButtonDown("Roll") && flatMove.magnitude > 0.1f && isGrounded && !attackController.isHitting)
                {
                    StartRoll(flatMove.normalized);
                }
            }
            if (Mathf.Abs(transform.position.y - oldPosition.y) <= 0.02f) treeTimer += Time.deltaTime;
            else treeTimer = 0;

            if (!isGrounded && treeTimer >= 1)
            {
                onTree = true;
                animator.SetBool("isFalling", true);
                rb.useGravity = true;
                controller.enabled = false;
                capsuleCollider.enabled = true;
            }

            oldPosition = transform.position;
        }
    }

    void StartRoll(Vector3 direction)
    {
        isRolling = true;
        rollDirection = direction;
        modelTransform.rotation = Quaternion.LookRotation(direction);
        rollTimer = rollDuration;
        totalRollDuration = rollDuration;
    }
}
