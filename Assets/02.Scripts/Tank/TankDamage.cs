using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Tank hp가 0 이하일 때 잠시 meshRenderer를 비활성화 하여 5초 후에 다시 활성화

public class TankDamage : MonoBehaviourPunCallbacks
{
    public MeshRenderer[] meshRenderer;
    public GameObject explosionPrefab;
    private int InitialHp = 100;            // 초기 hp
    public int currentHp = 0;               // 현재 hp
    private readonly string playerTag = "Tank";
    public Canvas hudCanvas;
    public Image hpBar;
    public int killCount = 0;
    public Text killText;

    void Start()
    {
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        explosionPrefab = Resources.Load<GameObject>("Explosion");
        currentHp = InitialHp;
        hpBar.color = Color.green;
        killText.text = "Kill : " + killCount.ToString();
    }

    public void OnDamage()
    {
        if (photonView.IsMine)
            photonView.RPC(nameof(OnDamagePun), RpcTarget.All);
    }

    [PunRPC]
    public void OnDamagePun()
    {
        if (currentHp > 0)
        {
            currentHp -= 10;
            HpBarUpdate();
            if (currentHp <= 0)
            {
                //StartCoroutine(ExplosionTank());
                Die(PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    private void HpBarUpdate()          // hp바 ui갱신
    {
        hpBar.fillAmount = (float)currentHp / InitialHp;
        if (hpBar.fillAmount < 0.3f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount < 0.6f)
            hpBar.color = Color.yellow;
        else
            hpBar.color = Color.green;
    }

    private IEnumerator ExplosionTank()
    {
        Object expEff = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(expEff, 2.0f);
        SetTankVisible(false);
        hudCanvas.enabled = false;
        yield return new WaitForSeconds(5.0f);
        currentHp = InitialHp;
        SetTankVisible(true);
        hudCanvas.enabled = true;
        HpBarUpdate();
    }

    private void SetTankVisible(bool isVisible)
    {
        foreach (var mesh in meshRenderer)
            mesh.enabled = isVisible;
    }

    [PunRPC]
    public void OnKilled(int killActorNumber)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        if (killActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            killActorNumber++;
            killText.text = "Kill : " + killCount.ToString();
            Debug.Log("Kill : " + killCount);
        }
    }

    public void Die(int killActorNumber)
    {
        photonView.RPC(nameof(OnKilled), RpcTarget.All, killActorNumber);
    }

}
