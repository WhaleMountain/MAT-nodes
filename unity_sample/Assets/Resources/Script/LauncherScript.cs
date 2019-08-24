using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LauncherScript : Photon.PunBehaviour
{
    #region Public変数定義
    //Public変数の定義はココで
    #endregion
    #region Private変数
    //Private変数の定義はココで
    string _gameVersion = "test";   //ゲームのバージョン。仕様が異なるバージョンとなったときはバージョンを変更しないとエラーが発生する。
    #endregion
    #region Public Methods
    //ログインボタンを押したときに実行される
    public void Connect()
    {
        if (!PhotonNetwork.connected)
        {                         //Photonに接続できていなければ
            PhotonNetwork.ConnectUsingSettings(_gameVersion);   //Photonに接続する
            Debug.Log("Photonに接続しました。");
        }
    }
    #endregion
    #region Photonコールバック
    //Auto-JoinLobbyにチェックを入れているとPhotonに接続後OnJoinLobby()が呼ばれる。
    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました。");
        //Randomで部屋を選び、部屋に入る（部屋が無ければOnPhotonRandomJoinFailedが呼ばれる）
        PhotonNetwork.JoinRandomRoom();
    }
    //JoinRandomRoomが失敗したときに呼ばれる
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("ルームの入室に失敗しました。");
        //TestRoomという名前の部屋を作成して、部屋に入る
        PhotonNetwork.CreateRoom("TestRoom");
    }
    //部屋に入った時に呼ばれる
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに入りました。");
        //battleシーンをロード
        PhotonNetwork.LoadLevel("battle");
    }
    #endregion
}