using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooterMovement : MonoBehaviour
{
    public Transform shooterTransform;
    public float speed = 2f;
    public float maxDistance = 10f;
    private Vector3 shooterPosition;
    private Vector3 startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = shooterTransform.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float axisVal = Input.GetAxisRaw("Horizontal");
        shooterPosition.x += axisVal * speed * Time.deltaTime;
        shooterPosition.x = Mathf.Clamp(shooterPosition.x, -maxDistance, maxDistance);
        shooterTransform.transform.position = startingPosition + shooterPosition;
    }
}
