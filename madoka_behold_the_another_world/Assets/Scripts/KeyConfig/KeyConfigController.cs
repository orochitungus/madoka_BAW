using UnityEngine;
using System.Collections;

public class KeyConfigController : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		string[] js = Input.GetJoystickNames();
		for(int i=0; i<js.Length; i++)
		{
			Debug.Log(js[i]);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.inputString != "")
		{
			Debug.Log(Input.inputString);
		}
		
	}
}
