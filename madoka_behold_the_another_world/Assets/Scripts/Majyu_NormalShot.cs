using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Majyu_NormalShot : Bullet 
{
	void Start () 
    {
        // 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
        this.m_LifeTime = 4.0f;
        this.m_MoveDirection = Vector3.zero;
        // 独立フラグを初期化
        this.m_IsIndependence = false;
        // 誘導カウンターを初期化
        this.m_InductionCounter = 0;
        // 誘導時間を初期化
        this.m_InductionBias = 30;
        // 親スクリプトを初期化
        this.m_ParentScript = "majyu_BattleControl";
        // 親オブジェクトを設定
        m_InjectionObjectName = "majyu_use_battle";
        // 親オブジェクトを取得(rootでツリーの一番上から検索をかける） 
        m_Obj_OR = transform.root.GetComponentInChildren<majyu_BattleControl>().gameObject;
        // 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）
        Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), m_Obj_OR.transform.GetComponent<Collider>());
        // 撃ったキャラが誰であるか保持
        m_CharacterIndex = (int)Character_Spec.CHARACTER_NAME.ENEMY_MAJYU;
        // 被弾時の挙動を設定
        m_Hittype = Character_Spec.cs[m_CharacterIndex][0].m_Hittype;
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateCore();
	}
}
