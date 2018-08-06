using UnityEngine;

public class SuccessLight : MonoBehaviour
{
    public Color noSuccess, yesSuccess;
    public int materialIndex;
    
    private Material _successMaterial;

    private void Start()
    {
        _successMaterial = GetComponent<MeshRenderer>().materials[materialIndex];
    }

    public void SetSuccess(bool success)
    {
        if (success)
            _successMaterial.color = yesSuccess;
        else
            _successMaterial.color = noSuccess;
    }
}
