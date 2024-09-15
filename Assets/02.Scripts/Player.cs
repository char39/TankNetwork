using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public bool playerInside = false;
    public bool isInside = false;
    private GameObject player;
    private Rigidbody rb;
    private Transform tr;

    void Start()
    {
        instance = this;
        OnOff();
    }

    void OnOff()
    {
        player = transform.GetChild(0).transform.gameObject;
        rb = player.GetComponent<Rigidbody>();
        tr = player.GetComponent<Transform>();
    }

    void Update()
    {
        if (playerInside && !isInside)
        {
            isInside = true;
            OnOff();
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (!playerInside && isInside)
        {
            isInside = false;
            OnOff();
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }

    }
}
