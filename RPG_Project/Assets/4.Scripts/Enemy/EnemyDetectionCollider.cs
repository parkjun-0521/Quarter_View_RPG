using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionCollider : MonoBehaviour
{
    EnemyController enemyController;
    void Awake()
    {
        enemyController = transform.parent.GetComponent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null) {
            if (other.gameObject.CompareTag("Player")) {
                enemyController.isDetection = true;
                enemyController.targetObj = other.gameObject;
            }
        }
    }
}
