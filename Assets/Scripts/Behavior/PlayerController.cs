using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class PlayerController : Health
{
    public Sprite crouch;
    public float speed = 5f;
    public Transform gunTransform;
    private GunBehavior2D gun;
    public float jumpForce = 10f;
    public float maxJumpHeight = 4f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float jumpTimeCounter;
    private bool isJumping;
    private float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    public Item[] inventory = new Item[6];
    public Follow[] inventorySlots = new Follow[6];
    private int inventoryIndex = 0;

    public LayerMask lootLayer;
    public float lootRadius = 0.5f;
    private Loot[] loot;
    public Transform lootHolder;
    private int lootIndex = 0;
    private Loot selectedLoot;
    public Loot lootPrefab;
    private SpriteRenderer sr;
    private Sprite def;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gun = gunTransform.GetComponent<GunBehavior2D>();
        sr = GetComponent<SpriteRenderer>();
        def = sr.sprite;
    }

    void Update()
    {

        sr.sprite = Input.GetKey(KeyCode.S) ? crouch : def;
        gun.equippedGun = inventory[inventoryIndex] is Gun ? (Gun)inventory[inventoryIndex] : null;

        if (Input.GetKey(KeyCode.Tab))
        {
            loot = Physics2D.OverlapCircleAll(transform.position, lootRadius, lootLayer)
                .Where(item => item.GetComponent<Loot>().defaultPosition.magnitude < (lootRadius / 2) && item.GetComponent<Loot>().grounded)
                .OrderBy(i => i.GetHashCode())
                .Select(item => item.GetComponent<Loot>())
                .ToArray();

            if (loot.Length > 0)
            {
                if (selectedLoot != null)
                {
                    lootIndex = Array.IndexOf(loot, selectedLoot);
                    if (lootIndex <= 0)
                    {
                        selectedLoot.selected = false;
                        selectedLoot = null;
                    }
                }
                else
                {
                    lootIndex = 0;
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    lootIndex = lootIndex - 1 < 0 ? loot.Length - 1 : lootIndex - 1;
                    if (selectedLoot != null) selectedLoot.selected = false;
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    lootIndex = lootIndex + 1 >= loot.Length ? 0 : lootIndex + 1;
                    if (selectedLoot != null) selectedLoot.selected = false;

                }
                else
                {
                    if (selectedLoot != null && Array.IndexOf(loot, selectedLoot) != lootIndex) selectedLoot.selected = false;
                }

                selectedLoot = loot[lootIndex < 0 ? 0 : lootIndex];
                selectedLoot.Snapshot();
            }
            else
            {
                if (selectedLoot != null)
                {
                    lootIndex = Array.IndexOf(loot, selectedLoot);
                    if (lootIndex <= 0)
                    {
                        selectedLoot.selected = false;
                        selectedLoot = null;
                    }
                }
            }
        }
        else
        {
            if (selectedLoot != null) selectedLoot.selected = false;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        sr.flipX = moveHorizontal > 0 ? false : (moveHorizontal < 0) ? true : sr.flipX;
        if (Input.GetKey(KeyCode.S))
        {
            moveHorizontal = 0;
        }
        Vector2 movement = new Vector2(moveHorizontal, 0);
        rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter < maxJumpHeight)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter += Time.deltaTime;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // Apply fall multiplier to make falling feel more natural
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Update the grounded status
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle gun rotation
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - gunTransform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void AddItem(int slotID, Transform slot)
    {
        if (selectedLoot != null)
        {
            switch (selectedLoot.corresponding.itemType)
            {
                case ItemType.Gun:
                case ItemType.Melee:
                case ItemType.Throwable:
                case ItemType.Consumable:
                    if (slotID < 3)
                    {
                        inventory[slotID] = selectedLoot.corresponding;
                        var icon = Instantiate(new GameObject(), slot.position, Quaternion.identity, slot);
                        icon.AddComponent<SpriteRenderer>();
                        icon.GetComponent<SpriteRenderer>().sprite = selectedLoot.corresponding.itemIcon;
                        icon.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        Destroy(selectedLoot.gameObject);
                    }
                    break;
                case ItemType.Chest:
                case ItemType.Boots:
                case ItemType.Helmet:
                case ItemType.Core:
                    if (slotID >= 3 && (!inventory.Any(x => selectedLoot.corresponding.itemType == x.itemType) || inventory[slotID].itemType == selectedLoot.corresponding.itemType))
                    {
                        inventory[slotID] = selectedLoot.corresponding;
                        var icon = Instantiate(new GameObject(), slot.position, Quaternion.identity, slot);
                        icon.AddComponent<SpriteRenderer>();
                        icon.GetComponent<SpriteRenderer>().sprite = selectedLoot.corresponding.itemIcon;
                        icon.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        Destroy(selectedLoot.gameObject);
                    }
                    break;
            }
        }
    }

    public void SwapItem(int slotID, Transform slot)
    {
        DropItem(slotID, slot);
        AddItem(slotID, slot);
    }

    public void DropItem(int slotID, Transform slot)
    {
        if (inventory[slotID] != null)
        {
            var loot = Instantiate(lootPrefab, position: slot.position, rotation: Quaternion.identity, parent: lootHolder);
            loot.name = "" + slot.GetHashCode();
            loot.GetComponent<SpriteRenderer>().sprite = inventory[slotID].itemIcon;
            loot.GetComponent<Loot>().grounded = false;
            loot.GetComponent<Loot>().corresponding = inventory[slotID];
            loot.GetComponent<Loot>().player = transform;
            loot.GetComponent<Rigidbody2D>().velocity = Vector3.up * 2f + ((slotID < 3) ? Vector3.left : Vector3.right);
            Destroy(slot.GetChild(0).gameObject);
            inventory[slotID] = null;
        }
    }

    public Vector3 Protection()
    {
        Vector3 armor = new Vector3(0, 0, 0);
        foreach (Item item in inventory)
        {
            if (item != null)
            {
                if (item.itemType == ItemType.Chest)
                {
                    armor += Vector3.up * ((Armor)item).protection;
                }
                else if (item.itemType == ItemType.Helmet)
                {
                    armor += Vector3.right * ((Armor)item).protection;
                }
                else if (item.itemType == ItemType.Boots)
                {
                    armor += Vector3.forward * ((Armor)item).protection;
                }
            }
        }
        return armor;
    }
}
