using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [field: SerializeField] public float Speed { get; private set; }

    void Update()
    {
        transform.position += Time.deltaTime * Speed * Vector3.right;
    }
}
