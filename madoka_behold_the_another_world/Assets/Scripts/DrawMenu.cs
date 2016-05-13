using UnityEngine;
using System.Collections;

// メニュー画面を描画する
//ルート
//ポーズ状態で起動
//・背景を描画	→OK
//・メニューパネルの中身を描画
//・開始時、各パネルに文字や絵を記述かつ画面端に配置
//・配置完了後、スライドイン
//・左側にはメニュー選択、右側には描画パネル、下側にはインフォメーションパネルを配置
public partial class DrawMenu : MonoBehaviour 
{
    // 背景
    public Texture2D    m_BackGroundofMenu;
    public GUISkin      m_Guiskin;                                  // GUISKIN
    public GameObject   Player;   // カメラが映すオブジェクト
    public Texture2D[]  m_FaceIcons;                                // メニュールートの顔アイコン
    public Texture2D    m_MenuRoot;                                 // メニュールート画面の各キャラの背景  
    public Texture2D    m_Information;                              // 下段のインフォメーション          
    // 描画フラグ
    private bool m_isDraw = false;
    // 決定ボタン押下フラグ（ポーズをかけている関係上、コントローラーで一瞬押し判定がとれないため）
    private bool m_Enterhold;
    // キャンセルボタン押下フラグ
    private bool m_Eschold;

    // 各パーツの座標
    private Vector2     m_menuSelectpos;                            // メニュー選択画面の配置座標 
    private Vector2 m_menuRootCharacterpos;                         // メニュールート画面の一人目のキャラクターの座標
    private Vector2 m_Informationpos;                               // インフォメーションボードの配置座標
    // メニュー選択画面の初期配置位置
    // Vector2はconst不可
    //private const Vector2 MENUSELECTFIRSTPOS = new Vector2(-300.0f, 0.0f);
    private const float MENUSELECTFIRSTPOS_X = -400.0f;
    private const float MENUSELECTFIRSTPOS_Y = 0.0f;
    // スライドイン/アウト時の移動速度
    private const float MENUSELECTMOVESPEED = 10.0f;

    // メニュー選択の文字の幅
    private const float MENUSELCTOFFSET = 50.0f; 
    // カーソル移動時の閾値
    private const float MOVECUSRORBIAS = 0.5f;
    // 方向キー連続押下の閾値
    private const float TENKEYBIAS = 0.8f;
    // 方向キーの押下開始時間(縦）
    private float m_tenKeyPressTime;
    // 方向キーの押下開始時間(横）
    private float m_tenKeyPressTime_H;
    // 方向キーカウント用の時間
    private float m_time;
    // ルートメニューにおけるキャラクター背景のオフセット値
    private const int MENUROOT_OFFSET = 150;
    // ルートメニューにおける基準座標からのキャラフェイスのオフセット値
    private const float MENUROOT_FACEOFFSET_X = 14;
    private const float MENUROOT_FACEOFFSET_Y = 0;
    // ルートメニューにおける一番上のキャラデータの配置座標
    private const float MENUROOT_CHARADATA_XFIRST = 1200;
    private const float MENUROOT_CHARADATA_X = 400;
    private const float MENUROOT_CHARADATA_Y = 0;
    // インフォメーションボードのサイズ
    private const float MENUROOT_INFORMATION_X = 1024;
    private const float MENUROOT_INFORMATION_Y = 1024;
    
    // ルートメニューにおけるキャラクターフェイスのサイズ(XY共通）
    private const float MENUROOT_FACESIZE = 128;
    // ルートメニューにおけるキャラクター背景のサイズ
    private const float MENUROOT_SIZEX = 512;
    private const float MENUROOT_SIZEY = 256;
    // ルートメニューにおける名前の位置
    private const float MENUROOT_NAMEX = 180;
    private const float MENUROOT_NAMEY = 4;
    // ルートメニューにおけるレベルの位置
    private const float MENUROOT_LEVELX = 360;
    private const float MENUROOT_LEVELY = 8;
    // ルートメニューにおけるHPの位置
    private const float MENUROOT_HPX = 300;
    private const float MENUROOT_HPY = 44;
    // ルートメニューにおける覚醒ゲージの位置
    private const float MENUROOT_AROUSALX = 300;
    private const float MENUROOT_AROUSALY = 74;
    // ルートメニューにおけるソウルジェム汚染率の位置
    private const float MENUROOT_CONTIMX = 405;
    private const float MENUROOT_CONTIMY = 106;
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

    // カーソル移動音
    public AudioClip m_MoveCursor;
    // 選択確定音
    public AudioClip m_Enter;
    // 回復音
    // HP
    public AudioClip RebirthHP1;
    // 蘇生
    public AudioClip Regenaration;
    // SG浄化
    public AudioClip PurificationSG;


    // 状態遷移管理
    private enum nowProc
    {
        INITIALIZE,                 // 初期状態(メニューから抜けたらここへ戻す）
        SLIDEIN,                    // スライドイン
        NORMAL,                     // 通常状態
        SLIDEOUT,                   // スライドアウト
        END,                        // 終了状態
        PROC_TOTAL
    }
    
    //どこを選択しているかというフラグを立てる
    //フラグの立った文字のみ大きいフォントにする
    public enum nowSelect_Menu
    {
        ROOT,                       // 初期画面
        STATUS,                     // ステータス
        ITEM,                       // アイテム
        SKILL,                      // スキル
        SYSTEM,                     // システム
        PARTY,                      // パーティー
        SAVE,                       // セーブ
        LOAD,                       // ロード
        EXIT,                       // 終了
        MENU_TOTAL
    }
    // 上記のenumに対応する配列
    private string[] m_menuList = new string[]
    {
        "",                         // 初期画面
        "STATUS",                   // ステータス
        "ITEM",                     // アイテム
        "SKILL",                    // スキル
        "SYSTEM",                   // システム
        "PARTY",                    // パーティー
        "SAVE",                     // セーブ
        "LOAD",                     // ロード
        "EXIT",
    };
             
    // 現在選択中の箇所
    private nowSelect_Menu  m_nowSelectMenu;    
    // 現在のカーソル位置
    private nowSelect_Menu  m_nowCursorPos;
    // 現在の状態遷移
    private nowProc         m_nowProc;
    // 現在のFPS
    private const float FPS = 60.0f;
   

    // ポーズコントローラー
    private GameObject m_pausecontroller;

    // 初回か否か
    private bool m_isfirst;

    // 描画命令制御
    // code     [in]:trueで描画、falseで描画カット
    public void DrawMenuActive(bool code)
    {
        m_isDraw = code;
    }

    // メニュー画面を描画する
	// Use this for initialization
	void Start () 
    {
        // ステート初期化
        m_nowCursorPos = nowSelect_Menu.STATUS;
        // ステータス画面関連を初期化
        m_nowSelectCursor = 0;
        // ホールド関係を初期化
        m_Enterhold = false;
        m_Eschold = false;
        // ポーズコントローラー取得
        m_pausecontroller = GameObject.Find("Pause Controller");
        m_isfirst = true;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    // 対象を取得する.
        var master = Player.GetComponentInChildren<CharacterControl_Base>();                // 戦闘用
        var master_quest = Player.GetComponentInChildren<CharacterControl_Base_Quest>();    // クエストパート用

        // 戦闘用
        if (master != null)
        {
            if (master.IsPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
            {
                return;
            }
            if (master.m_timstopmode == CharacterControl_Base.TimeStopMode.PAUSE)
            {
                // これをやっておかないとループする
                if (!m_isDraw)
                {
                    m_isDraw = true;
                    // メニュー内容を初期化
                    m_nowSelectMenu = nowSelect_Menu.ROOT;
                    // カーソルの位置を初期化
                    m_nowCursorPos = nowSelect_Menu.STATUS;
                    // 遷移状態を初期化
                    m_nowProc = nowProc.INITIALIZE;
                    // 時間を初期化
                    m_time = TENKEYBIAS;
                    // 押下時間を初期化
                    m_tenKeyPressTime = 0.0f;
                    m_tenKeyPressTime_H = 0.0f;
                }
            }
            else
            {
                m_isDraw = false;
            }
        }
        // クエストパート用
        else
        {
            if (master_quest.m_isPlayer != CharacterControl_Base_Quest.CHARACTERCODE.PLAYER)
            {
                return;
            }
            if (master_quest.m_timstopmode == CharacterControl_Base_Quest.TimeStopMode.PAUSE)
            {
                // これをやっておかないとループする
                if (!m_isDraw)
                {
                    m_isDraw = true;
                    // メニュー内容を初期化
                    m_nowSelectMenu = nowSelect_Menu.ROOT;
                    // カーソルの位置を初期化
                    m_nowCursorPos = nowSelect_Menu.STATUS;
                    // 遷移状態を初期化
                    m_nowProc = nowProc.INITIALIZE;
                    // 時間を初期化
                    m_time = TENKEYBIAS;
                    // 押下時間を初期化
                    m_tenKeyPressTime = 0.0f;
                    m_tenKeyPressTime_H = 0.0f;
                }
            }
            else
            {
                m_isDraw = false;
            }
        }
	}

    
    
    // 基本やることはここでのみ
    void OnGUI()
    {
        var master = Player.GetComponentInChildren<CharacterControl_Base>();
        var master_quest = Player.GetComponentInChildren<CharacterControl_Base_Quest>();

        // 戦闘用
        if (master != null)
        {
            if (master.IsPlayer != CharacterControl_Base.CHARACTERCODE.PLAYER)
            {
                return;
            }
        }
        // クエストパート用
        if (master_quest != null)
        {
            if (master_quest.m_isPlayer != CharacterControl_Base_Quest.CHARACTERCODE.PLAYER)
            {
                return;
            }
        }

        // GUIスキンを設定
        if (m_Guiskin)
        {
            GUI.skin = m_Guiskin;
        }

        if (m_isDraw)
        {            
            // リサイズ用のスケールを決定
            Vector3 scale = new Vector3(Screen.width / MadokaDefine.SCREENWIDTH, Screen.height / MadokaDefine.SCREENHIGET, 1);
            // GUIが解像度に合うように変換行列を設定(先にTRSの第1引数を0で埋めていくとリサイズにも対応可能ということらしい）
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);            
            GUI.BeginGroup(new Rect(0, 0, MadokaDefine.SCREENWIDTH, MadokaDefine.SCREENHIGET));
            // 背景を描画
            GUI.DrawTexture(new Rect(0.0f, 0.0f, MadokaDefine.SCREENWIDTH, MadokaDefine.SCREENHIGET), m_BackGroundofMenu);            
            GUI.EndGroup();
            // 開始時、各パネルに文字や絵を記述かつ画面端に配置
            // 配置完了後、スライドイン
            // 左側にはメニュー選択画面、右側には描画パネル、下側にはインフォメーションパネルを配置
            // 終了後スライドアウト
            switch (m_nowProc)
            {
                case nowProc.INITIALIZE:
                    // 開始時、各パネルに文字や絵を記述かつ画面端に配置(informationは消えないので初回のみ）
                    if (m_isfirst)
                    {
                        MenuInitialize();
                        m_isfirst = false;
                    }
                    else
                    {
                        MenuInitialize(false);
                    }
                    StatusInitialize();
                    SkillInitialize();
                    ItemInitialize();
                    m_nowProc = nowProc.SLIDEIN;
                    break;
                case nowProc.SLIDEIN:
                    // 配置完了後、スライドイン
                    m_menuSelectpos.x += MENUSELECTMOVESPEED;               // 選択
                    m_menuRootCharacterpos.x -= MENUSELECTMOVESPEED * 2;    // キャラのステート
                    if (m_isfirst)
                    {
                        m_Informationpos.y -= MENUSELECTMOVESPEED;              // インフォメーションボード
                    }
                    if (m_menuSelectpos.x > 0)
                    {
                        m_nowProc = nowProc.NORMAL;
                        m_menuSelectpos = Vector2.zero;
                        m_menuRootCharacterpos.x = MENUROOT_CHARADATA_X;
                        m_Informationpos.y = MENUROOT_INFORMATIONPOS_Y;
                    }
                    break;
                case nowProc.NORMAL:
                    // 左側にはメニュー選択画面、右側には描画パネル、下側にはインフォメーションパネルを配置
                    // 各メニューを操作
                    ControlMenu();
                    break;
                case nowProc.SLIDEOUT:
                    // 終了時、各パーツをスライドアウト
                    m_menuSelectpos.x -= MENUSELECTMOVESPEED;               // 選択
                    m_menuRootCharacterpos.x += MENUSELECTMOVESPEED * 2;    // キャラのステート
                    m_Informationpos.y += MENUSELECTMOVESPEED;              // インフォメーションボード
                    if (m_menuSelectpos.x < MENUSELECTFIRSTPOS_X)
                    {
                        m_nowProc = nowProc.END;
                    }
                    break;
                case nowProc.END:
                    // 描画命令をカット
                    m_isDraw = false;
                    // 対象の時間停止をカット
                    if(master != null)
                        master.ReleasePause();
                    if (master_quest != null)
                        master_quest.ReleasePause();
                    // 一時停止されている全てのポーズを解除する
                    var pausecontroller2 = m_pausecontroller.GetComponent<PauseControllerInputDetector>();
                    pausecontroller2.pauseController.DeactivatePauseProtocol();
                    // 位置固定を解除する
                    if(master != null)
                        master.UnFreezePositionAll();
                    if (master_quest != null)
                        master_quest.UnFreezePositionAll();
                    break;
            }                  

            // メニュー選択画面を描画
            GUI.BeginGroup(new Rect(0, 0, MadokaDefine.SCREENWIDTH, MadokaDefine.SCREENHIGET));
            DrawMenuSelect(m_nowCursorPos, m_menuSelectpos);
            GUI.EndGroup();
            // ルート画面を描画
            GUI.BeginGroup(new Rect(0, 0, MadokaDefine.SCREENWIDTH, MadokaDefine.SCREENHIGET));
            // パーティー
            int[] nowparty = new int[MadokaDefine.MAXPARTYMEMBER];
            for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
            {
                nowparty[i] = (int)savingparameter.GetNowParty(i);
            }
            DrawRootMenu(m_menuRootCharacterpos, nowparty);
            GUI.EndGroup();
            // インフォメーションボードを描画
            GUI.BeginGroup(new Rect(0, 0, 1024, 1024));
            DrawInformation(m_Informationpos, m_nowCursorPos);
            GUI.EndGroup();

            // 時間を加算
            m_time += 1/FPS;
        }
    }

    // 各メニューを操作する
    private void ControlMenu()
    {
        switch (m_nowSelectMenu)
        {
            case nowSelect_Menu.ROOT:
                // ルート画面カーソル制御
                ControlSelectMenuCursor(ref m_nowCursorPos);               
                break;
            case nowSelect_Menu.STATUS:
                // ステータス画面制御
                DrawStatus();
                break;
            case nowSelect_Menu.ITEM:
                // アイテム画面制御
                DrawItem();
                break;
            case nowSelect_Menu.SKILL:
                // スキル画面制御
                DrawSkill();
                break;
            case nowSelect_Menu.PARTY:
                break;
            case nowSelect_Menu.SYSTEM:
                break;
            case nowSelect_Menu.SAVE:
                // セーブ画面制御
                DrawSave();
                break;
            case nowSelect_Menu.LOAD:
                // ロード画面制御
                DrawLoad();
                break;
            case nowSelect_Menu.EXIT:
                break;
        }
        // ESCボタンホールド解除フラグ
        if (!Input.GetButton("Jump") && m_Eschold)
        {
            m_Eschold = false;
        }
    }

    // メニュー呼び出し時に各ステートを初期化する
    // informationboadapper[in]:下段のinformationをスライドインさせるか否か
    private void MenuInitialize(bool informationbodeappear = true)
    {
        m_isDraw = true;
        // メニュー内容を初期化
        m_nowSelectMenu = nowSelect_Menu.ROOT;
        // カーソルの位置を初期化
        m_nowCursorPos = nowSelect_Menu.STATUS;
        // 遷移状態を初期化
        m_nowProc = nowProc.INITIALIZE;
        // メニュー選択の位置を初期化
        m_menuSelectpos = new Vector2(MENUSELECTFIRSTPOS_X,MENUSELECTFIRSTPOS_Y);
        // ルート画面のキャラクターの位置を初期化
        m_menuRootCharacterpos = new Vector2(MENUROOT_CHARADATA_XFIRST, MENUROOT_CHARADATA_Y);         
        
        // インフォメーションボードの配置位置を初期化
        if (informationbodeappear)
        {
            m_Informationpos.x = MENUROOT_INFORMATIONPOS_X;
            m_Informationpos.y = MENUROOT_INFORMATIONFIRSTPOS_Y;
        }
    }
    
    // メニュー選択を描画(左側にある選択画面）
    // nowselect        [in]:現在選択中の内容
    // setpos           [in]:配置座標左上
    private void DrawMenuSelect(nowSelect_Menu select,Vector2 setpos)
    {
        for (int i = (int)nowSelect_Menu.STATUS; i < (int)nowSelect_Menu.MENU_TOTAL; i++)
        {
            // 選択状態のものだけ大きくしてアンカーを描画し、それ以外は小さくする(ROOTのときはStatusに配置）
            if (i == (int)select)
            {
                GUI.Label(new Rect(setpos.x, setpos.y + MENUSELCTOFFSET * (i - 1), 1500, 100), ">" + m_menuList[i], "RootMenu_L");
            }
            else
            {
                GUI.Label(new Rect(setpos.x, setpos.y + MENUSELCTOFFSET * (i - 1), 1500, 100), m_menuList[i], "RootMenu_S");
            }
        }
    }
    // 方向キーの上下でカーソルが移動する
    // nowselect        [in/out]:現在選択中の内容
    private void ControlSelectMenuCursor(ref nowSelect_Menu select)
    {
        //方向キーの上下でカーソルが移動する
        //押された瞬間下か上へ移動
        //一定時間押しっぱなしで下か上へ移動
        // 下
        if (Input.GetAxis("Vertical") < -MOVECUSRORBIAS && select < nowSelect_Menu.MENU_TOTAL - 1 && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            select++;
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime = m_time;
        }
        // 上
        else if(Input.GetAxis("Vertical") > MOVECUSRORBIAS && select > nowSelect_Menu.STATUS && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            select--;
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime = m_time;
        }
        // 離れた
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < MOVECUSRORBIAS)
        {
            m_tenKeyPressTime = 0.0f;
        }

       
        if (!m_Enterhold && (Input.GetButtonDown("Pause") || Input.GetButton("Shot") || Input.GetButtonDown("Enter")))    // ジョイスティック系は押しっぱなしでとらないと駄目(ポーズを使っている関係上）
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_Enter, transform.position);
            // コントローラーを使う場合ホールドフラグON
            m_Enterhold = true;
            switch (select)
            {
                case nowSelect_Menu.EXIT:       // EXIT選択時は抜けてポーズを解除
                    // ステートをスライドアウトへ
                    m_nowProc = nowProc.SLIDEOUT;                    
                    break;
                case nowSelect_Menu.STATUS:     // ステータス選択時
                    // ステートをステータスへ
                    StatusInitialize();
                    m_nowSelectMenu = nowSelect_Menu.STATUS;
                    break;
                case nowSelect_Menu.SKILL:      // スキル選択時
                    SkillInitialize();
                    m_nowSelectMenu = nowSelect_Menu.SKILL;
                    break;
                case nowSelect_Menu.ITEM:       // アイテム選択時
                    ItemInitialize();
                    m_nowSelectMenu = nowSelect_Menu.ITEM;
                    break;
                case nowSelect_Menu.SAVE:       // セーブ選択時
                    SaveInitialize();
                    m_nowSelectMenu = nowSelect_Menu.SAVE;
                    break;
                case nowSelect_Menu.LOAD:       // ロード選択時
                    LoadInitialize();
                    m_nowSelectMenu = nowSelect_Menu.LOAD;
                    break;
                default:                        // それ以外は各メニューへ
                    break;
            }
        }
        // 離すとホールドフラグ解除
        if (m_Enterhold && !Input.GetButton("Shot"))
        {
            m_Enterhold = false;
        }
    }    

    // カーソル制御（汎用）
    // nowselect        [in/out]:現在選択中の内容
    // maxselect        [in]:最大選択数
    // movecursor       [in]:カーソル移動音再生
    // return           [out]:決定ボタンの押下
    public bool ControlCurosr(ref int nowselect, int maxselect,bool movecursor = true)
    {
        //方向キーの上下でカーソルが移動する
        //押された瞬間下か上へ移動
        //一定時間押しっぱなしで下か上へ移動
        // 下
        if (Input.GetAxis("Vertical") < -MOVECUSRORBIAS && nowselect < maxselect - 1 && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            if(movecursor)
                AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            nowselect++;
            // 一定時間押しっぱなしで再度下か上へ移動し、押下時間をリセット（一回押されたら次の判定まで待つ）
            m_tenKeyPressTime = m_time;
        }
        // 上
        else if (Input.GetAxis("Vertical") > MOVECUSRORBIAS && nowselect > 0 && m_time - m_tenKeyPressTime > TENKEYBIAS)
        {
            // SE再生
            if(movecursor)
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
        if (!m_Enterhold && (Input.GetButton("Shot") || Input.GetButtonDown("Enter"))) 
        {
            m_Enterhold = true;
            // SE再生
            AudioSource.PlayClipAtPoint(m_Enter, transform.position);
            return true;
        }
        // 選択解除
        if (m_Enterhold && !Input.GetButton("Shot"))
        {
            m_Enterhold = false;
        }
        return false;
    }

    // ページ送り
    // nowpage          [in/out]:現在選択中のページ
    // maxpage          [in]:最大ページ
    public void ControlPage(ref int nowpage,int maxpage)
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
        if (Input.GetButtonDown("cancel") || Input.GetButton("Jump") && !m_Eschold)
        {
            // SE再生
            AudioSource.PlayClipAtPoint(m_MoveCursor, transform.position);
            m_Eschold = true;
            return true;
        }
        return false;
    }

    // カーソル制御（横汎用）
    // nowselect        [in/out]:現在選択中の内容
    // maxselect        [in]:最大選択数
    // useSE            [in]:決定音を出すか出さないか
    // return           [out]:決定ボタンの押下
    public bool ControlCursorHorizon(ref int nowselect, int maxselect,bool useSE = true)
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
        if (!m_Enterhold && (Input.GetButton("Shot") || Input.GetButtonDown("Enter")))
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
        if (m_Enterhold && !Input.GetButton("Shot"))
        {
            m_Enterhold = false;
        }
        return false;
    }

    

    // ルートメニューを描画
    // setpos           [in]:配置基準座標
    // nowparty         [in/array]:現在のパーティー
    private void DrawRootMenu(Vector2 setpos, int []nowparty)
    {
        for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
        {
            // NONEを拾ったら強制抜け
            if (nowparty[i] != (int)Character_Spec.CHARACTER_NAME.MEMBER_NONE)
            {
                // プレイヤーキャラの顔パネルを表示
                GUI.DrawTexture(new Rect(setpos.x + MENUROOT_FACEOFFSET_X, setpos.y + MENUROOT_FACEOFFSET_Y + MENUROOT_OFFSET * i,
                    MENUROOT_FACESIZE, MENUROOT_FACESIZE), m_FaceIcons[nowparty[i] - 1]);   // アイコンは0スタートだがCHARACTER_NAMEの0はNONEだから 
                // プレイヤーキャラのパネルを表示
                GUI.DrawTexture(new Rect(setpos.x, setpos.y + MENUROOT_OFFSET * i, MENUROOT_SIZEX, MENUROOT_SIZEY), m_MenuRoot);
                // プレイヤーキャラの名前を表示
                DrawText(new Vector2(setpos.x + MENUROOT_NAMEX, MENUROOT_NAMEY + MENUROOT_OFFSET * i), Character_Spec.Name[nowparty[i]], "Japanese");
                // プレイヤーキャラのレベルを表示(2桁表示）
                DrawText(new Vector2(setpos.x + MENUROOT_LEVELX, MENUROOT_LEVELY + MENUROOT_OFFSET * i), "LEVEL-" + savingparameter.GetNowLevel(nowparty[i]).ToString("d2"), "NumberAndEng"); 
                // プレイヤーキャラの現在HPを取得して表示（４桁表示）
                DrawText(new Vector2(setpos.x + MENUROOT_HPX, MENUROOT_HPY + MENUROOT_OFFSET * i), savingparameter.GetNowHP(nowparty[i]).ToString("d4") + "/" + savingparameter.GetMaxHP(nowparty[i]).ToString("d4"), "NumberAndEng"); 
                // プレイヤーキャラの現在覚醒ゲージ量を取得して表示
                DrawText(new Vector2(setpos.x + MENUROOT_AROUSALX, MENUROOT_AROUSALY + MENUROOT_OFFSET * i), savingparameter.GetNowArousal(nowparty[i]).ToString("000.00") + "/" + savingparameter.GetMaxArousal(nowparty[i]).ToString("000.00"), "NumberAndEng");
                // ソウルジェム汚染率
                DrawText(new Vector2(setpos.x + MENUROOT_CONTIMX, MENUROOT_CONTIMY + MENUROOT_OFFSET * i), savingparameter.GetGemContimination(nowparty[i]).ToString("000.00") + "%", "NumberAndEng");
            }
        }
    }

    // インフォメーションを描画
    // setpos       [in]:配置基準座標
    // select       [in]:現在のカーソルの位置
    private void DrawInformation(Vector2 setpos, nowSelect_Menu select)
    {
        // 背景を描画
        GUI.DrawTexture(new Rect(setpos.x, setpos.y, MENUROOT_INFORMATION_X, MENUROOT_INFORMATION_Y), m_Information);
        // 文字列を描画
        // Information
        DrawText(new Vector2(setpos.x + MENUROOT_INFORMATION_TITLE_X, setpos.y + MENUROOT_INFORMATION_TITLE_Y), "INFORMATION", "NumberAndEng"); 
        // 各メニューごとの文章
        string Words = "";
        string Words2 = "";
        // 選択モードがルート状態
        if (m_nowSelectMenu == nowSelect_Menu.ROOT)
        {
            switch (select)
            {
                case nowSelect_Menu.STATUS:
                    Words = "キャラクターのステータスを成長または確認します";
                    break;
                case nowSelect_Menu.ITEM:
                    Words = "アイテムを使用します。また、装備するアイテムを変更できます";
                    break;
                case nowSelect_Menu.PARTY:
                    Words = "パーティーを編成します";
                    break;
                case nowSelect_Menu.SKILL:
                    Words = "スキルの確認を行います";
                    break;
                case nowSelect_Menu.SYSTEM:
                    Words = "ゲームの設定を変更します";
                    break;
                case nowSelect_Menu.SAVE:
                    Words = "データをセーブします";
                    break;
                case nowSelect_Menu.LOAD:
                    Words = "データをロードします";
                    break;
                case nowSelect_Menu.EXIT:
                    Words = "ゲームに戻ります";
                    break;
            }
        }
        // 選択モードがステータス状態
        else if (m_nowSelectMenu == nowSelect_Menu.STATUS)
        {
            switch (m_nowStatusmode)
            {
                case StatusState.TARGETSELECT:
                    Words = "対象キャラクターを選択してください";
                    break;
                case StatusState.STATUS_ROOT:       // スキル、強化、終了を選択させる
                    if (m_nowSelectStatus == StatusMode.SKILL)
                    {
                        Words = "習得しているスキルを確認します";
                    }
                    else if (m_nowSelectStatus == StatusMode.STRENGTH)
                    {
                        Words = "スキルポイントを使用して、キャラクターを強化します";
                    }
                    else
                    {
                        Words = "ステータス画面を終了します";
                    }
                    break;
                case StatusState.STATUS_SKILL:
                    Words = "使用可能なスキル一覧です";
                    break;
                case StatusState.STATUS_GROWSTATUSSELECT:
                    Words = "スキルポイントを使用して成長させたいステートを選択してください";
                    switch (m_nowGrowCursor)
                    {
                        case 0:     // 攻撃力レベル（STR）
                            Words2 = "キャラクターの攻撃力が上がります。回復技の場合は回復量が上がります";
                            break;
                        case 1:     // 防御力レベル（CON）
                            Words2 = "キャラクターの防御力が上がり、受けるダメージを減らします";
                            break;
                        case 2:     // 残弾数レベル（VIT）
                            Words2 = "キャラクターの武装の最大数が上がります";
                            break;
                        case 3:     // 覚醒ゲージレベル（DEX）
                            Words2 = "キャラクターのMagicゲージの最大量が増加します";
                            break;
                        case 4:     // ブーストゲージレベル(AGI)
                            Words2 = "キャラクターのBoostゲージの最大量が増加します";
                            break;
                    }
                    break;
                case StatusState.STATUS_GROWSELECT_1:
                case StatusState.STATUS_GROWSELECT_2:
                    Words = "方向キー左右で使用するスキルポイントの量を設定してください";
                    break;

            }
        }
        // 選択モードがアイテム状態
        else if (m_nowSelectMenu == nowSelect_Menu.ITEM)
        {
            switch (m_nowItemState)
            {
                case ItemState.SELECTITEMMODE:
                    Words = "アイテムを使用するか装備するか選択できます";
                    if (m_nowItemMode == ItemMode.USE)
                    {
                        Words2 = "使用するアイテムを選択します";
                    }
                    else if (m_nowItemMode == ItemMode.EQUIP)
                    {
                        Words2 = "装備するアイテムを選択します";
                    }
                    else if (m_nowItemMode == ItemMode.EXIT)
                    {
                        Words2 = "ルートメニューに戻ります";
                    }
                    break;
                case ItemState.ITEMUSESELECT:                    
                    Words = "使用するアイテムを選択してください";
                    Words2 = Item.itemspec[m_selectItemCursor].Information();
                    break;
                case ItemState.ITEMUSECHECK:                
                    Words = "誰に使用するか選択してください";
                    break;
                case ItemState.ITEMUSECHECK_ALL:
                    Words = "パーティー全員に使用します";
                    break;
                case ItemState.ITEMUSECHECK_FINAL:
                    Words = "これでよろしいですか？";
                    break;
                case ItemState.EQUIPSELECT:
                    Words = "装備するアイテムを選択してください";
                    Words2 = Item.itemspec[m_selectItemCursor].Information();
                    break;
                case ItemState.EQUIPSELECT_CHECK:
                    Words = "これでよろしいですか？";
                    break;

            }
        }
        // 選択モードがセーブ状態
        else if (m_nowSelectMenu == nowSelect_Menu.SAVE)
        {
            switch (m_nowsavestate)
            {
                case SaveState.INITIALIZE:                   
                case SaveState.SLIDEOUT_BEFORE:
                case SaveState.SLIDEIN:
                    Words = Words2 = "";
                    break;
                case SaveState.SELECT:
                    Words = "セーブする場所を選んでください";
                    Words2 = "方向キー左右でページを変えられます";
                    break;
                case SaveState.CHECKSELECT:
                case SaveState.CHECKSELECTFINAL:
                    break;
                case SaveState.SAVE:
                    Words = "セーブしています";
                    Words2 = "";
                    break;
                case SaveState.SLIDEOUT:
                    Words = Words2 = "";
                    break;
            }
        }
        DrawText(new Vector2(setpos.x + MENUROOT_INFORMATION_MAIN_X, setpos.y + MENUROOT_INFORMATION_MAIN_Y), Words, "Japanese_S");
        DrawText(new Vector2(setpos.x + MENUROOT_INFORMATION_MAIN_X, setpos.y + MENUROOT_INFORMATION_MAIN_Y + 20), Words2, "Japanese_S");
    }

   

    // 文字列
    // 第1引数：配置位置
    // 第2引数：描画する文字列 
    // 第3引数：表示するフォントの種類（セットしているGUISkinのCustomStyles/Nameの名前)
    void DrawText(Vector2 pos, string text, string style)
    {
        // 配置位置とスタイルを有効化する。スタイルはGUISkinのStyleの名前
        GUI.Label(new Rect(pos.x, pos.y, 1500, 100), text, style);
    }
}


