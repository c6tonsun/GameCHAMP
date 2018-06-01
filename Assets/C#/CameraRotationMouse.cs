using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationMouse : MonoBehaviour {

    public float maxInputSpeed;
    public float rotationSpeed;
    public float minX, maxX;

    private Vector3 euler;

    private void Update()
    {
        float mouseX = Mathf.Clamp(Input.GetAxis("Mouse X"), -maxInputSpeed, maxInputSpeed);
        float mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -maxInputSpeed, maxInputSpeed);

        transform.Rotate(Vector3.up, mouseX * rotationSpeed);
        transform.Rotate(Vector3.right, -mouseY * rotationSpeed);

        euler = transform.eulerAngles;
        // limit up and down
        if (euler.x > 180)
            euler.x -= 360;
        if (euler.x < minX)
            euler.x = minX;
        if (euler.x > maxX)
            euler.x = maxX;
        // force camera look straight
        euler.z = 0f;
        transform.eulerAngles = euler;
    }
}
