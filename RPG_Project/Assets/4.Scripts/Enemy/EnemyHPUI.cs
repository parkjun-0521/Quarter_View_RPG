using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUI : MonoBehaviour
{
    public Slider enemyHP;

    EnemyController enemyController;

    void Start()
    {
        enemyController = transform.parent.GetComponent<EnemyController>();
    }

    void Update()
    {
        enemyHP.value = enemyController.EnemyHP / enemyController.enemyMaxHP;
    }
}
