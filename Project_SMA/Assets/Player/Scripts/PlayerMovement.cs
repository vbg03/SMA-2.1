using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Componentes de personaje")]
    public PlayerInputActions PlayerInputAction;
    public CharacterController characterController;
    public Animator animator;
    public LayerMask whatIsGround;

    [Header("Estados del personaje")]
    public bool isFalling = false;
    public bool isMoving = false;
    public bool isMovementPressed = false;
    public bool isGrounded = true;
    public Transform groundCheck;

    [Header("Dirección del personaje")]
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;

    [Header("Valores de movimiento")]
    public float walkMultiplier = 2.0f;
    public float rotationFactorPerFrame = 13.0f;
    public float groundCheckDistance = 0.4f;
    public float gravityScale = -1.0f;

    [Header("Animación")]
    public float vertDamp = 0.1f;
    public float stateDamp = 0.1f;

    private int hash_Vert  = Animator.StringToHash("Vert");
    private int hash_State = Animator.StringToHash("State");
    private int hash_isGrounded = Animator.StringToHash("isGrounded");
    private int hash_isFalling  = Animator.StringToHash("isFalling");
    private int hash_isMoving   = Animator.StringToHash("isMoving");

    private float targetSpeed;
    private float realMovementSpeed;
    private float vertValue;
    private float stateValue;

    private void Awake()
    {
        PlayerInputAction = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        PlayerInputAction.PlayerBasicActions.Move.started   += onMovementInput;
        PlayerInputAction.PlayerBasicActions.Move.performed += onMovementInput;
        PlayerInputAction.PlayerBasicActions.Move.canceled  += onMovementInput;

        if (animator) animator.applyRootMotion = false;
    }

    void Update()
    {
        handleRotation();
        handleMovement();
        handleGravity();
        handleAnimation();
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = !Mathf.Approximately(currentMovementInput.x, 0f) ||
                            !Mathf.Approximately(currentMovementInput.y, 0f);
        isMoving = isMovementPressed;

        // Rotación isométrica
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        Vector3 inputAsVector = new Vector3(currentMovementInput.x, 0, currentMovementInput.y);
        Vector3 rotatedInput = isoMatrix.MultiplyPoint3x4(inputAsVector);

        targetSpeed = walkMultiplier;

        currentMovement.x = rotatedInput.x * targetSpeed;
        currentMovement.z = rotatedInput.z * targetSpeed;
    }

    void handleMovement()
    {
        realMovementSpeed = new Vector3(currentMovement.x, 0f, currentMovement.z).magnitude;
        characterController.Move(currentMovement * Time.deltaTime);
    }

    void handleRotation()
    {
        Vector3 positionToLookAt = new Vector3(currentMovement.x, 0.0f, currentMovement.z);
        if (isMovementPressed && positionToLookAt.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, whatIsGround);

        if (isGrounded)
        {
            isFalling = false;
            if (currentMovement.y < 0f) currentMovement.y = -2f;
        }
        else
        {
            isFalling = true;
            currentMovement.y += gravityScale * Time.deltaTime;
        }
    }

    void handleAnimation()
    {
        // Vert → magnitud de movimiento normalizada
        float targetVert = isMoving ? Mathf.Clamp01(realMovementSpeed / walkMultiplier) : 0f;
        vertValue = Mathf.Lerp(vertValue, targetVert, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, vertDamp)));
        animator.SetFloat(hash_Vert, vertValue);

        // State fijo en 0 → siempre walk
        stateValue = Mathf.Lerp(stateValue, 0f, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, stateDamp)));
        animator.SetFloat(hash_State, stateValue);

        animator.SetBool(hash_isGrounded, isGrounded);
        animator.SetBool(hash_isFalling, isFalling);
        animator.SetBool(hash_isMoving, isMoving);
    }

    public void OnEnable()
    {
        PlayerInputAction.PlayerBasicActions.Enable();
    }
    public void OnDisable()
    {
        PlayerInputAction.PlayerBasicActions.Disable();
    }
}
