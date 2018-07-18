using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    [Range(-1, 1)]
    public float min, max;
    private Material _infoMaterial;
    private Color _defaultColor;

    public float speed;
    private float _fadeTimer;

    private void Start()
    {
        _infoMaterial = GetComponent<MeshRenderer>().material;
        _defaultColor = _infoMaterial.color;
    }

    private void Update()
    {
        // timer
        _fadeTimer += Time.deltaTime * speed;
        // set colors alpha value (between min and max)
        _defaultColor.a = min + (Mathf.Sin(_fadeTimer) * 0.5f + 0.5f) * (max - min);
        // set color to material
        _infoMaterial.color = _defaultColor;
    }
}
