using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform plrTrans;

    public float mouseSensitivity;

    public float yaw;
    public float pitch;

    public float rotationLimit = 90f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        rotateCamera();

    }

    public void rotateCamera()
    {

        float deltaX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float deltaY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += deltaX;
        pitch -= deltaY; 
        pitch = Mathf.Clamp(pitch, -rotationLimit, rotationLimit);

        plrTrans.localRotation = Quaternion.Euler(pitch, yaw, 0f);

    }
}
