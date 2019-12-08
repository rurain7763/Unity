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
    CameraScript cs;

    
    [Header("X: right , Y: forward")]
    public Vector2 zoomPosOffset = new Vector3(0.5f, 2.0f);
    public float zoomSpeed = 10.0f;

    Vector3 zoomPoint;
    //float startTime=0;
    //float timeDuration = .8f;

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

        this.cs = cs;
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

            cs.lookTarget = true;
            anim = Animation.Idle;
            return;
        }

        switch (anim)
        {
            case Animation.Idle:
               
                cs.lookTarget = false;
                //startTime = Time.time;
                anim = Animation.Moving;

                transform.forward = Camera.main.transform.forward;

                transform.position = target.transform.position;

                zoomPoint = target.transform.position + target.transform.forward * zoomPosOffset.y +
                    target.transform.right * zoomPosOffset.x;

                break;

            case Animation.Moving:

                //float timer = (Time.time - startTime) / timeDuration;
                //timer = Mathf.Clamp01(timer);

                transform.position =
                    Vector3.Lerp(transform.position, zoomPoint, Time.deltaTime * zoomSpeed);

                if (Vector3.Distance(transform.position, zoomPoint) <= 0.01f)
                {
                    anim = Animation.Target;
                    //startTime = 0;
                }

                break;

            case Animation.Target:

                cs.gameObject.transform.forward = transform.forward;
                
                Vector3 pos = target.transform.position + target.transform.forward * zoomPosOffset.y +
                    target.transform.right * zoomPosOffset.x;

                transform.position = pos;

                break;
        }
    }
}
