using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraScript : MonoBehaviour
{
    private const float YAngle_MIN = -89.0f;   //カメラのY方向の最小角度
    private const float YAngle_MAX = 89.0f;     //カメラのY方向の最大角度
    public Transform target;    //追跡するオブジェクトのtransform
    public Vector3 offset;      //追跡対象の中心位置調整用オフセット
    private Vector3 lookAt;     //targetとoffsetによる注視する座標
    private float distance = 10.0f;    //キャラクターとカメラ間の角度
    private float distance_min = 1.0f;  //キャラクターとの最小距離
    private float distance_max = 20.0f; //キャラクターとの最大距離
    private float currentX = 0.0f;  //カメラをX方向に回転させる角度
    private float currentY = 0.0f;  //カメラをY方向に回転させる角度
                                    //カメラ回転用係数(値が大きいほど回転速度が上がる)
    private float moveX = 4.0f;     //マウスドラッグによるカメラX方向回転係数
    private float moveY = 2.0f;     //マウスドラッグによるカメラY方向回転係数
    private float moveX_QE = 2.0f;  //QEキーによるカメラX方向回転係数
    void Start()
    {
    }
    void Update()
    {
        //QとEキーでカメラ回転
        if (Input.GetKey(KeyCode.Q))
        {
            currentX += -moveX_QE;
        }
        if (Input.GetKey(KeyCode.E))
        {
            currentX += moveX_QE;
        }
        //マウス右クリックを押しているときだけマウスの移動量に応じてカメラが回転
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * moveX;
            currentY += Input.GetAxis("Mouse Y") * moveY;
            currentY = Mathf.Clamp(currentY, YAngle_MIN, YAngle_MAX);
        }
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel"), distance_min, distance_max);
    }
    void LateUpdate()
    {
        if (target != null)  //targetが指定されるまでのエラー回避
        {
            lookAt = target.position + offset;  //注視座標はtarget位置+offsetの座標
                                                //カメラ旋回処理
            Vector3 dir = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);
            transform.position = lookAt + rotation * dir;   //カメラの位置を変更
            transform.LookAt(lookAt);   //カメラをLookAtの方向に向けさせる
        }
    }
}