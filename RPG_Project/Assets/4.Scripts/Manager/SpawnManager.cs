using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Vector3 spawnPos;

    void Start()
    { 
        PoolingManager.Instance.GetObject("Enemy_1", spawnPos);
    }
}
