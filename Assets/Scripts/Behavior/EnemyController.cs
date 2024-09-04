using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class EnemyController : Health
{
    public Follow slot;
    private Vector2 pos;
    public void Update()
    {
        if (pos == Vector2.zero)
        {
            pos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized / 5f;
        }
        if (hearts <= 0)
        {
            slot.offset = pos;
            slot.hide = false;
        }
    }
}
