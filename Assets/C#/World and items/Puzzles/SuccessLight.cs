﻿using UnityEngine;

public class SuccessLight : MonoBehaviour
{
    public Color noSuccess, yesSuccess;
    public int materialIndex;
    
    private Material _successMaterial;

    public void SetSuccess(bool success)
    {
        if (_successMaterial == null)
            _successMaterial = GetComponent<MeshRenderer>().materials[materialIndex];

        if (success)
            _successMaterial.color = yesSuccess;
        else
            _successMaterial.color = noSuccess;
    }
}
