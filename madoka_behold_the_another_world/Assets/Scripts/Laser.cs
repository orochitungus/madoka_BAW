using UnityEngine;
using System.Collections;

// レーザータイプの弾丸
public class Laser : MonoBehaviour 
{

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
	
    // 接触時に相手にダメージを与える
    void OnParticleCollision(GameObject collision)
    {
        string player;
        string enemy;

        // 親オブジェクトを拾う
        // 自機がPLAYERかPLAYER_ALLYの場合
        if (m_Obj_OR.GetComponent<CharacterControlBase>().IsPlayer != CharacterControlBase.CHARACTERCODE.ENEMY)
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
        var target = collision.gameObject.GetComponent<CharacterControlBase>();
        // 接触対象を取得する
        m_HitTarget = collision.gameObject;

        // 持っていなかったら強制抜け（自壊はパーティクルの設定でやってくれる）
        if (target == null)
            return;

        // ダウン中かダウン値MAXならダメージを与えない
        if (target.GetDownTime() > 0 || (target.GetDownRatioBias() <= target.GetNowDownRatio()))
        {
            return;
        }
        else
        {
            HitDamage(player, enemy, collision);
        }

        // のけぞりか吹き飛ばしを行う
        // ヒット時にダメージの種類をCharacterControl_Baseに与える
        // ダウン値を超えていたら吹き飛びへ移行
        // Blow属性の攻撃を与えた場合も吹き飛びへ移行
        if (target.GetNowDownRatio() >= target.GetDownRatioBias() || this.m_Hittype == CharacterSkill.HitType.BLOW)
        {   // 吹き飛びの場合、相手に方向ベクトルを与える            
            // Y軸方向は少し上向き
            target.MoveDirection.y += 10;
            // 吹き飛び方向を計算する
            Vector3 blowDirection_OR = transform.position - m_Obj_OR.transform.position;
            m_MoveDirection = Vector3.Normalize(blowDirection_OR);
            target.BlowDirection = this.m_MoveDirection;
            // 吹き飛びの場合、攻撃を当てた相手を浮かす（MadokaDefine.LAUNCHOFFSET)            
            target.GetComponent<Rigidbody>().position = target.GetComponent<Rigidbody>().position + new Vector3(0, MadokaDefine.LAUNCHOFFSET, 0);
            target.GetComponent<Rigidbody>().AddForce(this.m_MoveDirection.x * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.y * MadokaDefine.LAUNCHOFFSET, this.m_MoveDirection.z * MadokaDefine.LAUNCHOFFSET);
            //rigidbody.position = rigidbody.position + ;
			// TODO：吹き飛びに移行する
            //target.m_AnimState[0] = CharacterControl_Base.AnimationState.BlowInit;
        }
        // それ以外はのけぞりへ
        else
        {
            // ただしアーマー時ならダウン値とダメージだけ加算する(Damageにしない）
            if (!target.GetIsArmor())
            {
                //target.m_AnimState[0] = CharacterControl_Base.AnimationState.DamageInit;
            }
            // アーマーなら以降の処理が関係ないのでオブジェクトを自壊させてサヨウナラ                
        }
    }

    // ダメージを有効化する
    // 第1引数：敵か味方か
    // 第2引数：敵か味方か
    // 第3引数：コリジョン
    private void HitDamage(string player, string enemy, GameObject collision)
    {
        // 敵に触れた場合
        if (collision.gameObject.tag == enemy)
        {
            // 覚醒補正
            DamageCorrection();
            // ダメージ
            collision.gameObject.GetComponent<CharacterControlBase>().DamageHP(m_CharacterIndex, m_OffemsivePower);
        }
        // 味方に触れた場合
        else if (collision.gameObject.tag == player)
        {
            // 覚醒補正
            DamageCorrection();
            // ダメージ
            collision.gameObject.GetComponent<CharacterControlBase>().DamageHP(m_CharacterIndex, (int)((float)m_OffemsivePower / MadokaDefine.FRENDLYFIRE_RATIO));
        }
        // ダウン値加算
        collision.gameObject.SendMessage("DownRateInc", m_DownRatio);
        // 覚醒ゲージ加算（覚醒時除く）
        if (m_Obj_OR.GetComponent<CharacterControlBase>().IsArousal == false)
        {
            // 攻撃を当てた側が味方側の場合
            if (m_Obj_OR.GetComponent<CharacterControlBase>().IsPlayer != CharacterControlBase.CHARACTERCODE.ENEMY)
            {
                savingparameter.AddArousal(m_CharacterIndex, m_ArousalRatio);
				m_Obj_OR.GetComponent<CharacterControlBase>().AddArousal(m_ArousalRatio);
			}
            // 攻撃を当てた側が敵側の場合
            else
            {
                m_Obj_OR.GetComponent<CharacterControlBase>().AddArousal(m_ArousalRatio);
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
            bool injection = m_Obj_OR.GetComponent<CharacterControlBase>().IsArousal;
            if (injection)
            {
                m_OffemsivePower = (int)(m_OffemsivePower * MadokaDefine.AROUSAL_OFFENCE_UPPER);
            }
        }
        // 防御側が覚醒中の場合
        if (m_HitTarget != null)
        {
            bool target = m_HitTarget.GetComponent<CharacterControlBase>().IsArousal;
            if (target)
            {
                m_OffemsivePower = (int)(m_OffemsivePower * MadokaDefine.AROUSAL_DEFFENSIVE_UPPER);
            }
        }
    }
}
