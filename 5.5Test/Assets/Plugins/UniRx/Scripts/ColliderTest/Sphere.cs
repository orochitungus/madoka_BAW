using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void OnCollisionEnter(Collision collision)
	{
		var target = collision.gameObject.GetComponent<Floor>();
		if (target == null)
		{
			Destroy(gameObject);
		}
	}
}
