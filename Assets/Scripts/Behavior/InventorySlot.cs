using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public int slotID;
    public Color defaultColor;
    public Color hoverColor;
    public LayerMask inventoryLayer;
    public float transitionRate;
    private SpriteRenderer spriteRenderer;
    private PlayerController controller;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Follow>().player.GetComponent<PlayerController>();
    }

    public void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = Physics2D.OverlapPoint(mousePosition, inventoryLayer);
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, collider != null && collider.transform == this.transform ? hoverColor : defaultColor, transitionRate * Time.deltaTime);
        if (collider != null && collider.transform == this.transform)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (this.transform.childCount > 0)
                {
                    controller.SwapItem(slotID, this.transform);
                }
                else
                {
                    controller.AddItem(slotID, this.transform);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                controller.DropItem(slotID, this.transform);
            }
        }
    }
}
