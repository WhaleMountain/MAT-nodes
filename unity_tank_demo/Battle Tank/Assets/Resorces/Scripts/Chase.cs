using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ★追加する（ポイント）
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public GameObject target;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // ターゲットの位置を目的地に設定する。
        agent.destination = target.transform.position;
    }
}