using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public CubeHandler cubeHandler;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.A))
        {
            cubeHandler.rb.velocity = Vector3.zero;
            cubeHandler.SetOwnGravity(new Vector3(0, 5, 0));
        }

        if(Input.GetKeyUp(KeyCode.A)) 
        {
            cubeHandler.rb.velocity = Vector3.zero;
        }
       

	}
}
