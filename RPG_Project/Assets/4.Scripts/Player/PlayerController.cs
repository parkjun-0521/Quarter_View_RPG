using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class PlayerController : MonoBehaviour, IController
{
    public float playerMaxHP;                   // �÷��̾� �ִ� ü��
    [SerializeField]
    private float playerHP;                     // �÷��̾� ���� ü��
    public float PlayerHP                       // �÷��̾� ü�� ������Ƽ
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
    public float playerMaxMP;                   // �÷��̾� �ִ� ����
    public float playerMP;                      // �÷��̾� ���� ����
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
    public float playerPower;                   // �÷��̾� �� 
    public float skillPower;                    // ��ų ������
    public float playerArmor;                   // �÷��̾� ����

    [SerializeField]
    protected float moveSpeed;                  // �̵� �ӵ� 
    protected bool isDash = false;              // ���
    protected bool isAttack = false;            // ����
    protected bool isSkill = false;             // ��ų ���
    protected bool isMpMax = false;

    // ���� ����
    protected Coroutine comdoAttack;
    protected int comboCount = 0;
    protected int maxComboCount = 2;

    protected NavMeshAgent navAgent;            // NavMesh
    protected InputKeyManager inputKeyManager;  // Ű �޴��� 

    [HideInInspector]
    public Rigidbody rigid;                     // rigidbody ������Ʈ 
    [HideInInspector]
    public CapsuleCollider capsuleCollider;     // collider ������Ʈ 
    protected Animator animator;                // animator ������Ʈ

    public abstract void Move();
    public virtual void Dash() { }
    public abstract void Attack();
    public virtual void Skill() { }
    public virtual void MPup() { }
    public abstract void Hit();
    public abstract void Dead();
}
