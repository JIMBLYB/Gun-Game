using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    [SerializeField]
    private Transform lookatPoint;
    [SerializeField]
    private float rotateSpeed = 12f;

    void Update()
    {
        transform.RotateAround(lookatPoint.position, Vector3.up, rotateSpeed * Time.deltaTime);
        transform.LookAt(lookatPoint, Vector3.up);
    }

}
