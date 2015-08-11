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
        // 1:崩壊見滝原1
        new Vector3[][]
        { 
            new Vector3[]{ new Vector3(1033, 149, 523), new Vector3( 1028, 149, 523), new Vector3(1038, 149, 523 ) },
            new Vector3[]{ new Vector3(1120, 143, 1899), new Vector3(1125, 143, 1899),new Vector3(1115, 143, 1899) }
        },   
        // 2:崩壊見滝原2
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(1014,142,2107), new Vector3(1009,142,2107), new Vector3(1019,142,2107)},
            new Vector3[]{ new Vector3(971,142,3907), new Vector3(976,142,3907), new Vector3(966,142,3907)}
        },
        // 3:イマジカショック・グラウンドゼロ
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(965,142,4110), new Vector3(960,142,4110),new Vector3(970,142,4110)}
        },
        // 4:イマジカショック・グラウンドゼロ（VSスコノシュート）
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(979,141,4811), new Vector3(974,141,4811), new Vector3(984,141,4811)}
        },
        // 5:見滝原病院・ほむらの病室
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,3.5f),new Vector3(5.0f,0,3.5f),new Vector3(-5.0f,0,3.5f)},       // 56階廊下から来た場合
            new Vector3[]{ new Vector3(0,0,0), new Vector3(5.0f,0,0), new Vector3(-5.0f, 0, 0) }            // プロローグ5から来た場合
        },
        // 6:見滝原病院・56階廊下
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,3.5f),new Vector3(5.0f,0,3.5f),new Vector3(-5.0f,0,3.5f)},       // ほむら病室から来た場合
            new Vector3[]{ new Vector3(26.70615f,0,6.525547f),new Vector3(26.70615f,0,6.525547f),new Vector3(26.70615f,0,6.525547f)},   // プロローグ6から来た場合
            new Vector3[]{ new Vector3(30,0,11), new Vector3(30,0,6), new Vector3(30,0,16) },               // 階段から来た場合
            new Vector3[]{ new Vector3(26,0,36), new Vector3(26,0,31), new Vector3(30,0,41) },              // エレベーターから来た場合
        },
		// 7:見滝原病院・106階廊下
		new Vector3[][]
		{
			new Vector3[]{ new Vector3(0,0,3.5f),new Vector3(5.0f,0,3.5f),new Vector3(-5.0f,0,3.5f)},		// 恭介病室から来た場合
			new Vector3[]{ new Vector3(30,0,11), new Vector3(30,0,6), new Vector3(30,0,16) },               // 階段から来た場合
			new Vector3[]{ new Vector3(26,0,36), new Vector3(26,0,31), new Vector3(30,0,41) },              // エレベーターから来た場合
		},
		// 8:見滝原病院・150階(屋上)

		// 9:見滝原病院・1階

		// 10:見滝原病院入口

		// 11:見滝原病院・恭介の病室
    };

    // 各ステージのキャラクターの初期配置角度
    public static Vector3[][][] m_InitializeCharacterRot = new Vector3[][][]
    {
        // 0:テストステージ
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)}
        },
        // 1:崩壊見滝原1
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)},
            new Vector3[]{ new Vector3(0,180,0), new Vector3(0,180,0), new Vector3(0,180,0)}
        },
        // 2:崩壊見滝原2
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)},
            new Vector3[]{ new Vector3(0,180,0), new Vector3(0,180,0), new Vector3(0,180,0)}
        },
        // 3:イマジカショック・グラウンドゼロ
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)},
        },
        // 4:イマジカショック・グラウンドゼロ（VSスコノシュート）
        new Vector3[][]
        {
            new Vector3[]{ new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)},
        },
        // 5:見滝原病院・ほむらの病室
        new Vector3[][]
        {
            new Vector3[]{new Vector3(0,180.0f,0),new Vector3(0,180.0f,0),new Vector3(0,180.0f,0)}, // 56階廊下から来た場合
            new Vector3[]{new Vector3(0,0.0f,0),new Vector3(0,0.0f,0),new Vector3(0,0.0f,0)}        // プロローグ5から来た場合
        },
        // 6:見滝原病院・56階廊下
        new Vector3[][]
        {
            new Vector3[]{new Vector3(0,0.0f,0), new Vector3(0,0.0f,0), new Vector3(0,0.0f,0)},       // ほむらの病室から来た場合
            new Vector3[]{Vector3.zero,Vector3.zero,Vector3.zero},                                    // プロローグ6から来た場合
            new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)}, // 階段から来た場合
            new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)}, // エレベーターから来た場合
        },
		// 7:見滝原病院・106階廊下
		new Vector3[][]
		{
			new Vector3[]{new Vector3(0,0.0f,0), new Vector3(0,0.0f,0), new Vector3(0,0.0f,0)},			// 恭介の病室から来た場合
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// 階段から来た場合
			new Vector3[]{new Vector3(0,270.0f,0), new Vector3(0,270.0f,0), new Vector3(0,270.0f,0)},	// エレベーターから来た場合
		},
		// 8:見滝原病院・150階(屋上)

		// 9:見滝原病院・1階

		// 10:見滝原病院入口

		// 11:見滝原病院・恭介の病室
    };
    // 各ステージのステージ名
    public static string[] m_StageName = new string[]
    {
        "テスト",
        "崩壊見滝原１",
        "崩壊見滝原２",
        "イマジカショック・グラウンドゼロ",
        "ISGZ VS Sconosciuto",
        "見滝原病院・ほむらの病室",
        "見滝原病院・56階廊下",
		"見滝原病院・106階廊下",
		"見滝原病院・150階",
		"見滝原病院・1階",
		"見滝原病院入口",
		"見滝原病院・恭介の病室",
    };
}
