using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IController {
    void Move();                // �̵�
    void Attack();              // ����
    void Hit(Collider other);   // �ǰ�
    void Dead();                // ���
}
