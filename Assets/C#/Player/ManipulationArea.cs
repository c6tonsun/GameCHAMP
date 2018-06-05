using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationArea : MonoBehaviour {

    public float radius;
    public LayerMask itemLayer;

    private Collider[] itemsInside;
    private Collider[] oldItems;
    
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, radius);    
    }

    public void ManipulateArea(CubeHandler.GravityMode mode, Vector3 followMovement)
    {
        oldItems = itemsInside;
        itemsInside = Physics.OverlapSphere(transform.position, radius);
        CubeHandler cubeHandler;

        bool[] keepOld;

        if (oldItems != null)
            keepOld = new bool[oldItems.Length];
        else
            keepOld = new bool[0];

        for (int i = 0; i < itemsInside.Length; i++)
        {
            cubeHandler = itemsInside[i].GetComponent<CubeHandler>();

            if (cubeHandler)
            {
                cubeHandler.Move(followMovement);
                cubeHandler.SetGravityMode(mode);

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
            cubeHandler = oldItems[i].GetComponent<CubeHandler>();
            if (cubeHandler && !keepOld[i])
                cubeHandler.SetGravityMode(CubeHandler.GravityMode.World);
        }
    }

}
