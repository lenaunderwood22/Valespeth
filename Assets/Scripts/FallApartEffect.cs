using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallApartEffect : MonoBehaviour
{
    float sphereSize = 0.05f;
    int spheresInRow = 5;

    public float radius;

    public float up;

    public float force;

    public Material newMat;

    Vector3 cubePivot;

    Vector3 explosionPos;
    

    

    // Start is called before the first frame update
    void Start()
    {
        cubePivot = new Vector3(0.5f, 0.5f, 0.5f);
        explosionPos = new Vector3(0.0f, 0.0f, 0.0f);

    }
    // private void OnCollisionEnter(Collision other) {
    //     if(GetComponent<MeshRenderer>().material.color == 
    //     other.gameObject.GetComponent<MeshRenderer>().material.color){
    //         explode();
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
       Collider[] cls = Physics.OverlapSphere(transform.position, 1f);
       if(cls.Length != 0){
            if(GetComponent<MeshRenderer>().material.color == 
            cls[0].gameObject.GetComponentInChildren<MeshRenderer>().material.color){
                explode();
            }
       }
    }

    private void explode(){

        // gameObject.SetActive(false);

        for(int x = 0; x < spheresInRow; x++){
            for(int y = 0; y < spheresInRow; y++){
                for(int z = 0; z < spheresInRow; z++){
                    creatPieces(x,y,z);
                }
            }
        }

        explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach(Collider nearby in colliders){
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            GameObject gmo = nearby.gameObject;
            gmo.SetActive(true);
            if(rb != null){
                rb.AddExplosionForce(force, transform.position, radius, up, ForceMode.Impulse);

            }
        }
    }


     private void creatPieces(int x, int y, int z){
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        piece.transform.position = transform.position + new Vector3(x*sphereSize, y*sphereSize, z*sphereSize) - cubePivot;
        piece.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);

        
        piece.GetComponent<MeshRenderer>().material = newMat;
        piece.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", GetComponent<MeshRenderer>().material.color);
    
        piece.GetComponent<SphereCollider>().isTrigger = true;

        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = sphereSize;
        
        
    }

    
}
 