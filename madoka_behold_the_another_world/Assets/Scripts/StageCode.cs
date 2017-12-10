using UnityEngine;
using System.Collections;

public static class StageCode
{
    // 各ステージが何処から来たのかを返す（イベント用ステージは含まない。イベントステージから戻ったときは9999を返す
    // 崩壊見滝原は例外
    public static int[][] stagefromindex = new int[][]
    {
        // 0:テストステージ
        new int[]{},
        // 1:崩壊見滝原
        new int[]{0},        
        // 2:イマジカショック・グラウンドゼロ・VSスコノシュート
        new int[]{0},
        // 3:ほむら病室
        new int[]{601,8888},
        // 4:見滝原病院56階(ほむら病室・プロローグ6・階段・エレベーター）
        new int[]{501,7777,701,603},
        // 5:見滝原病院106階(恭介病室・階段・エレベーター・EX1）
		new int[]{1101,602,603,1102},
		// 6:見滝原病院150階(エレベーター)
		new int[]{603},
		// 7:見滝原病院1階
		new int[]{603,1001},
		// 8:見滝原病院入口
		new int[]{901,902},
		// 9:見滝原全体マップ
		new int[]{ },
		// 10:歩道橋

		// 11:商店街

		// 12:路地裏
    };
    // 各ステージのBGM
    public static string[] stageBGM = new string[]
    {
        // 0:テストステージ
        "",
        // 1:崩壊見滝原1
        "Kanariya",        
        // 2:イマジカショック・グラウンドゼロ・VSスコノシュート
        "Fury",
        // 3:ほむら病室
        "arifuretahitokoma",
        // 4:見滝原病院56階
        "arifuretahitokoma",
        // 5:見滝原病院106階
		"arifuretahitokoma",
		// 6:見滝原病院150階(屋上)
		"arifuretahitokoma",
		// 7:見滝原病院1階
		"arifuretahitokoma",
		// 8:見滝原病院入口
		"arifuretahitokoma",
		// 9:見滝原全体マップ
		"",
		// 10:歩道橋

		// 11:商店街

		// 12:路地裏
    };
    // BGM変更の有無
    public static bool[][] changebgm = new bool[][]
    {
        // 0:テストステージ
        new bool[]{false},
        // 1:崩壊見滝原
        new bool[]{true},        
        // 2:イマジカショック・グラウンドゼロ・VSスコノシュート
        new bool[]{false},
        // 3:ほむら病室
        new bool[]{false,true},
        // 4:見滝原病院56階
        new bool[]{false,false,false,false},
        // 5:見滝原病院106階
		new bool[]{false,false,false,true},
		// 6:見滝原病院150階(屋上)
		new bool[]{false},
		// 7:見滝原病院1階
		new bool[]{false,false},
		// 8:見滝原病院入口
		new bool[]{false,true},
		// 9:見滝原全体マップ
		new bool[]{true}
		// 10:歩道橋

		// 11:商店街

		// 12:路地裏
    };
}
