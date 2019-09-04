using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ★この１行を忘れないこと。
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    // 先頭に「public」が付いていることを確認する（ポイント）
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Battle Tank");
    }
}