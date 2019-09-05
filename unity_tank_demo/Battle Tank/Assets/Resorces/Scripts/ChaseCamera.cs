using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    public GameObject target;
    private Vector3 offset;

    void Start()
    {

    }

    void Update()
    {
        if (target != null)
        {
            if (offset == null)
            {
                offset = transform.position - target.transform.position;
            }
            transform.position = target.transform.position + offset;
        }
        
    }
}