using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MATNet;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    private MATNet.Unity.NetworkManager networkManager;

    public Transform[] playerSpawnPoints;
    public string myPlayerObjectId;

    public GameObject MainCamera;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        networkManager = MATNet.Unity.NetworkManager.Instance;
        networkManager.SendSyncRequest();

        Invoke("PlayerSpawn", 1f);
	}

    void PlayerSpawn()
    {
        myPlayerObjectId = MATNet.MNManager.Instance.player.displayName + "-" + Guid.NewGuid().ToString();
        int number = UnityEngine.Random.Range(0, playerSpawnPoints.Length);
        GameObject obj = networkManager.InstantiateGameObj(myPlayerObjectId, 0, playerSpawnPoints[number].position, playerSpawnPoints[number].rotation);
        networkManager.RegisterObjToBeSync(0, obj);

        //MainCamera.GetComponent<ChaseCamera>().target = obj;
        obj.GetComponent<CameraController>().mainCamera = MainCamera.GetComponent<Camera>();
        obj.GetComponent<CameraController>().Set();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
