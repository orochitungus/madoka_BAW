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
    protected int m_offemsivePower;

    // ダウン値
    protected float m_downRatio;

    // 覚醒ゲージ増加量
    protected float m_arousalRatio;

    // 動作元のゲームオブジェクト
    protected GameObject m_Obj_OR;

    // ヒットタイプ
    protected CharacterSkill.HitType m_Hittype;

    // 吹き飛びになったときの打ち上げ量
    protected float m_launchOffset;

    // 吹き飛びになったときの打ち上げる力
    private float m_launchforce;

    // ヒット時のSE
    public AudioClip m_Insp_HitSE;

    // 接触したゲームオブジェクト
    private GameObject m_HitTarget;
    
	// 出現時の初期化.攻撃力やダウン値の設定はSetStatusでPCから呼ぶ
    // Startに書くとSetStatus（Awakeの直後？）より後に実行される
	void Awake () 
    {
	    // 各ステータスを初期化
        m_offemsivePower = 0;
        m_downRatio = 0;
        m_arousalRatio = 0;
        m_Obj_OR = null;
        m_Hittype = CharacterSkill.HitType.BEND_BACKWARD;
        m_launchOffset = 0.0f;       
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
        m_Obj_OR = transform.root.GetComponentInChildren<CharacterControl_Base>().gameObject;
        // 自機をダメージ対象から除外する
        Physics.IgnoreCollision(this.transform.collider, m_Obj_OR.transform.collider); 
        // 各ステートを設定
        m_offemsivePower = offensive;
        m_downRatio = downR;
        m_arousalRatio = arousal;
        m_Hittype = hittype;
        m_launchOffset = launch;
        m_launchforce = force;
    }
	
	// Update is called once per frame
	void Update () 
    {
	   
	}
    // ヒット処理
    // 当たったらその相手にダメージを与える.破壊は親元で行う
    public void OnCollisionEnter(Collision collision)
    {
        string player;
        string enemy;
        // ヒットSEを鳴らす
        if (m_Insp_HitSE != null)
        {
            AudioSource.PlayClipAtPoint(m_Insp_HitSE, transform.position);
        }
        // 着弾した位置にヒットエフェクトを置く
        UnityEngine.Object HitEffect = null;
        HitEffect = Resources.Load("DamageEffect");
        Instantiate(HitEffect, transform.position, transform.rotation);

        // ガードされた場合は強制抜け（ガードオブジェクトはCharacterContorol_Baseを継承しない）
        if (m_Obj_OR == null)
        {
            return;
        }

        // 親オブジェクトを拾う
        var master = m_Obj_OR.GetComponent<CharacterControl_Base>();
       
        // 自機がPLAYERかPLAYER_ALLYの場合
        if (master.m_isPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
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
        var target = collision.gameObject.GetComponent<CharacterControl_Base>();
        m_HitTarget = collision.gameObject;

        // targetがCharacterControl_Baseクラスでなければ強制抜け
        if (target == null)
        {
            return;
        }

        // ダウン中かダウン値MAXならダメージを与えない
        if (target.m_AnimState[0] == CharacterControl_Base.AnimationState.Down || (target.m_DownRatio <= target.m_nowDownRatio))
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
                int CharacterIndex = (int)(m_Obj_OR.GetComponent<CharacterControl_Base>().m_character_name);
                // ダメージ
                collision.gameObject.GetComponent<CharacterControl_Base>().DamageHP(CharacterIndex, m_offemsivePower);
            }
            // 味方に触れた場合
            else if (collision.gameObject.tag == player)
            {
                // 覚醒時ダメージ補正
                DamageCorrection();
                // 攻撃したキャラクター
                int AttackedCharacter = (int)(m_Obj_OR.GetComponent<CharacterControl_Base>().m_character_name);
                // ダメージ量
                int AttackedDamage = (int)((float)m_offemsivePower / MadokaDefine.FRENDLYFIRE_RATIO);
                //var arr = new int[AttackedCharacter, AttackedDamage];
                // ダメージ
                //collision.gameObject.SendMessage("DamageHP", m_offemsivePower / 4.0f);
                //collision.gameObject.SendMessage("DamageHP", arr);
                collision.gameObject.GetComponent<CharacterControl_Base>().DamageHP(AttackedCharacter, AttackedDamage);
            }
            // ダウン値加算
            collision.gameObject.SendMessage("DownRateInc", m_downRatio);
            // 殴られた相手への覚醒ゲージ加算
            collision.gameObject.SendMessage("DamageArousal", m_arousalRatio);
        }

        // 本体の覚醒ゲージを増やす(覚醒時除く）
        if (master.m_isArousal == false)
        {
            // プレイヤーの場合
            if (master.GetComponent<CharacterControl_Base>().m_isPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
            {
                savingparameter.AddArousal((int)master.m_character_name, m_arousalRatio);
            }
            // 敵の場合
            else
            {
                master.m_Arousal += m_arousalRatio;
            }
        }
        
        // ヒット時にダメージの種類をCharacterControl_Baseに与える
        // ダウン値を超えていたら吹き飛びへ移行
        // Blow属性の攻撃を与えた場合も吹き飛びへ移行
        if (target.m_nowDownRatio >= target.m_DownRatio || this.m_Hittype == CharacterSkill.HitType.BLOW)
        {
            // 吹き飛びの場合、相手に方向ベクトルを与える            
            // Y軸方向は少し上向き
            target.m_MoveDirection.y += 5;

            target.m_BlowDirection = m_Obj_OR.GetComponent<CharacterControl_Base>().m_MoveDirection;
            // 吹き飛びの場合、攻撃を当てた相手を浮かす（m_launchOffset)            
            target.rigidbody.position = target.rigidbody.position + new Vector3(m_launchforce, this.m_launchOffset, m_launchforce);
            target.rigidbody.AddForce(master.m_MoveDirection.x * m_launchOffset, master.m_MoveDirection.y * m_launchOffset, master.m_MoveDirection.z * m_launchOffset);
            target.m_AnimState[0] = CharacterControl_Base.AnimationState.BlowInit;
        }
        // それ以外は多段ヒットしない程度に飛ばす
        else
        {
            // ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
            if (!target.m_IsArmor)
            {
                target.m_AnimState[0] = CharacterControl_Base.AnimationState.DamageInit;
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
        if (m_Obj_OR.GetComponent<CharacterControl_Base>().m_isArousal)
        {
            m_offemsivePower = (int)(m_offemsivePower * MadokaDefine.AROUSAL_OFFENCE_UPPER);
        }
        // 防御側が覚醒中の場合
        if (m_HitTarget.GetComponent<CharacterControl_Base>().m_isArousal)
        {
            m_offemsivePower = (int)(m_offemsivePower * MadokaDefine.AROUSAL_DEFFENSIVE_UPPER);
        }
    }
}
