using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static InputKeyManager;

public class Player : PlayerController
{
    public delegate void PlayerHandle();
    public static event PlayerHandle OnMove, OnDash, OnAttack, OnMP, OnSkill, OnHit, OnDead;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        inputKeyManager = GameObject.FindGameObjectWithTag("InputKeyManager").GetComponent<InputKeyManager>();
    }

    void OnEnable()
    {
        OnMove += Move;
        OnDash += Dash;
        OnAttack += Attack;
        OnMP += MPup;
        OnSkill += Skill;
        OnHit += Hit;
        OnDead += Dead;

        // 기본 스킬 파워
        skillPower = 1;
    }

    private void OnDisable()
    {
        OnMove -= Move;
        OnDash -= Dash;
        OnAttack -= Attack;
        OnMP -= MPup;
        OnSkill -= Skill;
        OnHit -= Hit;
        OnDead -= Dead;
    }

    void Start()
    {    
        // 나중에 장착하고 있는 장비의 초기값을 주면 됨 
        playerMaxHP = 10000;                      
        playerMaxMP = 1000;                      
        playerPower = 100;                   
        playerArmor = 100;      
        
        Debug.Log(PlayerHP + "  " + playerMP + "  " + playerPower + "  " + playerArmor);
    }

    void Update()
    {
        // 대시 ( 피하기 )
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Dash)) && !isDash) {
            OnDash?.Invoke();
        }

        // 공격
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Attack)) && !isAttack && !animator.GetBool("IsDash")) {
            OnMove -= Move;
            RotateAttack();
            OnAttack?.Invoke();
        }

        // 마나 회복
        if (isMpMax) {
            if (PlayerMP >= playerMaxMP) return;
            isMpMax = false;
            OnMP += MPup;
        } 
        else {  
            OnMP?.Invoke();
        }

        // 스킬 동작
        OnSkill?.Invoke();

        // 사망
        if(PlayerHP <= 0) {
            OnDead?.Invoke();
        }
    }

    void FixedUpdate()
    {
        // 마우스 클릭 이동 
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Move))) {
            OnMove?.Invoke();
        }

        // 해당 위치에 이동을 하면 멈추기 
        if(!isAttack)
            navAgent.isStopped = (navAgent.remainingDistance <= navAgent.stoppingDistance) ? true : false;
        else 
            navAgent.isStopped = true;

        // 멈추면 애니메이션 변경 
        if(navAgent.isStopped == true) {
            animator.SetFloat("Speed", 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public override void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            navAgent.SetDestination(hit.point);
            animator.SetFloat("Speed", 1);
        }
    }

    public override void Dash()
    {
        animator.SetBool("IsDash", true);
        isDash = true;
        moveSpeed *= 1.5f;
        StartCoroutine(ExitDash());
    }

    IEnumerator ExitDash()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsDash", false);
        yield return new WaitForSeconds(1f);
        moveSpeed /= 1.5f;
        yield return new WaitForSeconds(6f);
        isDash = false;
    }

    public override void Attack()
    {
        if (isAttack) return;

        isAttack = true;
        animator.SetBool("IsAttack", true);
        comboCount = 0;
        comdoAttack = StartCoroutine(ComboAttack());
    }

    private void RotateAttack()
    {
        // 마우스 위치를 화면 좌표에서 세계 좌표로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 마우스 위치에서 Raycast 수행
        if (Physics.Raycast(ray, out hit)) {
            Vector3 direction = (hit.point - transform.position).normalized; // 캐릭터 방향 계산
            direction.y = 0; // y축 방향을 0으로 설정하여 수평 회전만 가능

            // 회전
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);
        }
    }

    IEnumerator ComboAttack()
    {
        while(comboCount <= maxComboCount) {
            switch (comboCount) {
                case 1:
                    animator.SetBool("IsCombo1", true);
                    break;
                case 2:
                    animator.SetBool("IsCombo2", true);
                    break;
            }
            // 공격 사이의 딜레이 시간
            yield return new WaitForSeconds(0.5f); 

            float inputTimer = 0f;
            bool inputReceived = false;

            // 1초 안에 입력이 있어야 다음 콤보로 이동
            while(inputTimer < 2f) {
                if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Attack))) {
                    comboCount++;
                    inputReceived = true;
                    break;
                }
                inputTimer += Time.deltaTime;
                yield return null;
            }

            if (!inputReceived) {
                ResetCombo();
                yield break;
            }

            if (comboCount > maxComboCount) {
                yield return new WaitForSeconds(2f);
                ResetCombo();
                yield break;
            }
        }
    }

    void ResetCombo()
    {
        OnMove += Move;
        isAttack = false;
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsCombo1", false);
        animator.SetBool("IsCombo2", false);
        comboCount = 0;
    }

    public override void MPup()
    {
        // 마나 회복
        if (PlayerMP >= playerMaxMP) {
            PlayerMP = playerMaxMP;
            isMpMax = true;
            OnMP -= MPup;
        }
        else {
            PlayerMP += 10.0f * Time.deltaTime;
        }
    }

    public override void Skill()
    {
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_1)) && !isSkill) {
            isSkill = true;
            Debug.Log("Q스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }
        
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_2)) && !isSkill) {
            isSkill = true;
            Debug.Log("W스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }
        
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_3)) && !isSkill) {
            isSkill = true;
            Debug.Log("E스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_4)) && !isSkill) {
            isSkill = true;
            Debug.Log("R스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_5)) && !isSkill) {
            isSkill = true;
            Debug.Log("A스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_6)) && !isSkill) {
            isSkill = true;
            Debug.Log("S스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_7)) && !isSkill) {
            isSkill = true;
            Debug.Log("D스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_8)) && !isSkill) {
            isSkill = true;
            Debug.Log("F스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_9)) && !isSkill) {
            isSkill = true;
            Debug.Log("V스킬 실행");
            // 스킬 애니메이션 끝난 후 isSkill을 false로 변경하는 애니메이션 이벤트 추가 예정 
            isSkill = false;
        }
    }

    public override void Hit()
    {

    }

    public override void Dead()
    {
        // 체력이 0보다 작을 때 사망
        animator.SetTrigger("OnDead");
        OnMove -= Move;
        OnDash -= Dash;
        OnAttack -= Attack;
        OnSkill -= Skill;
        OnHit -= Hit;
        OnDead -= Dead;
    }
}
