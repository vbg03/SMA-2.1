//using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Componentes de personaje")]
    public PlayerInputActions PlayerInputAction;
    public CharacterController characterController;
    public Animator animator;
    public LayerMask whatIsGround; // Asigna el layer del suelo desde el Inspector
    
    [Header("Estados del personaje")]
    public bool isFalling = false;
    public bool isMoving = false;
    public bool isMovementPressed = false;
    public bool isGrounded = true;
    public Transform groundCheck;  // Un objeto vacío (Empty GameObject) que marque el punto desde el cual lanzamos el Raycast (ubicado en los pies del personaje)

    [Header("Direccion del personaje")]
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;

    [Header("Valores de movimiento")]
    public float walkMultiplier = 2.0f;
    public float rotationFactorPerFrame = 13.0f;
    public float groundCheckDistance = 0.4f; // La distancia entre el personaje y el suelo para considerar que está tocando el suelo
    public float gravityScale = -1.0f;

    private float realMovementSpeed;
    private float animMovementSpeed;


    private void Awake()
    {
        PlayerInputAction = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        PlayerInputAction.PlayerBasicActions.Move.started += onMovementInput;
        PlayerInputAction.PlayerBasicActions.Move.canceled += onMovementInput;
        PlayerInputAction.PlayerBasicActions.Move.performed += onMovementInput;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //handlAnimation();
        handleRotation();
        handleMovement();
        handleGravity();
    }
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();

        // Calcular si el jugador se esta moviendo o no
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        isMoving = isMovementPressed;

        // Aplicar rotación isométrica a la entrada
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        Vector3 inputAsVector = new Vector3(currentMovementInput.x, 0, currentMovementInput.y);
        Vector3 rotatedInput = isoMatrix.MultiplyPoint3x4(inputAsVector);

        // Asignar el movimiento a las variables de movimiento
        currentMovement.x = rotatedInput.x * walkMultiplier;
        currentMovement.z = rotatedInput.z * walkMultiplier;
    }
    void handleMovement()
    {
        realMovementSpeed = currentMovement.magnitude;
        if (!isMoving)
        {
            animMovementSpeed = 0.0f;
            characterController.Move(currentMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
            animMovementSpeed = (realMovementSpeed / walkMultiplier * 1.0f);
        }

    }
    void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleGravity()
    {
        // Verificar si el personaje está en contacto con el suelo usando CheckSphere
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, whatIsGround);

        if (isGrounded)
        {
            // El jugador acaba de aterrizar
            isFalling = false;

            // Resetea el movimiento en Y cuando toca el suelo
            currentMovement.y = -2f;
        }
        else
        {
            // El jugador está en el aire
            isFalling = true;

            // Aplica la gravedad mientras está en el aire
            currentMovement.y += gravityScale * Time.deltaTime;
        }
    }
    void handlAnimation()
    {
        // Chequeo de condiciones de animación
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("Speed", animMovementSpeed, 0.05f, Time.deltaTime);

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
