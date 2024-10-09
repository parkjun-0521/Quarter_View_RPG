using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;                  // �̱������� ���� 

    public GameObject[] prefabs;                            // ���������� ������Ʈ 
    private Dictionary<string, List<GameObject>> pools;     // pool ��ųʸ�

    void Awake()
    {
        // �̱��� �ʱ�ȭ 
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(Instance);
            return;
        }

        // ��ųʸ� �ʱ�ȭ
        pools = new Dictionary<string, List<GameObject>>();
        foreach(GameObject prefab in prefabs) {
            pools[prefab.name] = new List<GameObject>();
        }
    }

    public GameObject GetObject(string key, Vector3 pos)        // ������ ������Ʈ, ���� ��ġ 
    {
        // ��ųʸ��� ��������� return
        if(!pools.ContainsKey(key)) {
            return null;
        }

        GameObject select = null;

        // ������Ʈ ���� 
        foreach(GameObject obj in pools[key]) {
            if(obj != null && !obj.activeSelf) {
                select = obj;
                select.gameObject.SetActive(true);      // ������Ʈ ����
                select.transform.position = pos;        // ������Ʈ ���� ��ġ 
                break;
            }
        }

        if(select == null) {
            GameObject prefab = null;
            foreach(GameObject obj in prefabs) {
                if(obj.name == key) {
                    prefab = obj;
                    break;
                }
            }
            // Ű���� ���� �� return 
            if (prefab == null)
                return null;

            // �ش� ������Ʈ�� ���� �� ������Ʈ ���� 
            select = Instantiate(prefab, pos, Quaternion.identity);

            // ������ ������Ʈ�� Ǯ���� �߰� 
            pools[key].Add(select);
        }

        return select;
    }
}
