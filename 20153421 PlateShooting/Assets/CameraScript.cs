using UnityEngine;

public class CameraScript : MonoBehaviour
{
    FollowCam followCam;

    public FollowCam FollowCam
    {
        set
        {
            followCam = value;
        }
    }

    Transform target;

    public Transform Target
    {
        set
        {
            target = value;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FollowCameraPoint();

        transform.LookAt(followCam.target);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
            transform.eulerAngles.y, 0);

    }

    void FollowCameraPoint()
    {
        if (followCam.smoothMovement)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followCam.smoothSpeed);
        }

        else
        {
            transform.position = target.position;
        }
    }
}
