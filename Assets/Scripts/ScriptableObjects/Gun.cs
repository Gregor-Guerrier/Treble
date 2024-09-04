using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Inventory/Gun")]
public class Gun : Item
{
    public enum BulletType
    {
        Reflective,
        Magnetic
    }

    public BulletType bulletType = BulletType.Reflective;
    public Vector3 damage = new Vector3(5, 3, 1);
    public float range = 100f;
    public float fireRate = 1f;
    public int maxBounces = 3;
    public LayerMask mirrorLayer;
    public float bulletSpeed = 50f;
    public Transform bulletPrefab;

    public override void Use()
    {
        // Implement the logic to use the gun (e.g., shooting)
    }
}
