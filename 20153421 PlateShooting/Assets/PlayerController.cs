using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;

    [Header("Options")]
    public float speed = 10.0f;
    
    [Range(0.1f, 1f)] public float airPercent;

    float speedSmoothTime = 0.1f;
    float gravityVelocity;
    float currentSpeed;
    bool fire;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (currentSpeed >= 0.2f || fire) Rotate();

        Move(new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")));
    }

    private void Move(Vector2 input)
    {
        Vector3 velocity = Vector3.forward * input.y + Vector3.right * input.x;
        velocity.Normalize();

        velocity *= speed;

        gravityVelocity += Time.deltaTime * Physics.gravity.y;

        velocity = velocity + Vector3.up * gravityVelocity;

        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded) gravityVelocity = 0;
    }

    void Rotate()
    {

    }
}
