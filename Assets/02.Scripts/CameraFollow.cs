using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Camera Position")]
    public float distance = 6f;
    public float height = 3f;
    public float lookAtHeight = 1.5f;

    [Header("Smoothing")]
    public float rotationDamping = 3f;
    public float heightDamping = 2f;

    [Header("Mouse Look")]
    public bool useMouseLook = true;
    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    [Header("Zoom")]
    public float minDistance = 3f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    private float currentRotationAngle = 0f;
    private float currentHeight = 0f;
    private float currentVerticalAngle = 20f;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (useMouseLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (target != null)
        {
            currentRotationAngle = target.eulerAngles.y;
            currentHeight = target.position.y + height;
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        if (useMouseLook)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            currentRotationAngle += mouseX * mouseSensitivity;
            currentVerticalAngle -= mouseY * mouseSensitivity;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                distance -= scroll * zoomSpeed;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }
        }

        float wantedRotationAngle = currentRotationAngle;
        float wantedHeight = target.position.y + height;

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(currentVerticalAngle, wantedRotationAngle, 0f);

        Vector3 offset = currentRotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.y = currentHeight;

        transform.position = desiredPosition;

        Vector3 lookAtPosition = target.position + Vector3.up * lookAtHeight;
        transform.LookAt(lookAtPosition);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}