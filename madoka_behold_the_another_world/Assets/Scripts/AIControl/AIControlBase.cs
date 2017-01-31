using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// AI制御の基底クラス。各キャラはこれを基に独自部分だけを追記する（一部の大型キャラを除く）
public class AIControlBase : MonoBehaviour
{
    /// <summary>
    /// 制御対象
    /// </summary>
    public GameObject ControlTarget;

    /// <summary>
    /// 制御対象のカメラ
    /// </summary>
    public GameObject ControlTarget_Camera;

    /// <summary>
    /// ロックオン対象
    /// </summary>
    public GameObject RockonTarget;

    // 制御対象がCPUであるか否か
    protected CharacterControlBase.CHARACTERCODE IsPlayer;

    // 操作の種類
    public enum CPUMODE
    {
        NONE,                   // なし
        PLAYER,                 // プレイヤー制御
        NOMOVE,                 // 停止
        OUTWARD_JOURNEY,        // 規定位置を哨戒（往路）
        RETURN_PATH,            // 規定位置を哨戒（復路）
        NORMAL_STANDBY1,        // 通常に入る準備1（ロックオンボタンを離す）
        NORMAL_STANDBY2,        // 通常に入る準備2（ロックオンボタンを押す）
        NORMAL,                 // 通常
        NORMAL_RISE1,           // 通常で上昇(飛び上がる）
        NORMAL_RISE2,           // 通常で上昇（落下する）
        NORMAL_RISE3,           // 壁の側まで歩いて行く
        NORMAL_RISE4,           // 通常で上昇（壁がなくなるまで上昇する)
        NORMAL_FALL,            // 落下中
        NORMAL_FLYING,          // 通常で飛行
        FIREFIGHT,              // 射撃戦
        DOGFIGHT_STANDBY,       // 格闘戦準備
        DOGFIGHT,               // 格闘戦（地上準備）
        DOGFIGHT_DONE,          // 格闘戦（地上）
        DOGFIGHT_FRONT,         // 格闘戦（前格闘）
        DOGFIGHT_LEFT,          // 格闘戦（横格闘左）
        DOGFIGHT_RIGHT,         // 格闘戦（横格闘右）
        DOGFIGHT_EX,			// 格闘戦（特殊格闘）
        DOGFIGHT_UPPER,         // 格闘戦（上昇）
        DOGFIGHT_DOWNER,        // 格闘戦（下降）
        GUARD,                  // 防御
        AROUSAL,                // 覚醒
        AROUSAL_END,            // 覚醒終了
        GUARDEND,               // ガード終了
        VARIANCE,               // 分散
        CROSSFIRE,              // 集中
        ASSAULT,                // 突撃
        AVOIDANCE,              // 回避        
    };

    /// <summary>
    /// 飛び越えられない壁を避けるときの測距用コライダ左
    /// </summary>
    public GameObject SearcherL;

    /// <summary>
    /// 飛び越えられない壁を避けるときの測距用コライダ右
    /// </summary>         
    public GameObject SearcherR;

    /// <summary>
    /// 現在のCPUの状態
    /// </summary>
    public CPUMODE Cpumode;

    /// <summary>
    /// 方向キーの出力
    /// </summary>
    public enum TENKEY_OUTPUT
    {
        NEUTRAL,                // 入力なし
        TOP,                    // 上
        TOPLEFT,                // 左上
        TOPRIGHT,               // 右上
        LEFT,                   // 左
        RIGHT,                  // 右
        UNDER,                  // 下
        UNDERLEFT,              // 左下
        UNDERRIGHT,             // 右下
        LEFTSTEP,               // 左ステップ
        RIGHTSTEP,              // 右ステップ
    };

    /// <summary>
    /// 方向キー出力のVECTOR2版
    /// </summary>
    public Vector2[] Lever = new Vector2[]
    {
        new Vector2(0,0),
        new Vector2(0,1),       //TOP,                    // 上
        new Vector2(-1,1),      //TOPLEFT,                // 左上
        new Vector2(1,1),       //TOPRIGHT,               // 右上
        new Vector2(-1,0),      //LEFT,                   // 左
        new Vector2(1,0),       //RIGHT,                  // 右
        new Vector2(0,-1),      //UNDER,                  // 下
        new Vector2(-1,-1),     //UNDERLEFT,              // 左下
        new Vector2(1,-1)       //UNDERRIGHT,             // 右下
    };

    /// <summary>
    /// ボタンの出力
    /// </summary>
    public enum KEY_OUTPUT
    {
        NONE,
        SHOT,                   // 射撃
        JUMP,                   // ジャンプ（一応ダッシュキャンセルとは別扱い）
        DASHCANCEL,             // ダッシュキャンセル
        AIRDASH,                // 空中ダッシュ
        SEARCH,                 // サーチ
        SEACHCANCEL,            // サーチキャンセル（現在未使用）
        WRESTLE,                // 格闘
        SUBSHOT,                // サブ射撃
        EXSHOT,                 // 特殊射撃
        EXWRESTLE,              // 特殊格闘
        ITEM,                   // アイテム
        AROUSAL,                // 覚醒
        AROUSALATTACK,          // 覚醒技
        CHARGESHOT,             // 射撃チャージ
        CHARGEWRESTE,           // 格闘チャージ
        PAUSE,                  // ポーズ
    }

    /// <summary>
    /// テンキー（レバー）の出力
    /// </summary>
    public TENKEY_OUTPUT Tenkeyoutput;

    /// <summary>
    /// ボタンの出力
    /// </summary>
    public KEY_OUTPUT Keyoutput;

    /// <summary>
    /// 1F前のcpumode
    /// </summary>
    protected CPUMODE Latecpumode;

    /// <summary>
    /// ロックオン前が往路か復路か
    /// </summary>
    protected CPUMODE PastCPUMODE;

    /// <summary>
    /// 上昇する時間
    /// </summary>
    protected float Risetime;

    /// <summary>
    /// 累積上昇
    /// </summary>
    protected float Totalrisetime;

    /// <summary>
    /// 近接レンジ
    /// </summary>
    protected float FightRange;

    /// <summary>
    /// 近接レンジ（前後特殊格闘を使って上下するか）
    /// </summary>
    protected float FightRangeY;

    // Start内で初期化する
    protected void Initialize()
    {
        // 対象がプレイヤーであるか否かのフラグを拾う
        var target = ControlTarget.GetComponentInChildren<CharacterControlBase>();

        IsPlayer = target.IsPlayer;

        // 一応キー初期化(ルーチンはインスペクタで拾う）
        Tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
        Keyoutput = KEY_OUTPUT.NONE;
        PastCPUMODE = CPUMODE.NONE;
        Latecpumode = CPUMODE.NONE;
    }

    // update内で実行する
    protected void UpdateCore()
    {
        // 自機の時は一応やらせない
        if (IsPlayer != CharacterControlBase.CHARACTERCODE.PLAYER)
        {
            // 自機の現在位置を取得
            var target = ControlTarget.GetComponentInChildren<CharacterControlBase>();
            Vector3 nowpos = target.transform.position; // 本体の位置
            Vector3 targetpos;
            // ロックオン対象を取得
            var Enemy = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
            if (Enemy.Enemy != null)
                RockonTarget = Enemy.Enemy;
            // 往路時
            if (Cpumode == CPUMODE.OUTWARD_JOURNEY)
            {
                targetpos = target.EndingPoint.transform.position;
                Control(nowpos, targetpos, ref Tenkeyoutput, ref Keyoutput);
            }
            // 復路時
            else if (Cpumode == CPUMODE.RETURN_PATH)
            {
                targetpos = target.StartingPoint.transform.position;
                Control(nowpos, targetpos, ref Tenkeyoutput, ref Keyoutput);
            }
            // それ以外
            else
            {
                Control(nowpos, Vector3.zero, ref Tenkeyoutput, ref Keyoutput);
            }
        }
    }


    /// <summary>
    /// モード毎の制御
    /// </summary>
    /// <param name="nowpos">現在の位置</param>
    /// <param name="targetpos">哨戒時の目標点</param>
    /// <param name="tenkeyoutput">テンキー出力</param>
    /// <param name="keyoutput">ボタン出力</param>
    private void Control(Vector3 nowpos, Vector3 targetpos, ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        switch (Cpumode)
        {
            case CPUMODE.OUTWARD_JOURNEY:        // 規定位置を哨戒（往路）
                Outward_journey(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.RETURN_PATH:            // 規定位置を哨戒（復路）
                Return_path(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_STANDBY1:         // ロックオンボタンを離す
                Normal_standby1(ref keyoutput);
                break;
            case CPUMODE.NORMAL_STANDBY2:         // ロックオンボタンを押す
                Normal_standby2(ref keyoutput);
                break;
            case CPUMODE.NORMAL:
                Normal(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_RISE1:           // 上昇中
                Noraml_rise1(ref keyoutput);
                break;
            case CPUMODE.NORMAL_RISE2:
                normal_rise2(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_RISE3:          // 再上昇準備
                Normal_rise3(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_FALL:            // 壁に当たって落下中
                Normal_fall();
                break;
            case CPUMODE.NORMAL_RISE4:           // 壁に接触している間は上昇し、登り切ったら空中ダッシュへ
                Normal_rise4(ref tenkeyoutput);
                break;
            case CPUMODE.NORMAL_FLYING:          // 飛行中
                Normal_flying(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.FIREFIGHT:              // 射撃戦
                Firefight(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_STANDBY:
                DogfightStandby(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT:               // 格闘戦（地上準備）
                Dogfight();
                break;
            case CPUMODE.DOGFIGHT_DONE:			 // 格闘戦（N格闘）
                Dogfight_done(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_FRONT:         // 格闘戦（前格闘）
                Dogfight_front(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_LEFT:          // 格闘戦（横格闘左）
                Dogfight_left(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_RIGHT:         // 格闘戦（横格闘右）
                Dogfight_right(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_EX:            // 格闘戦（特殊格闘）
                Dogfight_ex(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_UPPER:         // 格闘戦（上昇）
                Dogfight_upper(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_DOWNER:        // 格闘戦（下降）
                dogfight_downer(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.GUARD:                  // 防御
                guard(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.GUARDEND:               // 防御終了
                guardend(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.AROUSAL:                // 覚醒
                arousal(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.AROUSAL_END:            // 覚醒終了
                arousal_end(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.VARIANCE:               // 分散
                variance();
                break;
            case CPUMODE.CROSSFIRE:              // 集中
                crossfire();
                break;
            case CPUMODE.ASSAULT:                // 突撃
                assault();
                break;
            case CPUMODE.AVOIDANCE:              // 回避
                avoidance();
                break;
            default:
                break;
        }
        Latecpumode = Cpumode;
    }


    /// <summary>
    /// 規定位置を哨戒（往路）
    /// </summary>
    /// <param name="nowpos"></param>
    /// <param name="targetpos"></param>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Outward_journey(Vector3 nowpos, Vector3 targetpos, ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        PatrolTenkey(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
        rockonCheck();
    }

    /// <summary>
    /// 規定位置を哨戒（復路）
    /// </summary>
    /// <param name="nowpos"></param>
    /// <param name="targetpos"></param>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Return_path(Vector3 nowpos, Vector3 targetpos, ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        PatrolTenkey(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
        rockonCheck();
    }
    
    /// <summary>
    /// 通常に入る準備1（ロックオンボタンを離す）
    /// </summary>
    /// <param name="keyoutput"></param>
    protected virtual void Normal_standby1(ref KEY_OUTPUT keyoutput)
    {
        keyoutput = KEY_OUTPUT.NONE;
        Cpumode = CPUMODE.NORMAL_STANDBY2;
    }

    /// <summary>
    /// 通常に入る準備2（ロックオンボタンを押す）
    /// </summary>
    /// <param name="keyoutput"></param>
    protected virtual void Normal_standby2(ref KEY_OUTPUT keyoutput)
    {
        keyoutput = KEY_OUTPUT.SEARCH;
        Cpumode = CPUMODE.NORMAL;
    }
    
    /// <summary>
    /// 通常
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Normal(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        CharacterControlBase target = ControlTarget.GetComponent<CharacterControlBase>();
        // 一端ロックオンボタンを離す
        keyoutput = KEY_OUTPUT.NONE;

        // TODO:地上にいてダウンしていなくブーストゲージがあった場合、飛行させる（着地硬直中などは飛べない）
        //if (target.IsGrounded && target.AnimatorUnit && target.m_AnimState[0] != CharacterControl_Base.AnimationState.Reversal
        //    && target.Boost > 0)
        //{
        //    keyoutput = KEY_OUTPUT.JUMP;
        //    m_cpumode = CPUMODE.NORMAL_RISE1;
        //    tenkeyoutput = TENKEY_OUTPUT.TOP;
        //    m_totalrisetime = Time.time;
        //    return;
        //}
        // ロックオン状態で赤ロックになったら戦闘開始
        if (engauge(ref keyoutput))
        {
            return;
        }

        // 往路・復路以外で敵が離れたらロックオン対象をスタートかゴールにする
        // カメラ
        Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
        // 敵がいなかったら、この時点で哨戒に戻る
        if (pcc.Enemy == null)
        {
            ReturnPatrol(target);
            return;
        }
        // 敵との距離
        float distance = Vector3.Distance(pcc.Player.transform.position, pcc.Enemy.transform.position);
        if ((int)Latecpumode > (int)CPUMODE.RETURN_PATH && distance > target.RockonRangeLimit)
        {
            ReturnPatrol(target);
        }

        // 何らかの理由で哨戒起点か終点をロックしたまま攻撃体制に入った場合は元に戻す
        if (UnRockAndReturnPatrol())
            return;
    }

    /// <summary>
    /// 通常で上昇(飛び上がる）
    /// </summary>
    /// <param name="keyoutput"></param>
    protected virtual void Noraml_rise1(ref KEY_OUTPUT keyoutput)
    {
        // 何らかの理由で哨戒起点か終点をロックしたまま攻撃体制に入った場合は元に戻す
        if (UnRockAndReturnPatrol())
            return;

        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();

        // 地上から離れて一定時間後ジャンプボタンを離す
        if (Time.time > Totalrisetime + Risetime && !target.IsGrounded)
        {
            keyoutput = KEY_OUTPUT.NONE;
            Cpumode = CPUMODE.NORMAL_RISE2;
            Totalrisetime = Time.time;
        }
        // 地上から離れずに一定時間いるとNORMALへ戻って仕切り直す
        if (Time.time > Totalrisetime + Risetime && target.IsGrounded)
        {
            Cpumode = CPUMODE.NORMAL;
        }
        // 敵との距離が離れすぎるとロックオンを解除して哨戒に戻る
        // カメラ
        Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
        float distance = Vector3.Distance(pcc.Player.transform.position, pcc.Enemy.transform.position);
        if ((int)Latecpumode > (int)CPUMODE.RETURN_PATH && distance > target.RockonRangeLimit)
        {
            ReturnPatrol(target);
        }
    }

    /// <summary>
    /// 哨戒起点か終点をロックしたまま攻撃態勢に入った場合、ロックオン対象を元に戻す
    /// </summary>
    /// <returns>ロックオン対象を元に戻した</returns>
    protected bool UnRockAndReturnPatrol()
    {
        // カメラ
        Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
        // 制御対象
        CharacterControlBase target = ControlTarget.GetComponent<CharacterControlBase>();
        if (pcc.Enemy == null || pcc.Enemy.GetComponent<CharacterControlBase>() == null)
        {
            ReturnPatrol(target);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 通常で上昇（落下する）
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Normal_rise2(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 空中ダッシュ入力を行う
        tenkeyoutput = TENKEY_OUTPUT.TOP;
        keyoutput = KEY_OUTPUT.DASHCANCEL;
        Cpumode = CPUMODE.NORMAL_FLYING;
    }
   
    /// <summary>
    /// 壁の側まで歩いて行く
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Normal_rise3(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        // 壁に接触すると飛び上がる
        if (target.Gethitjumpover())
        {
            keyoutput = KEY_OUTPUT.JUMP;
            Cpumode = CPUMODE.NORMAL_RISE4;
        }
        // 地面にいる間は壁まで進む
        else if (target.IsGrounded && !target.Gethitjumpover())
        {
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            keyoutput = KEY_OUTPUT.NONE;
        }
    }
    
    /// <summary>
    /// 通常で上昇（壁がなくなるまで上昇する)
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    protected virtual void Normal_rise4(ref TENKEY_OUTPUT tenkeyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        if (!target.Gethitjumpover())
        {
            Cpumode = CPUMODE.NORMAL_RISE1;
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            Totalrisetime = Time.time;
        }
    }
         
    /// <summary>
    /// 落下中
    /// </summary>
    protected virtual void Normal_fall()
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        if (target.IsGrounded)
        {
            Cpumode = CPUMODE.NORMAL_RISE3;   // 着陸したら次へ
        }
        // 飛び越えた後のFALLの可能性もあるので、ここでチェックする
        // レイキャストで引っかからなければNORMALにする
        RaycastHit hit;
        Vector3 RayStartPosition = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z);
        if (!Physics.Raycast(RayStartPosition, Vector3.forward, out hit, 10.0f))
        {
            Keyoutput = KEY_OUTPUT.NONE;
            Cpumode = CPUMODE.NORMAL;
        }
    }

    //NORMAL_FLYING,          // 通常で飛行
    protected virtual void Normal_flying(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        keyoutput = KEY_OUTPUT.JUMP;
        tenkeyoutput = TENKEY_OUTPUT.TOP;
        // 飛び越えられる壁に接触した
        if (target.Gethitjumpover())
        {
            // 一旦ジャンプボタンとテンキーを放す
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            Cpumode = CPUMODE.NORMAL_FALL;
        }
        // 飛び越えられない壁に接触した
        else if (target.Gethitunjumpover())
        {
            // レーダー左とロックオン対象の距離を求める
            float distL = Vector3.Distance(SearcherL.transform.position, RockonTarget.transform.position);
            // レーダー右とロックオン対象の距離を求める
            float distR = Vector3.Distance(SearcherR.transform.position, RockonTarget.transform.position);
            // 左の方が大きければ左上方向へ飛行開始
            if (distL >= distR)
            {
                tenkeyoutput = TENKEY_OUTPUT.TOPLEFT;
            }
            // 右の方が大きければ右上方向へ飛行開始
            else
            {
                tenkeyoutput = TENKEY_OUTPUT.TOPRIGHT;
            }
        }
        // ロックオン状態で赤ロックになったら戦闘開始
        if (engauge(ref keyoutput))
        {
            return;
        }
        // なっていなくて着陸していればNORMALへ戻ってもう一度飛んでもらう
        if (target.IsGrounded)
        {
            Cpumode = CPUMODE.NORMAL;
        }
    }
         
    /// <summary>
    /// 射撃戦
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Firefight(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        keyoutput = KEY_OUTPUT.NONE;
        // 地上にいた場合（→再度飛行）
        if (target.IsGrounded)
        {
            keyoutput = KEY_OUTPUT.JUMP;
            Cpumode = CPUMODE.NORMAL_RISE1;
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            Totalrisetime = Time.time;
        }
        // 飛び越えられる壁に接触していた場合（→NORMAL_RISE3へ）
        else if (target.Gethitjumpover())
        {
            // 一旦ジャンプボタンとテンキーを放す
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            Cpumode = CPUMODE.NORMAL_FALL;
        }
        // 飛び越えられない壁に接触していた場合（→
        // 空中にいた場合（→再度ダッシュしてビームずんだ）
        else if (target.Gethitunjumpover())
        {
            // レーダー左とロックオン対象の距離を求める
            float distL = Vector3.Distance(SearcherL.transform.position, RockonTarget.transform.position);
            // レーダー右とロックオン対象の距離を求める
            float distR = Vector3.Distance(SearcherR.transform.position, RockonTarget.transform.position);
            // 左の方が大きければ左上方向へ飛行開始
            if (distL >= distR)
            {
                tenkeyoutput = TENKEY_OUTPUT.TOPLEFT;
            }
            // 右の方が大きければ右上方向へ飛行開始
            else
            {
                tenkeyoutput = TENKEY_OUTPUT.TOPRIGHT;
            }
        }
        // それ以外（再度ダッシュ）
        else
        {
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            keyoutput = KEY_OUTPUT.AIRDASH;
            Cpumode = CPUMODE.NORMAL_FLYING;
        }
    }

    /// <summary>
    /// 格闘戦
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void DogfightStandby(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        if (RockonTarget == null)
        {
            Cpumode = CPUMODE.OUTWARD_JOURNEY;
            return;
        }
        // ロックオン対象
        var rockonTarget = RockonTarget.GetComponent<CharacterControlBase>();
        if (rockonTarget == null)
        {
            Cpumode = CPUMODE.OUTWARD_JOURNEY;
            return;
        }
        // 相手が格闘を振ってきた場合、横ステップして格闘を入れる
        if(rockonTarget.IsWrestle)
        {
            // 現在時刻の秒の値を取り、奇数なら左、偶数なら右に避ける
            DateTime dt = DateTime.Now;
            if (dt.Second % 2 != 0)
            {
                tenkeyoutput = TENKEY_OUTPUT.LEFTSTEP;
            }
            else
            {
                tenkeyoutput = TENKEY_OUTPUT.RIGHTSTEP;
            }
        }
        // そうでなければN格闘で攻撃
        else
        {
            keyoutput = KEY_OUTPUT.WRESTLE;
        }
    }

  
    /// <summary>
    /// 格闘戦（地上準備）
    /// </summary>
    protected virtual void Dogfight()
    {
      
    }
    
    /// <summary>
    /// 格闘戦（地上）
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Dogfight_done(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
        keyoutput = KEY_OUTPUT.WRESTLE;
        Cpumode = CPUMODE.DOGFIGHT_DONE;
        // 格闘を振り終わったらNORMALへ戻る
        if (!target.IsWrestle)
        {
            Cpumode = CPUMODE.NORMAL;
        }
    }

    /// <summary>
	/// 格闘戦（前格闘）
	/// </summary>
	/// <param name="tenkeyoutput"></param>
	/// <param name="keyoutput"></param>
	protected virtual void Dogfight_front(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        tenkeyoutput = TENKEY_OUTPUT.TOP;
        keyoutput = KEY_OUTPUT.WRESTLE;
        Cpumode = CPUMODE.DOGFIGHT_FRONT;
        // 格闘を振り終わったらNORMALへ戻る
        if (!target.IsWrestle)
        {
            Cpumode = CPUMODE.NORMAL;
        }
    }

    /// <summary>
	/// 格闘戦(横格闘左）
	/// </summary>
	/// <param name="tenkeyoutput"></param>
	/// <param name="keyoutput"></param>
	protected virtual void Dogfight_left(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        tenkeyoutput = TENKEY_OUTPUT.LEFT;
        keyoutput = KEY_OUTPUT.WRESTLE;
        Cpumode = CPUMODE.DOGFIGHT_LEFT;
        // 格闘を振り終わったらNORMALへ戻る
        if (!target.IsWrestle)
        {
            Cpumode = CPUMODE.NORMAL;
        }
    }

    /// <summary>
	/// 格闘戦（横格闘右)
	/// </summary>
	/// <param name="tenkeyoutput"></param>
	/// <param name="keyoutput"></param>
	protected virtual void Dogfight_right(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        tenkeyoutput = TENKEY_OUTPUT.RIGHT;
        keyoutput = KEY_OUTPUT.WRESTLE;
        Cpumode = CPUMODE.DOGFIGHT_RIGHT;
        // 格闘を振り終わったらNORMALへ戻る
        if (!target.IsWrestle)
        {
            Cpumode = CPUMODE.NORMAL;
        }
    }

    /// <summary>
    /// 格闘戦（特殊格闘）
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Dogfight_ex(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
        keyoutput = KEY_OUTPUT.EXWRESTLE;
        Cpumode = CPUMODE.DOGFIGHT_EX;
        // 格闘を振り終わったらNORMALへ戻る
        if (!target.IsWrestle)
        {
            Cpumode = CPUMODE.NORMAL;
        }
    }
    
    /// <summary>
    /// 格闘戦（上昇）
    /// </summary>
    /// <param name="tenkeyoutput"></param>
    /// <param name="keyoutput"></param>
    protected virtual void Dogfight_upper(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControlBase>();
        // ブーストがある限り上昇
        if (target.Boost > 0)
        {
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            keyoutput = KEY_OUTPUT.EXWRESTLE;
        }
        // 上昇しきったらNORMALへ
        else
        {
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            Cpumode = CPUMODE.NORMAL;
        }
    }


}
