using UnityEngine;
using System.Collections;

public class Scono_SubShot : Bullet
{

	// Use this for initialization
	void Start () 
    {
        // 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
        this.LifeTime = 6.0f;
        this.MoveDirection = Vector3.zero;
        // 独立フラグを初期化
        this.IsIndependence = false;
        // 誘導カウンターを初期化
        this.InductionCounter = 0;
        // 誘導時間を初期化
        this.InductionBias = 30;
        // 親スクリプトを初期化
        this.ParentScriptName = "Scono_Battle_Control";
        InjectionObjectName = "Scono_battle_use";
        // TODO:親オブジェクトを取得(rootでツリーの一番上から検索をかける）        
        //InjectionObject = transform.root.GetComponentInChildren<Scono_Battle_Control>().gameObject;
        // 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）
        Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), InjectionObject.transform.GetComponent<Collider>());
        // 撃ったキャラが誰であるか保持
        InjectionCharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
        // 被弾時の挙動を設定
        Hittype = Character_Spec.cs[InjectionCharacterIndex][1].m_Hittype;
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateCore();
	}
}
