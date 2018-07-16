using UnityEngine;

public class VisualizedOverlaps : MonoBehaviour
{
    public bool isBall;
    public float ballRadius = 1f;
    public Vector3 boxSize = Vector3.one;
    public bool offsetIngoresLocalRotation;
    public Vector3 offset;

    private Quaternion rotation;
    private Vector3 center;
    private Vector3 localX;
    private Vector3 localY;
    private Vector3 localZ;
    private Vector3[] corners;

    protected Collider[] _colliders;

    private void OnDrawGizmos()
    {
        if (isBall)
            Gizmos.DrawWireSphere(transform.position, ballRadius);
        else
        {
            if (offsetIngoresLocalRotation)
                center = transform.position + offset;
            else
                center = transform.position + 
                    transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;

            // when object is rotated recalculate corners
            if (transform.rotation != rotation || corners == null)
            {
                localX = transform.right * boxSize.x * 0.5f;
                localY = transform.up * boxSize.y * 0.5f;
                localZ = transform.forward * boxSize.z * 0.5f;

                corners = new Vector3[2 * 2 * 2];
                int index = 0;
                for (int xSign = -1; xSign < 2; xSign += 2)
                {
                    for (int ySign = -1; ySign < 2; ySign += 2)
                    {
                        for (int zSign = -1; zSign < 2; zSign += 2)
                        {
                            corners[index] = localX * xSign + localY * ySign + localZ * zSign;
                            index++;
                        }
                    }
                }

                rotation = transform.rotation;
            }

            // draws cube
            for (int i = 0; i < corners.Length; i++)
            {
                if (i % 2 == 0)
                    Gizmos.DrawLine(center + corners[i], center + corners[i + 1]);

                if (i < 4)
                    Gizmos.DrawLine(center + corners[i], center + corners[i + 4]);

                if (i < 2 || i == 4 || i == 5)
                    Gizmos.DrawLine(center + corners[i], center + corners[i + 2]);
            }
        }
    }

    protected void Update()
    {
        if (isBall)
            _colliders = Physics.OverlapSphere(transform.position, ballRadius);
        else
            _colliders = Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation);
    }
}
