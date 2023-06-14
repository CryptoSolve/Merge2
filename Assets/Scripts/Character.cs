using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [field: SerializeField] public float Speed { get; private set; }
    [SerializeField] public UnityEvent<Vector2> OnMoving;

    void Update()
    {
        transform.position += Time.deltaTime * Speed * Vector3.right;
        OnMoving?.Invoke(transform.position);
    }
}
