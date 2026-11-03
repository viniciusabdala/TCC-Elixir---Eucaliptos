using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset InputActions;
    public CharacterController playerCharacterController;

    [SerializeField] private Transform playerCamera;

    private InputAction playerMoveAction;
    private InputAction playerJumpAction;

    private Vector2 playerMoveAmount;

    public bool podeMover = true;

    public float playerWalkSpeed;
    public float playerRotateDampening;
    public float turnSmoothingVelocity;

    private float verticalVelocity = 0f;
    private float gravity = -9.81f;
    public float jumpHeight = 7.0f;

    public int modoLocomocao = 0;
    public float staminaMaxima = 100f;
    private float staminaAtual = 100f;
    private float tempoSemSprint = 0f;
    public float tempoParaRegenerar = 2f;
    public float velocidadeRegeneracao = 15f;

    public float sprintMultiplier = 1.3f;
    private bool isSprinting = false;

    public event System.Action<float, float> OnStaminaChanged;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        playerMoveAction = InputSystem.actions.FindAction("Move");
        playerJumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        playerMoveAmount = playerMoveAction.ReadValue<Vector2>();

        Jump();
        Stamina();
        PlayerMoveAndRotate();
        TrocarModo();
    }
    private void Jump()
    {
        if (playerCharacterController.isGrounded && podeMover)
        {
            verticalVelocity = -1f;

            if (playerJumpAction.WasPressedThisFrame() && modoLocomocao == 0)
            {
                verticalVelocity = jumpHeight;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void PlayerMoveAndRotate()
    {
        Vector3 playerDirection = new Vector3(playerMoveAmount.x, 0f, playerMoveAmount.y).normalized;
        Vector3 verticalMove = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;

        if (playerDirection.magnitude >= 0.1f && podeMover)
        {
            float targetAngle = Mathf.Atan2(playerDirection.x, playerDirection.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothingVelocity, playerRotateDampening);

            transform.rotation = Quaternion.Euler(0f, smoothTargetAngle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, smoothTargetAngle, 0f) * Vector3.forward;
            playerCharacterController.Move(moveDirection.normalized * playerWalkSpeed * sprintMultiplier * Time.deltaTime + verticalMove);
        }
        else
        {
            playerCharacterController.Move(verticalMove);
        }
    }

    private void TrocarModo()
    {
        if (Input.GetKeyDown(KeyCode.Q) && podeMover)
        {
            modoLocomocao++;
        }

        if (modoLocomocao == 0)
        {
            playerWalkSpeed = 5.0f;
            playerRotateDampening = 0.1f;
        }

        if (modoLocomocao == 1)
        {
            playerWalkSpeed = 9.0f;
            playerRotateDampening = 0.3f;
        }

        if (modoLocomocao == 2)
        {
            playerWalkSpeed = 12.0f;
            playerRotateDampening = 0.6f;
        }

        if (modoLocomocao >= 3)
        {
            modoLocomocao = 0;
        }
    }

    private void Stamina()
    {
        if (Input.GetKey(KeyCode.LeftControl) && staminaAtual > 0)
        {
            sprintMultiplier = 1.8f;
            staminaAtual -=25f * Time.deltaTime;
            isSprinting = true;
            tempoSemSprint = 0f;
        }
        else
        {
            sprintMultiplier = 1f;
            isSprinting = false;
            tempoSemSprint += Time.deltaTime;

            if (tempoSemSprint >= tempoParaRegenerar && staminaAtual < staminaMaxima)
            {
                staminaAtual += velocidadeRegeneracao * Time.deltaTime;
                staminaAtual = Mathf.Min(staminaAtual, staminaMaxima);
            }
        }

        staminaAtual = Mathf.Max(staminaAtual, 0f);

        OnStaminaChanged?.Invoke(staminaAtual, staminaMaxima);
    }
}