using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ★追加
using UnityEngine.SceneManagement;

public class TankHealth : MonoBehaviour
{
    public GameObject effectPrefab1;
    public GameObject effectPrefab2;
    public int tankHP;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyShell")
        {
            tankHP -= 1;
            Destroy(other.gameObject);

            if (tankHP > 0)
            {
                GameObject effect1 = Instantiate(effectPrefab1, transform.position, Quaternion.identity);
                Destroy(effect1, 1.0f);
            }
            else
            {
                GameObject effect2 = Instantiate(effectPrefab2, transform.position, Quaternion.identity);
                Destroy(effect2, 1.0f);

                // ★修正
                // Destroy(gameObject); この1行はコメントアウトする（「//」を文頭に付ける）

                // ★追加
                // プレーヤーを破壊せずに画面から見えなくする（ポイント・テクニック）
                // プレーヤーを破壊すると、その時点でメモリー上から消えるので、以降のコードが実行されなくなる。
                this.gameObject.SetActive(false);

                // ★追加
                // 1.5秒後に「GoToGameOver()」メソッドを実行する。
                Invoke("GoToGameOver", 1.5f);
            }
        }
    }

    // ★追加
    void GoToGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}