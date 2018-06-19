using UnityEngine;

public class Item : MonoBehaviour {

    [HideInInspector]
    public Rigidbody rb;
    private MeshRenderer mr;
    private Collider[] _colliders;
    //private Transform camTransform;

    public Transform leftTrasform;
    public Transform rightTrasform;
    private float defLerpTime;

    public GravityMode currentMode;

    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    //Vector3 posOffset;
    //Vector3 tempPos;

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
        _colliders = GetComponentsInChildren<Collider>();
        SetGravityMode(GravityMode.World);
        //posOffset = transform.position;
        //camTransform = FindObjectOfType<Camera>().transform;
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
            
            if (defLerpTime < 1)
                defLerpTime += Time.deltaTime;

            float rotationInput = Input.GetAxisRaw("Rotate Item");

            if (rotationInput > 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, leftTrasform.rotation, 0.6f * Time.deltaTime);
            if (rotationInput < 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, rightTrasform.rotation, 0.6f * Time.deltaTime);

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, transform.eulerAngles.y, 0f), defLerpTime);
        }
        else if(currentMode == GravityMode.Self)
        {
            rb.useGravity = true;
        }

    }

    public bool CanMoveCheck(Vector3 movement)
    {
        RaycastHit[] hits;
        Vector3 direction = movement.normalized;
        float maxDistance = movement.magnitude;

        foreach (Collider col in _colliders)
        {
            if (col is BoxCollider)
            {   // box info
                Vector3 center = col.transform.position;
                Vector3 fromCenterToCorner = col.transform.localScale * 0.5f;
                Quaternion orientation = col.transform.rotation;

                hits = Physics.BoxCastAll(center, fromCenterToCorner, direction, orientation, maxDistance);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.isStatic)
                        return false;
                }
            }
            else if (col is SphereCollider)
            {   // sphere info
                Vector3 origin = col.transform.position;
                float radius = Mathf.Max(new float[] { Mathf.Abs(col.transform.localScale.x), Mathf.Abs(col.transform.localScale.y), Mathf.Abs(col.transform.localScale.z) }) * 0.5f;

                hits = Physics.SphereCastAll(origin, radius, direction, maxDistance);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.isStatic)
                        return false;
                }
            }
            else if (col is CapsuleCollider)
            {   // capsule info
                float radius = Mathf.Max(Mathf.Abs(col.transform.localScale.x), Mathf.Abs(col.transform.localScale.z)) * 0.5f;
                Vector3 point1 = col.transform.position - (col.transform.up * col.transform.localScale.y - col.transform.up * radius);
                Vector3 point2 = col.transform.position + (col.transform.up * col.transform.localScale.y - col.transform.up * radius);

                hits = Physics.CapsuleCastAll(point1, point2, radius, direction, maxDistance);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.isStatic)
                        return false;
                }
            }
        }

        return true;
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
            defLerpTime = 0f;
            mr.material = actMaterial;
        }
        else if(currentMode == GravityMode.Self)
        {
            mr.material = highMaterial;
        }
    }

}
