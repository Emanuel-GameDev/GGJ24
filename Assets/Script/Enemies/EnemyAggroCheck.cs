using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroCheck : MonoBehaviour
{
    private BaseEnemy enemy;
    private LayerMask targetMask;

    private void Start()
    {
        enemy = GetComponentInParent<BaseEnemy>();
        targetMask = enemy.GetAggroMask();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Utility.LayerDetectedInMask(targetMask, collision.gameObject.layer))
        {
            Vector2 lastCollisionPos = collision.transform.position;
            enemy?.SetAggro(true, lastCollisionPos);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enemy?.SetAggro(false, new Vector2(0f, 0f));
    }
}
