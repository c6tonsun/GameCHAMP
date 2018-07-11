using UnityEngine;

public class ItemShadow : MonoBehaviour
{
    private Transform _shadow;

    private void Start()
    {
        _shadow = GetComponentInChildren<Projector>().transform;
    }

    private void Update()
    {
        _shadow.forward = Vector3.down;
    }
}
