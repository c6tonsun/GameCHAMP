#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class HandleStatic : MonoBehaviour
{
    private GameObject[] _objects;
    private StaticObject _staticObject;
    private bool _hasCollider;
    private bool _canNeedStatic;

    private void Update()
    {
        _objects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in _objects)
        {
            _hasCollider = go.GetComponent<Collider>() != null;
            _staticObject = go.GetComponent<StaticObject>();

            _canNeedStatic = go.isStatic || go.tag.Contains("Door");

            if (_canNeedStatic && _hasCollider && _staticObject == null)
                go.AddComponent<StaticObject>();
            else if (!_canNeedStatic && _staticObject != null)
                DestroyImmediate(_staticObject);
        }
    }
}
#endif
