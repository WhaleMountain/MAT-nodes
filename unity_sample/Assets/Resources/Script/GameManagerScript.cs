using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManagerScript : Photon.PunBehaviour
{
    //誰かがログインする度に生成するプレイヤーPrefab
    public GameObject playerPrefab;
    void Start()
    {
        if (!PhotonNetwork.connected)   //Phootnに接続されていなければ
        {
            SceneManager.LoadScene("Launcher"); //ログイン画面に戻る
            return;
        }
        //Photonに接続していれば自プレイヤーを生成
        GameObject Player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
    }
    // Update is called once per frame
    void Update()
    {
    }
}