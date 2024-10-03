using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyManager : MonoBehaviour
{
    public enum KeyCodeTypes {
        Move,       // �̵� 
        Attack,     // �⺻ ��Ŭ�� ���� 
        Dash,       // ��� 
        Skill_1,    // ��ų1 ( �⺻�� Q )
        Skill_2,    // ��ų2 ( �⺻�� W )
        Skill_3,    // ��ų3 ( �⺻�� E )
        Skill_4,    // ��ų4 ( �⺻�� R )
        Skill_5,    // ��ų5 ( �⺻�� A )
        Skill_6,    // ��ų6 ( �⺻�� S )
        Skill_7,    // ��ų7 ( �⺻�� D )
        Skill_8,    // ��ų8 ( �⺻�� F )
        Skill_9,    // ��ų9 ( �⺻�� V )
        Item_1,     // ������1 ( �⺻�� 1 )
        Item_2,     // ������2 ( �⺻�� 2 )
        Item_3,     // ������3 ( �⺻�� 3 )
        Item_4      // ������4 ( �⺻�� 4 )
    }

    private Dictionary<KeyCodeTypes, KeyCode> keyMapping;

    void Awake()
    {
        keyMapping = new Dictionary<KeyCodeTypes, KeyCode>();

        keyMapping[KeyCodeTypes.Move] = KeyCode.Mouse1;         // �⺻�� ���콺 ��Ŭ�� 
        keyMapping[KeyCodeTypes.Attack] = KeyCode.Mouse0;       // �⺻�� ���콺 ��Ŭ�� 
        keyMapping[KeyCodeTypes.Dash] = KeyCode.Space;          // �⺻�� �����̽��� 
        keyMapping[KeyCodeTypes.Skill_1] = KeyCode.Q;
        keyMapping[KeyCodeTypes.Skill_2] = KeyCode.W;
        keyMapping[KeyCodeTypes.Skill_3] = KeyCode.E;
        keyMapping[KeyCodeTypes.Skill_4] = KeyCode.R;
        keyMapping[KeyCodeTypes.Skill_5] = KeyCode.A;
        keyMapping[KeyCodeTypes.Skill_6] = KeyCode.S;
        keyMapping[KeyCodeTypes.Skill_7] = KeyCode.D;
        keyMapping[KeyCodeTypes.Skill_8] = KeyCode.F;
        keyMapping[KeyCodeTypes.Skill_9] = KeyCode.V;
        keyMapping[KeyCodeTypes.Item_1] = KeyCode.Alpha1;
        keyMapping[KeyCodeTypes.Item_2] = KeyCode.Alpha2;
        keyMapping[KeyCodeTypes.Item_3] = KeyCode.Alpha3;
        keyMapping[KeyCodeTypes.Item_4] = KeyCode.Alpha4;
    }

    // ��� �޼ҵ� 
    public KeyCode GetKeyCode(KeyCodeTypes value)
    {
        return keyMapping[value];
    }

    // ���߿� ���� Ű���� ������ ���� �޼ҵ�
    public void SetKeyCode(KeyCodeTypes value, KeyCode keyCode)
    {
        keyMapping[value] = keyCode;
    }
}
