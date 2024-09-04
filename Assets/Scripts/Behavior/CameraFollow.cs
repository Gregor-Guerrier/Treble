using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float defaultSmoothSpeed = 0.125f; // How smooth the camera follows
    public float zoomedSmoothSpeed = 0.5f;
    public Vector3 offset; // Offset from the player position
    public float defaultSize = 5f; // Default orthographic size
    public float zoomedSize = 2f; // Orthographic size when zoomed in
    public float zoomTransitionSpeed = 2f; // Speed of the zoom transition

    private float targetSize;
    private float currentZoomTransitionSpeed;

    private void Start()
    {
        targetSize = defaultSize; // Initialize targetSize to defaultSize
        currentZoomTransitionSpeed = zoomTransitionSpeed; // Initialize currentZoomTransitionSpeed
    }

    private void FixedUpdate()
    {
        // Calculate the desired position with the offset
        Vector3 desiredPosition = player.position + offset;
        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Input.GetKey(KeyCode.Tab) ? zoomedSmoothSpeed : defaultSmoothSpeed);
        // Update the camera's position
        transform.position = smoothedPosition;

        // Handle zoom in and out with the Tab key
        if (Input.GetKey(KeyCode.Tab))
        {
            targetSize = zoomedSize;
        }
        else
        {
            targetSize = defaultSize;
        }

        // Smoothly transition between the current and target size
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, Time.deltaTime * currentZoomTransitionSpeed);
    }
}
