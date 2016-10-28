using UnityEngine;
using System.Collections;

public class PhysicsCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( GetComponent<GUIText>() )
		{
			GetComponent<GUIText>().text = GameObject.FindObjectsOfType( typeof( Rigidbody ) ).Length + " physics objects in the scene" ; 
		}
	}
}
