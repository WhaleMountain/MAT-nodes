using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public GameObject effectPrefab;

    // 2種類目のエフェクトを入れるための箱
    public GameObject effectPrefab2;
    public int objectHP;
    // このメソッドはぶつかった瞬間に呼び出される
    private void OnTriggerEnter(Collider other)
    {
        // もしもぶつかった相手のTagにShellという名前が書いてあったならば（条件）
        if (other.CompareTag("Shell"))
        {

            // オブジェクトのHPを１ずつ減少させる。
            objectHP -= 1;

            // もしもHPが0よりも大きい場合には（条件）
            if (objectHP > 0)
            {
                // このスクリプトがついているオブジェクトを破壊する（thisは省略が可能）
                Destroy(other.gameObject);

                // ぶつかってきたオブジェクトを破壊する
                Destroy(other.gameObject);

                // エフェクトを実体化（インスタンス化）する。
                GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

                // エフェクトを２秒後に画面から消す
                Destroy(effect, 2.0f);
            }
            else
            { // ★★追加  そうでない場合（HPが0以下になった場合）には（条件）
                Destroy(other.gameObject);

                // もう１種類のエフェクを発生させる。
                GameObject effect2 = Instantiate(effectPrefab2, transform.position, Quaternion.identity);
                Destroy(effect2, 2.0f);

                Destroy(this.gameObject);
            }
        }
    }
}