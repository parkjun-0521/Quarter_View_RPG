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
    public static event EnemyHandle OnMove, OnResetPos, OnTracking, OnAttack, OnDead;

    public delegate void EnemyHitHandle(Collider other);
    public static event EnemyHitHandle OnHit;


    void Awake()
    {
        enemyRigid = GetComponent<Rigidbody>();
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
        
        // 체력 초기화 
        EnemyHP = enemyMaxHP;
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
        // 초기 생성 위치 저장 
        spawnPosition = transform.position;

        // x, z 최대 반경 저장 ( 초기 생성 위치에서 x, y 최대 반경을 더해서 저장 ) 
        maxPosX = spawnPosition.x + circuitRange;
        maxPosZ = spawnPosition.z + circuitRange;
        minPosX = spawnPosition.x - circuitRange;
        minPosZ = spawnPosition.z - circuitRange;
    }

    void Update()
    {
        if (isAttack) {
            OnAttack?.Invoke();
        }
        // 죽었을 때 
        if (EnemyHP <= 0) {
            OnDead?.Invoke();
        } 
    }

    void FixedUpdate()
    {
        if (targetObj != null) {
            OnTracking?.Invoke();   // 추적
        }
        else {
            OnMove?.Invoke();       // 범위 순회
        }

        // 적이 지정된 범위를 벗어났을 때
        ResetPosition();
    }

    public override void ResetPosition()
    {
        // 특정 범위를 나가면 초기 생성 지점으로 돌아온다. 이때는 무적 및 추적을 하지 않음 
        if ((transform.position.x > maxPosX || transform.position.x < minPosX ||
            transform.position.z > maxPosZ || transform.position.z < minPosZ) && !isZero) {

            isZero = true;
            enemyNavMeshAgent.isStopped = false;

            // 스폰 위치로 걸어가는 기능 
            if (enemyNavMeshAgent.destination != spawnPosition) {
                // 플레이어 탐지 비활성화 
                detectionRange.SetActive(false);
                // 추적 오브젝트 초기화
                targetObj = null;
                // 추격 취소
                isDetection = false;
                // 추격할 대상이 사라지면 추적 종료 
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
            // 플레이어 탐지 활성화 
            detectionRange.SetActive(true);
            // 추적 가능 상태 
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
        // 특정 범위를 순회한다.
        if (enemyNavMeshAgent.remainingDistance <= enemyNavMeshAgent.stoppingDistance + 0.5f) {
            // 0.5f 이내에 도착하면 새로운 위치로 이동
            int randomPosX = (int)Random.Range(minPosX, maxPosX);  // X 범위
            int randomPosZ = (int)Random.Range(minPosZ, maxPosZ);  // Z 범위

            // 랜덤 위치 설정
            Vector3 randomDirection = new Vector3(randomPosX, 0, randomPosZ);

            // 해당 방향으로 몬스터 회전
            transform.rotation = Quaternion.LookRotation(randomDirection);

            // 현재 위치에서 새로운 목표 위치 계산
            Vector3 targetPosition = transform.position + randomDirection;

            // NavMeshHit를 통해 유효한 위치 확인 및 이동
            NavMeshHit hit;
            float maxSampleDistance = 1.0f;  // NavMesh 상에서 허용되는 반경
            if (NavMesh.SamplePosition(targetPosition, out hit, maxSampleDistance, NavMesh.AllAreas)) {
                enemyNavMeshAgent.SetDestination(hit.position);
                animator.SetBool("IsRun", false);
                animator.SetBool("IsWalk", true);
            }
        }
    }
    public override void Tracking()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(targetObj.transform.position, transform.position);

        // 거리 조건에 따라 이동 결정
        if (distanceToPlayer > 1.5f) {
            // 방향 탐색 
            Vector3 direction = (targetObj.transform.position - transform.position).normalized;

            // 이동
            transform.position += direction * enemyMoveSpeed * Time.deltaTime;
            animator.SetBool("IsRun", true);

            // 적이 플레이어를 바라보도록 회전
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime);
        }
        else {
            // 가까워지면 이동 멈춤 및 애니메이션 상태 변경 (필요 시)
            animator.SetBool("IsRun", false);
            animator.SetBool("IsWalk", false);

            // 방향 탐색 
            Vector3 direction = (targetObj.transform.position - transform.position).normalized;

            // 적이 플레이어를 바라보도록 회전
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 범위에 닿을 시 타겟을 플레이어로 지정 
        if (other != null) {
            if (isDetection) {
                enemyNavMeshAgent.isStopped = true;
            }

            if (other.gameObject.CompareTag("PlayerAttackRange")) {
                OnHit?.Invoke(other);
            }
        }
    }

    public override void Attack()
    {
        // 플레이어를 추격하고 있는 상태에서 
        // 플레이어가 공격 범위에 들어오면 공격 ( 추적하는 플레이어 정보로 거리 측정 )
        attackCurDelay += Time.deltaTime;
        if(attackCurDelay >= attackMaxDelay) {
            if (targetObj.GetComponent<Player>().PlayerHP <= 0) {
                isAttack = false;
                enemyNavMeshAgent.isStopped = false;

                // 스폰 위치로 걸어가는 기능 
                if (enemyNavMeshAgent.destination != spawnPosition) {
                    // 플레이어 탐지 비활성화 
                    detectionRange.SetActive(false);
                    // 추적 오브젝트 초기화
                    targetObj = null;
                    // 추격 취소
                    isDetection = false;
                    // 추격할 대상이 사라지면 추적 종료 
                    OnTracking -= Tracking;

                    enemyNavMeshAgent.SetDestination(spawnPosition);
                    transform.rotation = Quaternion.LookRotation(spawnPosition);
                    enemyRigid.constraints = RigidbodyConstraints.FreezeAll;
                    animator.SetBool("IsRun", false);
                    animator.SetBool("IsWalk", true);
                }
            }
            else {
                OnMove -= Move;
                OnTracking -= Tracking;
                animator.SetBool("IsAttack", true);
                attackCurDelay = 0;
                StartCoroutine(AttackExit());
            }
        }
    }

    IEnumerator AttackExit()
    {
        yield return new WaitForSeconds(1.5f);
        if (animator.GetBool("IsAttack")) {
            OnMove += Move;
            OnTracking += Tracking;
            animator.SetBool("IsAttack", false);
        }
    }

    public override void Hit(Collider other)
    {
        float playerAttackPower = other.gameObject.transform.parent.GetComponentInParent<Player>().playerPower;
        float playerSkillPower = other.gameObject.transform.parent.GetComponentInParent<Player>().skillPower;

        // 체력 감소 
        EnemyHP -= playerAttackPower * playerSkillPower;
        // 공격을 받을 시 hit 애니메이션 동작 
    }

    public override void Dead()
    {
        // 체력이 0보다 작을 때 사망
        animator.SetTrigger("OnDead");
        OnMove -= Move;
        OnResetPos -= ResetPosition;
        OnTracking -= Tracking;
        OnAttack -= Attack;
        OnHit -= Hit;
        OnDead -= Dead;
    }
}
