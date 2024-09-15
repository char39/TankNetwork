using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;

public class DisplayUserID : MonoBehaviourPun
{
    public Text userID;

    void Start()
    {
        userID.text = photonView.Owner.NickName;
    }

    void Update()
    {
        
    }
}
