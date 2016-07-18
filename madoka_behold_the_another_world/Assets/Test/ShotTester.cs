using UnityEngine;
using System.Collections;

public class ShotTester : MonoBehaviour 
{

	public Animator AnimatorUnit;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
            AnimatorUnit.SetTrigger("shoot");
		}
	}

	void LateUpdate()
	{
	}

	public void FollowThrowDone()
	{
        AnimatorUnit.SetTrigger("followthrow");
	}

	public void ReturnToIdle()
	{
        AnimatorUnit.SetTrigger("Idle");
	}
}
