using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IController
{
    public float playerMaxHP;                   // �÷��̾� �ִ� ü��
    public float playerHP;                      // �÷��̾� ���� ü��
    public float playerMaxMP;                   // �÷��̾� �ִ� ����
    public float playerMP;                      // �÷��̾� ���� ����
    public float playerPower;                   // �÷��̾� �� 
    public float playerArmor;                   // �÷��̾� ����

    [SerializeField]
    protected float moveSpeed;                  // �̵� �ӵ� 
    protected bool isDash = false;              // ���
    protected bool isAttack = false;            // ����

    // ���� ����
    protected Coroutine comdoAttack;
    protected int comboCount = 0;
    protected int maxComboCount = 2;

    protected NavMeshAgent navAgent;            // NavMesh
    protected InputKeyManager inputKeyManager;  // Ű �޴��� 

    public Rigidbody rigid;                     // rigidbody ������Ʈ 
    public CapsuleCollider capsuleCollider;     // collider ������Ʈ 
    protected Animator animator;                // animator ������Ʈ

    public virtual void Move() { }
    public virtual void Dash() { }
    public virtual void Attack() { }
    public virtual void Hit() { }
    public virtual void Dead() { }
}
