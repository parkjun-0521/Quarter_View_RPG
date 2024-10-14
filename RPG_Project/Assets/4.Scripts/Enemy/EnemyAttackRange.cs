using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRange: MonoBehaviour
{
    EnemyController enemyController;
    void Awake()
    {
        enemyController = transform.parent.GetComponent<EnemyController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other != null) {
            if (other.gameObject.CompareTag("Player")) {
                enemyController.isAttack = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null) {
            if (other.gameObject.CompareTag("Player")) {
                enemyController.isAttack = false;
            }
        }
    }
}
