using UnityEngine;
using System.Collections;

// 各ステージのキャラクターの初期配置位置と初期配置角度（多次元配列はインスペクターに置けないのでここに書いてstagesettingで呼び出す）
public static class StagePosition 
{
    // 各ステージのキャラクターの初期配置位置
    public static Vector3[][][] m_InitializeCharacterPos = new Vector3[][][]
    {
        // 0:テストステージ
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(10,0,0),new Vector3(-10,0,0)}
        },
        // 1:崩壊見滝原
        new Vector3[][]
        { 
            new Vector3[]{ new Vector3(254, 0, 145), new Vector3( 244, 0, 145), new Vector3(264, 0, 145 ) },	// プロローグ1から来た場合 
        },
        // 2:イマジカショック・グラウンドゼロ（VSスコノシュート）
        new Vector3[][]
        {
			new Vector3[]{ new Vector3(270, 0, 1163), new Vector3(260,0,1163),new Vector3(280,0,1163)}		// プロローグ2から来た場合
		},
        // 3:見滝原病院・ほむらの病室
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,3.5f),new Vector3(5.0f,0,3.5f),new Vector3(-5.0f,0,3.5f)},       // 56階廊下から来た場合
            new Vector3[]{ new Vector3(0,0,0), new Vector3(5.0f,0,0), new Vector3(-5.0f, 0, 0) }            // プロローグ5から来た場合
        },
        // 4:見滝原病院・56階廊下
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,3.5f),new Vector3(5.0f,0,3.5f),new Vector3(-5.0f,0,3.5f)},       // ほむら病室から来た場合
            new Vector3[]{ new Vector3(26.70615f,0,6.525547f),new Vector3(26.70615f,0,6.525547f),new Vector3(26.70615f,0,6.525547f)},   // プロローグ6から来た場合
            new Vector3[]{ new Vector3(30,0,11), new Vector3(30,0,6), new Vector3(30,0,16) },               // 階段から来た場合
            new Vector3[]{ new Vector3(26,0,36), new Vector3(26,0,31), new Vector3(30,0,41) },              // エレベーターから来た場合
        },
		// 5:見滝原病院・106階廊下
		new Vector3[][]
		{
			new Vector3[]{ new Vector3(0,0,3.5f),new Vector3(5.0f,0,3.5f),new Vector3(-5.0f,0,3.5f)},		// 恭介病室から来た場合
			new Vector3[]{ new Vector3(30,0,11), new Vector3(30,0,6), new Vector3(30,0,16) },               // 階段から来た場合
			new Vector3[]{ new Vector3(26,0.1f,36), new Vector3(26,0,31), new Vector3(30,0,41) },           // エレベーターから来た場合
			new Vector3[]{ new Vector3(0,0,3.5f),new Vector3(5.0f,0,3.5f),new Vector3(-5.0f,0,3.5f)},		// EXエピソードから来た場合
		},
		// 6:見滝原病院・150階(屋上)
		new Vector3[][]
		{
			new Vector3[]{ new Vector3(30,0,0), new Vector3(30,0,-5f), new Vector3(30,0,5f)},	// エレベーターから来た場合
		},
		// 7:見滝原病院・1階
		new Vector3[][]
		{
			new Vector3[]{ new Vector3(9.731555f,0,-27.33751f), new Vector3(4.731555f,0,-27.33751f), new Vector3(14.731555f,0,-27.33751f)},	// エレベーターから来た場合
			new Vector3[]{ new Vector3(24.49534f,0,12.14979f), new Vector3(19.49534f,0,12.14979f), new Vector3(29.49534f,0,12.14979f)},	// 入口から来た場合
		},
		// 8:見滝原病院入口
		new Vector3[][]
		{
			new Vector3[]{ new Vector3(3.787974f,0,0.1080822f), new Vector3(-2.787974f,0,0.1080822f), new Vector3(8.787974f,0,0.1080822f)},	// 1階から来た場合
			new Vector3[]{ new Vector3(-49.40907f,-0.96f,-0.5917053f),new Vector3(-54.40907f,-0.96f,-0.5917053f),new Vector3(-44.40907f,-0.96f,-0.5917053f)},	// 全体マップから来た場合
		},
		// 9:
    };

    // 各ステージのキャラクターの初期配置角度
    public static Vector3[][][] m_InitializeCharacterRot = new Vector3[][][]
    {
        // 0:テストステージ
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)}
        },
        // 1:崩壊見滝原
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)},
            new Vector3[]{ new Vector3(0,180,0), new Vector3(0,180,0), new Vector3(0,180,0)}
        },
        // 2:イマジカショック・グラウンドゼロ（VSスコノシュート）
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)},
        },
        // 3:見滝原病院・ほむらの病室
        new Vector3[][]
        {
            new Vector3[]{new Vector3(0,180.0f,0),new Vector3(0,180.0f,0),new Vector3(0,180.0f,0)}, // 56階廊下から来た場合
            new Vector3[]{new Vector3(0,0.0f,0),new Vector3(0,0.0f,0),new Vector3(0,0.0f,0)}        // プロローグ5から来た場合
        },
        // 4:見滝原病院・56階廊下
        new Vector3[][]
        {
            new Vector3[]{new Vector3(0,0.0f,0), new Vector3(0,0.0f,0), new Vector3(0,0.0f,0)},       // ほむらの病室から来た場合
            new Vector3[]{Vector3.zero,Vector3.zero,Vector3.zero},                                    // プロローグ6から来た場合
            new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)}, // 階段から来た場合
            new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)}, // エレベーターから来た場合
        },
		// 5:見滝原病院・106階廊下
		new Vector3[][]
		{
			new Vector3[]{new Vector3(0,0.0f,0), new Vector3(0,0.0f,0), new Vector3(0,0.0f,0)},			// 恭介の病室から来た場合
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// 階段から来た場合
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// エレベーターから来た場合
			new Vector3[]{new Vector3(0,0.0f,0), new Vector3(0,0.0f,0), new Vector3(0,0.0f,0)},			// EXエピソードから来た場合
		},
		// 6:見滝原病院・150階(屋上)
		new Vector3[][]
		{
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// エレベーターから来た場合
		},
		// 7:見滝原病院・1階
		new Vector3[][]
		{
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// エレベーターから来た場合
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// 入口から来た場合
		},
		// 8:見滝原病院入口
		new Vector3[][]
		{
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// 1階から来た場合
			new Vector3[]{new Vector3(0,90.0f,0), new Vector3(0,90.0f,0), new Vector3(0,90.0f,0)},	// 全体マップから来た場合
		},
		// 9:見滝原病院・恭介の病室
    };
    // 各ステージのステージ名
    public static string[] m_StageName = new string[]
    {
        "テスト",
        "崩壊見滝原",
        "イマジカショック・グラウンドゼロ",
        "見滝原病院・ほむらの病室",
        "見滝原病院・56階廊下",
		"見滝原病院・106階廊下",
		"見滝原病院・150階",
		"見滝原病院・1階",
		"見滝原病院入口",
		"見滝原病院・恭介の病室",
    };

	// 戦闘ステージの場合の目標文言
	public static string[] Purpose = new string[]
	{
		"",
		"先に進め！",
		"謎の魔法少女を倒せ！",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
	};

}
