using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectEraser : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(SelfBreak());
	}

	private IEnumerator SelfBreak()
	{
		yield return new WaitForSeconds(1.0f);
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
