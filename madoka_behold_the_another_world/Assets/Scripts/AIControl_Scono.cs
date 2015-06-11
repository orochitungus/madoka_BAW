using UnityEngine;
using System.Collections;


// CPUによるキャラクター操作を行う(スコノシュート専用）
public class AIControl_Scono : AIControl_Base 
{
    // Use this for initialization
	void Start () 
    {
        Initialize();
        // 上昇する時間
        m_risetime = 0.1f;
        // 近接レンジ
        m_fightRange = 2.0f;
        // 近接レンジ（前後特殊格闘を使って上下するか）
        m_fightRangeY = 2.5f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdateCore();
	}

    // ノーマル（ゲージがあれば覚醒する）
    protected override void normal(ref AIControl_Base.TENKEY_OUTPUT tenkeyoutput, ref AIControl_Base.KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // 一端ロックオンボタンを離す
        keyoutput = KEY_OUTPUT.NONE;

        // 覚醒ゲージが溜まっていて覚醒していなかったら覚醒する
        if (target.m_Arousal >= target.m_Arousal_OR && !target.m_isArousal)
        {
            keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.AROUSAL;
            return;
        }
               
        // 地上にいてダウンしていなくブーストゲージがあった場合、飛行させる（着地硬直中などは飛べない）
        if (target.GetInGround() && target.m_AnimState[0] != CharacterControl_Base.AnimationState.Down && target.m_AnimState[0] != CharacterControl_Base.AnimationState.Reversal
            && target.m_Boost > 0)
        {
            keyoutput = KEY_OUTPUT.JUMP;
            m_cpumode = CPUMODE.NORMAL_RISE1;
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            m_totalrisetime = Time.time;
            return;
        }
        // ロックオン状態で赤ロックになったら戦闘開始
        if (engauge(ref keyoutput))
        {
            return;
        }
    }



    protected override bool engauge(ref AIControl_Base.KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // ロックオン距離内にいる
        if (RockonTarget != null && Vector3.Distance(target.transform.position, RockonTarget.transform.position) <= target.m_Rockon_Range)
        {
            // 頭上をとったか取られたかした場合、前後特殊格闘（DOGFIGHT_AIR）を行ってもらう
            // 自機のXZ座標
            Vector2 nowposXZ = new Vector2(target.transform.position.x, target.transform.position.z);
            // ロックオン対象のXZ座標
            Vector2 RockonXZ = new Vector2(RockonTarget.transform.position.x, RockonTarget.transform.position.z);
            // XZ距離算出
            float DistanceXZ = Vector2.Distance(nowposXZ, RockonXZ);
            // 高低差算出
            float DistanceY = target.transform.position.y - RockonTarget.transform.position.y;
            // 格闘レンジに入ったか
            if (DistanceXZ <= m_fightRange)
            {
                keyoutput = KEY_OUTPUT.NONE;
                // 高低差が一定以上か
                if (m_fightRangeY <= DistanceY)
                {
                    // 自分が高ければ後特殊格闘(降下攻撃）
                    if (target.transform.position.y >= RockonTarget.transform.position.y)
                    {
                        m_cpumode = CPUMODE.DOGFIGHT_DOWNER;
                        return true;
                    }
                    // 自分が低ければ前特殊格闘（上昇攻撃）
                    else
                    {
                        m_cpumode = CPUMODE.DOGFIGHT_UPPER;
                        return true;
                    }
                }
                // 高低差がないならば通常格闘へ移行
                else
                {
                    m_cpumode = CPUMODE.DOGFIGHT_STANDBY;
                    return true;
                }
            }
            // そうでなければ空中におり、敵との間に遮蔽物がなければ射撃攻撃（現行、地上にいると射撃のループをしてしまう）
            else if (!target.GetInGround())
            {
                // 前面に向かってレイキャストする
                RaycastHit hit;
                Vector3 RayStartPosition = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z);
                if (Physics.Raycast(RayStartPosition, transform.forward, out hit, 230.0f))
                {
                    // 衝突した「何か」が障害物であったら測距する
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("ground"))
                    {
                        // 障害物との距離が敵との距離より近ければ、falseを返す
                        // 敵との距離
                        float EnemyDistance = Vector3.Distance(target.transform.position, RockonTarget.transform.position);
                        // 障害物との距離
                        float hitDistance = Vector3.Distance(target.transform.position, hit.transform.position);
                        if (hitDistance < EnemyDistance)
                        {
                            return false;
                        }
                    }
                }
                // 残弾数がなければなにもしない（NORMALへ戻る）
                var targetState = ControlTarget.GetComponent<Scono_Battle_Control>();
                // 通常射撃・サブ射撃・特殊射撃・覚醒技（覚醒時のみ）のいずれかを行う
                // 乱数を取得
                float attacktype = Random.value;         // 攻撃手段(0-0.2:覚醒技(非覚醒時は何もしない)、0.2-0.6(通常射撃）、0.6-0.8(サブ射撃）、0.8-1.0（特殊射撃）
                if (0.0f <= attacktype && attacktype < 0.2f && target.m_isArousal)
                {
                    m_cpumode = CPUMODE.FIREFIGHT;
                    keyoutput = KEY_OUTPUT.AROUSALATTACK;
                    return true;
                }
                else if (0.2f <= attacktype && attacktype < 0.6f && targetState.m_BulletNum[0] > 0)                
                {
                    m_cpumode = CPUMODE.FIREFIGHT;
                    keyoutput = KEY_OUTPUT.SHOT;
                    return true;
                }
                else if (0.6f <= attacktype && attacktype < 0.75f && targetState.m_BulletNum[1] > 0)
                {
                    m_cpumode = CPUMODE.FIREFIGHT;
                    keyoutput = KEY_OUTPUT.SUBSHOT;
                    return true;
                }
                else if (0.75f <= attacktype && attacktype <= 1.0f && targetState.m_BulletNum[2] > 0)
                {
                    m_cpumode = CPUMODE.FIREFIGHT;
                    keyoutput = KEY_OUTPUT.EXSHOT;
                    return true;
                }
                else
                {
                    m_cpumode = CPUMODE.NORMAL;
                    keyoutput = KEY_OUTPUT.NONE;
                    return false;
                }
            }
        }
        return false;
    }
}
