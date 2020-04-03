using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CollisionExample : MonoBehaviour {

    public float Speed = 1f;

    public float Radius = 0.5f;

    void Update () {
        transform.position += transform.forward * Time.deltaTime * Speed;

        Collider[] cols = Physics.OverlapSphere(transform.position, Radius);

        if (cols.Length != 0) {
            Debug.Log(cols[0].gameObject.name);
        }
    }

    void OnDrawGizmos  () {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

}