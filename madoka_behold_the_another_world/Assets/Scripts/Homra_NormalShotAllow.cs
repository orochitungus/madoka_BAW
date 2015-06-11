using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Homra_NormalShotAllow : Bullet 
{
    
	void Start () 
    {
	    // 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
        this.m_LifeTime = 6.0f;
        this.m_MoveDirection = Vector3.zero;
		// 独立フラグを初期化
		this.m_IsIndependence = false;
        // 誘導カウンターを初期化
        this.m_InductionCounter = 0;
        // 誘導時間を初期化
        this.m_InductionBias = 30;
        // 親スクリプトを初期化
        this.m_ParentScript = "Homura_Final_BattleControl";
        m_InjectionObjectName = "homura_ribon_magica_battle_use";
        // 親オブジェクトを取得(rootでツリーの一番上から検索をかける）        
        m_Obj_OR = transform.root.GetComponentInChildren<Homura_Final_BattleControl>().gameObject;
        // 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）
        Physics.IgnoreCollision(this.transform.collider, m_Obj_OR.transform.collider);
        // 撃ったキャラが誰であるか保持
        m_CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B;
	}


	
	// Update is called once per frame
	void Update () 
    {
        UpdateCore();       
	}

    void LateUpdate()
    {
       
    }
}
