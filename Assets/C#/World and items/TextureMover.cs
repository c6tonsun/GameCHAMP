using UnityEngine;

public class TextureMover : MonoBehaviour
{
    public bool unScaledTime;
    public Vector2 offsetSpeed;
    public int materialIndex;

    private Material _material;
    private Vector2 _offset;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().materials[materialIndex];
    }

    private void Update()
    {
        if (unScaledTime)
            _offset += Time.unscaledDeltaTime * offsetSpeed;
        else
            _offset += Time.deltaTime * offsetSpeed;

        _material.mainTextureOffset = _offset;
    }
}
