using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("ī�޶� ��ġ")]
    [SerializeField]
    private float posX;
    [SerializeField]
    private float posY;
    [SerializeField]
    private float posZ;
    [Header("ī�޶� ȸ�� ����")]
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
        // ��ġ ���� ����
        playerPos = transform.parent.position;
        mainCamera.transform.position = new Vector3 (posX, posY, posZ) + playerPos;
        mainCamera.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }
}
