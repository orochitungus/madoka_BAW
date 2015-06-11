using UnityEngine;
using System.Collections;

public class PhysicsCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( guiText )
		{
			guiText.text = GameObject.FindObjectsOfType( typeof( Rigidbody ) ).Length + " physics objects in the scene" ; 
		}
	}
}
