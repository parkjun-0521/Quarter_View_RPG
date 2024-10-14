using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerController : MonoBehaviour, IController
{
    public float playerMaxHP;                   // 플레이어 최대 체력
    [SerializeField]
    private float playerHP;                     // 플레이어 남은 체력
    public float PlayerHP                       // 플레이어 체력 프로퍼티
    {
        get {
            return playerHP;
        }
        set {
            if(playerHP > playerMaxHP) {
                playerHP = playerMaxHP;
            }

            if (value < 0)
                playerHP = 0;
            else
                playerHP = value;
        }
    }
    public float playerMaxMP;                   // 플레이어 최대 마나
    public float playerMP;                      // 플레이어 남은 마나
    public float PlayerMP
    {
        get {
            return playerMP;
        }
        set {
            if (value < 0)
                playerMP = 0;
            else
                playerMP = value;
        }
    }
    public float playerPower;                   // 플레이어 힘 
    public float skillPower;                    // 스킬 데미지
    public float playerArmor;                   // 플레이어 방어력

    [SerializeField]
    protected float moveSpeed;                  // 이동 속도 
    protected bool isDash = false;              // 대시
    protected bool isAttack = false;            // 공격
    protected bool isSkill = false;             // 스킬 사용
    protected bool isMpMax = false;

    // 연속 공격
    protected Coroutine comdoAttack;
    protected int comboCount = 0;
    protected int maxComboCount = 2;

    protected NavMeshAgent navAgent;            // NavMesh
    protected InputKeyManager inputKeyManager;  // 키 메니저 

    [HideInInspector]
    public Rigidbody rigid;                     // rigidbody 컴포넌트 
    [HideInInspector]
    public CapsuleCollider capsuleCollider;     // collider 컴포넌트 
    protected Animator animator;                // animator 컴포넌트

    public abstract void Move();
    public virtual void Dash() { }
    public abstract void Attack();
    public virtual void Skill() { }
    public virtual void MPup() { }
    public abstract void Hit();
    public abstract void Dead();
}
