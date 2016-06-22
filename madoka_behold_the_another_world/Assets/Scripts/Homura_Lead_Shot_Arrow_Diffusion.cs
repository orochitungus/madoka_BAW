using UnityEngine;
using System.Collections;

public class Homura_Lead_Shot_Arrow_Diffusion : Bullet 
{

	// Use this for initialization
	void Start ()
    {
        // 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
        this.LifeTime = 4.0f;
        this.MoveDirection = Vector3.zero;
        // 独立フラグを初期化
        this.IsIndependence = false;
        // 誘導カウンターを初期化
        this.InductionCounter = 0;
        // 誘導時間を初期化
        this.InductionBias = 30;
        // 親オブジェクトを取得
        //GameObject m_Obj_OR = this.transform.parent.gameObject;
        // 接触対象に自分と親は除く
        //Physics.IgnoreCollision(this.transform.collider, m_Obj_OR.transform.collider);  
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}
}
