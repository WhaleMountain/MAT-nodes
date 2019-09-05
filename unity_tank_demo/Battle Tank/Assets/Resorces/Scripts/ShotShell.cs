using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MATNet;

public class ShotShell : MonoBehaviour
{
    public GameObject shellPrefab;
    public float shotSpeed;
    public AudioClip shotSound;

    public bool isMyCharacter;

    private string shotMethodId;
    private bool shotNextFrame;

    void Start()
    {
        shotMethodId = gameObject.transform.parent.gameObject.name + " - Shot";
        MNManager.Instance.RegisterRpcMethod(shotMethodId, this, "Shot");
        isMyCharacter = gameObject.transform.parent.gameObject.name == GameManager.Instance.myPlayerObjectId;
    }

    void Update()
    {
        if (isMyCharacter && Input.GetKeyDown(KeyCode.Space))
        {
            MNManager.Instance.SendRPC(shotMethodId);
        }
        if (shotNextFrame)
        {
            shotNextFrame = false;
            GameObject shell = Instantiate(shellPrefab, transform.position, Quaternion.identity);
            Rigidbody shellRb = shell.GetComponent<Rigidbody>();
            shellRb.AddForce(transform.forward * shotSpeed);
            Destroy(shell, 3.0f);
            AudioSource.PlayClipAtPoint(shotSound, transform.position);
        }
    }

    public void Shot()
    {
        shotNextFrame = true;
    }
}