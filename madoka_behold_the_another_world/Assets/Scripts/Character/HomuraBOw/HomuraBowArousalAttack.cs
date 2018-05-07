using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomuraBowArousalAttack : MonoBehaviour 
{
	// 攻撃力
	protected int OffemsivePower;

	// ダウン値
	protected float DownRatio;

	// 動作元のゲームオブジェクト
	protected GameObject Obj_OR;

	// ヒットタイプ
	protected CharacterSkill.HitType Hittype;

	// 吹き飛びになったときの打ち上げ量
	protected float LaunchOffset;

	// 吹き飛びになったときの打ち上げる力
	private float Launchforce;

	// ヒット時のSE
	public AudioClip InspHitSE;

	// 接触したゲームオブジェクト
	private GameObject HitTarget;

	/// <summary>
	/// 着弾時のヒットエフェクト
	/// </summary>
	public GameObject HitEffect;

	/// <summary>
	/// 除外対象
	/// </summary>
	public string Exclusion;

	// 出現時の初期化.攻撃力やダウン値の設定はSetStatusでPCから呼ぶ
	// Startに書くとSetStatus（Awakeの直後？）より後に実行される
	void Awake()
	{
		// 各ステータスを初期化
		OffemsivePower = 0;
		DownRatio = 0;
		Obj_OR = null;
		Hittype = CharacterSkill.HitType.BEND_BACKWARD;
		LaunchOffset = 0.0f;
	}

	// ステート設定
	// offensive    [in]:攻撃力
	// downR        [in]:ダウン値
	// hittype      [in]:ヒットタイプ
	// launch       [in]:打ち上げ量
	// force        [in]:打ち上げ時に加える力
	public virtual void SetStatus(int offensive, float downR, CharacterSkill.HitType hittype, float launch = 10.0f, float force = 5.0f)
	{
		// 親のオブジェクトを拾う
		Obj_OR = transform.root.GetComponentInChildren<CharacterControlBase>().gameObject;
		// 自機をダメージ対象から除外する
		Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), Obj_OR.transform.GetComponent<Collider>());
		// 各ステートを設定
		OffemsivePower = offensive;
		DownRatio = downR;
		Hittype = hittype;
		LaunchOffset = launch;
		Launchforce = force;
	}

	// ヒット処理
	// 当たったらその相手にダメージを与える.破壊は親元で行う
	public void OnCollisionEnter(Collision collision)
	{
		// 除外対象がある場合、除外対象に当たったら何もしない
		if (Exclusion != "")
		{
			if (collision.gameObject.name == Exclusion)
			{
				return;
			}
		}
		// 地面に当たった場合は何もしない
		if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
		{
			return;
		}
				
		string player;
		string enemy;
		// ヒットSEを鳴らす
		if (InspHitSE != null)
		{
			AudioSource.PlayClipAtPoint(InspHitSE, transform.position);
		}
		// 着弾した位置にヒットエフェクトを置く           
		GameObject hiteffect = (GameObject)Instantiate(HitEffect, transform.position, transform.rotation);
		Instantiate(hiteffect, transform.position, transform.rotation);

		// ガードされた場合は強制抜け（ガードオブジェクトはCharacterContorol_Baseを継承しない）
		if (Obj_OR == null)
		{
			return;
		}

		// 親オブジェクトを拾う
		var master = Obj_OR.GetComponent<CharacterControlBase>();

		// 自機がPLAYERかPLAYER_ALLYの場合
		if (master.IsPlayer != CharacterControlBase.CHARACTERCODE.ENEMY)
		{
			player = "Player";
			enemy = "Enemy";
		}
		// 自機がEnemyの場合
		else
		{
			player = "Enemy";
			enemy = "Player";
		}
		// 接触対象を取得
		var target = collision.gameObject.GetComponent<CharacterControlBase>();
		HitTarget = collision.gameObject;

		// targetがCharacterControl_Baseクラスでなければ強制抜け
		if (target == null)
		{
			return;
		}

		// ダウン中かダウン値MAXならダメージを与えない
		if (target.GetInvincible())
		{
			// オブジェクトを自壊させる
			Destroy(gameObject);
			return;
		}
		// そうでないならダメージとダウン値加算を確定
		else
		{
			// 敵に触れた場合
			if (collision.gameObject.tag == enemy)
			{
				// 覚醒時ダメージ補正
				DamageCorrection();
				// 攻撃したキャラクター
				int CharacterIndex = (int)(Obj_OR.GetComponent<CharacterControlBase>().CharacterName);
				// ダメージ
				collision.gameObject.GetComponent<CharacterControlBase>().DamageHP(CharacterIndex, OffemsivePower);
			}
			// 味方に触れた場合
			else if (collision.gameObject.tag == player)
			{
				// 覚醒時ダメージ補正
				DamageCorrection();
				// 攻撃したキャラクター
				int AttackedCharacter = (int)(Obj_OR.GetComponent<CharacterControlBase>().CharacterName);
				// ダメージ量
				int AttackedDamage = (int)((float)OffemsivePower / MadokaDefine.FRENDLYFIRE_RATIO);
				collision.gameObject.GetComponent<CharacterControlBase>().DamageHP(AttackedCharacter, AttackedDamage);
			}
			// ダウン値加算
			collision.gameObject.SendMessage("DownRateInc", DownRatio);
		}		

		// ヒット時にダメージの種類をCharacterControl_Baseに与える
		// ダウン値を超えていたら吹き飛びへ移行
		// Blow属性の攻撃を与えた場合も吹き飛びへ移行
		if (target.GetNowDownRatio() >= target.GetDownRatioBias() || this.Hittype == CharacterSkill.HitType.BLOW)
		{
			// 吹き飛びの場合、相手に方向ベクトルを与える            
			// Y軸方向は少し上向き
			target.MoveDirection.y += 5;

			target.BlowDirection = Obj_OR.GetComponent<CharacterControlBase>().MoveDirection;
			// 吹き飛びの場合、攻撃を当てた相手を浮かす（m_launchOffset)            
			target.GetComponent<Rigidbody>().position = target.GetComponent<Rigidbody>().position + new Vector3(Launchforce, this.LaunchOffset, Launchforce);
			target.GetComponent<Rigidbody>().AddForce(master.MoveDirection.x * LaunchOffset, master.MoveDirection.y * LaunchOffset, master.MoveDirection.z * LaunchOffset);
			target.DamageInit(target.AnimatorUnit, 41, true, 43, 44);
		}
		// それ以外は多段ヒットしない程度に飛ばす
		else
		{
			// ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
			if (!target.GetIsArmor())
			{
				target.DamageInit(target.AnimatorUnit, 41, true, 43, 44);
			}
			// アーマーなら以降の処理が関係ないのでオブジェクトを自壊させてサヨウナラ           
		}		
	}
	// 覚醒時のダメージ補正を行う
	private void DamageCorrection()
	{
		// 攻撃側が覚醒中の場合
		if (Obj_OR.GetComponent<CharacterControlBase>().IsArousal)
		{
			OffemsivePower = (int)(OffemsivePower * MadokaDefine.AROUSAL_OFFENCE_UPPER);
		}
		// 防御側が覚醒中の場合
		if (HitTarget.GetComponent<CharacterControlBase>().IsArousal)
		{
			OffemsivePower = (int)(OffemsivePower * MadokaDefine.AROUSAL_DEFFENSIVE_UPPER);
		}
	}
}
