using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ApacheAISpawner : MonoBehaviourPunCallbacks
{
    public static ApacheAISpawner instance;

    public Transform[] Points;
    public List<Transform> PointList;
    public int nextPoint;

    public GameObject ApacheAI;
    public List<GameObject> ApacheAIList;
    private int maxApacheAI = 0;



    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        ApacheAI = Resources.Load<GameObject>("ApacheAI");
        ApacheAIList = new List<GameObject>();

        FindPoint();
        InvokeRepeating(nameof(CreateApacheAI), 0f, 3f);
    }

    void FindPoint()
    {
        Points = GameObject.FindWithTag("Point").GetComponentsInChildren<Transform>();
        for (int i = 1; i < Points.Length; i++)
            PointList.Add(Points[i].transform);
    }

    [PunRPC]
    void CreateApacheAI()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        int count = GameObject.FindGameObjectsWithTag("Apache").Length;
        if (count >= maxApacheAI)
            return;
        nextPoint = Random.Range(0, PointList.Count);
        float randomXOffset = Random.Range(-70f, 70f); // x축 랜덤 오프셋
        float randomZOffset = Random.Range(-70f, 70f); // z축 랜덤 오프셋
        float randomYOffset = Random.Range(-10f, 10f);    // y축 랜덤 오프셋
        Vector3 randomPosition = PointList[nextPoint].position + new Vector3(randomXOffset, randomYOffset, randomZOffset);

        GameObject apacheAI = PhotonNetwork.InstantiateRoomObject(ApacheAI.name, randomPosition, PointList[nextPoint].rotation, 0, null);
        apacheAI.transform.SetParent(transform);
        ApacheAIList.Add(apacheAI);

    }
}