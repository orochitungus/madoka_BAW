using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SconosciutoCPU : AIControlBase
{

    // 上昇限界高度
    private float RiseLimit = 75.0f;
       

    // Use this for initialization
    void Start ()
    {
        Initialize();
        // 上昇する時間
        Risetime = 0.1f;
        // 近接レンジ
        FightRange = 2.0f;
        // 近接レンジ（前後特殊格闘を使って上下するか）
        FightRangeY = 2.5f;
        
    }

    // Update is called once per frame
    void Update ()
    {
        if (IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
        {
            UpdateCore();
        }
    }

    /// <summary>
    /// 接触したときの行動
    /// </summary>
    /// <param name="keyoutput"></param>
    /// <returns></returns>
    protected override bool Engauge(ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        // ロックオン距離内にいる
        // 
        if (RockonTarget != null && Vector3.Distance(target.transform.position, RockonTarget.transform.position) <= target.RockonRange)
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
            if (DistanceXZ <= FightRange)
            {
                keyoutput = KEY_OUTPUT.NONE;
                // 地上にいるなら通常格闘開始
                if (target.IsGrounded)
                {
                    Cpumode = CPUMODE.DOGFIGHT_STANDBY;
                    return true;
                }
                // 高低差が一定以上なら特殊格闘で移動攻撃
                else if (FightRangeY <= DistanceY)
                {
                    // 自分が高ければ後特殊格闘(降下攻撃）
                    if (target.transform.position.y >= RockonTarget.transform.position.y)
                    {
                        Cpumode = CPUMODE.DOGFIGHT_DOWNER;
                        return true;
                    }
                    // 自分が低ければ前特殊格闘（上昇攻撃）
                    else
                    {
                        Cpumode = CPUMODE.DOGFIGHT_UPPER;
                        return true;
                    }
                }
            }
            // そうでなければ空中におり、敵との間に遮蔽物がなければ射撃攻撃（現行、地上にいると射撃のループをしてしまう）
            else if (!target.IsGrounded)
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
                var targetState = ControlTarget.GetComponent<SconosciutoControl>();
                // 弾が有ったら射撃攻撃
                                
                // 射撃のフォロースルーに入ったら再度空中ダッシュさせる
                if (targetState.AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == targetState.FollowThrowAirShotID)
                {
                    Cpumode = CPUMODE.BOOSTDASH;
                    keyoutput = KEY_OUTPUT.BOOSTDASH;
                    return false;
                }
            }
        }
        return false;
    }
}
