#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class HandleStatic : MonoBehaviour
{
    private GameObject[] objects;
    private StaticObject staticObject;
    private bool hasCollider;

    private void Update()
    {
        objects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in objects)
        {
            hasCollider = go.GetComponent<Collider>() != null;
            staticObject = go.GetComponent<StaticObject>();

            if (go.isStatic && hasCollider && staticObject == null)
                go.AddComponent<StaticObject>();
            else if (!go.isStatic && staticObject != null)
                DestroyImmediate(staticObject);
        }
    }
}
#endif
