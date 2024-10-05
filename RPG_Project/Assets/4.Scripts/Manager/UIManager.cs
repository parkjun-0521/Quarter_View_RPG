using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // ½Ì±ÛÅæ ±¸Çö
    public static UIManager Instance;

    [Header("Ã¼·Â")]
    public Text maxHP;
    public Text curHP;
    public Slider HP;
    [Header("¸¶³ª")]
    public Text maxMP;
    public Text curMP;
    public Slider MP;

    Player playerComponent;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerComponent = player.GetComponent<Player>();
    }

    void Update()
    {

        maxHP.text = "/ " + playerComponent.playerMaxHP.ToString("N0");
        maxMP.text = "/ " + playerComponent.playerMaxMP.ToString("N0");
        curHP.text = playerComponent.PlayerHP.ToString("N0");
        curMP.text = playerComponent.playerMP.ToString("N0");

        HP.value = playerComponent.PlayerHP / playerComponent.playerMaxHP;
        MP.value = playerComponent.playerMP / playerComponent.playerMaxMP;
    }
}
