using UnityEngine;
using System.Collections;

public class Homura_Quest_Control : CharacterControl_Base_Quest
{

	// Use this for initialization
	void Start () 
    {
        this.m_AnimationNames = new string[]
        {
            "homura_uniform_neutral_copy",      // Idle,                   // 通常
            "homura_uniform_walk_copy",         // Walk,                   // 歩行
            "",                                 // Walk_underonly,         // 歩行（下半身のみ）
            "homura_uniform_jump_copy",         // Jump,                   // ジャンプ開始
            "",                                 // Jump_underonly,         // ジャンプ開始（下半身のみ）
            "homura_uniform_jumping_copy",      // Jumping,                // ジャンプ中（上昇状態）
            "",                                 // Jumping_underonly,      // ジャンプ中（下半身のみ）
            "homura_uniform_fall_copy",         // Fall,                   // 落下
            "homura_uniform_landing_copy",      // Landing,                // 着地
            "homura_uniform_run_copy",          // Run,                    // 走行
            "",                                 // Run_underonly,          // 走行（下半身のみ）
            "homura_uniform_airdash_copy"       // AirDash,                // 空中ダッシュ
        };

        // ジャンプ硬直
        this.m_JumpWaitTime = 0.5f;

        //着地硬直
        this.m_LandingWaitTime = 1.0f;

        this.m_WalkSpeed = 1.0f;                                // 移動速度（歩行の場合）
        this.m_RunSpeed = 15.0f;                                // 移動速度（走行の場合）
        this.m_AirDashSpeed = 20.0f;                            // 移動速度（空中ダッシュの場合）
        this.m_AirMoveSpeed = 7.0f;                             // 移動速度（空中慣性移動の場合）
        this.m_RateofRise = 5.0f;                               // 上昇速度

        // ブースト消費量
        this.m_JumpUseBoost = 10;       // ジャンプ時
        this.m_DashCancelUseBoost = 10;   // ブーストダッシュ時
        this.m_BoostLess = 0.5f;        // ジャンプの上昇・BD時の1F当たりの消費量

        // コライダの地面からの高さ
        this.m_Collider_Height = 1.5f;

        FirstSetting();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Update_Core())
        {
            Update_Animation();
        }
	}

   
}
