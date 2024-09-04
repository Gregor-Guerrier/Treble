using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public ItemType itemType;
    public ItemRarity itemRarity;

    // Common method for using the item
    public abstract void Use();
}

public enum ItemType
{
    Chest,
    Helmet,
    Boots,
    Gun,
    Melee,
    Throwable,
    Consumable,
    Core
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
