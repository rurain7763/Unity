using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    Stat myStat;

    public Stat MyStat
    {
        get
        {
            return myStat;
        }

        set
        {
            myStat = value;
        }
    }

    public static PlayerController Instance
    {
        get
        {
            return _Instance;
        }
    }

    public PlayerMode mode = PlayerMode.Idle;

    CharacterController characterController;
    
    float gravityVelocity;
    float currentSpeed 
        =>new Vector2(characterController.velocity.x,characterController.velocity.z).magnitude;

    public GameObject crossHair;
    public Transform muzzlePos;
    bool fire;

    public Hp hp;
    private void Awake()
    {
        _Instance = this;
        myStat.healthPoint = 100;
        myStat.power = 2;
        myStat.speed = 10.0f;
        hp.curhealthPoint = myStat.healthPoint;
        hp.maxhealthPoint = myStat.healthPoint;
        hp.curSize = 500;
        hp.maxSize = 500;
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

        velocity *= myStat.speed;

        gravityVelocity += Time.deltaTime * Physics.gravity.y;

        velocity = velocity + Vector3.up * gravityVelocity;

        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded) gravityVelocity = 0;
    }

    public float turnSmoothVelocity = 0.1f;
    public float turnSmoothTime = 0.1f;

    void Rotate()
    {
        var targetRot = Camera.main.transform.eulerAngles.y;

        targetRot = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            targetRot, ref turnSmoothVelocity, turnSmoothTime);

        transform.eulerAngles = Vector3.up * targetRot;
    }

    public LayerMask layer;

    void FireSystem()
    {
        if (Input.GetMouseButton(1))
        {
            mode = PlayerMode.Zoom;
            crossHair.SetActive(true);

            if (Input.GetMouseButton(0) && canShoot)
            {
                if(Physics.Raycast(Camera.main.transform.position
                    ,Camera.main.transform.forward,out var hit,layer))
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();

                    if (enemy != null)
                    {
                        enemy.TakeDamage(myStat.power);
                    }

                    print(hit.collider.gameObject);
                }

                canShoot = false;
                StartCoroutine(ShootDelay(shootDelay));
            }
        }

        else
        {
            crossHair.SetActive(false);
            mode = PlayerMode.Idle;
        }
    }

    bool canShoot = true;
    public float shootDelay = 0.05f; 

    IEnumerator ShootDelay(float shootDelay)
    {
        yield return new WaitForSeconds(shootDelay);

        canShoot = true;
    }

    public void TakeDamage(int damage)
    {
        myStat.healthPoint -= damage;

        hp.TakeDamage(damage);

        if (myStat.healthPoint <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
