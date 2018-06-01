using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterManager : SingletonMonoBehaviour<ParameterManager>
{
	/// <summary>
	/// 各マップのシーンファイル名
	/// </summary>
	public MapSceneName Mapscenename;

	/// <summary>
	/// インフォメーションテキスト
	/// </summary>
	public Entity_Information EntityInformation;

	/// <summary>
	/// 全体マップから各フィールドへ移動するときの情報
	/// </summary>
	public MitakiharaCity_MapData MitakiharacityMapdata;

	/// <summary>
	/// 各ステージに移動したときのキャラクターの配置位置の情報
	/// </summary>
	public StagePositionRotData StagepositionrotData;

	/// <summary>
	/// 各ステージに移動したときにどこから来たか、及びBGMの変更情報
	/// </summary>
	public StageCodeData StagecodeData;

	/// <summary>
	/// 各ステージの環境光の角度情報
	/// </summary>
	public StageSkyData StageskyData;

	/// <summary>
	/// キャラクターの基本パラメータ
	/// </summary>
	public CharacterBasicSpec CharacterbasicSpec;

	/// <summary>
	/// キャラクターの武装に関するデータ
	/// </summary>
	public CharacterSkillData Characterskilldata;

	private void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// 武装の被弾時の挙動を返す
	/// </summary>
	/// <param name="characterindex"></param>
	/// <param name="skillindex"></param>
	public CharacterSkill.HitType GetHitType(int characterindex, int skillindex)
	{
		string hittype = Instance.Characterskilldata.sheets[characterindex].list[skillindex].HitType;

		CharacterSkill.HitType hittype2 = CharacterSkill.HitType.BEND_BACKWARD;
		switch (hittype)
		{
			case "BEND_BACKWARD":			// のけぞる
				hittype2 = CharacterSkill.HitType.BEND_BACKWARD;
				break;
			case "BLOW":					// 吹き飛ばす
				hittype2 = CharacterSkill.HitType.BLOW;
				break;
			case "GUARD":					// 防御
				hittype2 = CharacterSkill.HitType.GUARD;
				break;
			case "RECOVERY":				// 回復
				hittype2 = CharacterSkill.HitType.RECOVERY;
				break;
			case "RESUSCITATION":			// 蘇生込回復
				hittype2 = CharacterSkill.HitType.RESUSCITATION;
				break;
			case "CURE":					// ST異常含め回復
				hittype2 = CharacterSkill.HitType.CURE;
				break;
			case "WEAPON_ZERO":             // 武装ゼロ
				hittype2 = CharacterSkill.HitType.WEAPON_ZERO;
				break;
			case "ADD_ARMOR":               // アーマー付加
				hittype2 = CharacterSkill.HitType.ADD_ARMOR;
				break;
			case "TIME_STOP":               // 時間停止
				hittype2 = CharacterSkill.HitType.TIME_STOP;
				break;
			case "TIME_DELAY":              // 時間遅延
				hittype2 = CharacterSkill.HitType.TIME_DELAY;
				break;
			case "RELOAD":                  // マニュアルリロード
				hittype2 = CharacterSkill.HitType.RELOAD;
				break;
			case "RESTRAINT":               // 拘束
				hittype2 = CharacterSkill.HitType.RESTRAINT;
				break;
			case "MODE_CHANGE":             // モードチェンジ
				hittype2 = CharacterSkill.HitType.MODE_CHANGE;
				break;
			case "AVATAR":		            // 分身生成
				hittype2 = CharacterSkill.HitType.AVATAR;
				break;
			case "BOOST_WATCH":             // ブーストゲージ看破
				hittype2 = CharacterSkill.HitType.BOOST_WATCH;
				break;
		}

		return hittype2;
	}

	/// <summary>
	/// スキルの種類を返す
	/// </summary>
	/// <param name="characterindex"></param>
	/// <param name="skillindex"></param>
	/// <returns></returns>
	public CharacterSkill.SkillType GetSkillType(int characterindex, int skillindex)
	{
		string skilltype = Instance.Characterskilldata.sheets[characterindex].list[skillindex].SkillType;
		CharacterSkill.SkillType skillType2 = CharacterSkill.SkillType.SHOT;

		switch (skilltype)
		{
			case "SHOT":
				skillType2 = CharacterSkill.SkillType.SHOT;
				break;
			case "SHOT2":
				skillType2 = CharacterSkill.SkillType.SHOT2;
				break;
			case "CHARGE_SHOT":
				skillType2 = CharacterSkill.SkillType.CHARGE_SHOT;
				break;
			case "SUB_SHOT":
				skillType2 = CharacterSkill.SkillType.SUB_SHOT;
				break;
			case "SUB_SHOT_2":
				skillType2 = CharacterSkill.SkillType.SUB_SHOT2;
				break;
			case "EX_SHOT":
				skillType2 = CharacterSkill.SkillType.EX_SHOT;
				break;
			case "WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.WRESTLE_1;
				break;
			case "WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.WRESTLE_2;
				break;
			case "WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.WRESTLE_3;
				break;
			case "WRESTLE_4":
				skillType2 = CharacterSkill.SkillType.WRESTLE_4;
				break;
			case "WRESTLE_5":
				skillType2 = CharacterSkill.SkillType.WRESTLE_5;
				break;
			case "CHARGE_WRESTLE":
				skillType2 = CharacterSkill.SkillType.CHARGE_WRESTLE;
				break;
			case "FRONT_WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.FRONT_WRESTLE_1;
				break;
			case "FRONT_WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.FRONT_WRESTLE_2;
				break;
			case "FRONT_WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.FRONT_WRESTLE_3;
				break;
			case "LEFT_WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.LEFT_WRESTLE_1;
				break;
			case "LEFT_WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.LEFT_WRESTLE_2;
				break;
			case "LEFT_WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.LEFT_WRESTLE_3;
				break;
			case "RIGHT_WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.RIGHT_WRESTLE_1;
				break;
			case "RIGHT_WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.RIGHT_WRESTLE_2;
				break;
			case "RIGHT_WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.RIGHT_WRESTLE_3;
				break;
			case "BACK_WRESTLE":
				skillType2 = CharacterSkill.SkillType.BACK_WRESTLE;
				break;
			case "AIRDASH_WRESTLE":
				skillType2 = CharacterSkill.SkillType.AIRDASH_WRESTLE;
				break;
			case "EX_WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.EX_WRESTLE_1;
				break;
			case "EX_WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.EX_WRESTLE_2;
				break;
			case "EX_WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.EX_WRESTLE_3;
				break;
			case "EX_FRONT_WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.EX_FRONT_WRESTLE_1;
				break;
			case "EX_FRONT_WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.EX_FRONT_WRESTLE_2;
				break;
			case "EX_FRONT_WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.EX_FRONT_WRESTLE_3;
				break;
			case "EX_LEFT_WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.EX_LEFT_WRESTLE_1;
				break;
			case "EX_LEFT_WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.EX_LEFT_WRESTLE_2;
				break;
			case "EX_LEFT_WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.EX_LEFT_WRESTLE_3;
				break;
			case "EX_RIGHT_WRESTLE_1":
				skillType2 = CharacterSkill.SkillType.EX_RIGHT_WRESTLE_1;
				break;
			case "EX_RIGHT_WRESTLE_2":
				skillType2 = CharacterSkill.SkillType.EX_RIGHT_WRESTLE_2;
				break;
			case "EX_RIGHT_WRESTLE_3":
				skillType2 = CharacterSkill.SkillType.EX_RIGHT_WRESTLE_3;
				break;
			case "BACK_EX_WRESTLE":
				skillType2 = CharacterSkill.SkillType.BACK_EX_WRESTLE;
				break;
			case "SHOT_M2":
				skillType2 = CharacterSkill.SkillType.SHOT_M2;
				break;
			case "CHARGE_SHOT_M2":
				skillType2 = CharacterSkill.SkillType.CHARGE_SHOT_M2;
				break;
			case "SUB_SHOT_M2":
				skillType2 = CharacterSkill.SkillType.SUB_SHOT_M2;
				break;
			case "EX_SHOT_M2":
				skillType2 = CharacterSkill.SkillType.EX_SHOT_M2;
				break;
			case "WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.WRESTLE_1_M2;
				break;
			case "WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.WRESTLE_2_M2;
				break;
			case "WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.WRESTLE_3_M2;
				break;
			case "CHARGE_WRESTLE_M2":
				skillType2 = CharacterSkill.SkillType.CHARGE_WRESTLE_M2;
				break;
			case "FRONT_WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.FRONT_WRESTLE_1_M2;
				break;
			case "FRONT_WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.FRONT_WRESTLE_2_M2;
				break;
			case "FRONT_WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.FRONT_WRESTLE_3_M2;
				break;
			case "LEFT_WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.LEFT_WRESTLE_1_M2;
				break;
			case "LEFT_WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.LEFT_WRESTLE_2_M2;
				break;
			case "LEFT_WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.LEFT_WRESTLE_3_M2;
				break;
			case "RIGHT_WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.RIGHT_WRESTLE_1_M2;
				break;
			case "RIGHT_WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.RIGHT_WRESTLE_2_M2;
				break;
			case "RIGHT_WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.RIGHT_WRESTLE_3_M2;
				break;
			case "BACK_WRESTLE_M2":
				skillType2 = CharacterSkill.SkillType.BACK_WRESTLE_M2;
				break;
			case "AIRDASH_WRESTLE_M2":
				skillType2 = CharacterSkill.SkillType.AIRDASH_WRESTLE_M2;
				break;
			case "EX_WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.EX_WRESTLE_1_M2;
				break;
			case "EX_WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.EX_WRESTLE_2_M2;
				break;
			case "EX_WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.EX_WRESTLE_3_M2;
				break;
			case "EX_FRONT_WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.EX_FRONT_WRESTLE_1_M2;
				break;
			case "EX_FRONT_WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.EX_FRONT_WRESTLE_2_M2;
				break;
			case "EX_FRONT_WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.EX_FRONT_WRESTLE_3_M2;
				break;
			case "EX_LEFT_WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.EX_LEFT_WRESTLE_1_M2;
				break;
			case "EX_LEFT_WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.EX_LEFT_WRESTLE_2_M2;
				break;
			case "EX_LEFT_WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.EX_LEFT_WRESTLE_3_M2;
				break;
			case "EX_RIGHT_WRESTLE_1_M2":
				skillType2 = CharacterSkill.SkillType.EX_RIGHT_WRESTLE_1_M2;
				break;
			case "EX_RIGHT_WRESTLE_2_M2":
				skillType2 = CharacterSkill.SkillType.EX_RIGHT_WRESTLE_2_M2;
				break;
			case "EX_RIGHT_WRESTLE_3_M2":
				skillType2 = CharacterSkill.SkillType.EX_RIGHT_WRESTLE_3_M2;
				break;
			case "BACK_EX_WRESTLE_M2":
				skillType2 = CharacterSkill.SkillType.BACK_EX_WRESTLE_M2;
				break;
			case "AROUSAL_ATTACK":
				skillType2 = CharacterSkill.SkillType.AROUSAL_ATTACK;
				break;
			case "DISABLE_BLUNT_FOOT":
				skillType2 = CharacterSkill.SkillType.DISABLE_BLUNT_FOOT;
				break;
			case "DISABLE_ROCKON_IMPOSSIBLE":
				skillType2 = CharacterSkill.SkillType.DISABLE_ROCKON_IMPOSSIBLE;
				break;
			case "DISABLE_DESTRUCTION_MAGIC":
				skillType2 = CharacterSkill.SkillType.DISABLE_DESTRUCTION_MAGIC;
				break;
			case "DISABLE_POISON":
				skillType2 = CharacterSkill.SkillType.DISABLE_POISON;
				break;
			case "DISABLE_MENTAL_CONTAMINATION":
				skillType2 = CharacterSkill.SkillType.DISABLE_MENTAL_CONTAMINATION;
				break;
			case "DISABLE_HALLUCINATION":
				skillType2 = CharacterSkill.SkillType.DISABLE_HALLUCINATION;
				break;
			case "DISABLE_MENTALS":
				skillType2 = CharacterSkill.SkillType.DISABLE_MENTALS;
				break;
			case "AROUSAL_EX_WRESTLE":
				skillType2 = CharacterSkill.SkillType.AROUSAL_EX_WRESTLE;
				break;
			case "NONE":
				break;
		}

		return skillType2;
	}

	
}
