using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static InputKeyManager;

public class Player : PlayerController
{
    public delegate void PlayerHandle();
    public static event PlayerHandle OnMove, OnDash, OnAttack, OnMP, OnSkill, OnDead;

    public delegate void PlayerHitHandle(Collider other);
    public static event PlayerHitHandle OnHit;

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

        // �⺻ ��ų �Ŀ�
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
        // ���߿� �����ϰ� �ִ� ����� �ʱⰪ�� �ָ� �� 
        playerMaxHP = 10000;                      
        playerMaxMP = 5000;                      
        playerPower = 500;                   
        playerArmor = 100;      
        
        Debug.Log(PlayerHP + "  " + playerMP + "  " + playerPower + "  " + playerArmor);
    }

    void Update()
    {
        // ��� ( ���ϱ� )
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Dash)) && !isDash) {
            OnDash?.Invoke();
        }

        // ����
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Attack)) && !isAttack && !animator.GetBool("IsDash")) {
            OnMove -= Move;
            RotateAttack();
            OnAttack?.Invoke();
        }

        // ���� ȸ��
        if (isMpMax) {
            if (PlayerMP >= playerMaxMP) return;
            isMpMax = false;
            OnMP += MPup;
        } 
        else {  
            OnMP?.Invoke();
        }

        // ��ų ����
        OnSkill?.Invoke();

        // ���
        if(PlayerHP <= 0) {
            OnDead?.Invoke();
        }
    }

    void FixedUpdate()
    {
        // ���콺 Ŭ�� �̵� 
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Move))) {
            OnMove?.Invoke();
        }

        // �ش� ��ġ�� �̵��� �ϸ� ���߱� 
        if(!isAttack)
            navAgent.isStopped = (navAgent.remainingDistance <= navAgent.stoppingDistance) ? true : false;
        else 
            navAgent.isStopped = true;

        // ���߸� �ִϸ��̼� ���� 
        if(navAgent.isStopped == true) {
            animator.SetFloat("Speed", 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other != null) {
            if (other.gameObject.CompareTag("EnemyAttackRange")) {
                OnHit?.Invoke(other);
            }
        }
    }

    public override void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            navAgent.SetDestination(hit.point);
            navAgent.updateRotation = true;
        }

        if (navAgent.remainingDistance > navAgent.stoppingDistance) {
            animator.SetFloat("Speed", 1); // �̵� ���� �� �ִϸ��̼� �ӵ� ����
        }
        else {
            animator.SetFloat("Speed", 0); // �������� �����ϸ� �ִϸ��̼� ����
            navAgent.updateRotation = true;
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
        // ���콺 ��ġ�� ȭ�� ��ǥ���� ���� ��ǥ�� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ���콺 ��ġ���� Raycast ����
        if (Physics.Raycast(ray, out hit)) {
            Vector3 direction = (hit.point - transform.position).normalized; // ĳ���� ���� ���
            direction.y = 0; // y�� ������ 0���� �����Ͽ� ���� ȸ���� ����

            // ȸ��
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
            // ���� ������ ������ �ð�
            yield return new WaitForSeconds(0.5f); 

            float inputTimer = 0f;
            bool inputReceived = false;

            // 1�� �ȿ� �Է��� �־�� ���� �޺��� �̵�
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
        // ���� ȸ��
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
            Debug.Log("Q��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }
        
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_2)) && !isSkill) {
            isSkill = true;
            Debug.Log("W��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }
        
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_3)) && !isSkill) {
            isSkill = true;
            Debug.Log("E��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_4)) && !isSkill) {
            isSkill = true;
            Debug.Log("R��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_5)) && !isSkill) {
            isSkill = true;
            Debug.Log("A��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_6)) && !isSkill) {
            isSkill = true;
            Debug.Log("S��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_7)) && !isSkill) {
            isSkill = true;
            Debug.Log("D��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_8)) && !isSkill) {
            isSkill = true;
            Debug.Log("F��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }

        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Skill_9)) && !isSkill) {
            isSkill = true;
            Debug.Log("V��ų ����");
            // ��ų �ִϸ��̼� ���� �� isSkill�� false�� �����ϴ� �ִϸ��̼� �̺�Ʈ �߰� ���� 
            isSkill = false;
        }
    }

    public override void Hit(Collider other)
    {
        // ���� ���� ���� �� 
        Debug.Log("�¾ҽ��ϴ�. ü�� ����");
        PlayerHP -= other.gameObject.transform.parent.GetComponentInParent<EnemyController>().enemyPower;
        StartCoroutine(HitEffect());
    }

    IEnumerator HitEffect()
    {
        hitImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        hitImage.gameObject.SetActive(false);
    }

    public override void Dead()
    {
        // ü���� 0���� ���� �� ���
        animator.SetTrigger("OnDead");
        gameObject.tag = "DeadPlayer";
        OnMove -= Move;
        OnDash -= Dash;
        OnAttack -= Attack;
        OnSkill -= Skill;
        OnHit -= Hit;
        OnDead -= Dead;
    }
}
