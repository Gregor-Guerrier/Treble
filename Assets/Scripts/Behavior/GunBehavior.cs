using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GunBehavior2D : MonoBehaviour
{
    public MagneticField[] fields;
    public Gun equippedGun;
    private float nextTimeToFire = 0f;

    private List<List<Vector2>> allDebugPoints = new List<List<Vector2>>();
    private const float mu0 = 4 * Mathf.PI * 1e-7f;

    void Start()
    {
        fields = FindObjectsOfType<MagneticField>();
    }
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && !Input.GetKey(KeyCode.Tab) && equippedGun != null)
        {
            nextTimeToFire = Time.time + 1f / equippedGun.fireRate;
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        var bullet = Instantiate(equippedGun.bulletPrefab, transform.position, Quaternion.Euler(transform.right));
        var currGun = equippedGun;
        Vector2 direction = transform.right;
        Vector2 origin = transform.position;
        int currentBounces = 0;
        List<Vector2> debugPoints = new List<Vector2> { origin };
        allDebugPoints.Add(debugPoints);

        while (currentBounces <= currGun.maxBounces)
        {
            RaycastHit2D hit;
            float remainingDistance = currGun.range;

            while (remainingDistance > 0)
            {
                bullet.position = origin;
                // Calculate the angle to rotate towards the target
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Rotate the transform to face the target
                bullet.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                Vector2 travelStep = direction * currGun.bulletSpeed * Time.deltaTime;
                remainingDistance -= travelStep.magnitude;

                if (currGun.bulletType == Gun.BulletType.Magnetic)
                {
                    var closestFields = fields.Where(x => x.range >= Vector2.Distance(origin, x.transform.position));
                    foreach (MagneticField field in closestFields)
                    {
                        direction += ((Vector2)field.transform.position - origin).normalized * field.strength / 25f;
                    }
                    direction = direction.normalized;
                }

                hit = Physics2D.Raycast(origin, direction, travelStep.magnitude);

                if (hit.collider != null)
                {
                    debugPoints.Add(hit.point);

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Mirror"))
                    {
                        if (currGun.bulletType == Gun.BulletType.Reflective)
                        {
                            direction = Vector2.Reflect(direction, hit.normal);
                            origin = hit.point + direction * currGun.bulletSpeed * Time.deltaTime; ;
                            currentBounces++;
                            break;
                        }
                        else
                        {
                            Destroy(bullet.gameObject);
                            yield break;
                        }
                    }
                    else
                    {
                        ApplyDamage(hit.collider, equippedGun.damage);
                        Destroy(bullet.gameObject);
                        yield break;
                    }
                }
                else
                {
                    origin += travelStep;
                    debugPoints.Add(origin);
                }

                yield return null; // Wait for the next frame
            }

            if (remainingDistance <= 0)
            {
                Destroy(bullet.gameObject);
                yield break;
            }
        }

        // Remove the debug points after a delay to clear the path from the Scene view
        Destroy(bullet.gameObject);
        yield return new WaitForSeconds(1f);
        allDebugPoints.Remove(debugPoints);
    }

    void ApplyDamage(Collider2D target, Vector3 damage)
    {
        // Apply damage logic here
        var hitHealth = target.GetComponentInParent<Health>();
        print(hitHealth);
        if (hitHealth != null)
        {
            hitHealth.Damage(target.gameObject.layer, damage, hitHealth is PlayerController ? ((PlayerController)hitHealth).Protection() : Vector3.zero);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var debugPoints in allDebugPoints)
        {
            if (debugPoints.Count > 1)
            {
                for (int i = 0; i < debugPoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(debugPoints[i], debugPoints[i + 1]);
                }
            }
        }
    }
}
