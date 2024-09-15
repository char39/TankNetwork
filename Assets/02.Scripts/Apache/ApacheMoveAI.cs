using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ApacheMoveAI : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private Transform tr;
    private float moveSpeed = 0;
    private const float ApplyMoveSpeed = 0.3f;
    private float rotSpeed = 0;
    private const float ApplyRotSpeed = 0.9f;
    private float verticalSpeed = 0;
    private const float ApplyVerticalSpeed = 0.1f;

    private float moveSpeedClamp = 15.0f;
    private float rotSpeedClamp = 45.0f;

    public Transform[] Points;
    public List<Transform> PointList;
    public int nextPoint = 0;

    public Vector3 applyMove;
    public int rotApply = 0;        // 1: right, -1: left, 0: none
    public bool moveApply = true;   // true: move, false: stop
    public int verticalApply = 0;   // 1: up, -1: down, 0: none

    public bool isSearch = false;
    public float TankFindDist;
    public Vector3 targetDist;
    public Quaternion targetRotation;

    private GameObject plasmaEffect;
    private GameObject bullet;

    public Transform[] firePos = new Transform[2];

    private Vector3 curPos = Vector3.zero;              // 동기화된 위치값
    private Quaternion curRot = Quaternion.identity;    // 동기화된 회전값

    private GameObject[] playerTanks = null;
    private const string TankTag = "Tank";
    private Transform target = null;

    void Awake()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable;
        photonView.ObservedComponents[0] = this;
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        moveSpeedClamp = Random.Range(15.0f, 30.0f);
        rotSpeedClamp = Random.Range(50.0f, 100.0f);
        curPos = tr.position;
        curRot = tr.rotation;
    }

    void Start()
    {
        Points = GameObject.FindWithTag("Point").GetComponentsInChildren<Transform>();
        PointList = new List<Transform>();
        for (int i = 1; i < Points.Length; i++)
        {
            PointList.Add(Points[i].transform);
        }
        plasmaEffect = Resources.Load<GameObject>("PlasmaExplosionEffect");
        bullet = Resources.Load<GameObject>("Bullet_ApacheAI");
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            ApacheMoveHorizontal();
            if (!isSearch)
            {
                ApacheRotate();
                ApacheMoveVertical();
            }
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            ApplyMoveCheckPoint();
            CheckPoint();
            Search();
            Attack();
            if (isFire && !onFire)
                StartCoroutine(Fire());

            if (!photonView.IsMine)
            {
                tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 30.0f);
                tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 30.0f);
            }
        }
    }



    private void CheckPoint()
    {
        if (Vector3.Distance(tr.position, PointList[nextPoint].position) < 50f)
        {
            nextPoint++;
            if (nextPoint >= PointList.Count)
                nextPoint = 0;
        }
    }

    private void ApplyMoveCheckPoint()
    {
        applyMove = transform.InverseTransformPoint(PointList[nextPoint].position).normalized;

        if (applyMove.x >= -1f && applyMove.x <= 0f)                       // right
        {
            rotApply = -1;
            moveApply = true;
        }
        else if (applyMove.x < 1f && applyMove.x > 0f)                     // left
        {
            rotApply = 1;
            moveApply = true;
        }

        if (isSearch) moveApply = false;
        else moveApply = true;

        if (applyMove.y >= -1f && applyMove.y <= 0f)                       // down
        {
            verticalApply = -1;
        }
        else if (applyMove.y < 1f && applyMove.y > 0f)                     // up
        {
            verticalApply = 1;
        }
    }
    private void ApacheRotate()
    {
        if (rotApply == -1)
        {
            if (rotSpeed > 0f) rotSpeed += -2.5f * ApplyRotSpeed;
            else rotSpeed += -1 * ApplyRotSpeed;
        }
        else if (rotApply == 1)
        {
            if (rotSpeed < 0f) rotSpeed += 2.5f * ApplyRotSpeed;
            else rotSpeed += 1 * ApplyRotSpeed;
        }
        else
        {
            if (rotSpeed > 2f) rotSpeed += -2 * ApplyRotSpeed;
            else if (rotSpeed > 0f) rotSpeed += -1 * ApplyRotSpeed;
            else if (rotSpeed < -2f) rotSpeed += 2 * ApplyRotSpeed;
            else if (rotSpeed < 0f) rotSpeed += 1 * ApplyRotSpeed;
        }
        rotSpeed = Mathf.Clamp(rotSpeed, rotSpeedClamp * -1, rotSpeedClamp);    
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, tr.rotation.eulerAngles.y, 0);      // x, z 축을 0으로 설정하여 부드럽게 회전하기 위함
        tr.rotation = Quaternion.Slerp(tr.rotation, targetRotation, Time.deltaTime * 2.0f);

        Quaternion smoothRotation = Quaternion.Euler(Mathf.LerpAngle(tr.rotation.eulerAngles.x, 0, Time.deltaTime * 2.0f), tr.rotation.eulerAngles.y, Mathf.LerpAngle(tr.rotation.eulerAngles.z, 0, Time.deltaTime * 2.0f));
        tr.rotation = Quaternion.Slerp(tr.rotation, smoothRotation, Time.deltaTime * 2.0f);

    }
    private void ApacheMoveHorizontal()
    {
        if (moveApply)
        {
            if (moveSpeed < 0f) moveSpeed += 2 * ApplyMoveSpeed;
            else moveSpeed += 1 * ApplyMoveSpeed;
        }
        else if (moveApply)
        {
            if (moveSpeed > 0f) moveSpeed += -2 * ApplyMoveSpeed;
            else moveSpeed += -1 * ApplyMoveSpeed;
        }
        else
        {
            if (moveSpeed > 2f) moveSpeed += -2 * ApplyMoveSpeed;
            else if (moveSpeed > 0f) moveSpeed += -1 * ApplyMoveSpeed;
            else if (moveSpeed < -2f) moveSpeed += 2 * ApplyMoveSpeed;
            else if (moveSpeed < 0f) moveSpeed += 1 * ApplyMoveSpeed;
        }
        moveSpeed = Mathf.Clamp(moveSpeed, moveSpeedClamp * -1, moveSpeedClamp);
        tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
    }
    private void ApacheMoveVertical()
    {
        if (verticalApply == 1 && !isSearch)
        {
            if (verticalSpeed < 0f) verticalSpeed += 2 * ApplyVerticalSpeed;
            else verticalSpeed += ApplyVerticalSpeed;
        }
        else if (verticalApply == -1 && !isSearch)
        {
            if (verticalSpeed > 0f) verticalSpeed += -2 * ApplyVerticalSpeed;
            else verticalSpeed += -ApplyVerticalSpeed;
        }
        else
        {
            if (verticalSpeed > 0f) verticalSpeed += -ApplyVerticalSpeed;
            else if (verticalSpeed < 0f) verticalSpeed += ApplyVerticalSpeed;
        }
        tr.Translate(Vector3.up * verticalSpeed * Time.deltaTime, Space.Self);
    }

    [PunRPC]
    private void Search()
    {
        // if (GameObject.FindWithTag("Tank") == null) return;
        // TankFindDist = (GameObject.FindWithTag("Tank").transform.position - tr.position).magnitude;
        // if (TankFindDist <= 75f)
        //     isSearch = true;
        // else
        //     isSearch = false;
        if (GameObject.FindWithTag(TankTag) == null) return;
        playerTanks = GameObject.FindGameObjectsWithTag(TankTag);     // 탱크 태그를 가진 모든 게임오브젝트를 찾음
        target = playerTanks[0].transform;                    // 가장 가까운 탱크를 찾기 위함
        float distance = Vector3.Distance(target.position, tr.position);    // 탱크와의 거리를 계산
        float distanceAll = 0f;                                         // 모든 탱크와의 거리를 계산
        foreach (var tank in playerTanks)
        {
            distanceAll = (tank.transform.position - tr.position).sqrMagnitude;     // 제곱근을 사용하여 계산
            if (distanceAll < distance * distance)
            {
                target = tank.transform;
                distance = Mathf.Sqrt(distanceAll);
            }
        }
        TankFindDist = distance;
        if (distance > 100f)
            isSearch = false;
        else
            isSearch = true;
    }

    [PunRPC]
    private void Attack()
    {
        if (GameObject.FindWithTag(TankTag) == null) return;
        if (isSearch)
        {
            targetDist = target.position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(targetDist.normalized), Time.deltaTime * 2.0f);
            isFire = true;
        }
        else
            isFire = false;
    }
    private bool isFire = false;
    private bool onFire = false;

    [PunRPC]
    IEnumerator Fire()
    {
        onFire = true;
        Ray ray = new Ray(tr.position, tr.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Tank"))
            {
                Instantiate(plasmaEffect, hit.point, Quaternion.identity);
                hit.collider.transform.parent.parent.SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);
            }
        }
        // GameObject b_1 = Instantiate(bullet, firePos[0].position, firePos[0].rotation);
        // GameObject b_2 = Instantiate(bullet, firePos[1].position, firePos[1].rotation);
        // b_1.transform.SetParent(transform);
        // b_2.transform.SetParent(transform);

        yield return new WaitForSeconds(1f);
        onFire = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if (stream.IsReading)
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

