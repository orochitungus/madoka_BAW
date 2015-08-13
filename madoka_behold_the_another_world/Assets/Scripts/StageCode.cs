using UnityEngine;
using System.Collections;

public static class StageCode
{
    // 各ステージが何処から来たのかを返す（イベント用ステージは含まない。イベントステージから戻ったときは9999を返す
    // 崩壊見滝原1は例外
    public static int[][] stagefromindex = new int[][]
    {
        // 0:テストステージ
        new int[]{},
        // 1:崩壊見滝原1（プロローグ・崩壊見滝原2）
        new int[]{0, 201},
        // 2:崩壊見滝原2
        new int[]{101,301},
        // 3:イマジカショック・グラウンドゼロ
        new int[]{202},
        // 4:イマジカショック・グラウンドゼロ・VSスコノシュート
        new int[]{302},
        // 5:ほむら病室
        new int[]{601,8888},
        // 6:見滝原病院56階(ほむら病室・プロローグ6・階段・エレベーター）
        new int[]{501,7777,701,603},
        // 7:見滝原病院106階(恭介病室・階段・エレベーター・EX1）
		new int[]{1101,602,603,1102},
		// 8:見滝原病院150階(エレベーター)
		new int[]{603},
		// 9:見滝原病院1階
		new int[]{603,1001},
		// 10:見滝原病院入口
		new int[]{901,1201},
		// 11:恭介病室
    };
    // 各ステージのBGM
    public static string[] stageBGM = new string[]
    {
        // 0:テストステージ
        "",
        // 1:崩壊見滝原1
        "Kanariya",
        // 2:崩壊見滝原2
        "Kanariya",
        // 3:イマジカショック・グラウンドゼロ
        "Kanariya",
        // 4:イマジカショック・グラウンドゼロ・VSスコノシュート
        "Fury",
        // 5:ほむら病室
        "arifuretahitokoma",
        // 6:見滝原病院56階
        "arifuretahitokoma",
        // 7:見滝原病院106階
		"arifuretahitokoma",
		// 8:見滝原病院150階(屋上)
		"arifuretahitokoma",
		// 9:見滝原病院1階
		"arifuretahitokoma",
		// 10:見滝原病院入口
		"arifuretahitokoma",
		// 11:恭介の病室
    };
    // BGM変更の有無
    public static bool[][] changebgm = new bool[][]
    {
        // 0:テストステージ
        new bool[]{false},
        // 1:崩壊見滝原1
        new bool[]{true,false},
        // 2:崩壊見滝原2
        new bool[]{false,false},
        // 3:イマジカショック・グラウンドゼロ
        new bool[]{false},
        // 4:イマジカショック・グラウンドゼロ・VSスコノシュート
        new bool[]{false},
        // 5:ほむら病室
        new bool[]{false,true},
        // 6:見滝原病院56階
        new bool[]{false,true,false,false},
        // 7:見滝原病院106階
		new bool[]{false,false,false,true},
		// 8:見滝原病院150階(屋上)
		new bool[]{false},
		// 9:見滝原病院1階
		new bool[]{false,false},
		// 10:見滝原病院入口
		new bool[]{false,true},
		// 11:恭介の病室
		new bool[]{false}
    };
}
