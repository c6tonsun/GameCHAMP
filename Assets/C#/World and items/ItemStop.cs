using UnityEngine;

public class ItemStop : MonoBehaviour
{
    [HideInInspector]
    public float stopTime;

    private void Update()
    {
        stopTime -= Time.deltaTime;
        if (stopTime < 0)
            enabled = false;
    }
}
