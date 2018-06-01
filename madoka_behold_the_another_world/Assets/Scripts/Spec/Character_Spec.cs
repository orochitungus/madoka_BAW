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
        // 敵キャラ(NPC)
        ENEMY_MAJYU,        // 魔獣
		MEMBER_DEVIL_HOMURA,// 悪魔ほむら
		MEMBER_NAGISA,		// 百江なぎさ
		MEMBER_SAYAKA_GODSIBB,	// 円環のさやか
		MEMBER_MICHEL,		// ミッチェル
		ENEMY_MICHAELA1,	// ミヒャエラ（ムンク)
		ENEMY_MICHAELA2,    // ミヒャエラ（人間）
		ENEMY_IZABEL,		// イザベル（芸術家の魔女）


		CHARACTER_ALL_NUM,  // 全キャラ
    };


	
}
