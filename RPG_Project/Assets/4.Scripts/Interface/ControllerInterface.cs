using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IController {
    void Move();        // �̵�
    void Attack();      // ����
    void Hit();         // �ǰ�
    void Dead();        // ���
}
