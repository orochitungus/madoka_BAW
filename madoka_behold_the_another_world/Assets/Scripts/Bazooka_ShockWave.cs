using UnityEngine;
using System.Collections;

public class Bazooka_ShockWave : MonoBehaviour 
{
    //・Bazooka_Effectをインポートしたオブジェクトを射出する
    //ランダムでBazooka_Effectを放射する
    //総回数/出現間隔はインスペクターで

    // Bazooka_Effect
    public GameObject m_Insp_Bazooka_Effect;

    // Bazooka_Effect総出現回数
    public int m_Insp_MaxAppearNum;
    // Bazooka_Effect出現間隔
    public float m_Insp_IntervalAppear;
    // ショックウェーブの維持時間
    public float m_Insp_Lifetime;

    // Bazooka_Effect累積出現回数
    private int m_appearNum = 0;

    // 1回前のBazooka_Effectの出現時間
    private float m_beforAppearTime = 0;
    // 前に出現した時間からの時間
    private float m_accumrationTime = 0;

    // 生成したBazooka_Effectが与えるダウン値
    private float m_downratio;
    public void SetDownratio(float downratio)
    {
        m_downratio = downratio;
    }

    // 生成したBazooka_Effectが与えるダメージ
    private int m_damage;
    public void SetDamage(int damage)
    {
        m_damage = damage;
    }
   
    // 生成したBazooka_Effectが与える覚醒ゲージ
    private float m_arousal;
    public void SetArousal(float arousal)
    {
        m_arousal = arousal;
    }

    // 発動したキャラ
    private int m_CharacterIndex;
    public void SetCharacter(int index)
    {
        m_CharacterIndex = index;
    }

	// Use this for initialization
	void Awake () 
    {
        m_accumrationTime = 0;
        m_beforAppearTime = 0;
        m_appearNum = 0;
	}

    // ほむらの侵食する黒き翼の場合、ダメージを自分には与えない
    public bool m_HomuraWing;
    // ほむらの侵食する黒き翼の時、プレイヤーか否か
    public bool m_IsPlayer;
	
	// Update is called once per frame
	void Update () 
    {
        // インターバル時間ごとにBazooka_Effectを生成する
        if (m_appearNum < m_Insp_MaxAppearNum && m_accumrationTime - m_beforAppearTime >= m_Insp_IntervalAppear)
        {
            // このオブジェクトの子オブジェクトとしてBazooka_Effect作成（複数種類同時に発動した場合、発生保証がなくなる。下の行は要修正
            var bazooka_Effect = (GameObject)Instantiate(m_Insp_Bazooka_Effect, transform.position, transform.rotation);
            // ほむらの侵食する黒き翼の場合
            if (m_HomuraWing)
            {
                bazooka_Effect.GetComponent<Bazooka_Effect>().HomuraWingSet();
            }
            // このオブジェクトを親に設定
            bazooka_Effect.transform.parent = this.transform;
            // ステートを取得
            var Bazooka_State = bazooka_Effect.GetComponentInChildren<Bazooka_Effect>();
            // キャラクター・ダメージ・ダウン値・射出方向・覚醒ゲージ・スタート位置を決定
            Bazooka_State.SetCharacter(m_CharacterIndex);
            Bazooka_State.SetDamage(m_damage);
            Bazooka_State.SetDownRatio(m_downratio);
            Bazooka_State.SetMoveDirection(new Vector3(Random.Range(0, 359)-180, Random.Range(0, 359)-180, Random.Range(0, 359)-180));
            Bazooka_State.SetArousal(m_arousal);
            Bazooka_State.SetStartPoint(transform.position);
            Bazooka_State.m_Isplayer = m_IsPlayer;
            // 出現時間保持
            m_beforAppearTime = m_accumrationTime;
            // 出現回数加算
            m_appearNum++;
            // 生成したBazooka_Effectオブジェクトを切り離す
            bazooka_Effect.transform.parent = null;
        }
        // 累積時間加算
        m_accumrationTime += Time.deltaTime;
        // 一定時間後自壊させる
        if (m_accumrationTime > m_Insp_Lifetime)
        {
            Destroy(gameObject);
        }
	}
}
