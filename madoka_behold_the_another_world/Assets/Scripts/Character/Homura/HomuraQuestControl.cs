using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター「暁美　ほむら」（クエスト）を制御するためのスクリプト
/// </summary>
public class HomuraQuestControl : CharacterControlBaseQuest
{
	/// <summary>
	/// 本体のアニメーター制御ユニット
	/// </summary>
	[SerializeField]
	private Animator AnimatorUnit;

	#region アニメーションID
	private int RunID;
	private int IdleID;
	private int JumpID;
	private int FallID;
	#endregion



	// Use this for initialization
	void Start () 
	{
		// ジャンプ硬直
		JumpWaitTime = 0.5f;

		//着地硬直
		LandingWaitTime = 1.0f;

		WalkSpeed = 1.0f;                                // 移動速度（歩行の場合）
		RunSpeed = 15.0f;                                // 移動速度（走行の場合）
		AirDashSpeed = 20.0f;                            // 移動速度（空中ダッシュの場合）
		AirMoveSpeed = 7.0f;                             // 移動速度（空中慣性移動の場合）
				
		// コライダの地面からの高さ
		ColliderHeight = 0.785f;

		IdleID = Animator.StringToHash("Base Layer.HomuraQuestIdle");
		RunID = Animator.StringToHash("Base Layer.HomuraQuestRun");
		JumpID = Animator.StringToHash("Base Layer.HomuraQuestJump");
		FallID = Animator.StringToHash("Base Layer.HomuraQuestFall");

		FirstSetting(AnimatorUnit);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Update_Core(AnimatorUnit, IdleID, RunID))
		{
			UpdateAnimation();
		}
	}

	void UpdateAnimation()
	{
		// 通常
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == IdleID)
		{
			Animation_Idle(AnimatorUnit);
		}
		// 走行
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RunID)
		{
			Animation_Run(AnimatorUnit);
		}
		// ジャンプ
		else if(AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpID)
		{
			
		}
		// 落下
	}
}
