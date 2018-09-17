using UnityEngine;
using System.Collections;

// スコノシュートの特殊射撃
public class Scono_Exshot : Laser2 
{    
    void Start()
    {
        // 親オブジェクトを取得(rootでツリーの一番上から検索をかける）        
        m_Obj_OR = transform.root.GetComponentInChildren<SconosciutoControl>().gameObject;
		
        // 撃ったキャラが誰であるか保持
        m_CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
		// 被弾時の挙動を設定
		m_Hittype = ParameterManager.Instance.GetHitType(m_CharacterIndex, 2); 
        // 攻撃力レベルを取得
        int StrLevel = m_Obj_OR.GetComponent<CharacterControlBase>().StrLevel;
		// 攻撃力を決定
		m_OffemsivePower = ParameterManager.Instance.Characterskilldata.sheets[m_CharacterIndex].list[2].OriginalStr + ParameterManager.Instance.Characterskilldata.sheets[m_CharacterIndex].list[2].GrowthCoefficientStr; 
		// ダウン値を決定
		m_DownRatio = ParameterManager.Instance.Characterskilldata.sheets[m_CharacterIndex].list[2].DownPoint;
        // 覚醒ゲージ増加量を決定
        m_ArousalRatio = ParameterManager.Instance.Characterskilldata.sheets[m_CharacterIndex].list[2].Arousal;
		// 対ブースト攻撃力を決定
		AntiBoostOffensivePower = ParameterManager.Instance.Characterskilldata.sheets[m_CharacterIndex].list[2].AntiBoostStr;

	}

    // ビームを消去する
    public void DestroyBeam()
    {
        Destroy(gameObject);
    }
}
