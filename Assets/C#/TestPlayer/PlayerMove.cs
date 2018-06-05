using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    public Vector3 ownGravity;
    public bool isOwnGravity;

    [Space(10)]
    public float movementSpeed = 5;

    private Vector3 movement;
    private Rigidbody rb;
    //public int ignoreFrames;
    //private int ignoreAngles;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        movement += transform.right * Input.GetAxisRaw("Horizontal") * movementSpeed * Time.deltaTime;
        movement += transform.forward * Input.GetAxisRaw("Vertical") * movementSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            isOwnGravity = !isOwnGravity;

        if (isOwnGravity && rb.velocity.sqrMagnitude > ownGravity.sqrMagnitude)
            isOwnGravity = false;
    }

    private void FixedUpdate()
    {
        if (PreCheckCollision())
        {
            rb.MovePosition(transform.position + movement * Time.fixedDeltaTime);
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        movement = Vector3.zero;

        if (isOwnGravity)
            rb.AddForce(ownGravity, ForceMode.Acceleration);
        else
            rb.AddForce(GameManager.worldGravity, ForceMode.Acceleration);
    }

    private bool PreCheckCollision()
    {
        Vector3 point1 = transform.position - transform.up * 0.5f;
        Vector3 point2 = transform.position + transform.up * 0.5f;

        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, 0.5f, movement.normalized, movement.magnitude * Time.fixedDeltaTime);

        Slope slope;
        foreach (RaycastHit hit in hits)
        {
            slope = hit.collider.GetComponent<Slope>();

            if (slope && !slope.walkable)
                return false;
        }

        return true;
    }

    /* legacy but keepable
    private bool CanWalkSlope(RaycastHit hit)
    {
        float angle = Vector3.Angle(hit.normal, Vector3.up);
        
        if (Vector3.Angle(hit.normal, Vector3.up) > 40f)
        {
            ignoreAngles--;
            if (ignoreAngles < 0)
            {
                ignoreAngles = ignoreFrames;
                return false;
            }
        }

        return true;
    }
    */
}
