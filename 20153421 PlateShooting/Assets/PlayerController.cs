using UnityEngine;

public enum PlayerMode
{
    Zoom,
    Fire,
    Idle
}

public class PlayerController : MonoBehaviour
{
    static PlayerController _Instance;

    public static PlayerController Instance
    {
        get
        {
            return _Instance;
        }
    }


    public PlayerMode mode = PlayerMode.Idle;

    CharacterController characterController;

    [Header("Options")]
    public float speed = 10.0f;
    
    float gravityVelocity;
    float currentSpeed 
        =>new Vector2(characterController.velocity.x,characterController.velocity.z).magnitude;
    bool fire;

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (currentSpeed >= 0.2f || mode == PlayerMode.Zoom) Rotate();

        Move(new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")));

        FireSystem();
    }

    private void Move(Vector2 input)
    {
        Vector3 velocity = transform.forward * input.y + transform.right * input.x;
        velocity.Normalize();

        velocity *= speed;

        gravityVelocity += Time.deltaTime * Physics.gravity.y;

        velocity = velocity + Vector3.up * gravityVelocity;

        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded) gravityVelocity = 0;
    }

    public float turnSmoothVelocity = 0.1f;
    public float turrnSmoothTime = 0.1f;

    void Rotate()
    {
        var targetRot = Camera.main.transform.eulerAngles.y;

        targetRot = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            targetRot, ref turnSmoothVelocity, turrnSmoothTime);

        transform.eulerAngles = Vector3.up * targetRot;
    }

    void FireSystem()
    {
        if (Input.GetMouseButton(1))
        {
            mode = PlayerMode.Zoom;
        }

        else
        {
            mode = PlayerMode.Idle;
        }
    }
}
