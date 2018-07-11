using UnityEngine;

public class VisualizedOverlaps : MonoBehaviour
{
    public bool isBall;
    public float ballRadius;
    public Vector3 boxSize;

    protected Collider[] _colliders;

    private void OnDrawGizmos()
    {
        if (isBall)
            Gizmos.DrawWireSphere(transform.position, ballRadius);
        else
        {
            // correct orientation
            Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(Vector3.zero), transform.rotation, transform.localScale);

            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // back to default
            Gizmos.matrix = Matrix4x4.TRS(transform.parent.TransformPoint(Vector3.zero), transform.parent.rotation, transform.parent.localScale);
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
