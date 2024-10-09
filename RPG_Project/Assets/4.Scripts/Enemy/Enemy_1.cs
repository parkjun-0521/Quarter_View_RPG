using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Enemy_1 : EnemyController
{
    public delegate void EnemyHandle();
    public static event EnemyHandle OnMove, OnResetPos, OnTracking, OnAttack, OnHit, OnDead;


    void Awake()
    {
        enemyRigid = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        OnMove += Move;
        OnResetPos += ResetPosition;
        OnTracking += Tracking;
        OnAttack += Attack;
        OnHit += Hit;
        OnDead += Dead;
    }

    private void OnDisable()
    {
        OnMove -= Move;
        OnResetPos -= ResetPosition;
        OnTracking -= Tracking;
        OnAttack -= Attack;
        OnHit -= Hit;
        OnDead -= Dead;
    }

    void Start()
    {
        // �ʱ� ���� ��ġ ���� 
        spawnPosition = transform.position;

        // x, z �ִ� �ݰ� ���� ( �ʱ� ���� ��ġ���� x, y �ִ� �ݰ��� ���ؼ� ���� ) 
        maxPosX = spawnPosition.x + circuitRange;
        maxPosZ = spawnPosition.z + circuitRange;
        minPosX = spawnPosition.x - circuitRange;
        minPosZ = spawnPosition.z - circuitRange;
    }

    void Update()
    {
        // �׾��� �� 
        if (EnemyHP <= 0) {
            OnDead?.Invoke();
        }
    }

    void FixedUpdate()
    {

        if (targetObj != null) {
            OnTracking?.Invoke();   // ����
        }
        else {
            OnMove?.Invoke();       // ���� ��ȸ
        }

        // ���� ������ ������ ����� ��
        ResetPosition();
    }

    public override void ResetPosition()
    {
        // Ư�� ������ ������ �ʱ� ���� �������� ���ƿ´�. �̶��� ���� �� ������ ���� ���� 
        if ((transform.position.x > maxPosX || transform.position.x < minPosX ||
            transform.position.z > maxPosZ || transform.position.z < minPosZ) && !isZero) {

            isZero = true;
            enemyNavMeshAgent.isStopped = false;

            // ���� ��ġ�� �ɾ�� ��� 
            if (enemyNavMeshAgent.destination != spawnPosition) {
                // �÷��̾� Ž�� ��Ȱ��ȭ 
                detectionRange.SetActive(false);
                // ���� ������Ʈ �ʱ�ȭ
                targetObj = null;
                // �߰��� ����� ������� ���� ���� 
                OnTracking -= Tracking;

                enemyNavMeshAgent.SetDestination(spawnPosition);
                transform.rotation = Quaternion.LookRotation(spawnPosition);
                enemyRigid.constraints = RigidbodyConstraints.FreezeAll;
                animator.SetBool("IsRun", false);
                animator.SetBool("IsWalk", true);
            }

        }

        if (isZero && Vector3.Distance(transform.position, spawnPosition) <= tolerance) {
            isZero = false;
            // �÷��̾� Ž�� Ȱ��ȭ 
            detectionRange.SetActive(true);
            // ���� ���� ���� 
            OnTracking += Tracking;
            enemyNavMeshAgent.isStopped = true;
            enemyRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            enemyNavMeshAgent.ResetPath();
            animator.SetBool("IsRun", false);
            animator.SetBool("IsWalk", false);
        }
    }

    public override void Move()
    {
        // Ư�� ������ ��ȸ�Ѵ�.
        if (enemyNavMeshAgent.remainingDistance <= enemyNavMeshAgent.stoppingDistance + 0.5f) {
            // 0.5f �̳��� �����ϸ� ���ο� ��ġ�� �̵�
            int randomPosX = (int)Random.Range(minPosX, maxPosX);  // X ����
            int randomPosZ = (int)Random.Range(minPosZ, maxPosZ);  // Z ����

            // ���� ��ġ ����
            Vector3 randomDirection = new Vector3(randomPosX, 0, randomPosZ);

            // �ش� �������� ���� ȸ��
            transform.rotation = Quaternion.LookRotation(randomDirection);

            // ���� ��ġ���� ���ο� ��ǥ ��ġ ���
            Vector3 targetPosition = transform.position + randomDirection;

            // NavMeshHit�� ���� ��ȿ�� ��ġ Ȯ�� �� �̵�
            NavMeshHit hit;
            float maxSampleDistance = 1.0f;  // NavMesh �󿡼� ���Ǵ� �ݰ�
            if (NavMesh.SamplePosition(targetPosition, out hit, maxSampleDistance, NavMesh.AllAreas)) {
                enemyNavMeshAgent.SetDestination(hit.position);
                animator.SetBool("IsRun", false);
                animator.SetBool("IsWalk", true);
            }
        }
    }
    public override void Tracking()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(targetObj.transform.position, transform.position);

        // �Ÿ� ���ǿ� ���� �̵� ����
        if (distanceToPlayer > 1.5f) {
            // ���� Ž�� 
            Vector3 direction = (targetObj.transform.position - transform.position).normalized;

            // �̵�
            transform.position += direction * enemyMoveSpeed * Time.deltaTime;
            animator.SetBool("IsRun", true);

            // ���� �÷��̾ �ٶ󺸵��� ȸ��
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
        }
        else {
            // ��������� �̵� ���� �� �ִϸ��̼� ���� ���� (�ʿ� ��)
            animator.SetBool("IsRun", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ������ ���� �� Ÿ���� �÷��̾�� ���� 
        if (other != null) {
            if (other.gameObject.CompareTag("Player")) {
                enemyNavMeshAgent.isStopped = true;
                targetObj = other.gameObject;
            }
        }
    }

    public override void Attack()
    {
        // �÷��̾ �߰��ϰ� �ִ� ���¿��� 
        // �÷��̾ ���� ������ ������ ���� ( �����ϴ� �÷��̾� ������ �Ÿ� ���� )
    }

    public override void Hit()
    {
        // ������ ���� �� hit �ִϸ��̼� ���� 
    }

    public override void Dead()
    {
        // ü���� 0���� ���� �� ���
        animator.SetTrigger("OnDead");
        OnMove -= Move;
        OnResetPos -= ResetPosition;
        OnTracking -= Tracking;
        OnAttack -= Attack;
        OnHit -= Hit;
        OnDead -= Dead;
    }
}