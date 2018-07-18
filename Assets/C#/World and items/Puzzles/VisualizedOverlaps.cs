using UnityEngine;

public class VisualizedOverlaps : MonoBehaviour
{
    public LayerMask layersToHit;
    public bool isBall;
    public float ballRadius = 1f;
    public Vector3 boxSize = Vector3.one;
    public bool offsetIngoresLocalRotation;
    public Vector3 offset;

    private Quaternion _rotation;
    private Vector3 _oldBoxSize;
    private Vector3 _center;
    private Vector3 _localX;
    private Vector3 _localY;
    private Vector3 _localZ;
    private Vector3[] _corners;
    
    protected Collider[] _colliders = new Collider[0];

    private void OnDrawGizmos()
    {
        if (offsetIngoresLocalRotation)
            _center = transform.position + offset;
        else
            _center = transform.position +
                transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;

        Gizmos.color = Color.black;

        if (isBall)
            Gizmos.DrawWireSphere(_center, ballRadius);
        else
        {
            // recalculate corners
            if (transform.rotation != _rotation || _oldBoxSize != boxSize || _corners == null)
            {
                _localX = transform.right * boxSize.x * 0.5f;
                _localY = transform.up * boxSize.y * 0.5f;
                _localZ = transform.forward * boxSize.z * 0.5f;

                _corners = new Vector3[2 * 2 * 2];
                int index = 0;
                for (int xSign = -1; xSign < 2; xSign += 2)
                {
                    for (int ySign = -1; ySign < 2; ySign += 2)
                    {
                        for (int zSign = -1; zSign < 2; zSign += 2)
                        {
                            _corners[index] = _localX * xSign + _localY * ySign + _localZ * zSign;
                            index++;
                        }
                    }
                }

                _rotation = transform.rotation;
                _oldBoxSize = boxSize;
            }

            // draws cube
            for (int i = 0; i < _corners.Length; i++)
            {
                if (i % 2 == 0)
                    Gizmos.DrawLine(_center + _corners[i], _center + _corners[i + 1]);

                if (i < 4)
                    Gizmos.DrawLine(_center + _corners[i], _center + _corners[i + 4]);

                if (i < 2 || i == 4 || i == 5)
                    Gizmos.DrawLine(_center + _corners[i], _center + _corners[i + 2]);
            }
        }
    }

    protected void Update()
    {
        if (offsetIngoresLocalRotation)
            _center = transform.position + offset;
        else
            _center = transform.position +
                transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;

        if (isBall)
            _colliders = Physics.OverlapSphere(_center, ballRadius, layersToHit);
        else
            _colliders = Physics.OverlapBox(_center, boxSize * 0.5f, transform.rotation, layersToHit);
    }
}
