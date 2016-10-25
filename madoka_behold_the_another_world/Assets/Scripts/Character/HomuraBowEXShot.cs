using UnityEngine;
using System.Collections;

public class HomuraBowEXShot : Bullet 
{
	/// <summary>
	/// 魔法陣から照射されるレーザー
	/// </summary>
	public Laser2 []Lasers;

	/// <summary>
	/// レーザーのスペック
	/// </summary>
	public ShotSpec Shotspec = new ShotSpec();

    

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
		// ファンネル時のステートを初期化
		Funnnelstate = FunnelState.Injcection;
		// 射出時の目標地点は本体で設定
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateCore();
		// 射出状態になったらステートを入れる
		if(Funnnelstate == FunnelState.Shoot)
		{
			for(int i=0; i<Lasers.Length; i++)
			{
				Lasers[i].m_Obj_OR = Shotspec.ObjOR;
				Lasers[i].m_CharacterIndex = Shotspec.CharacterIndex;
				Lasers[i].m_Hittype = Shotspec.Hittype;
				Lasers[i].m_DownRatio = Shotspec.DownRatio;
				Lasers[i].m_ArousalRatio = Shotspec.ArousalRatio;
			}
		}
	}
}

/// <summary>
/// レーザーのスペック
/// 射出時はディアクティブなのでパラメーターが送れないので一旦保持しておく
/// </summary>
public class ShotSpec
{
	/// <summary>
	/// 射出したキャラクター（オブジェクト）
	/// </summary>
	public GameObject ObjOR;

	/// <summary>
	/// 射出したキャラクター（種類）
	/// </summary>
	public int CharacterIndex;

	/// <summary>
	/// 被弾時の挙動
	/// </summary>
	public CharacterSkill.HitType Hittype;

	/// <summary>
	/// 攻撃力
	/// </summary>
	public int OffensivePower;

	/// <summary>
	/// ダウン値
	/// </summary>
	public float DownRatio;

	/// <summary>
	/// 覚醒ゲージ増加量
	/// </summary>
	public float ArousalRatio;
}