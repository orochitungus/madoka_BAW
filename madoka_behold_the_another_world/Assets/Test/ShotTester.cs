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
            //AnimatorUnit.SetInteger("NowState", 1);
            AnimatorUnit.SetTrigger("shoot");
		}
	}

	void LateUpdate()
	{
		AnimatorUnit.SetInteger("NowState", -1);
	}

	public void FollowThrowDone()
	{
        //AnimatorUnit.SetInteger("NowState", 2);
        AnimatorUnit.SetTrigger("followthrow");
	}

	public void ReturnToIdle()
	{
        //AnimatorUnit.SetInteger("NowState", 0);
        AnimatorUnit.SetTrigger("Idle");
	}
}
