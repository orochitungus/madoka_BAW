using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコノシュートの覚醒技のレーザー部分
/// </summary>
public class SconosciutoArousalLaser : Laser2
{

	/// <summary>
	/// 着弾時に生成される爆風
	/// </summary>
	public	Bazooka_ShockWave scono_Arousal_ShockWave;
	void Start()
	{
		// TODO:親オブジェクトを取得(rootでツリーの一番上から検索をかける）
		//m_Obj_OR = transform.root.GetComponentInChildren<Scono_Battle_Control>().gameObject;
		// 撃ったキャラが誰であるか保持
		m_CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
		// 被弾時の挙動を決定
		m_Hittype = CharacterSkill.HitType.BLOW;
		// 攻撃力レベルを取得
		int StrLevel = m_Obj_OR.GetComponent<CharacterControlBase>().StrLevel;
		// 攻撃力を決定
		m_OffemsivePower = 310 + 5 * (StrLevel - 1);
		// ダウン値を決定
		m_DownRatio = 0.2f;
		// 覚醒ゲージ増加量を決定
		m_ArousalRatio = 0;
	}

	void Update()
	{
		LaserUpdate();
		CreateShockWave();
	}

	// 地面接触時の処理（爆風を作る）
	void CreateShockWave()
	{
		if (m_groundhit)
		{
			// 爆風を作る
			Instantiate(scono_Arousal_ShockWave, m_hitPosition, Quaternion.Euler(Vector3.zero));
			// ステートを初期化する
			scono_Arousal_ShockWave.SetDamage(m_OffemsivePower);
			// ダウン値を初期化する
			scono_Arousal_ShockWave.SetDownratio(m_DownRatio);
			// 覚醒ゲージ増加量を決定する
			scono_Arousal_ShockWave.SetArousal(0.0f);
		}
	}
}
