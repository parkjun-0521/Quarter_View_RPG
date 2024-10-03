using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("카메라 위치")]
    [SerializeField]
    private float posX;
    [SerializeField]
    private float posY;
    [SerializeField]
    private float posZ;
    [Header("카메라 회전 각도")]
    [SerializeField]
    private float rotX;
    [SerializeField]
    private float rotY;
    [SerializeField]
    private float rotZ;

    Camera mainCamera;
    Vector3 playerPos;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        // 위치 각도 설정
        playerPos = transform.parent.position;
        mainCamera.transform.position = new Vector3 (posX, posY, posZ) + playerPos;
        mainCamera.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }
}
