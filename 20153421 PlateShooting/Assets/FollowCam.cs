using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public enum Animation
    {
        Moving,
        Idle,
        Target
    }

    Animation anim = Animation.Idle;

    [Header("Must be Setted")]
    public Transform target;

    [Header("Camer Setting")]
    public float cameraFollowSpeed = 110.0f;
    public Vector3 dist = new Vector3(1f, 2.15f, -5f);
    public bool smoothMovement = true;
    public float smoothSpeed =5.0f;

    [Range(1,200)]
    public float sensitivty = 150.0f;

    Transform cameraPoint;

    float zoomSpeed = 10.0f;
    float startTime=0;
    float timeDuration = .8f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraPoint = transform.Find("CameraPoint");

        SpawnCamera();
    }

    private void LateUpdate()
    {
        FollowTarget();
        RotateByMouse();
        Zoom();      
    }

    void SpawnCamera()
    {
        GameObject go = new GameObject();
        Camera.main.GetComponent<AudioListener>().enabled = false;
        Camera.main.tag = "Untagged";

        go.tag = "MainCamera";
        go.name = "Main Camera";

        CameraScript cs= go.AddComponent<CameraScript>();
        go.AddComponent<Camera>();
        go.AddComponent<AudioListener>();

        cs.FollowCam = this;
        cs.Target = cameraPoint;
        cs.enabled = true;
    }

    void FollowTarget()
    {
        
          
        if (PlayerController.Instance.mode == PlayerMode.Zoom) return;

        cameraPoint.localPosition = dist;

        /*transform.position = Vector3.Lerp(transform.position,
                        target.position, cameraFollowSpeed * Time.deltaTime);*/

        transform.position = Vector3.MoveTowards(transform.position,
                            target.position, cameraFollowSpeed * Time.deltaTime);

    }

    void RotateByMouse()
    {
        float axisX = Input.GetAxis("Mouse X");
        float axisY = Input.GetAxis("Mouse Y");

        Vector3 curRot = transform.eulerAngles;

        float rotX = curRot.x - axisY * sensitivty * Time.deltaTime;
        float rotY = curRot.y + axisX * sensitivty * Time.deltaTime;

        curRot = new Vector3(rotX, rotY,curRot.z);

        transform.eulerAngles = curRot;
    }

    void Zoom()
    {
        if (PlayerController.Instance.mode != PlayerMode.Zoom)
        {
            anim = Animation.Idle;
            return;
        }


        switch (anim)
        {
            case Animation.Idle:

                startTime = Time.time;
                anim = Animation.Moving;

                break;
            case Animation.Moving:
                float timer = (Time.time - startTime) / timeDuration;
                timer = Mathf.Clamp01(timer);

                transform.position =
                    Vector3.Lerp(transform.position,
                    transform.position + target.transform.forward, timer * zoomSpeed);
                
                if (timer >= 1f)
                {
                    anim = Animation.Target;
                    startTime = 0;
                }

                break;

            case Animation.Target:

                Vector3 pos = transform.position + target.transform.forward * 2.0f;

                if (pos.magnitude >= 4.0f)
                {
                    pos = pos.normalized * 4.0f;
                }

                transform.position = target.transform.position + pos;

                break;
        }
    }
}
