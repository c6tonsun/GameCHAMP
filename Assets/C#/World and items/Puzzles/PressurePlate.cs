using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour {

    private Transform _button;
    private Transform _base;

    private Vector3 _btnDefPos;

    public bool isBall;
    private float _ballRadius;

    private Collider[] _colliders;

    private void Start()
    {
        _button = transform.GetChild(0).transform;
        _base = transform.GetChild(1).transform;
        _colliders = new Collider[0];
    }

    private void Update()
    {

        if(isBall)
        {
            _colliders = Physics.OverlapSphere(_button.position, _ballRadius);
        }
        else
        {
            _colliders = Physics.OverlapBox(_button.position, _button.localScale);
        }

        if(_colliders.Length == 0)
        {
            return;
        }

        float totalMass = 0;

        for (int i = 0; i < _colliders.Length; i++)
        {
            if(_colliders[i] != null && _colliders[i].GetComponent<Item>())
            {
                totalMass += _colliders[i].GetComponent<Rigidbody>().mass;
            }            
        }

        Debug.Log("mass: " + totalMass);

    }

}
