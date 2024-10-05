using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IController
{
    [SerializeField]
    protected float enemyMoveSpeed;     // 적 이동 속도 
    [SerializeField]
    protected float enemyPower;         // 적 힘 

    [SerializeField]
    protected float enemyMaxHP;         // 적 최대 체력
    private float enemyHP;              // 적 현재 체력
    [SerializeField]
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

    protected bool isAttack;

    public Rigidbody enemyRigid;
    public CapsuleCollider enemyCollider;
    public NavMeshAgent enemyNavMeshAgent;

    public virtual void Move() { }      // 적 이동 
    public void Attack() { }            // 적 공격 
    public void Hit() { }               // 적 피격 
    public void Dead() { }              // 적 사망
}
