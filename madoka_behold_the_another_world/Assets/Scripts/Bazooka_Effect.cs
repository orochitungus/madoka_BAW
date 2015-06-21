using UnityEngine;
using System.Collections;

public class Bazooka_Effect : MonoBehaviour 
{    

    //float ダウン値
    private float m_downratio;

    public void SetDownRatio(float downratio)
    {
        if (downratio < 0)
            downratio = 0;
        m_downratio = downratio;
    }

    //int   ダメージ
    private int m_damage;
    public void SetDamage(int damage)
    {
        if (damage < 1)
        {
            damage = 1;
        }
        m_damage = damage;

    }
    public int GetDamage()
    {
        return m_damage;
    }

    // 覚醒ゲージ
    private float m_Arousal;
    public void SetArousal(float arousal)
    {
        m_Arousal = arousal;
    }

    // 弓ほむらの「侵食する黒き翼」の場合は自分にはダメージを与えない
    private bool m_HomuraWing;
    public void HomuraWingSet()
    {
        m_HomuraWing = true;
    }

    // 被弾時の挙動
    private CharacterSkill.HitType m_Hittype;
    public void GetHittype(CharacterSkill.HitType hittype)
    {
        m_Hittype = hittype;
    }

    // 移動ベクトル
    private Vector3 m_MoveDirection;
    public void SetMoveDirection(Vector3 movedirection)
    {
        m_MoveDirection = movedirection;
    }
    public Vector3 GetMoveDirection()
    {
        return m_MoveDirection;
    }

    // 起点
    private Vector3 m_StartPoint;
    public void SetStartPoint(Vector3 startpoint)
    {
        m_StartPoint = startpoint;
    }
    
    // 移動限界距離
    public float m_Insp_MaxMove;
       
    // 移動速度
    public float m_Insp_MoveSpeed;

    // ほむらの侵食する黒き翼か
    public bool m_IsHomuraArousal;

    // ほむらの侵食する黒き翼の時、プレイヤーか否か
    public bool m_Isplayer;

    // 開始処理
    void Awake()
    {
        
    }
            
    // 移動処理
    void Update()
    { 
        rigidbody.position = rigidbody.position + m_MoveDirection * m_Insp_MoveSpeed * Time.deltaTime;        
        // 限界距離移動後に自己消滅
        if (Vector3.Distance(rigidbody.position, m_StartPoint) > m_Insp_MaxMove)
        {
            Destroy(gameObject);
        }
    }

    // 攻撃したキャラクター
    private int m_CharacterIndex;
    public void SetCharacter(int index)
    {
        m_CharacterIndex = index;
    }


    // 接触時処理
    public void OnCollisionEnter(Collision collision)
    {          
        // 接触対象を取得
        var target = collision.gameObject.GetComponent<CharacterControl_Base>();

        // targetがCharacterControl_Baseクラスでなければ「自壊させて」強制抜け
        // ほむらの「侵食する黒き翼」の時も自壊抜け
        var targetEx = collision.gameObject.GetComponent<Homura_Final_BattleControl>();

        if (target == null || (targetEx != null && m_HomuraWing))
        {
            // オブジェクトを自壊させる
            Destroy(gameObject);
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
            //ダメージを与える→ダメージとダウン値は弾丸側で与えておく(ほむらの侵食する黒き翼は例外）
            if (m_IsHomuraArousal)
            {
                if (!m_Isplayer && collision.gameObject.tag == "Player")        // 味方に誤射したらダメージを減らす
                {
                    m_damage = (int)(m_damage / MadokaDefine.FRENDLYFIRE_RATIO);
                }
            }
            //collision.gameObject.SendMessage("DamageHP", m_damage);
            //collision.gameObject.SendMessage("DamageHP", arr);
            collision.gameObject.GetComponent<CharacterControl_Base>().DamageHP(m_CharacterIndex,m_damage);
            // 覚醒ゲージを与える
            collision.gameObject.SendMessage("DamageArousal", m_Arousal);
            // ダウン値を与える
            collision.gameObject.SendMessage("DownRateInc", m_downratio);
            // 接触した相手を動作させる
            if (target.m_nowDownRatio >= target.m_DownRatio || this.m_Hittype == CharacterSkill.HitType.BLOW)
            {   // 吹き飛びの場合、相手に方向ベクトルを与える            
                // Y軸方向は少し上向き
                target.m_MoveDirection.y += 10;
                target.m_BlowDirection = this.m_MoveDirection;
                // 吹き飛びの場合、攻撃を当てた相手を浮かす（m_launchOffset)            
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
                // アーマーなら以降の処理が関係ないのでオブジェクトを自壊させてサヨウナラ           
            }
            //自己を消滅させる
            Destroy(gameObject);
        }
    }
}
