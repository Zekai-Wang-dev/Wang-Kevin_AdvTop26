using JetBrains.Annotations;
using NodeCanvas.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    public float playerSpeed = 5f;

    public Rigidbody rb;

    public Blackboard targetBlackboard; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
        controlPlayer();
        checkView();

    }

    public void controlPlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 input = new Vector2(horizontal, vertical);

        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z).normalized;

        Vector2 right = new Vector2(transform.right.x, transform.right.z).normalized;

        Vector2 moveDirection = forward * input.y + right * input.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f) * playerSpeed;


        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.y);

    }

    public void checkView()
    {

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {

            if (hit.collider.CompareTag("Enemy"))
            {
                targetBlackboard.SetVariableValue("isSeen", true);
            }
            else
            {
                targetBlackboard.SetVariableValue("isSeen", false);
            }

        }

    }

}
