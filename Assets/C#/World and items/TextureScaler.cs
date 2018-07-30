using UnityEngine;

public class TextureScaler : MonoBehaviour
{
    [Space(-10), Header("N times per meter.")]
    public Vector2 tiling;

    private Material _material;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;

        tiling.x *= transform.localScale.x;
        tiling.y *= transform.localScale.y;

        _material.mainTextureScale = tiling;
    }
}
