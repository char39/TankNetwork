using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public bool isGameOver = false;
    public Text textConnect;
    public Text textLogMessage;
    private List<string> logMessages = new();



    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        CreateTank();
        PhotonNetwork.IsMessageQueueRunning = true;
        string msg = "\n<color=#64FF00>Log:[" + PhotonNetwork.NickName + "]Connected.</color>";
        AddLogMessage(msg);
        photonView.RPC(nameof(LogMessage), RpcTarget.AllBuffered, msg);
    }

    void Update()
    {
        GetConnectPlayerCount();
        
    }

    private void CreateTank()
    {
        //float pos = Random.Range(-50f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(0f, 5f, 0f), Quaternion.identity);
    }

    [PunRPC]
    private void GetConnectPlayerCount()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        if (currentRoom != null)
            textConnect.text = currentRoom.PlayerCount.ToString() + " / " + currentRoom.MaxPlayers.ToString();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        GetConnectPlayerCount();
        //foreach (string msg in logMessages)
        //    photonView.RPC(nameof(LogMessage), newPlayer, msg);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) => GetConnectPlayerCount();
    public override void OnLeftRoom() => SceneManager.LoadScene("LobbyScene");
    public void OnClickExitRoom()
    {
        string msg = "\n<color=#FF0000>Log:[" + PhotonNetwork.NickName + "]Disconnected.</color>";
        AddLogMessage(msg);
        photonView.RPC(nameof(LogMessage), RpcTarget.AllBuffered, msg);
        PhotonNetwork.LeaveRoom();
    }
    [PunRPC]
    private void LogMessage(string msg)
    {
        textLogMessage.text += msg;
        AddLogMessage(msg);
    }

    private void AddLogMessage(string msg) => logMessages.Add(msg);
}
