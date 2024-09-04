using Unity.VisualScripting;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public bool hide = false;
    public bool show = false;
    public Transform player; // Reference to the player's transform
    public float smoothSpeed = 0.125f; // How smooth the camera follows
    public Vector3 offset; // Offset when the inventory is visible
    public Vector3 defaultPosition; // Offset when the inventory is hidden
    public float transitionSpeed = 5f; // Speed of the transition

    public Vector3 currentOffset;
    private void Start()
    {
        // Initialize the current offset to default position
        currentOffset = Vector3.zero;
    }

    private void FixedUpdate()
    {
        DoAllThatFr();
    }

    public void DoAllThatFr()
    {
        // Determine the target offset based on whether Tab is pressed
        Vector3 targetOffset = (Input.GetKey(KeyCode.Tab) && !hide) || show ? offset : defaultPosition;

        // Smoothly interpolate the current offset towards the target offset
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, transitionSpeed * Time.deltaTime);

        // Calculate the desired position with the current offset
        Vector3 desiredPosition = player.position + currentOffset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
