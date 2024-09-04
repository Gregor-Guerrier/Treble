using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Follow[] heartsTransforms;
    [Range(0, 5)]
    public int hearts = 5;

    public void Damage(int damage)
    {
        hearts -= damage;
        for (int i = 0; i < heartsTransforms.Length; i++)
        {
            heartsTransforms[i].GetComponent<SpriteRenderer>().enabled = i < hearts;
            heartsTransforms[i].offset.x = (i + 1) / 10f - 0.1f - (hearts / 10f - 0.1f) / 2f;
            heartsTransforms[i].defaultPosition.x = (i + 1) / 10f - 0.1f - (hearts / 10f - 0.1f) / 2f;
        }
    }

    public void Damage(int part, Vector3 damage, Vector3 armor)
    {
        print($"{LayerMask.NameToLayer("Head")} and {part}");
        if (part == LayerMask.NameToLayer("Head"))
        {
            hearts -= (int)(damage.x - armor.x);
        }
        else if (part == LayerMask.NameToLayer("Body"))
        {
            hearts -= (int)(damage.y - armor.y);
        }
        else
        {
            hearts -= (int)(damage.z - armor.z);
        }

        for (int i = 0; i < heartsTransforms.Length; i++)
        {
            heartsTransforms[i].GetComponent<SpriteRenderer>().enabled = i < hearts;
            heartsTransforms[i].offset.x = (i + 1) / 10f - 0.1f - (hearts / 10f - 0.1f) / 2f;
            heartsTransforms[i].defaultPosition.x = (i + 1) / 10f - 0.1f - (hearts / 10f - 0.1f) / 2f;
        }
    }
}
