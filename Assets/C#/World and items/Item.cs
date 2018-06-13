using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [HideInInspector]
    public Rigidbody rb;
    private MeshRenderer mr;

    private Transform camTransform;

    public Transform leftTrasform;
    public Transform rightTrasform;
    private float defLerpTime;

    public GravityMode currentMode;

    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    public Material defMaterial;
    public Material highMaterial;
    public Material actMaterial;
    public Material errorMaterial;

    public enum GravityMode
    {
        ERROR = 0,
        World = 1,
        Player = 2,
        Self = 3
    }


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        SetGravityMode(GravityMode.World);
        posOffset = transform.position;
        camTransform = FindObjectOfType<Camera>().transform;
    }
	
	// Update is called once per frame
	void Update () {
	
        if(currentMode == GravityMode.World)
        {
            rb.useGravity = true;
        }
        else if(currentMode == GravityMode.Player)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;

            /*
             Vector3 targetDir = camTransform.position - transform.position;
            float step = turnSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x + xAngle, transform.eulerAngles.y + yAngle, transform.eulerAngles.z);
            yAngle = 0;
            xAngle = 0;
             */

            

            if (defLerpTime < 1)
                defLerpTime += Time.deltaTime;

            float rotationInput = Input.GetAxisRaw("Rotate Item");

            if (rotationInput > 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, leftTrasform.rotation, 0.6f * Time.deltaTime);
            if (rotationInput < 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, rightTrasform.rotation, 0.6f * Time.deltaTime);

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.parent.eulerAngles.x, transform.eulerAngles.y, transform.parent.eulerAngles.z), defLerpTime);
        }
        else if(currentMode == GravityMode.Self)
        {
            rb.useGravity = true;
        }

    }

    private void DoFloating()
    {
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

    public void SetGravityMode(GravityMode mode)
    {
        if (currentMode == mode)
            return;

        currentMode = mode;
        
        if(currentMode == GravityMode.ERROR)
        {
            mr.material = errorMaterial;
        }
        else if(currentMode == GravityMode.World)
        {
            mr.material = defMaterial;
        }
        else if(currentMode == GravityMode.Player)
        {
            mr.material = actMaterial;
        }
        else if(currentMode == GravityMode.Self)
        {
            mr.material = highMaterial;
        }
    }

}
