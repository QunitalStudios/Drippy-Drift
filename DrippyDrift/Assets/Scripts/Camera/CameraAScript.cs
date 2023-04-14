using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CameraAScript : MonoBehaviour
{
    public GameObject attachedVehicle;
    public Transform follow;
    public Transform cameraPoint;
    public Transform cameraPoint2;
    [Range(0, 1)] public float smoothTime = 0.5f;
    public bool iffollow = true;
    Collider otherr;
    public bool show;

    public FlareLayer FlareLayer;

    void Start()
    {
        transform.LookAt(follow);

        attachedVehicle = GameObject.FindGameObjectWithTag("Player");
        FlareLayer = this.GetComponent<FlareLayer>();
        FlareLayer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            otherr = other;
            iffollow = false;
            transform.LookAt(follow);
        }
    }

    private void FixedUpdate()
    {
        if (iffollow)
        {
            if (attachedVehicle.GetComponent<CarController>().isGoingForward())
            {
                camerafollowForward();
            }
            else
            {
                camerafollowBackward();
            }
        }
        else
        {
            if (otherr != null)
            {
                if (transform.position.y < 6.8f)
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, otherr.transform.position.y + 7f, transform.position.z + 0.7f), 1f * Time.deltaTime);
            }
            transform.LookAt(follow);
        }

    }

    private void camerafollowForward()
    {
        transform.position = cameraPoint.position * (1 - smoothTime) + transform.position * smoothTime;
        transform.LookAt(follow);
    }

    private void camerafollowBackward()
    {
        transform.position = cameraPoint2.position * (1 - smoothTime) + transform.position * smoothTime;
        transform.LookAt(follow);
    }
}