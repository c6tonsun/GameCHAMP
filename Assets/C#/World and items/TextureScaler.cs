using UnityEngine;

public class TextureScaler : MonoBehaviour
{
    public Vector2 offset;
    [Space(-10), Header("N times per meter.")]
    public Vector2 tiling;

    [Space(-10), Header("Select axis that scales tiling X and Y")]
    public TilingAxis tilingX;
    public TilingAxis tilingY;
    public enum TilingAxis
    {
        xScale = 0,
        yScale = 1,
        zScale = 2
    }

    private Material _material;
    private Vector2 _scale;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;

        // Do update thing in here.
        Debug.Log("Fix me before release.");
    }

    private void Update()
    {
        switch (tilingX)
        {
            case TilingAxis.xScale:
                _scale.x = transform.localScale.x;
                break;

            case TilingAxis.yScale:
                _scale.x = transform.localScale.y;
                break;

            case TilingAxis.zScale:
                _scale.x = transform.localScale.z;
                break;
        }

        switch (tilingY)
        {
            case TilingAxis.xScale:
                _scale.y = transform.localScale.x;
                break;

            case TilingAxis.yScale:
                _scale.y = transform.localScale.y;
                break;

            case TilingAxis.zScale:
                _scale.y = transform.localScale.z;
                break;
        }

        _material.mainTextureScale = tiling * _scale;
        _material.mainTextureOffset = offset;
    }
}
