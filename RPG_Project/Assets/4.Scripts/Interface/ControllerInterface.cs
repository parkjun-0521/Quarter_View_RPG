using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IController {
    void Move();        // 이동
    void Attack();      // 공격
    void Hit();         // 피격
    void Dead();        // 사망
}
