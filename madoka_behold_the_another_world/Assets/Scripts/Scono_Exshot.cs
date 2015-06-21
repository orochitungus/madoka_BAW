using UnityEngine;
using System.Collections;

// スコノシュートの特殊射撃
public class Scono_Exshot : Laser2 
{    
    void Start()
    {
        // 親オブジェクトを取得(rootでツリーの一番上から検索をかける）        
        m_Obj_OR = transform.root.GetComponentInChildren<Scono_Battle_Control>().gameObject;
        // 接触対象に自分は除く（散弾や爆風は発生時に別のオブジェクトを呼ぶ）(パーティクルにcolliderは貼れないので本体を離す）
        //Physics.IgnoreCollision(this.transform.collider, m_Obj_OR.transform.collider);
        // 撃ったキャラが誰であるか保持
        m_CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
        // 被弾時の挙動を設定
        m_Hittype = Character_Spec.cs[m_CharacterIndex][2].m_Hittype;
        // 攻撃力レベルを取得
        int StrLevel = m_Obj_OR.GetComponent<CharacterControl_Base>().m_StrLevel;
        // 攻撃力を決定
        m_OffemsivePower = Character_Spec.cs[m_CharacterIndex][2].m_OriginalStr + Character_Spec.cs[m_CharacterIndex][2].m_GrowthCoefficientStr * (StrLevel - 1);
        // ダウン値を決定
        m_DownRatio = Character_Spec.cs[m_CharacterIndex][2].m_DownPoint;
        // 覚醒ゲージ増加量を決定
        m_ArousalRatio = Character_Spec.cs[m_CharacterIndex][2].m_arousal;
    }

    // ビームを消去する
    public void DestroyBeam()
    {
        Destroy(gameObject);
    }
}
