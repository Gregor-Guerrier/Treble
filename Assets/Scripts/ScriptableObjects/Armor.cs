using UnityEngine;

[CreateAssetMenu(fileName = "New Helmet", menuName = "Inventory/Helmet")]
public class Armor : Item
{
    public int protection;

    public override void Use()
    {
        // Implement the logic to use the gun (e.g., shooting)
    }
}
