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

    public Transform targetPos;

    public float cameraMoveSpeed = 120.0f;
    public float clampAngle = 80.0f;
    public float sensitivty = 150.0f;

    public float zoomSpeed = 10.0f;
    float startTime=0;
    float timeDuration = .8f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        Follow();
        FollowMouse();
        Zoom();      
    }

    void Follow()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            targetPos.position, cameraMoveSpeed * Time.deltaTime);
    }

    void FollowMouse()
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

        if (anim == Animation.Idle)
        {
            startTime = Time.time;
            anim = Animation.Moving;
        }

        float timer = (Time.time - startTime) / timeDuration;
        timer = Mathf.Clamp01(timer);

        transform.position =
            Vector3.Lerp(transform.position,
            transform.position + transform.forward * 2.0f, timer);

        if (timer >= 1f)
        {
            anim = Animation.Target;
            startTime = 0;
        }
    }
}
