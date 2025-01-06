using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransformGizmoFollowRigidBody : MonoBehaviour
{ 
    [SerializeField] public Rigidbody targetRigidbody;

    void Update()
    {
        if (targetRigidbody != null)
        {
            transform.position = targetRigidbody.position;
        }
    }
}
