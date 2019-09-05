
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TankMovement : MonoBehaviour
{

    public float moveSpeed;
    public float turnSpeed;
    private Rigidbody rb;
    private float movementInputValue;
    private float turnInputValue;

    private bool isMyCharacter;
    public float positionSyncInterval = 0.5f;
    private float posSyncTimeElapsed = 0.0f;
    private string syncPosMethodId;

    private bool syncNextFrame;
    private Vector3 nextPos;
    private Quaternion nextRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isMyCharacter = gameObject.name == GameManager.Instance.myPlayerObjectId;
        syncPosMethodId = gameObject.name + " - SyncPosition";
        MATNet.MNManager.Instance.RegisterRpcMethod(syncPosMethodId, this, "SyncPosition");
    }

    void Update()
    {
        if (isMyCharacter)
        {
            Move();
            Turn();
            posSyncTimeElapsed += Time.deltaTime;
            if (posSyncTimeElapsed >= positionSyncInterval)
            {
                Vector3 pos = gameObject.transform.position;
                Quaternion rotation = gameObject.transform.rotation;
                MATNet.MNManager.Instance.SendRPC(syncPosMethodId, pos.x, pos.y, pos.z, rotation.x, rotation.y, rotation.z, rotation.w);
                posSyncTimeElapsed = 0f;
            }
        }
        if (!isMyCharacter && syncNextFrame)
        {
            rb.MovePosition(nextPos);
            rb.MoveRotation(nextRotation);
            syncNextFrame = false;
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width-140, 20, 120, 60), "立ち直り"))
        {
            transform.position = transform.position + new Vector3(0, 2, 0);
            transform.rotation = new Quaternion();
        }
    }

    // 前進・後退のメソッド
    void Move()
    {
        movementInputValue = Input.GetAxis("Vertical");
        Vector3 movement = transform.forward * movementInputValue * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    // 旋回のメソッド
    void Turn()
    {
        turnInputValue = Input.GetAxis("Horizontal");
        float turn = turnInputValue * turnSpeed * Time.deltaTime;
        if (movementInputValue < 0)
        {
            turn = turn * -1;
        }
        Quaternion turnRotation = Quaternion.Euler(0, turn, 0);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    public void SyncPosition(float x, float y, float z, float rx, float ry, float rz, float rw)
    {
        nextPos = new Vector3(x, y, z);
        nextRotation = new Quaternion(rx, ry, rz, rw);
        syncNextFrame = true;
    }
}