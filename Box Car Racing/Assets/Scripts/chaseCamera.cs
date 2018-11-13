using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chaseCamera : MonoBehaviour {


    public Transform Car;
    public float distance;
    public float height;
    public float rotationDamping = 3f;
    public float heightDamping = 2f;
    private float desiredAngle = 0;

    private void LateUpdate()
    {
        float currentAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        //determine where we want to be
        
        float desiredHeight = Car.position.y + height;

        //now move towards our goal
        currentAngle = Mathf.LerpAngle(currentAngle, desiredAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, heightDamping * Time.deltaTime);
        Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);

        //set our new position
        Vector3 finalPosition = Car.position - (currentRotation * Vector3.forward * distance);
        finalPosition.y = currentHeight;
        transform.position = finalPosition;
        transform.LookAt(Car);
    }

    private void FixedUpdate()
    {
        desiredAngle = Car.eulerAngles.y;
        //if the car is going backwards add 180 to the wanted location
        Vector3 localVelocity = Car.InverseTransformDirection(Car.GetComponent<Rigidbody>().velocity);

        if (localVelocity.z < -0.5f)
        {
            desiredAngle += 180;
        }
    }
}
