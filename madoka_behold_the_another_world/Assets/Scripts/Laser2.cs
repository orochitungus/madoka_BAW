using UnityEngine;
using System.Collections;

public class Laser2 : MonoBehaviour 
{    
    // 射程限界距離
    public int mRange;
    // groundオブジェクトへの接触の有無
    bool mHitGround;
    // レイキャストの発射起点
    public GameObject[] laystarter = new GameObject[4];
    // エフェクト
    public GameObject mLaserEffect;

    // レーザー本体拡大率
    float LaserEnlargementFactor = 5.0f;

    // エフェクト拡大率
    float EffectEnlargementFactor = 0.3f;

    // エフェクトオフセット値
    float EffectOffset = 2.5f;

    // 射出元のゲームオブジェクト
    public GameObject m_Obj_OR;

    // 撃ったキャラのCharacterSpecのインデックス
    public int m_CharacterIndex;

    // 攻撃力
    public int m_OffemsivePower;

    // ダウン値
    public float m_DownRatio;

    // 覚醒ゲージ増加量
    public float m_ArousalRatio;

    // 着弾時のSE
    public AudioClip m_Insp_HitSE;

    // 接触したゲームオブジェクト
    private GameObject m_HitTarget;

    // ヒットタイプ(ダメージの種類.インスペクターで設定すること）
    public CharacterSkill.HitType m_Hittype;

    // 被弾時にダウン値を超えた時の吹き飛び方向
    private Vector3 m_MoveDirection;

    // 地面にヒットしたかというフラグ
    protected bool m_groundhit;
    
    // ヒットした地面の場所
    protected Vector3 m_hitPosition;
    
	// Use this for initialization
	void Start () 
    {
        	
	}
	
	// Update is called once per frame
	void Update () 
    {
        LaserUpdate();	    
	}

    protected void LaserUpdate()
    {
        // Laserの上下左右からレイキャストする
        int layermask = 1 << 8;   //groundに引っかける
        RaycastHit RH = new RaycastHit();
        // 中央の接触判定
        if (Physics.Raycast(transform.position, transform.forward, out RH, mRange, layermask))
        {
            m_hitPosition = RH.point;
            m_groundhit = true;
        }
        // 上下左右の接触判定
        for (int i = 0; i < laystarter.Length; ++i)
        {
            //Debug.DrawLine(laystarter[i].transform.position, new Vector3(laystarter[i].transform.position.x, laystarter[i].transform.position.y, laystarter[i].transform.position.z + mRange), Color.red); 
            if (Physics.Raycast(laystarter[i].transform.position, transform.forward, out RH, mRange, layermask))
            {
                m_hitPosition = RH.point;
                m_groundhit = true;
                break;
            }
        }
        // groundと接触しなければ、mRangeまで拡大する
        if (!m_groundhit)
        {
            // レーザー本体をリサイズさせる
            // 元サイズ(X,Yは変化させず）
            Vector3 scale = this.transform.localScale;
            // リサイズ後のサイズ
            float resizeObject = LaserEnlargementFactor * mRange;
            // リサイズする
            this.transform.localScale = new Vector3(scale.x, scale.y, resizeObject);

            // エフェクトをリサイズさせる
            float resizeEffect = EffectEnlargementFactor * mRange + EffectOffset;
            mLaserEffect.GetComponent<ParticleSystemRenderer>().lengthScale = resizeEffect;
        }
        // groundにヒットした場合
        else
        {
            // オブジェクトをリサイズする
            // 元サイズ(X,Yは変化させず）
            Vector3 scale = this.transform.localScale;
            // リサイズ後のサイズ
            float resizeObject = LaserEnlargementFactor * RH.distance;
            // リサイズする
            this.transform.localScale = new Vector3(scale.x, scale.y, resizeObject);

            // エフェクトをリサイズする
            float resizeEffect = EffectEnlargementFactor * RH.distance + EffectOffset;
            mLaserEffect.GetComponent<ParticleSystemRenderer>().lengthScale = resizeEffect;
        }
    }


    // character接触時に相手にダメージを与える
    void OnTriggerStay(Collider collision)
    {
        string player;
        string enemy;

        // 親オブジェクトを拾う
        // 自機がPLAYERかPLAYER_ALLYの場合
        if (m_Obj_OR.GetComponent<CharacterControl_Base>().m_isPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
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
        // 接触対象がCharacterControl_Baseを持っているか否か判定する
        var target = collision.gameObject.GetComponent<CharacterControl_Base>();
        // 接触対象を取得する
        m_HitTarget = collision.gameObject;

        // 持っていなかったら強制抜け
        if (target == null)
            return;

        // ダウン中かダウン値MAXならダメージを与えない
        if (target.m_AnimState[0] == CharacterControl_Base.AnimationState.Down || (target.m_DownRatio <= target.m_nowDownRatio))
        {
            return;
        }
        else
        {
            Debug.Log("Hit");
            HitDamage(player, enemy, collision);
        }
        // のけぞりか吹き飛ばしを行う
        // ヒット時にダメージの種類をCharacterControl_Baseに与える
        // ダウン値を超えていたら吹き飛びへ移行
        // Blow属性の攻撃を与えた場合も吹き飛びへ移行
        if (target.m_nowDownRatio >= target.m_DownRatio || this.m_Hittype == CharacterSkill.HitType.BLOW)
        {   // 吹き飛びの場合、相手に方向ベクトルを与える            
            // Y軸方向は少し上向き
            target.m_MoveDirection.y += 10;
            // 吹き飛び方向を計算する
            Vector3 blowDirection_OR = transform.position - m_Obj_OR.transform.position;
            m_MoveDirection = Vector3.Normalize(blowDirection_OR);
            target.m_BlowDirection = this.m_MoveDirection;
            // 吹き飛びの場合、攻撃を当てた相手を浮かす（MadokaDefine.LAUNCHOFFSET)            
            target.rigidbody.position = target.rigidbody.position + new Vector3(0, MadokaDefine.LAUNCHOFFSET, 0);
            target.rigidbody.AddForce(this.m_MoveDirection.x * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.y * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.z * MadokaDefine.LAUNCHOFFSET);
            //rigidbody.position = rigidbody.position + ;
            target.m_AnimState[0] = CharacterControl_Base.AnimationState.BlowInit;
        }
        // それ以外はのけぞりへ
        else
        {
            // ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
            if (!target.m_IsArmor)
            {
                target.m_AnimState[0] = CharacterControl_Base.AnimationState.DamageInit;
            }                           
        }
    }

    // ダメージを有効化する
    // 第1引数：敵か味方か
    // 第2引数：敵か味方か
    // 第3引数：コリジョン
    private void HitDamage(string player, string enemy, Collider collision)
    {
        // 敵に触れた場合
        if (collision.gameObject.tag == enemy)
        {
            // 覚醒補正
            DamageCorrection();
            // ダメージ
            collision.gameObject.GetComponent<CharacterControl_Base>().DamageHP(m_CharacterIndex, m_OffemsivePower);
        }
        // 味方に触れた場合
        else if (collision.gameObject.tag == player)
        {
            // 覚醒補正
            DamageCorrection();
            // ダメージ
            collision.gameObject.GetComponent<CharacterControl_Base>().DamageHP(m_CharacterIndex, (int)((float)m_OffemsivePower / MadokaDefine.FRENDLYFIRE_RATIO));
        }
        // ダウン値加算
        collision.gameObject.SendMessage("DownRateInc", m_DownRatio);
        // 覚醒ゲージ加算（覚醒時除く）
        if (m_Obj_OR.GetComponent<CharacterControl_Base>().m_isArousal == false)
        {
            // 攻撃を当てた側が味方側の場合
            if (m_Obj_OR.GetComponent<CharacterControl_Base>().m_isPlayer != CharacterControl_Base.CHARACTERCODE.ENEMY)
            {
                float nextArousal = m_ArousalRatio + savingparameter.GetNowArousal(m_CharacterIndex);
                savingparameter.AddArousal(m_CharacterIndex, nextArousal);
            }
            // 攻撃を当てた側が敵側の場合
            else
            {
                m_Obj_OR.GetComponent<CharacterControl_Base>().AddArousal(m_ArousalRatio);
            }

        }
        // 食らった相手に覚醒ゲージを加算
        collision.gameObject.SendMessage("DamageArousal", m_ArousalRatio);
    }

    // 覚醒時のダメージ補正を行う
    private void DamageCorrection()
    {
        // 攻撃側が覚醒中の場合
        if (m_Obj_OR != null)
        {
            bool injection = m_Obj_OR.GetComponent<CharacterControl_Base>().m_isArousal;
            if (injection)
            {
                m_OffemsivePower = (int)(m_OffemsivePower * MadokaDefine.AROUSAL_OFFENCE_UPPER);
            }
        }
        // 防御側が覚醒中の場合
        if (m_HitTarget != null)
        {
            bool target = m_HitTarget.GetComponent<CharacterControl_Base>().m_isArousal;
            if (target)
            {
                m_OffemsivePower = (int)(m_OffemsivePower * MadokaDefine.AROUSAL_DEFFENSIVE_UPPER);
            }
        }
    }
}
