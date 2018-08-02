using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    public bool deleteSaveFile;
    public Color[] itemModeColors;
    
    public static IEnumerator ShootItem(Item item, Vector3 direction)
    {
        yield return new WaitForFixedUpdate();
        item.rb.AddForce(direction * 100, ForceMode.Impulse);
    }

    public static IEnumerator ShootItems(Item[] items, Vector3[] directions)
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                items[i].rb.AddForce(directions[i] * 100, ForceMode.Impulse);
        }
    }

#if UNITY_EDITOR
    
    private void Update()
    {
        if (deleteSaveFile)
        {
            SaveLoad.Delete();
            Debug.Log("Save file deleted");
            deleteSaveFile = false;
        }
    }

#endif
}
