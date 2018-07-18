using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Color[] itemColorsByMass;
    public float[] itemMassLessThan;
    public Color[] itemModeColors;
    
    public static IEnumerator Shoot(Item item, Vector3 direction)
    {
        yield return new WaitForFixedUpdate();
        item.rb.AddForce(direction * 100, ForceMode.Impulse);
    }

    public static IEnumerator ShootAll(Item[] items, Vector3[] directions)
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                items[i].rb.AddForce(directions[i] * 100, ForceMode.Impulse);
        }
    }
}
