using UnityEngine;
using System.Collections;

// 全画面共通の固定値
public class MadokaDefine : SingletonMonoBehaviour<MadokaDefine>
{
    // Destroyさせない
    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
    }

	/// <summary>
	/// ラウンドコール時のカウント
	/// </summary>
	public const float ROUNDCALLCOUNT = 3;
    
	// 画面サイズ
    public static float SCREENWIDTH = 1024.0f;
    public static float SCREENHIGET = 576.0f;

    // 落下速度
    public static float FALLSPEED = -4.9f;
    // 落下速度（クエストパート用）
    public static float FALLSPEEDQUEST = -9.8f;

    // 最大メンバー数(NONEをカウントしている点に注意）
    public static int MAXMEMBER = 12;

    // 1パーティーの最大メンバー
    public const int MAXPARTYMEMBER = 3;

    // ステートの名前(略号)
    public static string[] STATUSNAME = new string[]
    {
        "STR",  // 攻撃
        "CON",  // 防御
        "VIT",  // 残弾
        "DEX",  // 覚醒
        "AGI",  // ブースト
    };
    // 上記のDEFINE
    public const int STR = 0;
    public const int CON = 1;
    public const int VIT = 2;
    public const int DEX = 3;
    public const int AGI = 4;

    // アイテム所持上限数
    public const int MAXITEM = 20;
    // アイテムエフェクト有効時間
    public const float ITEMEFFECTTIME = 2.0f;
    // ダメージエフェクト有効時間
    public const float DAMEGEEFFECTIME = 1.3f;

    // ダウン値デフォルト閾値
    public const float DOWNRATIO = 5.0f;

    // ダメージ時打ち上げ量
    public const float LAUNCHOFFSET = 10.0f;

    // 誤射時の補正率
    public const float FRENDLYFIRE_RATIO = 4.0f;

    // 覚醒時の攻撃力上昇補正
    public const float AROUSAL_OFFENCE_UPPER = 1.2f;

    // 覚醒時の防御力上昇補正
    public const float AROUSAL_DEFFENSIVE_UPPER = 0.8f;

    // ソウルバースト時ソウルジェム汚染率増加量
    public const float SOULBURST_CONF = 25.0f;

    // レベルアップ時のスキルポイント増加量
    public const int SKILLPOINTINCREASE = 2;

    // セーブデータの数
    public const int SAVEDATANUM = 100;

    // アイテムボックスの総数
    public const int NUMOFITEMBOX = 9999;

    // 初期所持金
    public const int FIRSTMONEY = 1000;

    // レベルアップテーブル
    public static int[] LEVELUPEXP = new int[]
    {
        0,
        4,
        9,
        16,
        25,
        36,
        49,
        64,
        81,
        100,
        121,
        144,
        169,
        196,
        225,
        256,
        289,
        324,
        361,
        400,
        441,
        484,
        529,
        576,
        625,
        676,
        729,
        784,
        841,
        900,
        961,
        1024,
        1089,
        1156,
        1225,
        1296,
        1369,
        1444,
        1521,
        1600,
        1681,
        1764,
        1849,
        1936,
        2025,
        2116,
        2209,
        2304,
        2401,
        2500,
        2601,
        2704,
        2809,
        2916,
        3025,
        3136,
        3249,
        3364,
        3481,
        3600,
        3721,
        3844,
        3969,
        4096,
        4225,
        4356,
        4489,
        4624,
        4761,
        4900,
        5041,
        5184,
        5329,
        5476,
        5625,
        5776,
        5929,
        6084,
        6241,
        6400,
        6561,
        6724,
        6889,
        7056,
        7225,
        7396,
        7569,
        7744,
        7921,
        8100,
        8281,
        8464,
        8649,
        8836,
        9025,
        9216,
        9409,
        9604,
        9801,
    };
}
