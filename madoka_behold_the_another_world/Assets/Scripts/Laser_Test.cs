using UnityEngine;
using System.Collections;

public class Laser_Test : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
       
	}

    void Update()
    {
        int layermask = 1<<8;   //groundに引っかける
        if (Physics.Raycast(transform.position, Vector3.forward,layermask))
        {
            Destroy(gameObject);
        }
    }

}
