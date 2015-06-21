using UnityEngine;
using System.Collections;

public class SpawnPlayerBalls : MonoBehaviour {
	
	public GameObject spawnGameObject ; 
	
	// Use this for initialization
	void Start () {
//		ball = GameObject.CreatePrimitive( PrimitiveType.Sphere ) ; 
//		ball.transform.localScale = new Vector3( 20.0f, 20.0f, 20.0f ) ; 
	}
	
	// Update is called once per frame
	void Update () {
		
		if( Input.GetMouseButtonDown( 0 ) )
		{
			Ray ray = Camera.mainCamera.ScreenPointToRay( Input.mousePosition ) ;
			
			RaycastHit hit ; 
			if( Physics.Raycast( ray, out hit ) )
			{
				// ball.transform.position = hit.point ; 
				GameObject gobj = ( GameObject)GameObject.Instantiate( spawnGameObject, Camera.mainCamera.transform.position + ( ray.direction * 17.5f ), Quaternion.identity ) ; 

				if( gobj.rigidbody )
				{
					gobj.rigidbody.velocity = ray.direction * 275.0f ; 
				}
			}
			

		}
	}
}
