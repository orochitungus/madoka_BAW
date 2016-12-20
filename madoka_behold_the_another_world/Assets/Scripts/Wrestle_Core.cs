using UnityEngine;
using System.Collections;


//格闘判定用オブジェクトに乗る（本体→フック→Wrestle_Coreを持ったオブジェクトとなる）
//このクラスを乗せた透明オブジェクトを本体のフックに引っかける
//いわゆる格闘のあたり判定クラス
//接触→対象にダメージを与える（自分は除く）
//   対象を属性に応じてのけぞらせたり飛ばす
//持続の概念はない（親側で壊せるので考えなくてもいい）
public class Wrestle_Core : MonoBehaviour 
{   

    // 攻撃力
    protected int OffemsivePower;

    // ダウン値
    protected float DownRatio;

    // 覚醒ゲージ増加量
    protected float ArousalRatio;

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

	/// <summary>
	/// ガードオブジェクトの場合はこのフラグをＯＮにすること
	/// </summary>
	public bool IsShield;

	// 出現時の初期化.攻撃力やダウン値の設定はSetStatusでPCから呼ぶ
	// Startに書くとSetStatus（Awakeの直後？）より後に実行される
	void Awake () 
    {
	    // 各ステータスを初期化
        OffemsivePower = 0;
        DownRatio = 0;
        ArousalRatio = 0;
        Obj_OR = null;
        Hittype = CharacterSkill.HitType.BEND_BACKWARD;
        LaunchOffset = 0.0f;       
	}

   
    // ステート設定
    // offensive    [in]:攻撃力
    // downR        [in]:ダウン値
    // arousal      [in]:覚醒ゲージ増加量
    // hittype      [in]:ヒットタイプ
    // launch       [in]:打ち上げ量
    // force        [in]:打ち上げ時に加える力
    public virtual void SetStatus(int offensive, float downR, float arousal, CharacterSkill.HitType hittype, float launch = 10.0f, float force = 5.0f)
    {
        // 親のオブジェクトを拾う
        Obj_OR = transform.root.GetComponentInChildren<CharacterControlBase>().gameObject;
        // 自機をダメージ対象から除外する
        Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), Obj_OR.transform.GetComponent<Collider>()); 
        // 各ステートを設定
        OffemsivePower = offensive;
        DownRatio = downR;
        ArousalRatio = arousal;
        Hittype = hittype;
        LaunchOffset = launch;
        Launchforce = force;
    }
	
	// Update is called once per frame
	void Update () 
    {
	   
	}
    // ヒット処理
    // 当たったらその相手にダメージを与える.破壊は親元で行う
    public void OnCollisionEnter(Collision collision)
    {
		// 除外対象がある場合、除外対象に当たったら何もしない
		if(Exclusion != "")
		{
			if(collision.gameObject.name == Exclusion)
			{
				return;
			}
		}
		// 地面に当たった場合は何もしない
		if(collision.gameObject.layer == LayerMask.NameToLayer("ground"))
		{
			return;
		}

		// シールド時は何もしない
		if(IsShield)
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
        if (target.Invincible)
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
            // 殴られた相手への覚醒ゲージ加算
            collision.gameObject.SendMessage("DamageArousal", ArousalRatio);
        }

        // 本体の覚醒ゲージを増やす(覚醒時除く）
        if (master.IsArousal == false)
        {
            // プレイヤーの場合
            if (master.GetComponent<CharacterControlBase>().IsPlayer != CharacterControlBase.CHARACTERCODE.ENEMY)
            {
                savingparameter.AddArousal((int)master.CharacterName, ArousalRatio);
				master.Arousal += ArousalRatio;
            }
            // 敵の場合
            else
            {
                master.Arousal += ArousalRatio;
            }
        }
        
        // ヒット時にダメージの種類をCharacterControl_Baseに与える
        // ダウン値を超えていたら吹き飛びへ移行
        // Blow属性の攻撃を与えた場合も吹き飛びへ移行
        if (target.NowDownRatio >= target.DownRatioBias || this.Hittype == CharacterSkill.HitType.BLOW)
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
            if (!target.IsArmor)
            {
				target.DamageInit(target.AnimatorUnit, 41, true, 43, 44);
			}
            // アーマーなら以降の処理が関係ないのでオブジェクトを自壊させてサヨウナラ           
        }
        // オブジェクトを自壊させる
        // 複数の相手を巻き込みたい場合は以下のようにするとか
        //・何かに触れたら2回目以降はダメージを与えないようにする（連続ヒットを防ぐため。多段ヒット技は重ねておく。IgnoreCollisionを追加で呼ぶ）
        Destroy(gameObject);

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
