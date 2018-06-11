using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationArea : MonoBehaviour {

    public float radius;

    private Collider[] itemsInside;
    private Collider[] oldItems;
    
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, radius);    
    }

    public void ManipulateArea(ItemMovement.GravityMode mode, Vector3 followMovement)
    {
        oldItems = itemsInside;
        itemsInside = Physics.OverlapSphere(transform.position, radius);
        ItemMovement item;

        bool[] keepOld;

        if (oldItems != null)
            keepOld = new bool[oldItems.Length];
        else
            keepOld = new bool[0];

        for (int i = 0; i < itemsInside.Length; i++)
        {
            item = itemsInside[i].GetComponent<ItemMovement>();

            if (item)
            {
                item.Move(followMovement);
                item.SetGravityMode(mode);

                // check which colliders where inside last and this frame
                for (int old = 0; old < oldItems.Length; old++)
                {
                    if (itemsInside[i] == oldItems[old])
                    {
                        keepOld[old] = true;
                        break;
                    }
                }
            }
        }

        // colliders that left area set to default mode
        for (int i = 0; i < keepOld.Length; i++)
        {
            item = oldItems[i].GetComponent<ItemMovement>();
            if (item && !keepOld[i])
                item.SetGravityMode(ItemMovement.GravityMode.World);
        }
    }

}
