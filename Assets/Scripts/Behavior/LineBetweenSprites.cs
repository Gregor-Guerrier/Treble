using UnityEngine;

public class LineBetweenSprites : MonoBehaviour
{
    public Transform sprite1; // First sprite's transform
    public Transform sprite2; // Second sprite's transform
    private LineRenderer lineRenderer;

    void Start()
    {
        sprite2 = this.transform;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        lineRenderer.SetPosition(0, sprite1.position);
        lineRenderer.SetPosition(1, sprite2.position);
    }
}
