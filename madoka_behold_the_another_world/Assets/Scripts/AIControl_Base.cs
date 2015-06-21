using UnityEngine;
using System.Collections;


// AI制御の基底クラス。各キャラはこれを基に独自部分だけを追記する（一部の大型キャラを除く）
public class AIControl_Base : MonoBehaviour 
{

    // 制御対象(こっちから直接制御対象のキーフラグをいじる）
    public GameObject ControlTarget;
    public GameObject ControlTarget_Camera;
    // ロックオン対象
    public GameObject RockonTarget;
    // 制御対象がCPUであるか否か
    protected CharacterControl_Base.CHARACTERCODE m_isPlayer;

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

    public Vector3 InitialPosition; // 初期配置位置（哨戒時の起点）
    public Vector3 Destination;     // 哨戒時の移動先

    public GameObject m_SearcherL;  // 飛び越えられない壁を避けるときの測距用コライダ左
    public GameObject m_SearcherR;  // 飛び越えられない壁を避けるときの測距用コライダ右

    public CPUMODE m_cpumode;

    // 方向キーの出力
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

    // 上記出力のVECTOR2版
    public Vector2[] m_lever = new Vector2[]
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

    // ボタンの出力
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

    // テンキー（レバー）の出力
    public TENKEY_OUTPUT m_tenkeyoutput;
    // ボタンの出力
    public KEY_OUTPUT m_keyoutput;

    // 1F前のm_cpumode
    protected CPUMODE m_latecpumode;

    // ロックオン前が往路か復路か
    protected CPUMODE m_pastCPUMODE;

    // 上昇する時間
    protected float m_risetime;
    // 累積上昇
    protected float m_totalrisetime;
    // 近接レンジ
    protected float m_fightRange;
    // 近接レンジ（前後特殊格闘を使って上下するか）
    protected float m_fightRangeY;

    // 乱数設定時の値
    private float m_randnum;

    // Start内で初期化する
    protected void Initialize()
    {
        // 対象がプレイヤーであるか否かのフラグを拾う
        var target = ControlTarget.GetComponentInChildren<CharacterControl_Base>();

        this.m_isPlayer = target.m_isPlayer;

        // 一応キー初期化(ルーチンはインスペクタで拾う）
        m_tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
        m_keyoutput = KEY_OUTPUT.NONE;
        m_pastCPUMODE = CPUMODE.NONE;
        m_latecpumode = CPUMODE.NONE;
    }

    // update内で実行する
    protected void UpdateCore()
    {
        // 自機の時は一応やらせない
        if (this.m_isPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
        {
            // 自機の現在位置を取得
            var target = ControlTarget.GetComponentInChildren<CharacterControl_Base>();
            Vector3 nowpos = target.transform.position; // 本体の位置
            Vector3 targetpos;
            // ロックオン対象を取得
            var Enemy = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
            if(Enemy.Enemy != null)
                RockonTarget = Enemy.Enemy;
            // 往路時
            if (this.m_cpumode == CPUMODE.OUTWARD_JOURNEY)
            {
                targetpos = target.EndingPoint.transform.position;
                Control(nowpos, targetpos, ref this.m_tenkeyoutput, ref this.m_keyoutput);
            }
            // 復路時
            else if (this.m_cpumode == CPUMODE.RETURN_PATH)
            {
                targetpos = target.StartingPoint.transform.position;
                Control(nowpos, targetpos, ref this.m_tenkeyoutput, ref this.m_keyoutput);
            }
            // それ以外
            else
            {
                Control(nowpos, Vector3.zero, ref this.m_tenkeyoutput, ref this.m_keyoutput);
            }
        }
    }

    // モード毎の制御
    // nowpos(入力）          :現在の位置
    // targetpos(入力）       :哨戒時の目標点
    // tenkeyoutput(参照返し）:テンキー出力
    // keyoutput(参照返し）   :ボタン出力
    private void Control(Vector3 nowpos, Vector3 targetpos, ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        switch (m_cpumode)
        {
            case CPUMODE.OUTWARD_JOURNEY:        // 規定位置を哨戒（往路）
                outward_journey(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.RETURN_PATH:            // 規定位置を哨戒（復路）
                return_path(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_STANDBY1:         // ロックオンボタンを離す
                normal_standby1(ref keyoutput);
                break;
            case CPUMODE.NORMAL_STANDBY2:         // ロックオンボタンを押す
                normal_standby2(ref keyoutput);
                break;
            case CPUMODE.NORMAL:
                normal(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_RISE1:           // 上昇中
                noraml_rise1(ref keyoutput);
                break;
            case CPUMODE.NORMAL_RISE2:
                normal_rise2(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_RISE3:          // 再上昇準備
                normal_rise3(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.NORMAL_FALL:            // 壁に当たって落下中
                normal_fall();
                break;
            case CPUMODE.NORMAL_RISE4:           // 壁に接触している間は上昇し、登り切ったら空中ダッシュへ
                normal_rise4(ref tenkeyoutput);
                break;
            case CPUMODE.NORMAL_FLYING:          // 飛行中
                normal_flying(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.FIREFIGHT:              // 射撃戦
                firefight(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_STANDBY:
                dogfight_standby(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT:               // 格闘戦（地上準備）
                dogfight();
                break;
            case CPUMODE.DOGFIGHT_DONE:
                dogfight_done(ref tenkeyoutput, ref keyoutput);
                break;
            case CPUMODE.DOGFIGHT_UPPER:         // 格闘戦（上昇）
                dogfight_upper(ref tenkeyoutput, ref keyoutput);
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
        m_latecpumode = m_cpumode;
    }

    //OUTWARD_JOURNEY,        // 規定位置を哨戒（往路）
    protected virtual void outward_journey(Vector3 nowpos, Vector3 targetpos, ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        PatrolTenkey(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
        rockonCheck();
    }
    //RETURN_PATH,            // 規定位置を哨戒（復路）
    protected virtual void return_path(Vector3 nowpos, Vector3 targetpos, ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        PatrolTenkey(nowpos, targetpos, ref tenkeyoutput, ref keyoutput);
        rockonCheck();
    }
    //NORMAL_STANDBY1,        // 通常に入る準備1（ロックオンボタンを離す）
    protected virtual void normal_standby1(ref KEY_OUTPUT keyoutput)
    {
        keyoutput = KEY_OUTPUT.NONE;
        m_cpumode = CPUMODE.NORMAL_STANDBY2;
    }
    //NORMAL_STANDBY2,        // 通常に入る準備2（ロックオンボタンを押す）
    protected virtual void normal_standby2(ref KEY_OUTPUT keyoutput)
    {
        keyoutput = KEY_OUTPUT.SEARCH;
        m_cpumode = CPUMODE.NORMAL;
    }

    //NORMAL,                 // 通常
    protected virtual void normal(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        CharacterControl_Base target = ControlTarget.GetComponent<CharacterControl_Base>();
        // 一端ロックオンボタンを離す
        keyoutput = KEY_OUTPUT.NONE;

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
        if ((int)m_latecpumode > (int)CPUMODE.RETURN_PATH && distance > target.m_Rockon_RangeLimit)
        {
            ReturnPatrol(target);
        }

        // 何らかの理由で哨戒起点か終点をロックしたまま攻撃体制に入った場合は元に戻す
        if (UnRockAndReturnPatrol())
            return;
    }

    // 哨戒に戻る
    // target[in]   :制御対象
    void ReturnPatrol(CharacterControl_Base target)
    {
        // カメラに登録しておいたロックオンオブジェクトを破棄する
        Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
        pcc.RockOnTarget.Clear();        
        m_cpumode = m_pastCPUMODE;
        // ここでロックオン対象をスタートかゴールにする(AIControl_Base,カメラ双方）
        if (m_cpumode == CPUMODE.OUTWARD_JOURNEY)
        {
            RockonTarget = target.EndingPoint;
            pcc.Enemy = target.EndingPoint;
        }
        else
        {
            m_cpumode = CPUMODE.RETURN_PATH;
            pcc.Enemy = target.EndingPoint;
            RockonTarget = target.StartingPoint;
        }
     }

    //NORMAL_RISE1,           // 通常で上昇(飛び上がる）
    protected virtual void noraml_rise1(ref KEY_OUTPUT keyoutput)
    {
        // 何らかの理由で哨戒起点か終点をロックしたまま攻撃体制に入った場合は元に戻す
        if (UnRockAndReturnPatrol())
            return;

        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();

        // 地上から離れて一定時間後ジャンプボタンを離す
        if (Time.time > m_totalrisetime + m_risetime && !target.GetInGround())
        {
            keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.NORMAL_RISE2;
            m_totalrisetime = Time.time;
        }
        // 地上から離れずに一定時間いるとNORMALへ戻って仕切り直す
        if (Time.time > m_totalrisetime + m_risetime && target.GetInGround())
        {
            m_cpumode = CPUMODE.NORMAL;
        }
        // 敵との距離が離れすぎるとロックオンを解除して哨戒に戻る
        // カメラ
        Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
        float distance = Vector3.Distance(pcc.Player.transform.position, pcc.Enemy.transform.position);
        if ((int)m_latecpumode > (int)CPUMODE.RETURN_PATH && distance > target.m_Rockon_RangeLimit)
        {
            ReturnPatrol(target);
        }
       
    }

    // 哨戒起点か終点をロックしたまま攻撃態勢に入った場合、ロックオン対象を元に戻す
    // return   :ロックオン対象を元に戻した
    bool UnRockAndReturnPatrol()
    {
        // カメラ
        Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
        // 制御対象
        CharacterControl_Base target = ControlTarget.GetComponent<CharacterControl_Base>();
        if (pcc.Enemy == null || pcc.Enemy.GetComponent<CharacterControl_Base>() == null)
        {
            ReturnPatrol(target);
            return true;
        }
        return false;
    }

    //NORMAL_RISE2,           // 通常で上昇（落下する）
    protected virtual void normal_rise2(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 空中ダッシュ入力を行う
        tenkeyoutput = TENKEY_OUTPUT.TOP;
        keyoutput = KEY_OUTPUT.AIRDASH;
        m_cpumode = CPUMODE.NORMAL_FLYING;
    }
    //NORMAL_RISE3,           // 壁の側まで歩いて行く
    protected virtual void normal_rise3(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // 壁に接触すると飛び上がる
        if (target.Gethitjumpover())
        {
            keyoutput = KEY_OUTPUT.JUMP;
            m_cpumode = CPUMODE.NORMAL_RISE4;
        }
        // 地面にいる間は壁まで進む
        else if (target.GetInGround() && !target.Gethitjumpover())
        {
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            keyoutput = KEY_OUTPUT.NONE;
        }
    }
    //NORMAL_RISE4,           // 通常で上昇（壁がなくなるまで上昇する)
    protected virtual void normal_rise4(ref TENKEY_OUTPUT tenkeyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        if (!target.Gethitjumpover())
        {
            m_cpumode = CPUMODE.NORMAL_RISE1;
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            m_totalrisetime = Time.time;
        }
    }
    //NORMAL_FALL,            // 落下中
    protected virtual void normal_fall()
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        if (target.GetInGround())
        {
            m_cpumode = CPUMODE.NORMAL_RISE3;   // 着陸したら次へ
        }
        // 飛び越えた後のFALLの可能性もあるので、ここでチェックする
        // レイキャストで引っかからなければNORMALにする
        RaycastHit hit;
        Vector3 RayStartPosition = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z);
        if (!Physics.Raycast(RayStartPosition, Vector3.forward, out hit, 10.0f))
        {
            m_keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.NORMAL;
        }
    }
    //NORMAL_FLYING,          // 通常で飛行
    protected virtual void normal_flying(ref TENKEY_OUTPUT tenkeyoutput,ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        keyoutput = KEY_OUTPUT.JUMP;
        tenkeyoutput = TENKEY_OUTPUT.TOP;
        // 飛び越えられる壁に接触した
        if (target.Gethitjumpover())
        {
            // 一旦ジャンプボタンとテンキーを放す
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.NORMAL_FALL;
        }
        // 飛び越えられない壁に接触した
        else if (target.Gethitunjumpover())
        {
            // レーダー左とロックオン対象の距離を求める
            float distL = Vector3.Distance(m_SearcherL.transform.position, RockonTarget.transform.position);
            // レーダー右とロックオン対象の距離を求める
            float distR = Vector3.Distance(m_SearcherR.transform.position, RockonTarget.transform.position);
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
        if (target.GetInGround())
        {
            m_cpumode = CPUMODE.NORMAL;
        }
    }
    //FIREFIGHT,              // 射撃戦
    protected virtual void firefight(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        keyoutput = KEY_OUTPUT.NONE;
        // 地上にいた場合（→再度飛行）
        if (target.GetInGround())
        {
            keyoutput = KEY_OUTPUT.JUMP;
            m_cpumode = CPUMODE.NORMAL_RISE1;
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            m_totalrisetime = Time.time;
        }
        // 飛び越えられる壁に接触していた場合（→NORMAL_RISE3へ）
        else if (target.Gethitjumpover())
        {
            // 一旦ジャンプボタンとテンキーを放す
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.NORMAL_FALL;
        }
        // 飛び越えられない壁に接触していた場合（→
        // 空中にいた場合（→再度ダッシュしてビームずんだ）
        else if (target.Gethitunjumpover())
        {
            // レーダー左とロックオン対象の距離を求める
            float distL = Vector3.Distance(m_SearcherL.transform.position, RockonTarget.transform.position);
            // レーダー右とロックオン対象の距離を求める
            float distR = Vector3.Distance(m_SearcherR.transform.position, RockonTarget.transform.position);
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
            m_cpumode = CPUMODE.NORMAL_FLYING;
        }
    }
    //DOGFIGHT_STANDBY,       // 格闘戦準備
    protected virtual void dogfight_standby(ref TENKEY_OUTPUT tenkeyoutput,ref KEY_OUTPUT keyoutput)
    {
        if (RockonTarget == null)
        {
            m_cpumode = CPUMODE.OUTWARD_JOURNEY;
            return;
        }
        // ロックオン対象
        var rockonTarget = RockonTarget.GetComponent<CharacterControl_Base>();
        if (rockonTarget == null)
        {
            m_cpumode = CPUMODE.OUTWARD_JOURNEY;
            return;
        }
        // 相手が格闘(N・前・左・右・特殊)を振っているか？
        if (rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Wrestle_1 || rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Wrestle_2 ||
            rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Wrestle_3 || rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Front_Wrestle_1 ||
            rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Front_Wrestle_2 || rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Front_Wrestle_3 ||
            rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Left_Wrestle_1 || rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Left_Wrestle_2 ||
            rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Left_Wrestle_3 || rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Right_Wrestle_1 ||
            rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Right_Wrestle_2 || rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Right_Wrestle_3 ||
            rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Ex_Wrestle_1 || rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Ex_Wrestle_2 ||
            rockonTarget.m_AnimState[0] == CharacterControl_Base.AnimationState.Ex_Wrestle_3)
        {
            // 左右どっちかにステップ(これでは両方立つ可能性がある。左選択→再度ここに来る→右選択
            m_randnum = Random.value;
            // 左
            if (m_randnum >= 0.5)
            {
                m_tenkeyoutput = TENKEY_OUTPUT.LEFTSTEP;
            }
            // 右
            else
            {
                m_tenkeyoutput = TENKEY_OUTPUT.RIGHTSTEP;
            }
            m_cpumode = CPUMODE.DOGFIGHT;
        }
        // そうでなければN格か前格を振る
        else
        {
            // 乱数（50％の確率で前格闘を振る）
            m_randnum = Random.value;
            // m_randumが0.5以上なら前格闘
            if (m_randnum >= 0.5)
            {
                tenkeyoutput = TENKEY_OUTPUT.TOP;
            }
            else
            {
                m_tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            }
            keyoutput = KEY_OUTPUT.WRESTLE;
            m_cpumode = CPUMODE.DOGFIGHT;
        }
    }
    //DOGFIGHT,               // 格闘戦（地上準備）
    protected virtual void dogfight()
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // ステップか格闘を振り終わっていればNORMALへ戻る
        if (target.m_AnimState[0] == CharacterControl_Base.AnimationState.Idle || target.m_AnimState[0] == CharacterControl_Base.AnimationState.Fall)
        {
            m_cpumode = CPUMODE.NORMAL;
            target.StepStop();
        }
    }
    //DOGFIGHT_DONE,          // 格闘戦（地上）
    protected virtual void dogfight_done(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // m_randumが0.5以上なら前格闘
        if (m_randnum >= 0.5)
        {
            tenkeyoutput = TENKEY_OUTPUT.TOP;
        }
        keyoutput = KEY_OUTPUT.WRESTLE;
        m_cpumode = CPUMODE.DOGFIGHT_DONE;
        // 格闘を振り終わったらNORMALへ戻る
        if (target.m_AnimState[0] == CharacterControl_Base.AnimationState.Idle || target.m_AnimState[0] == CharacterControl_Base.AnimationState.Fall)
        {
            m_cpumode = CPUMODE.NORMAL;
        }
    }
    //DOGFIGHT_UPPER,         // 格闘戦（上昇）
    protected virtual void dogfight_upper(ref TENKEY_OUTPUT tenkeyoutput,ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // ブーストがある限り上昇
        if (target.m_Boost > 0)
        {
            tenkeyoutput = TENKEY_OUTPUT.TOP;
            keyoutput = KEY_OUTPUT.EXWRESTLE;
        }
        // 上昇しきったらNORMALへ
        else
        {
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.NORMAL;
        }
    }
    //DOGFIGHT_DOWNER,        // 格闘戦（下降）
    protected virtual void dogfight_downer(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // 地上に落ちきるまで下降
        if (!target.GetInGround())
        {
            tenkeyoutput = TENKEY_OUTPUT.UNDER;
            keyoutput = KEY_OUTPUT.EXWRESTLE;
        }
        // 着地したらNORMALへ
        else
        {
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.NORMAL;
        }
    }
    //GUARD                   // 防御
    protected virtual void guard(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        // 何らかの理由で哨戒起点か終点をロックしたまま攻撃体制に入った場合は元に戻す
        if (UnRockAndReturnPatrol())
            return;
        // 制御対象
        var target = ControlTarget.GetComponent<CharacterControl_Base>();
        // ブーストがある限りガード
        if (target.m_Boost > 0)
        {
            tenkeyoutput = TENKEY_OUTPUT.UNDER;
            keyoutput = KEY_OUTPUT.WRESTLE;
        }
        // ブースト切れならNORMALへ
        else
        {
            tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
            keyoutput = KEY_OUTPUT.NONE;
            m_cpumode = CPUMODE.NORMAL;
        }
    }

    //GUARDEND                // 防御終了
    protected virtual void guardend(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
        keyoutput = KEY_OUTPUT.NONE;
        m_cpumode = CPUMODE.NORMAL;
    }

    //AROUSAL               　// 覚醒
    protected virtual void arousal(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
        keyoutput = KEY_OUTPUT.AROUSAL;
        m_cpumode = CPUMODE.AROUSAL_END;
    }

    //AROUSAL_END             // 覚醒終了
    protected virtual void arousal_end(ref TENKEY_OUTPUT tenkeyoutput, ref KEY_OUTPUT keyoutput)
    {
        tenkeyoutput = TENKEY_OUTPUT.NEUTRAL;
        keyoutput = KEY_OUTPUT.NONE;
        m_cpumode = CPUMODE.NORMAL;
    }
    
    //VARIANCE,               // 分散
    protected virtual void variance()
    {
    }
    //CROSSFIRE,              // 集中
    protected virtual void crossfire()
    {
    }
    //ASSAULT,                // 突撃
    protected virtual void assault()
    {
    }
    //AVOIDANCE,              // 回避  
    protected virtual void avoidance()
    {
    }

    // 攻撃開始
    // ret      :攻撃開始した
    protected virtual bool engauge(ref KEY_OUTPUT keyoutput)
    {
        return false;
    }

    


    // 規定位置を哨戒する(哨戒時のロックオンはカメラ側で.飛び越えるために追加の参照でブーストボタンを出すのもありか）
    // nowpos(入力）            ：現在の位置
    // targetpos(入力）         ：哨戒時の目標点
    // output(参照返し）　　　　：方向キー
    // keyoutput(参照返し）     ：ボタン
    private void PatrolTenkey(Vector3 nowpos, Vector3 targetpos, ref TENKEY_OUTPUT output, ref KEY_OUTPUT keyoutput)
    {
        // 距離が一定になるまでは移動(完全に一致だと面倒なので閾値として5くらい）
        float distance = Vector3.Distance(nowpos, targetpos);

        // ノーロック状態なら強制的にロックオンフラグ
        var target = ControlTarget.GetComponentInChildren<CharacterControl_Base>();
        if (target.m_IsRockon == false)
        {
            keyoutput = KEY_OUTPUT.SEARCH;
            return;
        }

        // このキャラのメインカメラを拾う
        GameObject maincamera = this.transform.FindChild("Main Camera").gameObject;
        // スタート地点
        GameObject startpoint = ControlTarget.GetComponent<CharacterControl_Base>().StartingPoint;
        // エンド地点
        GameObject endpoint = ControlTarget.GetComponent<CharacterControl_Base>().EndingPoint;
        if (distance > 10.0f)
        {
            output = TENKEY_OUTPUT.TOP; // 対象をロックオンしていること前提
            keyoutput = KEY_OUTPUT.NONE;
        }
        // 距離が5以下になると、ロックオン対象を切り替え、往路復路を切り替える(startとendを逆にする）
        else
        {
            //keyoutput = KEY_OUTPUT.SEARCH;
            // 往路から復路へ
            if (this.m_cpumode == CPUMODE.OUTWARD_JOURNEY)
            {
                maincamera.GetComponent<Player_Camera_Controller>().Enemy = startpoint;
                this.m_cpumode = CPUMODE.RETURN_PATH;
            }
            // 復路から往路へ
            else
            {
                maincamera.GetComponent<Player_Camera_Controller>().Enemy = endpoint;
                this.m_cpumode = CPUMODE.OUTWARD_JOURNEY;
            }
        }
    }

    // 哨戒中の範囲内に敵が入ってきたら、ロックオンして通常へ移行
    private void rockonCheck()
    {
        // 敵が入ってきたか判定
        Player_Camera_Controller pcc = ControlTarget_Camera.GetComponent<Player_Camera_Controller>();
        // CPUMODEを保持
        m_pastCPUMODE = m_cpumode;
        // プレイヤーサイドの場合
        if (m_isPlayer == CharacterControl_Base.CHARACTERCODE.PLAYER_ALLY)
        {
            if (pcc.RockOnDone(true,true))
            {
                m_cpumode = CPUMODE.NORMAL;
            }
        }
        // エネミーサイドの場合
        else if (m_isPlayer == CharacterControl_Base.CHARACTERCODE.ENEMY)
        {
            if (pcc.RockOnDone(false,true))
            {
                m_cpumode = CPUMODE.NORMAL;
            }
        }
    }
   
}
