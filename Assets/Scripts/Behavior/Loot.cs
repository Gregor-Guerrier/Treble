using Unity.VisualScripting;
using UnityEngine;

public class Loot : Follow
{
    public bool selected;
    private Vector2 originalPosition;
    public Item corresponding;
    public bool grounded = false;
    private Vector2 newOffset;

    void Start()
    {

    }
    void Update()
    {
        transform.GetComponent<SpriteRenderer>().sprite = corresponding.itemIcon;
        defaultPosition = originalPosition - (Vector2)player.position;
        offset = selected ? new Vector3(0, -0.5f, 0) : defaultPosition;
        GetComponent<BoxCollider2D>().isTrigger = selected;

        if (selected)
        {
            Destroy(GetComponent<Rigidbody2D>());
        }
        else if (GetComponent<Rigidbody2D>() == null)
        {
            this.AddComponent<Rigidbody2D>();
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
        else
        {
            if (GetComponent<Rigidbody2D>().velocity.magnitude <= 0.01f && grounded == false)
            {
                grounded = true;
                originalPosition = transform.position;
                currentOffset = (Vector2)transform.position - (Vector2)player.position;
                newOffset = originalPosition;
            }
        }
    }

    void FixedUpdate()
    {
        if (grounded && selected)
        {
            DoAllThatFr();
            newOffset = currentOffset + player.position;
        }
        else if (!selected && grounded)
        {
            SometimesDoThatFr();
        }
    }

    public void SometimesDoThatFr()
    {
        // Determine the target offset based on whether Tab is pressed
        Vector3 targetOffset = originalPosition;

        // Smoothly interpolate the current offset towards the target offset
        newOffset = Vector3.Lerp(newOffset, targetOffset, transitionSpeed * Time.deltaTime);

        // Calculate the desired position with the current offset
        Vector3 desiredPosition = newOffset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        currentOffset = smoothedPosition - player.position;
        transform.position = smoothedPosition;
    }

    public void Snapshot()
    {
        selected = true;
    }
}
