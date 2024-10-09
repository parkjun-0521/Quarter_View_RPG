using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour, IController
{
    [SerializeField]
    protected float enemyMoveSpeed;     // 적 이동 속도 
    [SerializeField]
    protected float enemyPower;         // 적 힘 

    [SerializeField]
    protected float enemyMaxHP;         // 적 최대 체력
    [SerializeField]
    private float enemyHP;              // 적 현재 체력
    public float EnemyHP                // 현재 체력 프로퍼티          
    {
        get {
            return enemyHP;
        }
        set {
            if (value < 0)
                enemyHP = 0;
            else
                enemyHP = value;
        }
    }

    [SerializeField]
    protected GameObject targetObj;     // 추적할 오브젝트 

    public Vector3 spawnPosition;       // 생성될 위치

    public float circuitRange;          // 순회 하는 범위 
    protected float maxPosX;            // X 좌표 순회 범위 
    protected float minPosX;            // -X 좌표 순회 범위 
    protected float maxPosZ;            // Z 좌표 순회 범위 
    protected float minPosZ;            // -Z 좌표 순회 범위 
    public float tolerance = 0.5f;      // 원점 도착 오차범위 
    protected bool isZero = false;

    protected bool isAttack;            

    public Rigidbody enemyRigid;
    public CapsuleCollider enemyCollider;
    public Animator animator;
    public NavMeshAgent enemyNavMeshAgent;

    public GameObject detectionRange;           // 탐지 범위

    public abstract void Move();              // 적 이동 
    public virtual void Tracking() { }        // 적 추격
    public virtual void ResetPosition() { }   // 원점 이동
    public abstract void Attack();            // 적 공격 
    public abstract void Hit();               // 적 피격 
    public abstract void Dead();              // 적 사망
}
