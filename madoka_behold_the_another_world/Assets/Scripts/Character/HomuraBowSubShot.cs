using UnityEngine;
using System.Collections;

public class HomuraBowSubShot : Bullet
{

	// Use this for initialization
	void Start () 
	{
		// 維持時間を初期化（後で調整し、赤ロック範囲内を飛行中は生き残らせること）
		LifeTime = 6.0f;
		MoveDirection = Vector3.zero;
		// 独立フラグを初期化
		IsIndependence = false;
		// 誘導カウンターを初期化
		InductionCounter = 0;
		// 誘導時間を初期化
		InductionBias = 30;
		InjectionObjectName = "HomuraBowBattleUse";
		// 親オブジェクトを取得(rootでツリーの一番上から検索をかける）        
		InjectionObject = transform.root.GetComponentInChildren<HomuraBowControl>().gameObject;
		// 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）
		Physics.IgnoreCollision(transform.GetComponent<Collider>(), InjectionObject.transform.GetComponent<Collider>());
		// 撃ったキャラが誰であるか保持
		InjectionCharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateCore();
	}
}
