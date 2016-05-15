using UnityEngine;
using System.Collections;

// 画面全体を制御するスクリプト
// 解像度はビルド時に16：9で固定すること
[ExecuteInEditMode] // これを入れるとエディット中もスクリプトが有効になる
public partial class titlecamera_control : MonoBehaviour
{   
    // モードのenumを用意(カメラダウン、タイトル画面、モード選択、ロードファイル選択)
    public enum m_modecode 
    {
        INTRO,              // カメラダウン
        TITLESLIDEIN,       // ロゴ＆文字列スライドイン
        TITLE,              // タイトル
        MODESELECT,         // モード選択
        TITLESLIDEOUT,      // ロゴ＆文字列スライドアウト
        LOADPARTSSLIDEIN,   // ロード背景とinformationがスライドイン
        LOAD,               // ロード（背景を黒にする）
        LOADPARTSSLIDOUT,   // ロード背景とinfromationがスライドアウト（ロードをキャンセルしたとき）
        TUTORIALSLIDIN,     // チュートリアル背景がスライドイン
        TUTORIAL,           // チュートリアル
        TUTORIALSLIDOUT     // チュートリアル背景がスライドアウト
    };
    private m_modecode m_animechanged;      // 現在のモード
    private m_modecode m_nextmode;          // 次のモード

    // カメラ１(開幕と同時に降りてくる)
    public GameObject Camera1;
    // カメラ２(タイトル画面が描画された時、くるくるとタワーの周囲を回り全員を映す)
    public GameObject Camera2;

    public float m_Accumulated_time; // 累積時間

    // 1F前の時間
    private float m_oldtime;

    // 現在の時間
    private float m_nowtime;

    // カーソル制御用の時間
    private float m_time;
    // 方向キー連続押下の閾値
    private const float TENKEYBIAS = 0.8f;
    // カーソル移動時の閾値
    private const float MOVECUSRORBIAS = 0.5f;
    // 方向キーの押下開始時間(縦）
    private float m_tenKeyPressTime;
    // 方向キーの押下開始時間(横）
    private float m_tenKeyPressTime_H;
    // ロゴの配置位置
    private float LOGOPOS_X = 24;
    private float LOGOPOS_Y = 0;
    private float LOGOPOS_FIRST = -1000;
    // 文字列の配置位置
    private float TITLEPOS_X = 200;
    private float TITLEPOS_Y = 260;
    private float TITLEPOS_FIRST = 1200;
    private float LOGOSPEED = 20;
    // ロード背景の配置位置
    private const float SAVELOADBACK_X = 85;        // セーブロード背景の最終的な配置位置
    private const float SAVELOADBACK_Y = 60;
    private const float SAVELOADBACK_YFIRST = -400; // セーブロード背景の最初の配置位置
    private const int SAVEFILEPERPAGE = 10;         // 1ページあたりのファイル表示数
    private const float SAVEFILEPOSITION_X = 290;   // 一番上の行に表示されるファイルの位置
    private const float SAVEFILEPOSITION_Y = 65;
    private const float SAVEFILEPOSITION_YFIRST = -395;
    // インフォメーションの配置位置
    // インフォメーションボードの配置位置
    private const float MENUROOT_INFORMATIONPOS_X = 65;
    private const float MENUROOT_INFORMATIONPOS_Y = 425;
    // インフォメーションボードの初期配置位置
    private const float MENUROOT_INFORMATIONFIRSTPOS_Y = 825;
    // インフォメーションボードの「Information」の位置
    private const float MENUROOT_INFORMATION_TITLE_X = 32;
    private const float MENUROOT_INFORMATION_TITLE_Y = 16;
    // インフォメーションボードの説明文の開始位置
    private const float MENUROOT_INFORMATION_MAIN_X = 64;
    private const float MENUROOT_INFORMATION_MAIN_Y = 64;
    // インフォメーションボードのサイズ
    private const float MENUROOT_INFORMATION_X = 1024;
    private const float MENUROOT_INFORMATION_Y = 1024;
    private const float CURSORPOS_X = 260;          // カーソルの配置位置

    // 現在のFPS
    private const float FPS = 60.0f;
    // 現在のモードセレクトのカーソルの位置
    private int m_nowmodeselectcursor;
    // 現在のファイル選択のカーソルの位置
    private int m_nowfileselectcursor;
    // 現在のファイル選択のページ
    private int m_nowfilepage;
    // 決定ボタン押下フラグ
    private bool m_Enterhold;
    // タイトルロゴを入れるので、GUISkinを有効にする
    public GUISkin m_guiskin;

    // 描画対象
    public Texture2D m_TitleLogo;
    // ロード画面の背景
    public Texture2D saveloadbg;
    // 下段のインフォメーション
    public Texture2D m_Information;                              

    // カーソル移動音
    public AudioClip m_MoveCursor;
    // 選択確定音
    public AudioClip m_Enter;
    // タイトルスライドイン完了音
    public AudioClip m_SetTitle;

    //縦方向の解像度
    public float nativeVerticalResolution_h;
    //横方向の解像度
    public float nativeVerticalResolution_w;

    // タイトルロゴの配置位置
    private Vector2 m_logopos;
    // 文字列の配置位置
    private Vector2 m_titlepos;
    // ロード背景の配置位置
    private Vector2 m_saveloadpos;
    // インフォメーションボードの配置位置
    private Vector2 m_informationpos;
    // date.savの中の配列
    public string[] m_date = new string[100];
    private const float SPACEBETWEENLINES = 35;     // ファイル名同士の幅
    private Vector2 m_fileposTop;   // 一番上のファイルの位置    


    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    
	// Use this for initialization
	void Start ()
    {
        // カメラ関連のアニメーションファイルは１個しか登録できないらしい
        // そのためここで呼び制御する       
        // カメラワーク変更フラグを初期化
        m_animechanged = m_modecode.INTRO;
        // FPSを固定
        //Application.targetFrameRate = 60;
        // 一応カメラ2を無効化する
        Camera2.SetActive(false);
        // 累積時間をリセット
        m_Accumulated_time = 0;

        // 過去時間をリセット
        m_oldtime = 0;
        // 現在時間を取得(Time.timeは秒で取得する）
        m_nowtime = 0;

        // ホールド関係を初期化
        m_Enterhold = false;



        // 画面解像度を取得する(固定すること.機体の設定によって異なるため、使用機体に対してどれだけずれているかで判定を行う)
        nativeVerticalResolution_h = 576; // Screen.currentResolution.height;
        nativeVerticalResolution_w = 1024;  // Screen.currentResolution.width;


        // （スクリーンサイズ-画像サイズ）/2*拡大率で決定とはできない（右にずれる）
        // スライドで出入りする各パーツの位置を初期化
        m_logopos.x = LOGOPOS_FIRST;
        m_logopos.y = LOGOPOS_Y;
        m_titlepos.x = TITLEPOS_FIRST;
        m_titlepos.y = TITLEPOS_Y;
        m_saveloadpos.x = SAVELOADBACK_X;
        m_saveloadpos.y = SAVELOADBACK_YFIRST;
        m_informationpos.x = MENUROOT_INFORMATIONPOS_X;
        m_informationpos.y = MENUROOT_INFORMATIONFIRSTPOS_Y;
        m_fileposTop = new Vector2(SAVEFILEPOSITION_X, SAVEFILEPOSITION_YFIRST);

        // ページを初期化
        m_nowfilepage = 0;
        // カーソル位置を初期化
        m_nowfileselectcursor = 0;

        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject am = (GameObject)Instantiate(Resources.Load("AudioManager"));
            am.name = "AudioManager";   // このままだと名前にCloneがつくので消しておく
        }
		// FadeManagerがあるか判定
		if (GameObject.Find("FadeManager") == null)
		{
			// 無ければ作る
			GameObject fadeManager = (GameObject)Instantiate(Resources.Load("FadeManager"));
			fadeManager.name = "FadeManager";
		}
        // LoadManagerがあるか判定
        if (GameObject.Find("LoadManager") == null)
        {
            // 無ければ作る
            GameObject loadManager = (GameObject)Instantiate(Resources.Load("LoadManager"));
            loadManager.name = "LoadManager";
        }
        // PauseManagerがあるか判定
        //if (GameObject.Find("PauseManager") == null)
        //{
        //    // 無ければ作る
        //    GameObject pauseManager = (GameObject)Instantiate(Resources.Load("PauseManager"));
        //    pauseManager.name = "PauseManager";
        //}
        // BGM再生開始（ここに来るとこのBGMに切り替わるので、毎回通す）
        AudioManager.Instance.PlayBGM("Snow");
	}
	
	// Update is called once per frame
	void Update ()
    {
        // カメラが降り切ったらアニメをカメラワークその２（まどかたちを中心にぐるぐる回る）を実行
        // GUIもこれ以降出す
        if (m_Accumulated_time > 5.0f && m_animechanged == m_modecode.INTRO)
        {
            // カメラ１のアニメを止める
            Camera1.SetActive(false);
            // カメラ２を実行する
            Camera2.SetActive(true);
            
            // カメラワーク変更フラグを立てておく
            m_animechanged = m_modecode.TITLESLIDEIN;        

        }
        // タイトルスライドイン
        if (m_animechanged == m_modecode.TITLESLIDEIN)
        {
            if (m_logopos.x < LOGOPOS_X)
            {
                m_logopos.x += LOGOSPEED;
            }
            if (m_titlepos.x > TITLEPOS_X)
            {
                m_titlepos.x -= LOGOSPEED;
            }
            if (m_logopos.x >= LOGOPOS_X && m_titlepos.x <= TITLEPOS_X)
            {
                m_logopos.x = LOGOPOS_X;
                m_titlepos.x = TITLEPOS_X;
                m_animechanged = m_modecode.TITLE;
                AudioSource.PlayClipAtPoint(m_SetTitle, transform.position);
            }
        }
        // タイトルスライドアウト
        else if (m_animechanged == m_modecode.TITLESLIDEOUT)
        {
            if (m_logopos.x > LOGOPOS_FIRST)
            {
                m_logopos.x -= LOGOSPEED;
            }
            if (m_titlepos.x < TITLEPOS_FIRST)
            {
                m_titlepos.x += LOGOSPEED;
            }
            if (m_logopos.x <= LOGOPOS_FIRST && m_titlepos.x <= TITLEPOS_FIRST)
            {
                m_logopos.x = LOGOPOS_FIRST;
                m_titlepos.x = TITLEPOS_FIRST;
                m_animechanged = m_nextmode;
            }
        }
        // メインタイトル
        else if (m_animechanged == m_modecode.TITLE)
        {
            // ここでスペースを押すとモード選択へ移行)
            if (Input.GetButtonDown("Enter"))
            {
                // SE再生
                AudioSource.PlayClipAtPoint(m_Enter, transform.position);
                // 時間を初期化
                m_time = TENKEYBIAS;
                // 押下時間を初期化
                m_tenKeyPressTime = 0.0f;
                m_tenKeyPressTime_H = 0.0f;
                m_nowmodeselectcursor = 1;
                m_animechanged = m_modecode.MODESELECT;
            }
        }
        // 選択モード
        else if (m_animechanged == m_modecode.MODESELECT)
        {
            // カーソルを制御する
            if (ControlCurosr(ref m_nowmodeselectcursor, 4))
            {
                // レベルアップ状態で遷移するとロード先でレベルアップが出てしまうのでここでフラグを折っておく
                LevelUpManagement.m_characterName = 0;
				// 同じ理由でアイテム入手フラグも折っておく
				FieldItemGetManagement.ItemKind = -2;
				FieldItemGetManagement.ItemNum = 0;
                switch (m_nowmodeselectcursor)
                {
                    case 0: // NEW GAME
						FadeManager.Instance.LoadLevel("Prologue", 1.0f);
                        break;
                    case 1: // LOAD GAME
                        // ロードファイルを用意（なければ作る）
                        // saveフォルダがない場合、saveフォルダを作る
                        if (!System.IO.Directory.Exists("save"))
                        { 
                            System.IO.Directory.CreateDirectory(@"save");
                        }
                        // save.savがない場合、save.savを作る
                        if (!System.IO.File.Exists(@"save\save.sav"))
                        {
                            // データ作成
                            string[] savedata = new string[MadokaDefine.SAVEDATANUM];
                            // 以下のように埋める
                            // 01:----/--/--/--:-- ----------------------
                            // 02:----/--/--/--:-- ----------------------
                            for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
                            {
                                savedata[i] = (i + 1).ToString("D3") + ":----/--/-- --:-- ----------------------";
                            }
                            // savedataを保存する
                            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                            //書き込むファイルを開く
                            System.IO.StreamWriter sr = new System.IO.StreamWriter(@"save\save.sav", false, enc);
                            for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
                            {
                                sr.WriteLine(savedata[i]);
                            }
                            sr.Close();
                        }
                        // data.savの中身を読み取り、m_dateの中に入れる
                        // StreamReader の新しいインスタンスを生成する
                        System.IO.StreamReader cReader = (new System.IO.StreamReader(@"save\save.sav", System.Text.Encoding.UTF8));
                        // 読み込みできる文字がなくなるまで繰り返す
                        int index = 0;
                        while (cReader.Peek() >= 0)
                        {
                            // ファイルを 1 行ずつ読み込む
                            m_date[index] = cReader.ReadLine();
                            index++;
                        }
                        // cReader を閉じる
                        cReader.Close();       
                        m_nextmode = m_modecode.LOADPARTSSLIDEIN;
                        m_animechanged = m_modecode.TITLESLIDEOUT;
                        break;
                    case 2: // OPTION
                        break;
                    case 3: // HELP
                        m_nextmode = m_modecode.TUTORIALSLIDIN;
                        m_animechanged = m_modecode.TITLESLIDEOUT;
                        tutorialpage = 0;
                        mTutorialPos.x = TUTORIALXFIRST;
                        mTutorialPos.y = TUTORIALYFIRST;
                        break;
                    default:
                        break;
                }
                if (m_nowmodeselectcursor == 0)
                {
                    return;
                }
            }
        }
        // ロード背景/インフォメーションスライドイン
        else if (m_animechanged == m_modecode.LOADPARTSSLIDEIN)
        {
            // ロード背景
            if (m_saveloadpos.y < SAVELOADBACK_Y)
            {
                m_saveloadpos.y += LOGOSPEED*2;
            }
            // インフォメーションボード
            if (m_informationpos.y > MENUROOT_INFORMATIONPOS_Y)
            {
                m_informationpos.y -= LOGOSPEED*2;
            }
            // 背景の文字列
            if (m_fileposTop.y < SAVEFILEPOSITION_Y)
            {
                m_fileposTop.y += LOGOSPEED * 2;
            }
            if (m_saveloadpos.y >= SAVELOADBACK_Y && m_informationpos.y <= MENUROOT_INFORMATIONPOS_Y)
            {
                m_saveloadpos.y = SAVELOADBACK_Y;
                m_informationpos.y = MENUROOT_INFORMATIONPOS_Y;
                m_animechanged = m_modecode.LOAD;
            }
        }
        // ロードファイル選択画面
        else if (m_animechanged == m_modecode.LOAD)
        {
            // カーソル上下移動
            if(ControlCurosr(ref m_nowfileselectcursor,10))
            {
                // ロード処理開始
                // 対象のファイルが存在するか確認する
                string savefilename = @"save\" + (m_nowfilepage * 10 + m_nowfileselectcursor + 1).ToString("D3") + ".sav";
                // 存在した場合、ロード開始
                if (System.IO.File.Exists(savefilename))
                {
                    // セーブファイルとなるオブジェクト
                    SaveData sd = new SaveData();
                    // 保存したファイルをロード
                    sd = (SaveData)savingparameter.LoadFromBinaryFile(savefilename);
                    // ロードした内容をsavingparameterへ書き込む
                    // ストーリー進行度 
                    savingparameter.story = sd.story;
                    // キャラクターの配置位置
                    savingparameter.nowposition.x = sd.nowposition_x;
                    savingparameter.nowposition.y = sd.nowposition_y;
                    savingparameter.nowposition.z = sd.nowposition_z;
                    // キャラクターの配置角度
                    savingparameter.nowrotation.x = sd.nowrotation_x;
                    savingparameter.nowrotation.y = sd.nowrotation_y;
                    savingparameter.nowrotation.z = sd.nowrotation_z;
                    // アイテムボックスの開閉フラグ
                    for (int i = 0; i < MadokaDefine.NUMOFITEMBOX; ++i)
                    {
                        savingparameter.itemboxopen[i] = sd.itemboxopen[i];
                    }
                    // 現在の所持金
                    savingparameter.nowmoney = sd.nowmoney;
                    // 現在のパーティー(護衛対象も一応パーティーに含める)
                    for (int i = 0; i < 4; ++i)
                    {
                        savingparameter.SetNowParty(i,sd.nowparty[i]);
                    }
                    // キャラクター関連
                    for (int i = 0; i < (int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM; ++i)
                    {
                        // 各キャラのレベル
                        savingparameter.SetNowLevel(i, sd.nowlevel[i]);
                        // 各キャラの現在の経験値
                        savingparameter.SetExp(i, sd.nowExp[i]);
                        // 各キャラのHP
                        savingparameter.SetNowHP(i, sd.nowHP[i]);
                        // 各キャラの最大HP
                        savingparameter.SetMaxHP(i, sd.nowMaxHP[i]);
                        // 各キャラの覚醒ゲージ
                        savingparameter.SetNowArousal(i, sd.nowArousal[i]);
                        // 各キャラの最大覚醒ゲージ
                        savingparameter.SetMaxArousal(i);
                        // 各キャラの攻撃力レベル
                        savingparameter.SetStrLevel(i, sd.StrLevel[i]);
                        // 各キャラの防御力レベル
                        savingparameter.SetDefLevel(i, sd.DefLevel[i]);
                        // 各キャラの残弾数レベル
                        savingparameter.SetBulLevel(i, sd.BulLevel[i]);
                        // 各キャラのブースト量レベル
                        savingparameter.SetBoostLevel(i, sd.BoostLevel[i]);
                        // 覚醒ゲージレベル
                        savingparameter.SetArousalLevel(i, sd.ArousalLevel[i]);
                        // ソウルジェム汚染率
                        savingparameter.SetGemContimination(i, sd.GemContimination[i]);
                        // スキルポイント
                        savingparameter.SetSkillPoint(i, sd.SkillPoint[i]);
                    }
                    // 各アイテムの保持数
                    for (int i = 0; i < Item.itemspec.Length; ++i)
                    {
                        savingparameter.SetItemNum(i, sd.m_numofItem[i]);
                    }
                    // 現在装備中のアイテム
                    savingparameter.SetNowEquipItem(sd.m_nowequipItem);
                    // 現在の場所
                    savingparameter.nowField = sd.nowField;
                    // 前にいた場所
                    savingparameter.beforeField = 9999;
                    // 該当の場所へ遷移する
                    FadeManager.Instance.LoadLevel(SceneName.sceneName[savingparameter.nowField], 1.0f);
                }
            }
            // ページ送り
            ControlPage(ref m_nowfilepage, 10);
            // ESCでタイトルに戻る
            if (CheckESC())
            {
                m_animechanged = m_modecode.LOADPARTSSLIDOUT;
                m_nextmode = m_modecode.TITLESLIDEIN;
            }
        }
        // ロード背景/インフォメーションスライドアウト
        else if (m_animechanged == m_modecode.LOADPARTSSLIDOUT)
        {
            // ロード背景
            if (m_saveloadpos.y > SAVELOADBACK_YFIRST)
            {
                m_saveloadpos.y -= LOGOSPEED * 2;
            }
            // インフォメーションボード
            if (m_informationpos.y < MENUROOT_INFORMATIONFIRSTPOS_Y)
            {
                m_informationpos.y += LOGOSPEED * 2;
            }
            // 背景の文字列
            if (m_fileposTop.y > SAVEFILEPOSITION_YFIRST)
            {
                m_fileposTop.y -= LOGOSPEED * 2;
            }
            if (m_saveloadpos.y <= SAVELOADBACK_YFIRST && m_informationpos.y >= MENUROOT_INFORMATIONFIRSTPOS_Y)
            {
                m_saveloadpos.y = SAVELOADBACK_YFIRST;
                m_informationpos.y = MENUROOT_INFORMATIONFIRSTPOS_Y;
                m_animechanged = m_nextmode;
            }
           
        }
        // チュートリアルスライドイン
        else if(m_animechanged == m_modecode.TUTORIALSLIDIN)
        {
            // チュートリアル背景
            if(mTutorialPos.y > 0)
            {                
                mTutorialPos.y = 0;
                m_animechanged = m_modecode.TUTORIAL;
            }
            else
            {
                mTutorialPos.y += LOGOSPEED * 2;
            }
        }
        // チュートリアルスライドアウト
        else if(m_animechanged == m_modecode.TUTORIALSLIDOUT)
        {
            if(mTutorialPos.y < TUTORIALYFIRST)
            {
                mTutorialPos.y = TUTORIALYFIRST;
                m_animechanged = m_modecode.TITLESLIDEIN;
            }
            else
            {
                mTutorialPos.y -= LOGOSPEED * 2;
            }
        }
        // チュートリアル画面
        else if(m_animechanged == m_modecode.TUTORIAL)
        {
            // ページ送り
            ControlPage(ref tutorialpage, Tutorial.Length);
            // ESCでタイトルに戻る
            if (CheckESC())
            {
                m_animechanged = m_modecode.TUTORIALSLIDOUT;
                m_nextmode = m_modecode.TITLESLIDEIN;
            }
        }

        // 現在時間を取得（初回除く）
        m_nowtime = Time.time;
        // 現在時間と過去時間の差を求め
        // 累積時間をインクリメント
        m_Accumulated_time = m_Accumulated_time + (m_nowtime - m_oldtime);                
        // 現在時間を過去時間に
        m_oldtime = m_nowtime;
        // 時間を加算
        m_time += 1 / FPS;
	}

    // 描画関連
    void OnGUI()
    {
        // GUIスキンを設定
        if (m_guiskin)
        {
            GUI.skin = m_guiskin;
        }
        else
        {
            Debug.Log("No GUI skin has been set!");
        }
        //GUIが解像度に合うように変換行列を設定
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / nativeVerticalResolution_w, Screen.height / nativeVerticalResolution_h, 1));


        // モードがINTRO・MODESELECTの時のみタイトルロゴを描画する
        if (m_animechanged == m_modecode.TITLESLIDEIN || m_animechanged == m_modecode.TITLE || m_animechanged == m_modecode.TITLESLIDEOUT)
        {
            DrawImage(m_logopos, m_TitleLogo);
            DrawText(m_titlepos, "魔法少女まどか☆マギカ　ビホールド・ジ・アナザーワールド");

            // TITLE時のみ文字列を出す
            if (m_animechanged == m_modecode.TITLE)
            {
                DrawText(new Vector2(420, 500), "PRESS SPACE");
            }
        }
        else if (m_animechanged == m_modecode.MODESELECT)
        {
            DrawImage(m_logopos, m_TitleLogo);
            DrawText(m_titlepos, "魔法少女まどか☆マギカ　ビホールド・ジ・アナザーワールド");
            // 文字列を出す
            DrawText(new Vector2(420, 450), "NEW GAME");
            DrawText(new Vector2(420, 480), "LOAD GAME");
            DrawText(new Vector2(420, 510), "OPTION");
            DrawText(new Vector2(420, 540), "HELP");
            // カーソルを出す
            DrawText(new Vector2(390, 450 + 30 * m_nowmodeselectcursor), ">");            
        }
        else if (m_animechanged == m_modecode.LOADPARTSSLIDEIN || m_animechanged == m_modecode.LOADPARTSSLIDOUT || m_animechanged == m_modecode.LOAD)
        {
            DrawImage(m_saveloadpos, saveloadbg);
            // セーブファイル
            for (int i = 0; i < SAVEFILEPERPAGE; ++i)
            {
                DrawText(new Vector2(m_fileposTop.x, m_fileposTop.y + i * SPACEBETWEENLINES), m_date[i + m_nowfilepage * SAVEFILEPERPAGE], "Japanese_S");
            }
            // カーソル
            if (m_animechanged == m_modecode.LOAD)
            {
                DrawText(new Vector2(CURSORPOS_X, m_fileposTop.y + m_nowfileselectcursor * SPACEBETWEENLINES), ">", "Japanese_S");
            }
            DrawInformation(m_informationpos);
        }
        else if(m_animechanged == m_modecode.TUTORIALSLIDIN || m_animechanged == m_modecode.TUTORIAL || m_animechanged == m_modecode.TUTORIALSLIDOUT)
        {
            // チュートリアルページ
            DrawImage(mTutorialPos, Tutorial[tutorialpage]);
            // カーソル
            if (m_animechanged == m_modecode.TUTORIAL)
            {
                DrawImage(new Vector2(0, 300), TutorialArrowLeft);
                DrawImage(new Vector2(960, 300), TutorialArrowRight);
            }
        }
    }

    // 描画の関数
    // 第1引数：配置位置
    // 第2引数：描画対象
    void DrawImage(Vector2 pos, Texture2D image)
    {
        GUIStyle backgroundStyle = new GUIStyle();
        backgroundStyle.normal.background = image;
        GUI.Label(new Rect(pos.x, pos.y, image.width, image.height), image);
        // スタイルを設定すれば配置位置の調整及びリサイズ可能
        // Rectの第3引数が幅、第4引数が高さ
        //GUI.Label(new Rect(pos.x, pos.y, image.width, 256), "", backgroundStyle);
    }

    // 上記のwidth height設定可能版
    void DrawImage(Vector2 pos, Texture2D image, float width, float height)
    {
        GUIStyle backgroundStyle = new GUIStyle();
        backgroundStyle.normal.background = image;
        GUI.Label(new Rect(pos.x, pos.y, image.width, image.height), image);
        // スタイルを設定すれば配置位置の調整及びリサイズ可能
        // Rectの第3引数が幅、第4引数が高さ
        //GUI.Label(new Rect(pos.x, pos.y, image.width, 256), "", backgroundStyle);
    }

    // タイトル文字列
    // 第1引数：配置位置
    // 第2引数：描画する文字列 
    void DrawText(Vector2 pos, string text)
    {
        // 配置位置とスタイルを有効化する。スタイルはGUISkinのStyleの名前
        GUI.Label(new Rect(pos.x, pos.y, 1500, 100), text,"MainTitle"); 
    }

    // 文字列(フォント種類選択可能）
    // 第1引数：配置位置
    // 第2引数：描画する文字列 
    // 第3引数：表示するフォントの種類（セットしているGUISkinのCustomStyles/Nameの名前)
    void DrawText(Vector2 pos, string text, string style)
    {
        // 配置位置とスタイルを有効化する。スタイルはGUISkinのStyleの名前
        GUI.Label(new Rect(pos.x, pos.y, 1500, 100), text, style);
    }

   
     // インフォメーションを描画
    // setpos       [in]:配置基準座標
    private void DrawInformation(Vector2 setpos)
    {
        // 背景を描画
        GUI.DrawTexture(new Rect(setpos.x, setpos.y, MENUROOT_INFORMATION_X, MENUROOT_INFORMATION_Y), m_Information);
        // 文字列を描画
        // Information
        DrawText(new Vector2(setpos.x + MENUROOT_INFORMATION_TITLE_X, setpos.y + MENUROOT_INFORMATION_TITLE_Y), "INFORMATION", "NumberAndEng");
        // 各メニューごとの文章
        string Words = "ロードするファイルを選択してください";
        string Words2 = "";
        DrawText(new Vector2(setpos.x + MENUROOT_INFORMATION_MAIN_X, setpos.y + MENUROOT_INFORMATION_MAIN_Y), Words, "Japanese_S");
        DrawText(new Vector2(setpos.x + MENUROOT_INFORMATION_MAIN_X, setpos.y + MENUROOT_INFORMATION_MAIN_Y + 20), Words2, "Japanese_S");
    }

    // カーソル制御(縦）
    // nowselect        [in/out]:現在選択中の内容
    // maxselect        [in]:最大選択数
    // movecursor       [in]:カーソル移動音再生
    // return           [out]:決定ボタンの押下
    public bool ControlCurosr(ref int nowselect, int maxselect, bool movecursor = true)
    {
        //方向キーの上下でカーソルが移動する
        //押された瞬間下か上へ移動
        //一定時間押しっぱなしで下か上へ移動
        // 下
        if (Input.GetAxis("Vertical") < -MOVECUSRORBIAS && nowselect < maxselect - 1 && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            if (movecursor)
                AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            nowselect++;
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime = m_time;
        }
        // 上
        else if (Input.GetAxis("Vertical") > MOVECUSRORBIAS && nowselect > 0 && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            if (movecursor)
                AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            nowselect--;
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime = m_time;
        }
        // 離れた
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < MOVECUSRORBIAS)
        {
            m_tenKeyPressTime = 0.0f;
        }
        // 選択確定時の処理
        if (!m_Enterhold && (Input.GetButtonDown("Shot") || Input.GetButtonDown("Enter")))
        {
            m_Enterhold = true;
            // SE再生
            AudioSource.PlayClipAtPoint(m_Enter, transform.position);
            return true;
        }
        // 選択解除
        if (m_Enterhold && !Input.GetButtonDown("Shot"))
        {
            m_Enterhold = false;
        }
        return false;
    }

    // カーソル制御（横）
    // nowselect        [in/out]:現在選択中の内容
    // maxselect        [in]:最大選択数
    // useSE            [in]:決定音を出すか出さないか
    // return           [out]:決定ボタンの押下
    public bool ControlCursorHorizon(ref int nowselect, int maxselect, bool useSE = true)
    {
        //方向キーの左右でカーソルが移動する
        //押された瞬間左か右へ移動
        //一定時間押しっぱなしで左か右へ移動
        // 左
        if (Input.GetAxis("Horizontal") < -MOVECUSRORBIAS && nowselect > 0 && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            nowselect--;
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime = m_time;
        }
        // 右
        else if (Input.GetAxis("Horizontal") > MOVECUSRORBIAS && nowselect < maxselect && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            nowselect++;
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime = m_time;
        }
        // 離れた
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) < MOVECUSRORBIAS)
        {
            m_tenKeyPressTime = 0.0f;
        }
        // 選択確定時の処理
        if (!m_Enterhold && (Input.GetButtonDown("Shot") || Input.GetButtonDown("Enter")))
        {
            m_Enterhold = true;
            // SE再生
            if (useSE)
            {
                AudioSource.PlayClipAtPoint(m_Enter, transform.position);
            }
            return true;
        }
        // 選択解除
        if (m_Enterhold && !Input.GetButtonDown("Shot"))
        {
            m_Enterhold = false;
        }
        return false;
    }

    // ページ送り
    // nowpage          [in/out]:現在選択中のページ
    // maxpage          [in]:最大ページ
    public void ControlPage(ref int nowpage, int maxpage)
    {
        //方向キーの左右でカーソルが移動する
        //押された瞬間左か右へ移動
        //一定時間押しっぱなしで左か右へ移動
        //左で減算、右で加算し、0を下回ると最大値に、最大値を上回ると0になる
        // 左
        if (Input.GetAxis("Horizontal") < -MOVECUSRORBIAS && m_time - m_tenKeyPressTime_H > TENKEYBIAS)
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            nowpage--;

            if (nowpage < 0)
            {
                nowpage = maxpage - 1;
            }
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime_H = m_time;
        }
        // 右
        else if (Input.GetAxis("Horizontal") > MOVECUSRORBIAS && m_time - m_tenKeyPressTime_H > TENKEYBIAS)
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            nowpage++;
            if (nowpage > maxpage - 1)
            {
                nowpage = 0;
            }
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime_H = m_time;
        }
        // 離れた
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) < MOVECUSRORBIAS)
        {
            m_tenKeyPressTime_H = 0.0f;
        }
    }

    // ESC選択時の処理
    // return       :ESCボタン押下フラグの成立
    public bool CheckESC()
    {
        if (Input.GetButtonDown("cancel") || Input.GetButtonDown("Jump"))
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            return true;
        }
        return false;
    }
}
