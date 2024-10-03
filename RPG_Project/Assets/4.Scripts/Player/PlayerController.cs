using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IController
{
    public float playerMaxHP;                   // 플레이어 최대 체력
    public float playerHP;                      // 플레이어 남은 체력
    public float playerMaxMP;                   // 플레이어 최대 마나
    public float playerMP;                      // 플레이어 남은 마나
    public float playerPower;                   // 플레이어 힘 
    public float playerArmor;                   // 플레이어 방어력

    [SerializeField]
    protected float moveSpeed;                  // 이동 속도 
    protected bool isDash = false;              // 대시
    protected bool isAttack = false;            // 공격

    // 연속 공격
    protected Coroutine comdoAttack;
    protected int comboCount = 0;
    protected int maxComboCount = 2;

    protected NavMeshAgent navAgent;            // NavMesh
    protected InputKeyManager inputKeyManager;  // 키 메니저 

    public Rigidbody rigid;                     // rigidbody 컴포넌트 
    public CapsuleCollider capsuleCollider;     // collider 컴포넌트 
    protected Animator animator;                // animator 컴포넌트

    public virtual void Move() { }
    public virtual void Dash() { }
    public virtual void Attack() { }
    public virtual void Hit() { }
    public virtual void Dead() { }
}
