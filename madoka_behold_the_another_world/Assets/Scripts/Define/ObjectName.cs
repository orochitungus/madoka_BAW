using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectName
{
	// ロードするキャラクターのファイル名(ルート・戦闘用）
	public static string[] CharacterFileName = new string[]
	{
		"",                         // 0  なし
        "",                         // 1  鹿目まどか
        "",                         // 2  美樹さやか
        "",                         // 3  暁美ほむら（銃）
        "",                         // 4  巴マミ
        "",                         // 5  佐倉杏子
        "",                         // 6  千歳ゆま
        "",                         // 7  呉キリカ
        "",                         // 8  美国織莉子
        "SconosciutoBattleUse",		// 9  スコノシュート（PC)
        "HomuraBowBattleUse",		// 10 暁美ほむら（弓）
        "",                         // 11 アルティメットまどか（まどかの覚醒とは一応別扱い）
		"majyu_use_battle",			// 12 魔獣
    };

	// ロードするキャラクターのファイル名(ルート・クエストパート用）
	public static string[] CharacterFileName_Quest = new string[]
	{
		"",                                 // なし
        "",                                 // 鹿目まどか
        "",                                 // 美樹さやか
        "Homura_Quest_Use",                 // 暁美ほむら（銃）
        "",                                 // 巴マミ
        "",                                 // 佐倉杏子
        "",                                 // 千歳ゆま
        "",                                 // 呉キリカ
        "",                                 // 美国織莉子
        "SconosciutoBattleUse",             // スコノシュート（PC)
        "Homura_Quest_Use",                 // 暁美ほむら（弓）
        "",                                 // アルティメットまどか（まどかの覚醒とは一応別扱い）
		"majyu_use_battle",					// 魔獣
	};
}
