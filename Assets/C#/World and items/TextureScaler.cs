using UnityEngine;

public class TextureScaler : MonoBehaviour
{
    [Space(-10), Header("N times per meter.")]
    public Vector2 tiling;
    [Space(-10), Header("Select two axis for scaling.")]
    public bool x = true, y = true, z = false;

    private Material _material;
    private Vector2 scale;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;

        //tiling.x *= transform.localScale.x;
        //tiling.y *= transform.localScale.y;

        //_material.mainTextureScale = tiling;
        Debug.Log("Fix me before release.");
    }

    private void Update()
    {
        // x
        if (x)
            scale.x = transform.localScale.x;
        if (y && z)
            scale.x = transform.localScale.y;
        // y
        if (z)
            scale.y = transform.localScale.z;
        if (x && y)
            scale.y = transform.localScale.y;


        _material.mainTextureScale = tiling * scale;
    }
}
