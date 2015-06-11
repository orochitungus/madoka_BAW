using UnityEngine;
using System.Collections;

public class RandomVelocity : MonoBehaviour {
	
	
	public float speed ;
	
	void Reset()
	{
		speed = 10.0f ; 
	}
	// Use this for initialization
	void Start () {
		if( gameObject.rigidbody )
		{
			rigidbody.velocity = Random.onUnitSphere * speed ; 
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
