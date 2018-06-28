using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    private MeshRenderer _mr;
    private Collider[] _colliders;

    private float _freezeTime;

    [HideInInspector]
    public Vector3 offset;

    private float _defLerpTime;
    private Quaternion _startRot;
    private Quaternion _oldRot;
    public Transform leftTrasform;
    public Transform rightTrasform;

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
    private float _castSizeFactor = 0.99f;

    private void Start ()
    {
        rb = GetComponent<Rigidbody>();
        _mr = GetComponent<MeshRenderer>();
        _colliders = GetComponentsInChildren<Collider>();

        SetGravityMode(GravityMode.World);
    }
	
	private void Update ()
    {
        if (_freezeTime > 0)
        {
            _freezeTime -= Time.deltaTime;
            if (_freezeTime <= 0)
                UnfreezeRigidbody();
        }

        if (!rb.useGravity)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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
                Vector3 fromCenterToCorner = MathHelp.MultiplyVector3(col.transform.localScale, _boxCol.size) * 0.5f * _castSizeFactor;
                Quaternion orientation = col.transform.rotation;

                hits = Physics.BoxCastAll(center, fromCenterToCorner, direction, orientation, maxDistance, LayerMask.NameToLayer("Item"));
            }
            else if (col is SphereCollider)
            {   // sphere info
                _sphereCol = col as SphereCollider;
                Vector3 origin = col.transform.position + _sphereCol.center;
                float radius = MathHelp.AbsBiggest(col.transform.localScale, false) * _sphereCol.radius * _castSizeFactor;

                hits = Physics.SphereCastAll(origin, radius, direction, maxDistance);
            }
            else if (col is CapsuleCollider)
            {   // capsule info
                float radius;
                Vector3[] centers = MathHelp.CapsuleEndPoints(col as CapsuleCollider, out radius);
                radius *= _castSizeFactor;

                if (centers.Length == 1)
                    hits = Physics.SphereCastAll(centers[0], radius, direction, maxDistance);
                else
                    hits = Physics.CapsuleCastAll(centers[0], centers[1], radius, direction, maxDistance);
            }

            if (hits != null)
            {
                foreach (RaycastHit hit in hits)
                {   
                    if (hit.collider.GetComponent<StaticObject>() != null)
                    {
                        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.white, 1f);

                        return false;
                    }
                }
            }
        }

        return true;
    }

    public bool CheckOverlap()
    {
        Collider[] collidersInside;

        foreach (Collider col in _colliders)
        {
            collidersInside = null;

            if (col is BoxCollider)
            {
                _boxCol = col as BoxCollider;
                collidersInside = Physics.OverlapBox(
                    col.transform.position + _boxCol.center, 
                    MathHelp.MultiplyVector3(col.transform.localScale, _boxCol.size) * 0.5f,
                    col.transform.rotation,
                    LayerMask.NameToLayer("Item"));
            }
            else if (col is SphereCollider)
            {
                _sphereCol = col as SphereCollider;
                collidersInside = Physics.OverlapSphere(
                    col.transform.position + _sphereCol.center,
                    MathHelp.AbsBiggest(col.transform.localScale, ignoreY: false) * _sphereCol.radius,
                    LayerMask.NameToLayer("Item"));
            }
            else if (col is CapsuleCollider)
            {
                _capsuleCol = col as CapsuleCollider;
                float radius;
                Vector3[] points = MathHelp.CapsuleEndPoints(_capsuleCol, out radius);

                if (points.Length == 2)
                    collidersInside = Physics.OverlapCapsule(points[0], points[1], radius, LayerMask.NameToLayer("Item"));
                else
                    collidersInside = Physics.OverlapCapsule(points[0], points[0], radius, LayerMask.NameToLayer("Item"));
            }

            foreach (Collider c in collidersInside)
                Debug.DrawLine(col.transform.position, c.transform.position, Color.black, 1f);

            if (collidersInside.Length > 0)
                return true;
        }

        return false;
    }

    public void DoRotate(float rotationInput)
    {
        _oldRot = transform.rotation;

        if (_defLerpTime < 1)
        {
            _defLerpTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(_startRot, transform.parent.rotation, _defLerpTime);

            if (CheckOverlap())
            {
                _defLerpTime -= Time.deltaTime;
                transform.rotation = _oldRot;
            }
        }
        else
        {
            if (rotationInput > 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, leftTrasform.rotation, 0.6f * Time.deltaTime);
            if (rotationInput < 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, rightTrasform.rotation, 0.6f * Time.deltaTime);

            if (CheckOverlap())
            {
                transform.rotation = _oldRot;
            }
        }
    }

    public void SetGravityMode(GravityMode mode)
    {
        if (currentMode == mode || _freezeTime > 0)
            return;

        currentMode = mode;

        switch(currentMode)
        {
            case GravityMode.World:
                UnfreezeRigidbody();
                _mr.material = defMaterial;
                break;

            case GravityMode.Player:
                _defLerpTime = 0f;
                _startRot = transform.rotation;
                FreezeRigidbody();
                _mr.material = actMaterial;
                break;

            case GravityMode.Self:
                UnfreezeRigidbody();
                _mr.material = highMaterial;
                break;

            case GravityMode.Freeze:
                FreezeRigidbody();
                _freezeTime = 5f;
                _mr.material = freezeMaterial;
                break;

            default:
                FreezeRigidbody();
                _mr.material = null;
                break;

        }

    }

    public void FreezeRigidbody()
    {
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
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
