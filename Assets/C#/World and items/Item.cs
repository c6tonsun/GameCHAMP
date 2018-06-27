using UnityEngine;

public class Item : MonoBehaviour {

    private InputHandler _inputHandler;

    [HideInInspector]
    public Rigidbody rb;
    private MeshRenderer mr;
    private Collider[] _colliders;

    private float _freezeTime;

    public Vector3 offset;

    public Transform leftTrasform;
    public Transform rightTrasform;
    private float defLerpTime;

    private Quaternion _startRot;
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    public Material defMaterial;
    public Material highMaterial;
    public Material actMaterial;
    public Material freezeMaterial;

    public enum GravityMode
    {
        ERROR = 0,
        World = 1,
        Player = 2,
        Self = 3,
        Freeze = 4
    }
    public GravityMode currentMode;

    // Casting and drawing
    private BoxCollider _boxCol;
    private SphereCollider _sphereCol;
    private CapsuleCollider _capsuleCol;

    private void Start ()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        _colliders = GetComponentsInChildren<Collider>();
        _inputHandler = FindObjectOfType<InputHandler>();

        SetGravityMode(GravityMode.World);
    }
	
	private void Update ()
    {
        if (currentMode == GravityMode.Player)
        {
            if (defLerpTime < 1)
            {
                defLerpTime += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(_startRot, transform.parent.rotation, defLerpTime);
            }
            else
            {
                float rotationInput = _inputHandler.rotationInput;

                if (rotationInput > 0)
                    transform.rotation = Quaternion.Lerp(transform.rotation, leftTrasform.rotation, 0.6f * Time.deltaTime);
                if (rotationInput < 0)
                    transform.rotation = Quaternion.Lerp(transform.rotation, rightTrasform.rotation, 0.6f * Time.deltaTime);
            }
        }

        if (_freezeTime > 0)
        {
            _freezeTime -= Time.deltaTime;
            if (_freezeTime <= 0)
                UnfreezeRigidbody();
        }
    }

    public bool CanMoveCheck(Vector3 movement)
    {
        RaycastHit[] hits;
        Vector3 direction = movement.normalized;
        float maxDistance = movement.magnitude;

        foreach (Collider col in _colliders)
        {
            hits = null;

            if (col is BoxCollider)
            {   // box info
                _boxCol = col as BoxCollider;
                Vector3 center = col.transform.position + _boxCol.center;
                Vector3 fromCenterToCorner = MathHelp.MultiplyVector3(col.transform.localScale, _boxCol.size) * (0.5f - float.Epsilon * 4);
                Quaternion orientation = col.transform.rotation;

                hits = Physics.BoxCastAll(center, fromCenterToCorner, direction, orientation, maxDistance);
            }
            else if (col is SphereCollider)
            {   // sphere info
                _sphereCol = col as SphereCollider;
                Vector3 origin = col.transform.position + _sphereCol.center;
                float radius = MathHelp.AbsBiggest(col.transform.localScale, false) * _sphereCol.radius;
                radius -= float.Epsilon * 4;

                hits = Physics.SphereCastAll(origin, radius, direction, maxDistance);
            }
            else if (col is CapsuleCollider)
            {   // capsule info
                float radius;
                Vector3[] centers = MathHelp.CapsuleEndPoints(col as CapsuleCollider, out radius);
                radius -= float.Epsilon * 4;

                if (centers.Length == 1)
                    hits = Physics.SphereCastAll(centers[0], radius, direction, maxDistance);
                else
                    hits = Physics.CapsuleCastAll(centers[0], centers[1], radius, direction, maxDistance);
            }

            if (hits != null)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.point == Vector3.zero)
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        continue;
                    }

                    if (hit.collider.GetComponent<StaticObject>() != null)
                        return false;
                }
            }
        }

        return true;
    }

    public void SetGravityMode(GravityMode mode)
    {
        if (currentMode == mode || _freezeTime > 0)
            return;

        currentMode = mode;
        
        if(currentMode == GravityMode.Freeze)
        {
            FreezeRigidbody();
            _freezeTime = 5f;

            mr.material = freezeMaterial;
        }
        else if(currentMode == GravityMode.Player)
        {
            defLerpTime = 0f;
            _startRot = transform.rotation;

            FreezeRigidbody();

            mr.material = actMaterial;
        }
        else if (currentMode == GravityMode.World)
        {
            UnfreezeRigidbody();

            mr.material = defMaterial;
        }
        else if(currentMode == GravityMode.Self)
        {
            UnfreezeRigidbody();

            mr.material = highMaterial;
        }
        else
        {
            FreezeRigidbody();

            mr.material = null;
        }
    }

    public void FreezeRigidbody()
    {
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnfreezeRigidbody()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    private void OnDrawGizmos()
    {
        #region draw colliders
        /*
        if (_colliders == null)
            _colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in _colliders)
        {
            // color
            if (col.isTrigger)
                Gizmos.color = Color.gray;
            else
                Gizmos.color = Color.black;


            if (col is BoxCollider)
            {
                // correct orientation
                Gizmos.matrix = Matrix4x4.TRS(col.transform.TransformPoint(Vector3.zero), col.transform.rotation, col.transform.localScale);

                _boxCol = col as BoxCollider;
                Gizmos.DrawWireCube(_boxCol.center, _boxCol.size);

                // back to default
                Gizmos.matrix = Matrix4x4.TRS(transform.parent.TransformPoint(Vector3.zero), transform.parent.rotation, transform.parent.localScale);
            }
            else if (col is SphereCollider)
            {
                _sphereCol = col as SphereCollider;
                Gizmos.DrawWireSphere(col.transform.position + _sphereCol.center, MathHelp.AbsBiggest(transform.localScale, false) * _sphereCol.radius);
            }
            else if (col is CapsuleCollider)
            {
                float radius;
                Vector3[] centers = MathHelp.CapsuleEndPoints(col as CapsuleCollider, out radius);

                foreach (Vector3 center in centers)
                    Gizmos.DrawWireSphere(center, radius);
            }
        }
        
        // back to defaults
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        */
        #endregion
    }
}
