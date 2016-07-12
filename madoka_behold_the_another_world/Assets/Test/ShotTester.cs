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
			AnimatorUnit.SetInteger("NowState", 1);
		}
	}

	void LateUpdate()
	{
		AnimatorUnit.SetInteger("NowState", -1);
	}

	public void FollowThrowDone()
	{
		AnimatorUnit.SetInteger("NowStete", 2);
	}

	public void ReturnToIdle()
	{
		AnimatorUnit.SetInteger("NowState", 0);
	}
}
