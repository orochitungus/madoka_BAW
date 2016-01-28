using UnityEngine;
using System.Collections;


// スコノシュートの覚醒技のパーティクルが何かに接触したときにショックウェーブを作る
public class Scono_Arousal_ShockWave : MonoBehaviour 
{
    // 射出元のゲームオブジェクト
    public GameObject m_Obj_OR;

    // 撃ったキャラのCharacterSpecのインデックス
    public int m_CharacterIndex;

    // 攻撃力
    public int m_OffemsivePower;

    // ダウン値
    public float m_DownRatio;

    // 着弾時のSE
    public AudioClip m_Insp_HitSE;

    // 接触したゲームオブジェクト
    private GameObject m_HitTarget;

    // ヒットタイプ
    public CharacterSkill.HitType m_Hittype;

    // 被弾時のゲージ増加量
    public float m_arousalincrease;

	// Use this for initialization
	void Start () 
    {
        // 親オブジェクトを取得(rootでツリーの一番上から検索をかける）        
        m_Obj_OR = transform.root.GetComponentInChildren<Scono_Battle_Control>().gameObject;
        // 撃ったキャラが誰であるか保持
        m_CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
        // 被弾時の挙動を設定
        m_Hittype = CharacterSkill.HitType.BLOW;
        // 攻撃力レベルを設定
        int AttackLv = m_Obj_OR.GetComponent<CharacterControl_Base>().m_StrLevel;
        // ダメージを設定
        m_OffemsivePower = 310 + 5 * (AttackLv - 1); 
        // ダウン値を設定
        m_DownRatio = 5;
        // ヒットタイプを設定
        m_Hittype = CharacterSkill.HitType.BLOW;
        // 被弾時のゲージ増加量を設定
        m_arousalincrease = 100.0f;
	}

    	
	// 接触時に爆破エフェクトを作る
    void OnParticleCollision(GameObject collision)
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

        // SE再生
        AudioSource.PlayClipAtPoint(m_Insp_HitSE, transform.position);

        // targetがnullなら強制抜け
        if(target == null)
        {
            return;
        }
		// そうでないならcollisionが接触対象とみなす
		else 
		{
			m_HitTarget = collision; 
		}

        // ダウン中かダウン値MAXならダメージを与えない
        if (target.m_AnimState[0] == CharacterControl_Base.AnimationState.Down || (target.m_DownRatio <= target.m_nowDownRatio))
        {           
            return;
        }

        // 被弾側の覚醒ゲージを増やす
        target.DamageArousal(m_arousalincrease);
                 
        // ダメージ
        // 敵に触れた場合
        if (collision.gameObject.tag == enemy)
        {
            // 覚醒補正
            DamageCorrection();
            //target.DamageHP(m_OffemsivePower);
            target.DamageHP((int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO,m_OffemsivePower);
        }
        // 味方に触れた場合
        else if (collision.gameObject.tag == player)
        {
            // 覚醒補正
            DamageCorrection();
            //target.DamageHP(m_OffemsivePower);
            target.DamageHP((int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO,(int)((float)m_OffemsivePower/ MadokaDefine.FRENDLYFIRE_RATIO));
        }
        // ダウン値
        target.DownRateInc(m_DownRatio);

        // 吹き飛び方向のベクトルを作る
        Vector3 MoveDirection = Vector3.Normalize(target.transform.position - transform.position);

        // 食らった相手を吹き飛ばす
        // 接触した相手を動作させる
        if (target.m_nowDownRatio >= target.m_DownRatio || this.m_Hittype == CharacterSkill.HitType.BLOW)
        {   // 吹き飛びの場合、相手に方向ベクトルを与える            
            // Y軸方向は少し上向き
            target.m_MoveDirection.y += 10;
            target.m_BlowDirection = MoveDirection;
            // 吹き飛びの場合、攻撃を当てた相手を浮かす（m_launchOffset)            
            target.GetComponent<Rigidbody>().position = target.GetComponent<Rigidbody>().position + new Vector3(0, MadokaDefine.LAUNCHOFFSET, 0);
            target.GetComponent<Rigidbody>().AddForce(MoveDirection.x * MadokaDefine.LAUNCHOFFSET, MoveDirection.y * MadokaDefine.LAUNCHOFFSET, MoveDirection.z * MadokaDefine.LAUNCHOFFSET);
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
            // アーマーなら以降の処理が関係ないのでオブジェクトを自壊させてサヨウナラ           
        }
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
