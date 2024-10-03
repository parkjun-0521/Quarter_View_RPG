using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static InputKeyManager;

public class Player : PlayerController
{
    public delegate void PlayerHandle();
    public static event PlayerHandle OnMove, OnDash, OnAttack, OnHit, OnDead;

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
        OnHit += Hit;
        OnDead += Dead;
    }

    private void OnDisable()
    {
        OnMove -= Move;
        OnDash -= Dash;
        OnAttack -= Attack;
        OnHit -= Hit;
        OnDead -= Dead;
    }

    void Start()
    {    
        // ���߿� �����ϰ� �ִ� ����� �ʱⰪ�� �ָ� �� 
        playerMaxHP = 10000;                      
        playerMaxMP = 1000;                      
        playerPower = 100;                   
        playerArmor = 100;      
        
        Debug.Log(playerHP + "  " + playerMP + "  " + playerPower + "  " + playerArmor);
    }

    void Update()
    {
        if (Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Dash)) && !isDash) {
            OnDash?.Invoke();
        }

        // ����
        if(Input.GetKey(inputKeyManager.GetKeyCode(KeyCodeTypes.Attack)) && !isAttack && !animator.GetBool("IsDash")) {
            OnMove -= Move;
            OnAttack?.Invoke();
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

        if(navAgent.isStopped == true) {
            animator.SetFloat("Speed", 0);
        }
    }

    public override void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if(hit.collider.CompareTag("Ground")) {
                navAgent.SetDestination(hit.point);
                animator.SetFloat("Speed", 1);
            }
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

    /* public override void Hit()
     {

     }

     public override void Dead()
     {

     }*/
}
