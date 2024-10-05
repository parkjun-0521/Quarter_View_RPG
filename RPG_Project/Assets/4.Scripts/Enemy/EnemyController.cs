using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IController
{
    [SerializeField]
    protected float enemyMoveSpeed;     // �� �̵� �ӵ� 
    [SerializeField]
    protected float enemyPower;         // �� �� 

    [SerializeField]
    protected float enemyMaxHP;         // �� �ִ� ü��
    private float enemyHP;              // �� ���� ü��
    [SerializeField]
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

    protected bool isAttack;

    public Rigidbody enemyRigid;
    public CapsuleCollider enemyCollider;
    public NavMeshAgent enemyNavMeshAgent;

    public virtual void Move() { }      // �� �̵� 
    public void Attack() { }            // �� ���� 
    public void Hit() { }               // �� �ǰ� 
    public void Dead() { }              // �� ���
}
