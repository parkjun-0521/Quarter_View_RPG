using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyManager : MonoBehaviour
{
    public enum KeyCodeTypes {
        Move,       // 이동 
        Attack,     // 기본 좌클릭 공격 
        Dash,       // 대시 
        Skill_1,    // 스킬1 ( 기본값 Q )
        Skill_2,    // 스킬2 ( 기본값 W )
        Skill_3,    // 스킬3 ( 기본값 E )
        Skill_4,    // 스킬4 ( 기본값 R )
        Skill_5,    // 스킬5 ( 기본값 A )
        Skill_6,    // 스킬6 ( 기본값 S )
        Skill_7,    // 스킬7 ( 기본값 D )
        Skill_8,    // 스킬8 ( 기본값 F )
        Skill_9,    // 스킬9 ( 기본값 V )
        Item_1,     // 아이템1 ( 기본값 1 )
        Item_2,     // 아이템2 ( 기본값 2 )
        Item_3,     // 아이템3 ( 기본값 3 )
        Item_4      // 아이템4 ( 기본값 4 )
    }

    private Dictionary<KeyCodeTypes, KeyCode> keyMapping;

    void Awake()
    {
        keyMapping = new Dictionary<KeyCodeTypes, KeyCode>();

        keyMapping[KeyCodeTypes.Move] = KeyCode.Mouse1;         // 기본값 마우스 우클릭 
        keyMapping[KeyCodeTypes.Attack] = KeyCode.Mouse0;       // 기본값 마우스 좌클릭 
        keyMapping[KeyCodeTypes.Dash] = KeyCode.Space;          // 기본값 스페이스바 
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

    // 사용 메소드 
    public KeyCode GetKeyCode(KeyCodeTypes value)
    {
        return keyMapping[value];
    }

    // 나중에 만들 키보드 셋팅의 설정 메소드
    public void SetKeyCode(KeyCodeTypes value, KeyCode keyCode)
    {
        keyMapping[value] = keyCode;
    }
}
