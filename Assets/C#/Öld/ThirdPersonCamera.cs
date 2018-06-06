using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;

    [SerializeField]
    private float CameraMoveSpeed = 120.0f;

    [SerializeField]
    private float ClampAngle = 80.0f;

    [SerializeField]
    private float InputSensitivity = 150.0f;

    private float rotY = 0.0f;
    private float rotX = 0.0f;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotY += mouseX * InputSensitivity * Time.deltaTime;
        rotX += mouseY * InputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);

        transform.rotation = localRotation;
    }

    void LateUpdate()
    {
        Transform target = Target.transform;

        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
