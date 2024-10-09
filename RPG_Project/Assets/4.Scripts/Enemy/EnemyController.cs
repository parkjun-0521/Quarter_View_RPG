using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyController : MonoBehaviour, IController
{
    [SerializeField]
    protected float enemyMoveSpeed;     // �� �̵� �ӵ� 
    [SerializeField]
    protected float enemyPower;         // �� �� 

    [SerializeField]
    protected float enemyMaxHP;         // �� �ִ� ü��
    [SerializeField]
    private float enemyHP;              // �� ���� ü��
    public float EnemyHP                // ���� ü�� ������Ƽ          
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
    protected GameObject targetObj;     // ������ ������Ʈ 

    public Vector3 spawnPosition;       // ������ ��ġ

    public float circuitRange;          // ��ȸ �ϴ� ���� 
    protected float maxPosX;            // X ��ǥ ��ȸ ���� 
    protected float minPosX;            // -X ��ǥ ��ȸ ���� 
    protected float maxPosZ;            // Z ��ǥ ��ȸ ���� 
    protected float minPosZ;            // -Z ��ǥ ��ȸ ���� 
    public float tolerance = 0.5f;      // ���� ���� �������� 
    protected bool isZero = false;

    protected bool isAttack;            

    public Rigidbody enemyRigid;
    public CapsuleCollider enemyCollider;
    public Animator animator;
    public NavMeshAgent enemyNavMeshAgent;

    public GameObject detectionRange;           // Ž�� ����

    public abstract void Move();              // �� �̵� 
    public virtual void Tracking() { }        // �� �߰�
    public virtual void ResetPosition() { }   // ���� �̵�
    public abstract void Attack();            // �� ���� 
    public abstract void Hit();               // �� �ǰ� 
    public abstract void Dead();              // �� ���
}
