using UnityEngine;
using System.Collections;

// 各キャラクターの能力値を一元管理する
public static class Character_Spec 
{
    // 流石に面倒なのでキャラクター参照用インデックスの名前はつけておく
    // 並び自体はsaveingparameterと同じ
    public enum CHARACTER_NAME
    {
        MEMBER_NONE,        // なし
        // 魔法少女（PC)
        MEMBER_MADOKA,      // 鹿目まどか
        MEMBER_SAYAKA,      // 美樹さやか
        MEMBER_HOMURA,      // 暁美ほむら（銃）
        MEMBER_MAMI,        // 巴マミ
        MEMBER_KYOKO,       // 佐倉杏子
        MEMBER_YUMA,        // 千歳ゆま
        MEMBER_KIRIKA,      // 呉キリカ
        MEMBER_ORIKO,       // 美国織莉子
        MEMBER_SCHONO,      // スコノシュート（PC)
        MEMBER_HOMURA_B,    // 暁美ほむら（弓）
        MEMBER_UL_MADOKA,   // アルティメットまどか（まどかの覚醒とは一応別扱い）
        PLAYER_NUM,         // PC総数
        // 敵キャラ(NPC)
        ENEMY_MAJYU,        // 魔獣

        CHARACTER_ALL_NUM,  // 全キャラ
    };

    
    	
    // キャラクターの名前（表示用）
    public static string[] Name = new string[]
    {
        "",                 // なし
        "鹿目 まどか",
        "美樹 さやか",
        "暁美 ほむら",       //（銃）
        "巴 マミ",
        "佐倉 杏子",
        "千歳 ゆま",
        "呉 キリカ",
        "美国 織莉子",
        "sconosciuto",    
        "暁美 ほむら",       //（弓）
        "アルティメットまどか",//（まどかの覚醒とは一応別扱い）
        "",
        "魔獣"
    };

    // HP初期値
    public static int[] HP_OR = new int[]
    {
        0,                  // なし        
        200,                // 鹿目まどか
        210,                // 美樹さやか
        170,                // 暁美ほむら（銃）
        190,                // 巴マミ
        205,                // 佐倉杏子
        225,                // 千歳ゆま
        205,                // 呉キリカ
        214,                // 美国織莉子
        230,                // スコノシュート
        170,                // 暁美ほむら（弓）
        250,                // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,                  // 味方の最大数
        100,                // 魔獣

    };
    // HP成長係数
    public static int[] HP_Grouth = new int[]
    {
        0,                  // なし        
        23,                 // 鹿目まどか
        25,                 // 美樹さやか
        20,                 // 暁美ほむら（銃）
        21,                 // 巴マミ
        24,                 // 佐倉杏子
        26,                 // 千歳ゆま
        24,                 // 呉キリカ
        22,                 // 美国織莉子
        25,                 // スコノシュート
        20,                 // 暁美ほむら（弓）
        30,                 // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        20                  // 魔獣
    };
    // 防御力
    public static int[] Def_OR = new int[]
    {
        0,                  // なし        
        17,                 // 鹿目まどか
        18,                 // 美樹さやか
        10,                 // 暁美ほむら（銃）
        12,                 // 巴マミ
        18,                 // 佐倉杏子
        18,                 // 千歳ゆま
        19,                 // 呉キリカ
        12,                 // 美国織莉子
        15,                 // スコノシュート
        10,                 // 暁美ほむら（弓）
        20,                 // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        7,                  // 魔獣
    };
    // 防御力成長係数
    public static int[] Def_Growth = new int[]
    {
        0,                  // なし        
        3,                  // 鹿目まどか
        3,                  // 美樹さやか
        2,                  // 暁美ほむら（銃）
        2,                  // 巴マミ
        3,                  // 佐倉杏子
        3,                  // 千歳ゆま
        3,                  // 呉キリカ
        2,                  // 美国織莉子
        4,                  // スコノシュート
        2,                  // 暁美ほむら（弓）
        4,                  // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        2                   // 魔獣
    };
    // ブースト量初期値(Lv1の時の値）
    public static int[] Boost_OR = new int[]
    {
        0,                  // なし        
        220,                // 鹿目まどか
        220,                // 美樹さやか
        170,                // 暁美ほむら（銃）
        190,                // 巴マミ
        215,                // 佐倉杏子
        235,                // 千歳ゆま
        215,                // 呉キリカ
        170,                // 美国織莉子
        200,                // スコノシュート
        170,                // 暁美ほむら（弓）
        280,                // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        120,                // 魔獣            
    };
    // ブースト量成長係数
    public static int[] Boost_Growth = new int[]
    {
        0,                  // なし        
        7,                  // 鹿目まどか
        7,                  // 美樹さやか
        5,                  // 暁美ほむら（銃）
        5,                  // 巴マミ
        6,                  // 佐倉杏子
        8,                  // 千歳ゆま
        7,                  // 呉キリカ
        5,                  // 美国織莉子
        3,                  // スコノシュート
        5,                  // 暁美ほむら（弓）
        8,                  // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        5,                  // 魔獣
    };
    
    // 覚醒ゲージ量初期値(LV1の時の値）
    public static int[] Arousal_OR = new int[]
    {
        0,                  // なし        
        180,                // 鹿目まどか
        170,                // 美樹さやか
        170,                // 暁美ほむら（銃）
        175,                // 巴マミ
        170,                // 佐倉杏子
        160,                // 千歳ゆま
        170,                // 呉キリカ
        200,                // 美国織莉子
        170,                // スコノシュート
        170,                // 暁美ほむら（弓）
        200,                // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        120,                // 魔獣
    };
    // 覚醒ゲージ量成長係数
    public static int[] Arousal_Growth = new int[]
    {
        0,                  // なし        
        6,                  // 鹿目まどか
        4,                  // 美樹さやか
        4,                  // 暁美ほむら（銃）
        4,                  // 巴マミ
        4,                  // 佐倉杏子
        4,                  // 千歳ゆま
        4,                  // 呉キリカ
        6,                  // 美国織莉子
        4,                  // スコノシュート
        4,                  // 暁美ほむら（弓）
        8,                  // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        4,                  // 魔獣
    };
    // 撃破時の取得経験値
    public static int[] Exp = new int[]
    {
        0,                   // なし        
        300,                 // 鹿目まどか
        300,                 // 美樹さやか
        300,                 // 暁美ほむら（銃）
        300,                 // 巴マミ
        300,                 // 佐倉杏子
        300,                 // 千歳ゆま
        300,                 // 呉キリカ
        300,                 // 美国織莉子
        600,                 // スコノシュート
        300,                 // 暁美ほむら（弓）
        400,                 // アルティメットまどか（まどかの覚醒とは一応別扱い）
        0,
        2,                   // 魔獣
    };

	/// <summary>
	/// スキル(第1次元がキャラで、第2次元がスキル
	/// ファンネル系の弾速は発射位置に移動するまでの飛行速度であり、射出時の弾速は各キャラで操作すること
	/// </summary>
	public static CharacterSkill[][] cs = new CharacterSkill[][]
    {
        // なし
        new CharacterSkill[]{},
        // 鹿目まどか
        new CharacterSkill[]
        {
            // 射撃
            new CharacterSkill(CharacterSkill.SkillType.SHOT,               "マジカルアロー",               50,5,6,1,2.0f, 1,CharacterSkill.HitType.BEND_BACKWARD,0.3f,50.0f,CharacterSkill.ReloadType.ONE_BY_ONE,2.5f),
            new CharacterSkill(CharacterSkill.SkillType.CHARGE_SHOT,        "スプレッドアロー",             50,5,1,1,2.0f, 3,CharacterSkill.HitType.BEND_BACKWARD,1.0f,50.0f,CharacterSkill.ReloadType.NOTHING,0.0f),
            new CharacterSkill(CharacterSkill.SkillType.SUB_SHOT,           "スターライトアロー",            2,5,2,1,0.1f,10,CharacterSkill.HitType.BEND_BACKWARD,1.0f,70.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,8.0f),
            new CharacterSkill(CharacterSkill.SkillType.EX_SHOT,            "癒しの光",                     50,5,1,1,0.0f, 5,CharacterSkill.HitType.RECOVERY,     1.0f,70.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,10.0f),
            // 格闘
            // N格闘
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_1,          "マジカルスタッフ",             40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f,1.0f,10,2.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_2,          "マジカルスタッフ",             40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,0.0f,1.0f,10,2.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_3,          "マジカルスタッフ",             40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,0.0f,1.0f,10,2.0f),
            // 格闘CS
            new CharacterSkill(CharacterSkill.SkillType.CHARGE_SHOT,        "救済する白き光",               70,5,1,1,0.0f,20,CharacterSkill.HitType.BLOW,          1.0f,50.0f,CharacterSkill.ReloadType.NOTHING,0.0f),
            // 前格闘
            new CharacterSkill(CharacterSkill.SkillType.FRONT_WRESTLE_1,    "スーパーマジカルスタッフ",     50,5,2.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // 左格闘
            new CharacterSkill(CharacterSkill.SkillType.LEFT_WRESTLE_1,     "トゥインクルスタッフ",         45,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f),
            // 右格闘
            new CharacterSkill(CharacterSkill.SkillType.RIGHT_WRESTLE_1,    "トゥインクルスタッフ",         45,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f),
            // 後格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_WRESTLE,       "カリスマガード",                0,0,0.0f,1,CharacterSkill.HitType.GUARD,        1.0f,12.0f),
            // 空中ダッシュ格闘
            new CharacterSkill(CharacterSkill.SkillType.AIRDASH_WRESTLE,    "スーパーまどかキック",         50,5,3.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // 特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_WRESTLE_1,       "浄化の光",                     50,5,1,1,0.0f, 5,CharacterSkill.HitType.CURE,    1.0f,70.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,10.0f),
            // 上特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_FRONT_WRESTLE_1, "パニエロケット",               50,5,3.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // 下特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_EX_WRESTLE,    "急降下まどかキック",           50,5,5.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // アビリティ
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_MENTAL_CONTAMINATION,"精神汚染無効",25),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_DESTRUCTION_MAGIC,"魔力コントロール",30),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_HALLUCINATION,"幻覚無効",35), 
        },
        // 美樹さやか
        new CharacterSkill[]{},
        // 暁美ほむら（銃）
        new CharacterSkill[]{},
        // 巴マミ
        new CharacterSkill[]{},
        // 佐倉杏子
        new CharacterSkill[]{},
        // 千歳ゆま
        new CharacterSkill[]
        {
            // 射撃
            new CharacterSkill(CharacterSkill.SkillType.SHOT,               "ラウンドウェーブ",             60,5,5,1,2.0f, 1,CharacterSkill.HitType.BEND_BACKWARD,0.3f,50.0f,CharacterSkill.ReloadType.ONE_BY_ONE,2.5f),
            new CharacterSkill(CharacterSkill.SkillType.SUB_SHOT,           "リンクスネイル",               50,5,2,1,0.1f,10,CharacterSkill.HitType.BEND_BACKWARD,1.0f,70.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,9.0f),
            new CharacterSkill(CharacterSkill.SkillType.EX_SHOT,            "リバースエナジー",             70,5,1,1,0.0f, 5,CharacterSkill.HitType.RECOVERY,     1.0f,70.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,11.0f),
            // 格闘
            // N格闘
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_1,          "リンクススクラブル",           60,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,1.0f,12.0f,10,2.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_2,          "リンクススクラブル",           60,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,1.0f,0.0f,10,2.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_3,          "リンクススクラブル",           60,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,1.0f,0.0f,10,2.0f),
            // 前格闘
            new CharacterSkill(CharacterSkill.SkillType.FRONT_WRESTLE_1,    "牙突",                         60,5,2.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // 左格闘
            new CharacterSkill(CharacterSkill.SkillType.LEFT_WRESTLE_1,     "ローリングハンマー",           55,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f),
            // 右格闘
            new CharacterSkill(CharacterSkill.SkillType.RIGHT_WRESTLE_1,    "ローリングハンマー",           55,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f),
            // 後格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_WRESTLE,       "ガード",                        0,0,0.0f,1,CharacterSkill.HitType.GUARD,        1.0f,12.0f),
            // 空中ダッシュ格闘
            new CharacterSkill(CharacterSkill.SkillType.AIRDASH_WRESTLE,    "ヘッドバット",                 55,5,3.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // 特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_WRESTLE_1,       "リバースソウル",               50,5,1,1,0.0f, 5,CharacterSkill.HitType.RESUSCITATION,1.0f,70.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,15.0f),
            // 上特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_FRONT_WRESTLE_1, "リンクスサイクロン",           50,5,3.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // 下特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_EX_WRESTLE,    "タッチダウン",                 50,5,5.0f,1,CharacterSkill.HitType.BLOW,         1.0f,12.0f),
            // アビリティ
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_POISON,     "毒無効",20),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_MENTAL_CONTAMINATION,"精神汚染無効",25),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_DESTRUCTION_MAGIC,"魔力コントロール",30), 
        },
        // 呉キリカ
        new CharacterSkill[]{},
        // 美国織莉子
        new CharacterSkill[]{},
        // スコノシュート
        new CharacterSkill[]
        {
            // 射撃
            new CharacterSkill(CharacterSkill.SkillType.SHOT,"マジックビームキャノン",50,5,6,1,2,1,CharacterSkill.HitType.BEND_BACKWARD,0.3f,70.0f,CharacterSkill.ReloadType.ONE_BY_ONE,3.5f,10),
            new CharacterSkill(CharacterSkill.SkillType.SUB_SHOT,"マジックビームボム",70,5,2,1,2,1,CharacterSkill.HitType.BLOW,1.0f,70.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,9.0f,20),
            new CharacterSkill(CharacterSkill.SkillType.EX_SHOT,"マジックイレイザー",50,5,1,1,0.2f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,80.0f,CharacterSkill.ReloadType.ONE_BY_ONE,10.0f,5),
            // 格闘
            // N格闘
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_1,"ビームサーベル",40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f,0.6f,10,1.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_2,"ビームサーベル",40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,5.0f,0.6f,10,1.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_3,"前蹴り",40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,20.0f,0.6f,10,4.0f),
            // 前格闘
            new CharacterSkill(CharacterSkill.SkillType.FRONT_WRESTLE_1,"前蹴り",50,5,2.0f,1,CharacterSkill.HitType.BLOW,1.0f,12.0f,1.0f,15,1.0f),
            // 左格闘
            new CharacterSkill(CharacterSkill.SkillType.LEFT_WRESTLE_1,"回り込みサーベル",45,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f,1.0f,15,1.0f),
            // 右格闘
            new CharacterSkill(CharacterSkill.SkillType.RIGHT_WRESTLE_1,"回り込みサーベル",45,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f,1.0f,15,1.0f),
            // 後格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_WRESTLE,"防御",0,0,0.0f,1,CharacterSkill.HitType.GUARD,1.0f,0.0f),
            // 空中ダッシュ格闘
            new CharacterSkill(CharacterSkill.SkillType.AIRDASH_WRESTLE,"スパイラルアロー",50,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,30.0f,1.0f,15,1.0f),
            // 特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_WRESTLE_1,"浴びせ蹴り",70,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,12.0f,1.0f,30,1.0f),
            // 上特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_FRONT_WRESTLE_1,"リバースシャフトブレイカー",50,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,80.0f,1.0f,15,1.0f),
            // 下特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_EX_WRESTLE,"ヒップアタック",50,5,5.0f,1,CharacterSkill.HitType.BLOW,1.0f,80.0f,1.0f,15,1.0f),
            // アビリティ
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_BLUNT_FOOT,"鈍足無効",1),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_POISON,"毒無効",1),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_MENTALS,"精神系状態異常無効",1),
        },
        // 暁美ほむら（弓）
        new CharacterSkill[]
        {
            // 射撃
            new CharacterSkill(CharacterSkill.SkillType.SHOT,"マジカルアロー",40,5,6,1,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,0.3f,50.0f,CharacterSkill.ReloadType.ONE_BY_ONE, 3.5f,10),
            new CharacterSkill(CharacterSkill.SkillType.CHARGE_SHOT,"アローレーザー",70,5,1,0,1.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,10.0f,CharacterSkill.ReloadType.NOTHING,2.0f,30),
            new CharacterSkill(CharacterSkill.SkillType.SUB_SHOT,"スプレッドアロー",60,5,2,1,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,50.0f,CharacterSkill.ReloadType.IT_ALL_TOGETHER,9.0f,20),
            new CharacterSkill(CharacterSkill.SkillType.EX_SHOT,"アローレイン",50,5,2,1,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,50.0f,CharacterSkill.ReloadType.ONE_BY_ONE,8.0f,20),
            // 格闘
            // N格闘
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_1,"マジカルCQC",40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f,1.0f,10,3.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_2,"マジカルCQC",40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,5.0f,1.0f,10,3.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_3,"マジカルCQC",40,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,20.0f,1.0f,10,3.0f),
            // 前格闘
            new CharacterSkill(CharacterSkill.SkillType.FRONT_WRESTLE_1,"キック",50,5,2.0f,1,CharacterSkill.HitType.BLOW,1.0f,12.0f,1.0f,15,3.0f),
            // 左格闘
            new CharacterSkill(CharacterSkill.SkillType.LEFT_WRESTLE_1,"回し蹴り",45,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f,1.0f,15,1.0f),
            // 右格闘
            new CharacterSkill(CharacterSkill.SkillType.RIGHT_WRESTLE_1,"回し蹴り",45,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,12.0f,1.0f,15,1.0f),
            // 後格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_WRESTLE,"防御",0,0,0.0f,1,CharacterSkill.HitType.GUARD,1.0f,0.0f),
            // 空中ダッシュ格闘
            new CharacterSkill(CharacterSkill.SkillType.AIRDASH_WRESTLE,"タックル",50,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,30.0f,1.0f,15,1.0f),
            // 特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_WRESTLE_1,"浴びせ蹴り",70,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,12.0f,1.0f,30,1.0f),
            // 上特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_FRONT_WRESTLE_1,"昇龍拳",50,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,80.0f,1.0f,15,1.0f),
            // 下特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_EX_WRESTLE,"天魔空刃脚",50,5,5.0f,1,CharacterSkill.HitType.BLOW,1.0f,80.0f,1.0f,15,1.0f),
            // アビリティ
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_BLUNT_FOOT,"鈍足無効",1),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_ROCKON_IMPOSSIBLE,"細心の注意",1),
            new CharacterSkill(CharacterSkill.SkillType.DISABLE_DESTRUCTION_MAGIC,"魔力コントロール",1),
        },
        // アルティメットまどか（まどかの覚醒とは一応別扱い）
        new CharacterSkill[]{},
        // 味方の最大数
        new CharacterSkill[]{},
        // 魔獣
        new CharacterSkill[]
        {
            // 射撃
            new CharacterSkill(CharacterSkill.SkillType.SHOT,"ハンドレーザー",20,5,3,1,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,40.0f,CharacterSkill.ReloadType.ONE_BY_ONE,4.0f),
            // N格闘
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_1,"格闘",20,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,10.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_2,"格闘",20,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,10.0f),
            new CharacterSkill(CharacterSkill.SkillType.WRESTLE_3,"格闘",20,5,2.0f,1,CharacterSkill.HitType.BEND_BACKWARD,1.0f,10.0f),
            // 前格闘
            new CharacterSkill(CharacterSkill.SkillType.FRONT_WRESTLE_1,"跳び膝蹴り",20,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,10.0f),
            // 後格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_WRESTLE,"ガード",0,0,0.0f,1,CharacterSkill.HitType.GUARD,1.0f,0.0f),
            // 空中ダッシュ格闘
            new CharacterSkill(CharacterSkill.SkillType.AIRDASH_WRESTLE,"飛び蹴り",20,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,10.0f),
            // 上特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.EX_FRONT_WRESTLE_1,"ヘッドバット",20,5,3.0f,1,CharacterSkill.HitType.BLOW,1.0f,60.0f),
            // 下特殊格闘
            new CharacterSkill(CharacterSkill.SkillType.BACK_EX_WRESTLE,"鷹爪脚",20,5,5.0f,1,CharacterSkill.HitType.BLOW,1.0f,60.0f), 
        },
    };
}
