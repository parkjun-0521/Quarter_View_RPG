using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;                  // 싱글톤으로 구현 

    public GameObject[] prefabs;                            // 생성가능한 오브젝트 
    private Dictionary<string, List<GameObject>> pools;     // pool 딕셔너리

    void Awake()
    {
        // 싱글톤 초기화 
        if(Instance == null) {
            Instance = this;
        }
        else {
            Destroy(Instance);
            return;
        }

        // 딕셔너리 초기화
        pools = new Dictionary<string, List<GameObject>>();
        foreach(GameObject prefab in prefabs) {
            pools[prefab.name] = new List<GameObject>();
        }
    }

    public GameObject GetObject(string key, Vector3 pos)        // 생성될 오브젝트, 생성 위치 
    {
        // 딕셔너리가 비어있으면 return
        if(!pools.ContainsKey(key)) {
            return null;
        }

        GameObject select = null;

        // 오브젝트 생성 
        foreach(GameObject obj in pools[key]) {
            if(obj != null && !obj.activeSelf) {
                select = obj;
                select.gameObject.SetActive(true);      // 오브젝트 생성
                select.transform.position = pos;        // 오브젝트 생성 위치 
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
            // 키값이 없을 때 return 
            if (prefab == null)
                return null;

            // 해당 오브젝트가 있을 때 오브젝트 생성 
            select = Instantiate(prefab, pos, Quaternion.identity);

            // 생성한 오브젝트는 풀링에 추가 
            pools[key].Add(select);
        }

        return select;
    }
}
