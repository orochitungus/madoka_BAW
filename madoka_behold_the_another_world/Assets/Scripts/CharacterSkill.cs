using UnityEngine;
using System.Collections;


// 各キャラクターの習得するスキル（使うときはこれを配列にする.モードチェンジ持ちは2個持ちにする）
public class CharacterSkill 
{
    // 種類
    public enum SkillType
    {
        // 攻撃系
        // 射撃属性
        SHOT,                   // 通常射撃
        CHARGE_SHOT,            // 射撃チャージ
        SUB_SHOT,               // サブ射撃
        EX_SHOT,                // 特殊射撃
        // 格闘属性は一応全部3段目まで作っておく(派生技に割り振るとかでもあり)
        WRESTLE_1,              // N格1段目
        WRESTLE_2,              // N格2段目
        WRESTLE_3,              // N格3段目
        CHARGE_WRESTLE,         // 格闘チャージ
        FRONT_WRESTLE_1,        // 前格闘1段目
        FRONT_WRESTLE_2,        // 前格闘2段目
        FRONT_WRESTLE_3,        // 前格闘3段目
        LEFT_WRESTLE_1,         // 左横格闘1段目
        LEFT_WRESTLE_2,         // 左横格闘2段目
        LEFT_WRESTLE_3,         // 左横格闘3段目
        RIGHT_WRESTLE_1,        // 右横格闘1段目
        RIGHT_WRESTLE_2,        // 右横格闘2段目
        RIGHT_WRESTLE_3,        // 右横格闘3段目
        BACK_WRESTLE,           // 後格闘（防御）
        AIRDASH_WRESTLE,        // 空中ダッシュ格闘
        EX_WRESTLE_1,           // 特殊格闘1段目
        EX_WRESTLE_2,           // 特殊格闘2段目
        EX_WRESTLE_3,           // 特殊格闘3段目
        EX_FRONT_WRESTLE_1,     // 前特殊格闘1段目
        EX_FRONT_WRESTLE_2,     // 前特殊格闘2段目
        EX_FRONT_WRESTLE_3,     // 前特殊格闘3段目
        EX_LEFT_WRESTLE_1,      // 左横特殊格闘1段目
        EX_LEFT_WRESTLE_2,      // 左横特殊格闘2段目
        EX_LEFT_WRESTLE_3,      // 左横特殊格闘3段目
        EX_RIGHT_WRESTLE_1,     // 右横特殊格闘1段目
        EX_RIGHT_WRESTLE_2,     // 右横特殊格闘2段目
        EX_RIGHT_WRESTLE_3,     // 右横特殊格闘3段目
        BACK_EX_WRESTLE,        // 後特殊格闘
        // モードチェンジ持ちの第2モード
        SHOT_M2,                   // 通常射撃
        CHARGE_SHOT_M2,            // 射撃チャージ
        SUB_SHOT_M2,               // サブ射撃
        EX_SHOT_M2,                // 特殊射撃
        // 格闘属性は一応全部3段目まで作っておく(派生技に割り振るとかでもあり)
        WRESTLE_1_M2,              // N格1段目
        WRESTLE_2_M2,              // N格2段目
        WRESTLE_3_M2,              // N格3段目
        CHARGE_WRESTLE_M2,         // 格闘チャージ
        FRONT_WRESTLE_1_M2,        // 前格闘1段目
        FRONT_WRESTLE_2_M2,        // 前格闘2段目
        FRONT_WRESTLE_3_M2,        // 前格闘3段目
        LEFT_WRESTLE_1_M2,         // 左横格闘1段目
        LEFT_WRESTLE_2_M2,         // 左横格闘2段目
        LEFT_WRESTLE_3_M2,         // 左横格闘3段目
        RIGHT_WRESTLE_1_M2,        // 右横格闘1段目
        RIGHT_WRESTLE_2_M2,        // 右横格闘2段目
        RIGHT_WRESTLE_3_M2,        // 右横格闘3段目
        BACK_WRESTLE_M2,           // 後格闘（防御）
        AIRDASH_WRESTLE_M2,        // 空中ダッシュ格闘
        EX_WRESTLE_1_M2,           // 特殊格闘1段目
        EX_WRESTLE_2_M2,           // 特殊格闘2段目
        EX_WRESTLE_3_M2,           // 特殊格闘3段目
        EX_FRONT_WRESTLE_1_M2,     // 前特殊格闘1段目
        EX_FRONT_WRESTLE_2_M2,     // 前特殊格闘2段目
        EX_FRONT_WRESTLE_3_M2,     // 前特殊格闘3段目
        EX_LEFT_WRESTLE_1_M2,      // 左横特殊格闘1段目
        EX_LEFT_WRESTLE_2_M2,      // 左横特殊格闘2段目
        EX_LEFT_WRESTLE_3_M2,      // 左横特殊格闘3段目
        EX_RIGHT_WRESTLE_1_M2,     // 右横特殊格闘1段目
        EX_RIGHT_WRESTLE_2_M2,     // 右横特殊格闘2段目
        EX_RIGHT_WRESTLE_3_M2,     // 右横特殊格闘3段目
        BACK_EX_WRESTLE_M2,        // 後特殊格闘
        AROUSAL_ATTACK,         // 覚醒技
        // アビリティ系
        DISABLE_BLUNT_FOOT,         // 鈍足無効
        DISABLE_ROCKON_IMPOSSIBLE,  // ロックオン不可無効
        DISABLE_DESTRUCTION_MAGIC,  // 魔力破壊無効
        DISABLE_POISON,             // 毒無効
        DISABLE_MENTAL_CONTAMINATION,// 精神汚染無効
        DISABLE_HALLUCINATION,      // 幻覚無効
        DISABLE_MENTALS,            // 精神系ST異常無効
        // なし(派生がないとき用）
        NONE
    };

    // ヒット効果
    public enum HitType
    {
        BEND_BACKWARD,          // のけぞる
        BLOW,                   // 吹き飛ばす(ダウン)
        GUARD,                  // 防御
        RECOVERY,               // 回復
        RESUSCITATION,          // 蘇生込回復
        CURE,                   // ST異常含め回復
    };

    // リロードタイプ（射撃属性のみ）
    public enum ReloadType
    {
        ONE_BY_ONE,             // 1発ずつ回復
        IT_ALL_TOGETHER,        // 撃ち切った後まとめて回復（一定時間たった後回復するタイプは一定時間分を負の値にしておく）
        MANUAL,                 // 手動回復
        INTERVAL_ALL,           // 回復開始まで一定時間待つ
        NOTHING,                // 回復なし(チャージショットのような弾数のない射撃はこれ）
    };

    // スキルのタイプ
    public SkillType m_Skilltype;

    // ヒット効果
    public HitType m_Hittype;

    // スキルの名前
    public string m_SkillName;

    // この辺は弾数消費タイプであると判定するために0で初期化
    // 初期攻撃力
    public int m_OriginalStr = 0;
    // 攻撃力成長係数  
    public int m_GrowthCoefficientStr = 0;
    // 初期弾数
    public int m_OriginalBulletNum = 0;
    // 弾数成長係数
    public int m_GrowthCoefficientBul = 0;

    // 与えられるダウン値
    public float m_DownPoint;

    // 習得するレベル
    public int m_LearningLevel;

    // 硬直時間
    public float m_WaitTime;

    // 移動速度
    public float m_Movespeed;

    // アニメーション速度
    public float m_Animspeed;

    // 覚醒ゲージ増加量(とりあえず成長係数は攻撃力に準じる）
    public int m_arousal;

    // リロードタイプ
    public ReloadType m_reloadtype;

    // リロード時間（チャージショットは溜め時間）
    public float m_reloadtime;

    // アニメーション実行時間(これを過ぎたら該当の格闘動作を終了する）
    public float m_animationTime;

    // コンストラクタ(射撃）弾数の概念があるもの
    public CharacterSkill(SkillType skilltype, string skillname, int Original_Str, int Growth_Coefficient_Str, int Original_Bullet_Num,
        int Growth_Coefficient_Bul, float Down_Point, int Learning_Level,HitType hittype,float waittime,float movespeed, ReloadType reloadtype, float reloadtime, int arousal = 10)
    {
        this.m_Skilltype = skilltype;
        this.m_SkillName = skillname;
        this.m_OriginalStr = Original_Str;
        this.m_GrowthCoefficientStr = Growth_Coefficient_Str;
        this.m_OriginalBulletNum = Original_Bullet_Num;
        this.m_GrowthCoefficientBul = Growth_Coefficient_Bul;
        this.m_DownPoint = Down_Point;
        this.m_LearningLevel = Learning_Level;
        this.m_Hittype = hittype;
        this.m_WaitTime = waittime;
        this.m_Movespeed = movespeed;
        this.m_reloadtype = reloadtype;
        this.m_reloadtime = reloadtime;
        this.m_arousal = arousal;
    }
    // コンストラクタ（格闘）弾数の概念がないもの
    public CharacterSkill(SkillType skilltype, string skillname, int Original_Str, int Growth_Coefficient_Str, float Down_Point, int Learning_Level, HitType hittype,float waittime,float movespeed,
        float animationTime = 1.0f,int arousal = 10, float animspeed = 1.0f)
    {
        this.m_Skilltype = skilltype;
        this.m_SkillName = skillname;
        this.m_OriginalStr = Original_Str;
        this.m_GrowthCoefficientStr = Growth_Coefficient_Str;
        this.m_DownPoint = Down_Point;
        this.m_LearningLevel = Learning_Level;
        this.m_Hittype = hittype;
        this.m_WaitTime = waittime;
        this.m_Movespeed = movespeed;
        this.m_arousal = arousal;
        this.m_Animspeed = animspeed;
        this.m_animationTime = animationTime;
        this.m_OriginalBulletNum = 0;           // 外部からの識別が容易なように、弾数と成長係数を0にする
        this.m_GrowthCoefficientBul = 0;

    }
    // コンストラクタ（アビリティ）レベルにより習得するもの
    public CharacterSkill(SkillType skilltype, string skillname, int Learning_Level)
    {
        this.m_Skilltype = skilltype;
        this.m_SkillName = skillname;
        this.m_LearningLevel = Learning_Level;
        this.m_OriginalBulletNum = -1;          // 外部からの識別が容易なように弾数と成長係数を-1にする
        this.m_GrowthCoefficientBul = -1;
    }
}
