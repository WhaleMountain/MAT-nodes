using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class NameInputFieldScript : MonoBehaviour
{
    #region Private変数定義
    static string playerNamePrefKey = "PlayerName";
    #endregion
    #region MonoBehaviourコールバック
    void Start()
    {
        string defaultName = "";
        InputField _inputField = this.GetComponent<InputField>();
        //前回プレイ開始時に入力した名前をロードして表示
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }
    }
    #endregion
    #region Public Method
    public void SetPlayerName(string value)
    {
        PhotonNetwork.playerName = value + " ";     //今回ゲームで利用するプレイヤーの名前を設定
        PlayerPrefs.SetString(playerNamePrefKey, value);    //今回の名前をセーブ
        Debug.Log(PhotonNetwork.player.NickName);   //playerの名前の確認。（動作が確認できればこの行は消してもいい）
    }
    #endregion
}