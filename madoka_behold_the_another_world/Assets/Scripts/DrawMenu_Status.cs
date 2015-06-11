using UnityEngine;
using System.Collections;

// ステータス画面を描画
public partial class DrawMenu 
{
    // 背景(キャラ全体画像・スイッチ・スキルポイント・ステート背景)
    // キャラ全体画像
    public Texture2D[] m_StatusCharacterGraphic_All;
    // スイッチ(非選択）
    public Texture2D m_StatusSwitch;
    // スイッチ(選択)
    public Texture2D m_StatusSwitch_Enter;
    // スキルポイント
    public Texture2D m_StatusSkillPoint;
    // ステート背景
    public Texture2D m_StatucBackGround;
    // ポップアップ
    public Texture2D m_Popup;
    // ポップアップカーソル
    public Texture2D m_PopupCurosr;
    // スキル背景
    public Texture2D m_SkillBackGround;
    // 現在のモード
    private StatusState m_nowStatusmode;

    // 現在選択中のキャラクター
    private int m_nowSelectCharacter;
    // ステータス選択画面のカーソルの位置
    // (パーティー中の誰を選択したか）
    private int m_nowSelectCursor;
    // (どのステータスを強化するか)
    private int m_nowGrowCursor;
    // 割り振るスキルポイントの量
    private int m_usingSkillPoint;
    // ポップアップでどれを選択しているか
    private int m_PopupSelect;

    // スキル・強化・終了選択のどれを選択しているか
    public enum StatusMode
    {
        SKILL,
        STRENGTH,
        EXIT,

        STATE_TOTAL
    };
    private StatusMode m_nowSelectStatus;
 

    // キャラクター選択用カーソルの基準位置
    private const float CURSORPOS_CHARACTER_X_OR = 350;
    private const float CURSORPOS_CHARACTER_Y_OR = 35;

    // 成長対象選択用カーソルの基準位置
    private const float CURSORPOS_GROWSELECT_X_OR = 420;
    private const float CURSORPOS_GROWSELECT_Y_OR = 160;
    // 成長対象選択用カーソルの選択対象間の幅
    private const float CURSORWIDTH_GROWSELECT = 54;

    // 成長量表示矢印の基準位置
    private const float CURSORPOS_GROWARROW_X_OR = 325;
   
    // ステータス画面のステート
    private enum StatusState
    {
        STATUS_INITIALISE,      // 各ステートを初期化
        TARGETSELECT,           // パーティーメンバーの誰を選択するか
        SLIDEOUT_ROOT,          // 選択を確定し、ルート画面のパーツをスライドアウトする
        SLIDEIN_ROOT,           // ステータスからパーティーメンバー選択に戻ってきたとき、ルート画面のパーツをスライドインする
        SLIDEIN,                // キャラクターのステートを表す部品をスライドインする
        EXIT_STATUS,            // ステータス表示を抜けてTARGETSELECTへ戻る
        STATUS_ROOT,            // ステータス画面のルート(キャラステート表示）
        STATUS_TOSKILL,         // スキル一覧へ移動のために顔グラとステートをスライドアウト
        STATUS_SKILLSLIDEIN,    // スキル一覧をスライドイン
        STATUS_SKILL,           // スキル一覧を表示
        STATUS_SKILLSLIDEOUT,   // スキル一覧をスライドアウト
        STATUS_SKILLBACK,       // 顔グラとステートをスライドイン
        STATUS_GROWSTATUSSELECT,// 成長ステータス選択
        STATUS_GROWSELECT_1,    // 対象ステータスを選択
        STATUS_GROWSELECT_2,    // 選択確定か否かをポップアップで出す
        MAX_STATE               // 最大数
    };
       

    // ステータス画面を描画する
    private void DrawStatus()
    {
        DrawStatusParts();
        DrawSkillSummary();
        switch (m_nowStatusmode)
        {
            case StatusState.STATUS_INITIALISE:
                m_nowSelectCursor = 0;
                m_nowStatusmode = StatusState.TARGETSELECT;
                m_nowSelectStatus = StatusMode.SKILL;
                break;
            case StatusState.TARGETSELECT:
                // カーソルを描画する
                DrawPartyCursor(m_nowSelectCursor);
                // カーソルの移動範囲を選択する
                int maxcursormove = 3;
                // カーソルを制御する
                if (ControlCurosr(ref m_nowSelectCursor, maxcursormove) && savingparameter.GetNowParty(m_nowSelectCursor) != (int)Character_Spec.CHARACTER_NAME.MEMBER_NONE)
                {
                    // 選択確定時の処理
                    m_nowStatusmode = StatusState.SLIDEOUT_ROOT;
                    // 各パーツをスライドイン開始前位置に配置する
                    StatusSetInitPosition();
                    // 現在の選択キャラを確定する
                    m_nowSelectCharacter = savingparameter.GetNowParty(m_nowSelectCursor);
                }
                // 戻るボタンを押したらステータス画面を抜ける
                if (CheckESC())
                {                    
                    // ルート画面へ戻る
                    m_nowSelectMenu = nowSelect_Menu.ROOT;
                }
                break;
            case StatusState.SLIDEOUT_ROOT:     // ルート画面をスライドアウトさせる
                m_menuSelectpos.x -= MENUSELECTMOVESPEED;               // 選択
                m_menuRootCharacterpos.x += MENUSELECTMOVESPEED * 2;    // キャラのステート                
                if (m_menuSelectpos.x < MENUSELECTFIRSTPOS_X)
                {
                    m_nowStatusmode = StatusState.SLIDEIN;
                }
                break;
            case StatusState.SLIDEIN_ROOT:      // ステータスからパーティーメンバー選択に戻ってきたとき、ルート画面のパーツをスライドインする
                m_menuSelectpos.x += MENUSELECTMOVESPEED;               // 選択
                m_menuRootCharacterpos.x -= MENUSELECTMOVESPEED * 2;    // キャラのステート 
                if (m_menuRootCharacterpos.x <= MENUROOT_CHARADATA_X)
                {
                    m_nowStatusmode = StatusState.STATUS_INITIALISE;
                }
                break;
            case StatusState.SLIDEIN:           // スライドイン                
                SlideInStatusParts();
                break;
            case StatusState.STATUS_ROOT:       // スキル、強化、終了を選択させる
                // 選択対象を変更
                int state = (int)m_nowSelectStatus;
                if(ControlCurosr(ref state,3))
                {
                    // スキルを選択
                    if (m_nowSelectStatus == StatusMode.SKILL)
                    {
                        m_nowStatusmode = StatusState.STATUS_TOSKILL;
                    }
                    // 強化を選択
                    else if (m_nowSelectStatus == StatusMode.STRENGTH)
                    {
                        m_nowGrowCursor = 0;
                        m_nowStatusmode = StatusState.STATUS_GROWSTATUSSELECT;
                    }
                    // 終了を選択
                    else if (m_nowSelectStatus == StatusMode.EXIT)
                    {
                        m_nowStatusmode = StatusState.EXIT_STATUS;
                    }
                }
                m_nowSelectStatus = (StatusMode)state;
                break;
            case StatusState.STATUS_TOSKILL:    // スキル一覧へ移動のために顔グラとステートをスライドアウト
                if (SlideOutStateAndFace())
                {
                    m_nowStatusmode = StatusState.STATUS_SKILLSLIDEIN;
                }
                break;
            case StatusState.STATUS_SKILLSLIDEIN:// スキル一覧をスライドイン
                SlidinSKillBG();
                break;
            case StatusState.STATUS_SKILL:      // スキル一覧を表示
                // キャンセルボタンを押したらスライドアウトさせる
                // 戻るボタンを押したらステータス画面を抜ける
                if (CheckESC())
                {                  
                    // ルート画面へ戻る
                    m_nowStatusmode = StatusState.STATUS_SKILLSLIDEOUT;
                }
                break;
            case StatusState.STATUS_SKILLSLIDEOUT:// スキル一覧をスライドアウト
                SlideoutSkillBG();
                break;
            case StatusState.STATUS_SKILLBACK:       // 顔グラとステートをスライドイン
                if (SlideInStateAndFace())
                {
                    m_nowStatusmode = StatusState.STATUS_ROOT;
                }
                break;
            case StatusState.EXIT_STATUS:            // ステータス表示を抜けてTARGETSELECTへ戻る
                SlideOutStatusParts();
                break;
            case StatusState.STATUS_GROWSTATUSSELECT:   // 成長対象を選択
                // カーソルを描画する
                DrawGrowCursor(m_nowGrowCursor);
                if (ControlCurosr(ref m_nowGrowCursor, 5))
                {
                    m_usingSkillPoint = 0;              // 割り振り量初期化
                    m_nowStatusmode = StatusState.STATUS_GROWSELECT_1;
                }
                // 戻るボタンを押したらSTATUS_ROOTへ戻る
                if (CheckESC())
                {
                    m_nowStatusmode = StatusState.STATUS_ROOT;
                }
                break;
            case StatusState.STATUS_GROWSELECT_1:   // 選択すると、Level-XXの横に->XX（現在のLEVEL）と出る
                // 戻るボタンを押したらSTATUS_GROWSTATUSSELECTに戻る
                if (CheckESC())
                {
                    m_nowStatusmode = StatusState.STATUS_GROWSTATUSSELECT;
                }
                // 方向キー左右でポイントの割り振り量を選び、選択確定するとポップアップを出す
                if (ControlCursorHorizon(ref m_usingSkillPoint, savingparameter.GetSkillPoint(m_nowSelectCharacter)) && savingparameter.GetSkillPoint(m_nowSelectCharacter) > 0 && m_usingSkillPoint > 0)
                {
                    m_nowStatusmode = StatusState.STATUS_GROWSELECT_2;
                    m_PopupSelect = 0;
                }
                break;
            case StatusState.STATUS_GROWSELECT_2:   // 選択確定すると、ポップアップ（State6）が出て選択確定の是非を問う
                // 方向キー左右で選択確定か戻るかの選択をする
                if(ControlCursorHorizon(ref m_PopupSelect,2))
                {
                    // OKを選択すると、SavingParameterと該当のキャラクターのステートを書き換える
                    if (m_PopupSelect == 0)
                    {
                        // スキルポイント減算
                        int usingsp = savingparameter.GetSkillPoint(m_nowSelectCharacter) - m_usingSkillPoint;
                        savingparameter.SetSkillPoint(m_nowSelectCharacter, usingsp);
                        // 対象キャラのステートを書き換える
                        switch (m_nowGrowCursor)
                        {
                            case MadokaDefine.STR:  // 攻撃
                                savingparameter.SetStrLevel(m_nowSelectCharacter, savingparameter.GetStrLevel(m_nowSelectCharacter) + m_usingSkillPoint);
                                break;
                            case MadokaDefine.CON:  // 防御
                                savingparameter.SetDefLevel(m_nowSelectCharacter, savingparameter.GetDefLevel(m_nowSelectCharacter) + m_usingSkillPoint);
                                break;
                            case MadokaDefine.VIT:  // 残弾
                                savingparameter.SetBulLevel(m_nowSelectCharacter, savingparameter.GetBulLevel(m_nowSelectCharacter) + m_usingSkillPoint);
                                break;        
                            case MadokaDefine.DEX:  // 覚醒
                                savingparameter.SetArousalLevel(m_nowSelectCharacter, savingparameter.GetArousalLevel(m_nowSelectCharacter) + m_usingSkillPoint);
                                break;
                            case MadokaDefine.AGI:  // ブースト
                                savingparameter.SetBoostLevel(m_nowSelectCharacter, savingparameter.GetBoostLevel(m_nowSelectCharacter) + m_usingSkillPoint);
                                break;
                        }
                        // STATUS_GROWSTATUSSELECTに戻る
                        m_nowStatusmode = StatusState.STATUS_GROWSTATUSSELECT;
                    }
                    // Cancelを選択すると、STATUS_GROWSELECT_1に戻る
                    else
                    {
                        m_nowStatusmode = StatusState.STATUS_GROWSELECT_1;
                    }
                }
                break;
        }
    }



    // カーソルを描画する（パーティー）
    // nowselect            [in]:現在の選択対象
    private void DrawPartyCursor(int nowselect)
    {
        GUI.Label(new Rect(CURSORPOS_CHARACTER_X_OR, CURSORPOS_CHARACTER_Y_OR + nowselect * MENUROOT_OFFSET, 1500, 100), ">", "RootMenu_L");
    }

    // カーソルを描画する（強化対象）
    // nowselect            [in]:現在の選択対象
    private void DrawGrowCursor(int nowselect)
    {
        GUI.Label(new Rect(CURSORPOS_GROWSELECT_X_OR, CURSORPOS_GROWSELECT_Y_OR + nowselect * CURSORWIDTH_GROWSELECT, 1500, 100), ">", "RootMenu_S");
    }
    // STATUSの初期位置
    private const float STATUS_STATUSFIRSTPOS_X = -400.0f;
    private const float STATUS_STATUSFIRSTPOS_Y = 0.0f;

    // STASUSの配置位置
    private Vector2 m_statusTitlePos;
    
    // STATUS関連の変数を初期化する
    private void StatusInitialize()
    {
        m_nowStatusmode = StatusState.STATUS_INITIALISE;
        StatusSetInitPosition();
    }
    // 各パーツをスライドイン開始前位置に配置する
    private void StatusSetInitPosition()
    {
        // 各パーツをスライドイン開始前位置に配置する
        // 文字列（STATUS)
        m_statusTitlePos.x = STATUS_STATUSFIRSTPOS_X;
        m_statusTitlePos.y = STATUS_STATUSFIRSTPOS_Y;
        // キャラクター全身画像
        m_statusCharacterbodyPos.x = STATUS_CHARACTERBODYPOS_X;
        m_statusCharacterbodyPos.y = STATUS_CHARACTERBODYPOS_Y;
        // SkillPoint
        m_statusSkillPointPos.x = STATUS_SKILLPOINTPOS_X;
        m_statusSkillPointPos.y = STATUS_SKILLPOINTPOS_Y;
        // スキル
        m_statusSkillPos.x = STATUS_SKILLPOS_X;
        m_statusSkillPos.y = STATUS_SKILLPOS_Y;
        // 強化
        m_statusStrengthingPos.x = STATUS_STRENGPOS_X;
        m_statusStrengthingPos.y = STATUS_STRENGPOS_Y;
        // 終了
        m_statusExitPos.x = STATUS_EXITPOS_X;
        m_statusExitPos.y = STATUS_EXITPOS_Y;
        // キャラクター背景
        m_statusPlayerPanelPos.x = MENUROOT_CHARADATA_XFIRST;
        m_statusPlayerPanelPos.y = MENUROOT_CHARADATA_Y;
        // 各ステータスレベル
        m_statusLevelPos.x = MENUROOT_CHARADATA_XFIRST;
        m_statusLevelPos.y = STATUSLEVELPOS_Y;
        // スキル背景
        m_SkillBGPos.x = SKILLBG_X;
        m_SkillBGPos.y = SKILLBG_Y;
    }

    // キャラクターの全身画像の配置位置
    private Vector2 m_statusCharacterbodyPos;

    // キャラクターの全身画像の初期位置
    private const float STATUS_CHARACTERBODYPOS_X = -512.0f;
    private const float STATUS_CHARACTERBODYPOS_Y = 0.0f;

    // キャラクターの全身画像のサイズ
    private const float STATUS_CHARACTERBODYSIZE = 512.0f;

    // SkillPointの配置位置
    private Vector2 m_statusSkillPointPos;
    // SkillPointの初期位置
    private const float STATUS_SKILLPOINTPOS_X = -128.0f;
    private const float STATUS_SKILLPOINTPOS_Y = 143.0f;
    // SkillPointのサイズ
    private const float STATUS_SKILLPOINTSIZE_X = 128.0f;
    private const float STATUS_SKILLPOINTSIZE_Y = 64.0f;
    // SkillPointの配置位置
    private const float STATUS_SKILLPOINTPOSSET_X = 285.0f;
    // SkillPointの数値の配置位置(相対)
    private const float STATUS_SKILLPOINTNUMPOS_X = 30.0f;
    private const float STATUS_SKILLPOINTNUMPOS_Y = 36.0f;

    // スキルの配置位置
    private Vector2 m_statusSkillPos;
    // スキルの初期位置
    private const float STATUS_SKILLPOS_X = -128.0f;
    private const float STATUS_SKILLPOS_Y = 221.0f;
    // スキルの文字列の配置位置（相対）
    private const float STATUS_SKILLWORDSPOS_X = 15.0f;
    private const float STATUS_SKILLWORDSPOS_Y = 10.0f;

    // 強化の配置位置
    private Vector2 m_statusStrengthingPos;
    // 強化の初期位置
    private const float STATUS_STRENGPOS_X = -128.0f;
    private const float STATUS_STRENGPOS_Y = 292.0f;
    // 強化の文字列の配置位置（相対）
    private const float STATUS_STRENGWORDSPOS_X = 30.0f;
    private const float STATUS_STRENGWORDSPOS_Y = 10.0f;

    // 終了の配置位置
    private Vector2 m_statusExitPos;
    // 終了の初期位置
    private const float STATUS_EXITPOS_X = -128.0f;
    private const float STATUS_EXITPOS_Y = 367.0f;

    // プレイヤーのパネル(ルートの一人目に合わせる）
    private Vector2 m_statusPlayerPanelPos;
    
    // 各ステータスレベルの配置基準位置
    private Vector2 m_statusLevelPos;
    private const float STATUSLEVELPOS_Y = 168.0f;
    // ステータスレベル描画領域(XY共通）
    private const float STATUSLEVELAREA = 512.0f;
    // パーツ1個当たりのサイズ
    private const float STATUSLEVELSIZE_Y = 32.0f;
    // パーツ間距離
    private const float STATUSOFFSET = 54.0f;

    // スキル背景の配置位置
    private Vector2 m_SkillBGPos;
    // スキル背景の初期位置
    private const float SKILLBG_X = 1025.0f;
    private const float SKILLBG_Y = 9.0f;
    // スキル背景の配置位置
    private const float SKILLBGPOS_X = 399.0f;
    // スキル背景のサイズ
    private const float SKILLBGSIZE = 1024.0f;
    // スキル文字列基準配置位置
    private const float SKILLWORDSPOS_X = 20.0f;
    private const float SKILLWORDSPOS_Y = 20.0f;
    // スキル名基準配置位置
    private const float SKILLNAMEPOS_X = 350.0f;

    // ポップアップ配置位置
    private const float POPUPPOS_X = 384;
    private const float POPUPPOS_Y = 243;
    // ポップアップ文字列配置位置
    private const float POPUPSTRING_X = 35;
    private const float POPUPSTRING_Y = 20;
    // ポップアップサイズ
    private const float POPUPSIZE_X = 256;
    private const float POPUPSIZE_Y = 128;
    // ポップアップカーソル配置位置
    private const float POPUPCURSORLEFT_X  = 38;
    private const float POPUPCUSSORRIGHT_X = 146;
    private const float POPUPCURSOR_Y = 66;
    // カーソルサイズ
    private const float POPUPCURSORSIZE_X = 68;
    private const float POPUPCURSORSIZE_Y = 36;

    // 退避後、必要なパーツをスライドインさせる
    private void SlideInStatusParts()
    {
        // 文字列「STATUS]
        m_statusTitlePos.x += MENUSELECTMOVESPEED;        
        // キャラの全身画像
        m_statusCharacterbodyPos.x += MENUSELECTMOVESPEED + 2.8f;
        // パーツ「Skill Point」
        m_statusSkillPointPos.x += MENUSELECTMOVESPEED + 0.325f;
        // パーツ「スキル」
        m_statusSkillPos.x += MENUSELECTMOVESPEED + 0.325f;
        // パーツ「強化」
        m_statusStrengthingPos.x += MENUSELECTMOVESPEED + 0.325f;
        // パーツ「終了」
        m_statusExitPos.x += MENUSELECTMOVESPEED + 0.325f;
        // プレイヤーキャラのパネル
        m_statusPlayerPanelPos.x -= MENUSELECTMOVESPEED + 9.2f;
        // 各ステートの基準座標
        m_statusLevelPos.x -= MENUSELECTMOVESPEED + 8.2f;

        if (m_statusTitlePos.x > 0)
        {
            m_nowStatusmode = StatusState.STATUS_ROOT;
            m_statusCharacterbodyPos.x = 0.0f;
        }
    }

    // 終了後、必要なパーツをスライドアウトさせる
    private void SlideOutStatusParts()
    {
        // 文字列「STATUS]
        m_statusTitlePos.x -= MENUSELECTMOVESPEED;
        // キャラの全身画像
        m_statusCharacterbodyPos.x -= MENUSELECTMOVESPEED + 2.8f;
        // パーツ「Skill Point」
        m_statusSkillPointPos.x -= MENUSELECTMOVESPEED + 0.325f;
        // パーツ「スキル」
        m_statusSkillPos.x -= MENUSELECTMOVESPEED + 0.325f;
        // パーツ「強化」
        m_statusStrengthingPos.x -= MENUSELECTMOVESPEED + 0.325f;
        // パーツ「終了」
        m_statusExitPos.x -= MENUSELECTMOVESPEED + 0.325f;
        // プレイヤーキャラのパネル
        m_statusPlayerPanelPos.x += MENUSELECTMOVESPEED + 9.2f;
        // 各ステートの基準座標
        m_statusLevelPos.x += MENUSELECTMOVESPEED + 8.2f;

        if (m_statusTitlePos.x < STATUS_STATUSFIRSTPOS_X)
        {           
            m_nowStatusmode = StatusState.SLIDEIN_ROOT;
        }
    }


    // スキル一覧画面をスライドインさせる
    private void SlidinSKillBG()
    {
        m_SkillBGPos.x -= MENUSELECTMOVESPEED + 4.7f;
        
        // 到達位置にきたら移動
        if (m_SkillBGPos.x <= SKILLBGPOS_X)
        {
            m_SkillBGPos.x = SKILLBGPOS_X;
            m_nowStatusmode = StatusState.STATUS_SKILL;
        }
    }

    // スキル一覧画面をスライドアウトさせる
    private void SlideoutSkillBG()
    {
        m_SkillBGPos.x += MENUSELECTMOVESPEED + 4.7f;
        // 到達位置に来たら次のステートへ
        if (m_SkillBGPos.x >= SKILLBG_X)
        {
            m_SkillBGPos.x = SKILLBG_X;
            m_nowStatusmode = StatusState.STATUS_SKILLBACK;
        }
    }

    // 必要なパーツを描画する
    private void DrawStatusParts()
    {
        int nowcharacter = savingparameter.GetNowParty(m_nowSelectCursor);
        // いないキャラのときは処理を飛ばす
        if (nowcharacter == 0)
        {
            return;
        }
        // 文字列「STATUS]
        GUI.Label(new Rect(m_statusTitlePos.x, m_statusTitlePos.y, 1500, 100), "STATUS", "RootMenu_L");
        // キャラの全身画像
        
        GUI.Label(new Rect(m_statusCharacterbodyPos.x, m_statusCharacterbodyPos.y, STATUS_CHARACTERBODYSIZE, STATUS_CHARACTERBODYSIZE), m_StatusCharacterGraphic_All[nowcharacter]);
        // パーツ「Skill Point」
        GUI.BeginGroup(new Rect(m_statusSkillPointPos.x, m_statusSkillPointPos.y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y));
        // 背景
        GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSkillPoint);
        // 数値
        int nowSkillpoint = savingparameter.GetSkillPoint(nowcharacter);
        GUI.Label(new Rect(STATUS_SKILLPOINTNUMPOS_X, STATUS_SKILLPOINTNUMPOS_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), nowSkillpoint.ToString("d3"), "NumberAndEng");
        GUI.EndGroup();
        // パーツ「スキル」
        GUI.BeginGroup(new Rect(m_statusSkillPos.x, m_statusSkillPos.y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y));
        // 背景
        if (m_nowSelectStatus == StatusMode.SKILL)
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch_Enter);
        }
        else
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch);
        }
        GUI.Label(new Rect(STATUS_SKILLWORDSPOS_X, STATUS_SKILLWORDSPOS_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "スキル", "Japanese");
        GUI.EndGroup();
        // パーツ「強化」
        GUI.BeginGroup(new Rect(m_statusStrengthingPos.x, m_statusStrengthingPos.y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y));
        // 背景
        if (m_nowSelectStatus == StatusMode.STRENGTH)
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch_Enter);
        }
        else
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch);
        }
        GUI.Label(new Rect(STATUS_STRENGWORDSPOS_X, STATUS_STRENGWORDSPOS_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "強化", "Japanese");
        GUI.EndGroup();
        // パーツ「終了」
        GUI.BeginGroup(new Rect(m_statusExitPos.x, m_statusExitPos.y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y));
        // 背景
        if (m_nowSelectStatus == StatusMode.EXIT)
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch_Enter);
        }
        else
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch);
        }
        GUI.Label(new Rect(STATUS_STRENGWORDSPOS_X, STATUS_STRENGWORDSPOS_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "終了", "Japanese");
        GUI.EndGroup();

        // 顔パーツ関係
        GUI.BeginGroup(new Rect(m_statusPlayerPanelPos.x, m_statusPlayerPanelPos.y, MENUROOT_SIZEX, MENUROOT_SIZEY));
        // プレイヤーキャラの顔パネル        
        GUI.Label(new Rect(MENUROOT_FACEOFFSET_X, 0, MENUROOT_FACESIZE, MENUROOT_FACESIZE), m_FaceIcons[nowcharacter - 1]);
        // プレイヤーキャラのパネル        
        GUI.Label(new Rect(0, 0, MENUROOT_SIZEX, MENUROOT_SIZEY), m_MenuRoot);       
        // プレイヤーキャラの名前を表示
        DrawText(new Vector2(MENUROOT_NAMEX, MENUROOT_NAMEY), Character_Spec.Name[nowcharacter], "Japanese");
        // プレイヤーキャラのレベルを表示(2桁表示）
        DrawText(new Vector2(MENUROOT_LEVELX, MENUROOT_LEVELY), "LEVEL-" + savingparameter.GetNowLevel(savingparameter.GetNowParty(m_nowSelectCursor)).ToString("d2"), "NumberAndEng");
        // プレイヤーキャラの現在HPを取得して表示（４桁表示）
        DrawText(new Vector2(MENUROOT_HPX, MENUROOT_HPY), savingparameter.GetNowHP(savingparameter.GetNowParty(m_nowSelectCursor)).ToString("d4") + "/" + savingparameter.GetMaxHP(savingparameter.GetNowParty(m_nowSelectCursor)).ToString("d4"), "NumberAndEng"); 
        // プレイヤーキャラの現在覚醒ゲージ量を取得して表示	
        DrawText(new Vector2(MENUROOT_AROUSALX, MENUROOT_AROUSALY), savingparameter.GetNowArousal(savingparameter.GetNowParty(m_nowSelectCursor)).ToString("000.00") + "/" +  savingparameter.GetMaxArousal(savingparameter.GetNowParty(m_nowSelectCursor)).ToString("000.00"), "NumberAndEng");
        // プレイヤーキャラのソウルジェム汚染率を取得して表示
        DrawText(new Vector2(MENUROOT_CONTIMX - 10, MENUROOT_CONTIMY), savingparameter.GetGemContimination(savingparameter.GetNowParty(m_nowSelectCursor)).ToString("000.00") + "%", "NumberAndEng");               
        GUI.EndGroup();
        // プレイヤーのパーツのステートを表示
        GUI.BeginGroup(new Rect(m_statusLevelPos.x,m_statusLevelPos.y,STATUSLEVELAREA,STATUSLEVELAREA));
        for (int i = 0; i < MadokaDefine.STATUSNAME.Length; i++)
        {
            // 背景
            GUI.Label(new Rect(0, i * STATUSOFFSET, STATUSLEVELAREA, STATUSLEVELSIZE_Y), m_StatucBackGround);
            // 文字列
            DrawText(new Vector2(30, i * STATUSOFFSET + 4), MadokaDefine.STATUSNAME[i], "EngWhite");            
        }
        // 各ステートのレベル
        // 攻撃力レベル（STR）
        DrawText(new Vector2(220, 4), "Level-" + savingparameter.GetStrLevel(m_nowSelectCharacter).ToString("d2"), "NumberAndEng"); 
        // 防御力レベル（CON）
        DrawText(new Vector2(220, STATUSOFFSET + 4), "Level-" + savingparameter.GetDefLevel(m_nowSelectCharacter).ToString("d2"), "NumberAndEng");
        // 残弾数レベル（VIT）
        DrawText(new Vector2(220, STATUSOFFSET * 2 + 4), "Level-" + savingparameter.GetBulLevel(m_nowSelectCharacter).ToString("d2"), "NumberAndEng");
        // 覚醒ゲージレベル（DEX）
        DrawText(new Vector2(220, STATUSOFFSET * 3 + 4), "Level-" + savingparameter.GetArousalLevel(m_nowSelectCharacter).ToString("d2"), "NumberAndEng");
        // ブーストゲージレベル(AGI)
        DrawText(new Vector2(220, STATUSOFFSET * 4 + 4), "Level-" + savingparameter.GetBoostLevel(m_nowSelectCharacter).ToString("d2"), "NumberAndEng");
        // スキルポイント割り振り時
        if (m_nowStatusmode == StatusState.STATUS_GROWSELECT_1 || m_nowStatusmode == StatusState.STATUS_GROWSELECT_2)
        {
            // 矢印描画
            DrawText(new Vector2(CURSORPOS_GROWARROW_X_OR, 4 + m_nowGrowCursor * STATUSOFFSET), "->", "NumberAndEng");
            // 割り振り量を描画
            DrawText(new Vector2(CURSORPOS_GROWARROW_X_OR + 30, 4 + m_nowGrowCursor * STATUSOFFSET), m_usingSkillPoint.ToString("d2"), "NumberAndEng");
        }
        GUI.EndGroup();

        // ポップアップ出現時
        if (m_nowStatusmode == StatusState.STATUS_GROWSELECT_2)
        {
            GUI.BeginGroup(new Rect(POPUPPOS_X, POPUPPOS_Y, 256, 256));
            // 背景
            GUI.Label(new Rect(0, 0, POPUPSIZE_X, POPUPSIZE_Y), m_Popup);
            // カーソル
            if (m_PopupSelect == 0)
            {
                GUI.Label(new Rect(POPUPCURSORLEFT_X, POPUPCURSOR_Y, POPUPCURSORSIZE_X, POPUPCURSORSIZE_Y), m_PopupCurosr);
            }
            else
            {
                GUI.Label(new Rect(POPUPCUSSORRIGHT_X, POPUPCURSOR_Y, POPUPCURSORSIZE_X, POPUPCURSORSIZE_Y), m_PopupCurosr);
            }
            // 文字列
            DrawText(new Vector2(POPUPSTRING_X, POPUPSTRING_Y), "これでいいですか？", "Japanese_S");
            GUI.EndGroup();
        }

    }

    // スキル一覧画面のパーツ
    private void DrawSkillSummary()
    {
        if (m_nowStatusmode == StatusState.STATUS_SKILL || m_nowStatusmode == StatusState.STATUS_SKILLSLIDEIN || m_nowStatusmode == StatusState.STATUS_SKILLSLIDEOUT)
        {
            // スキルの背景＆文字列
            GUI.BeginGroup(new Rect(m_SkillBGPos.x, m_SkillBGPos.y, SKILLBGSIZE, SKILLBGSIZE));
            // スキルの背景
            GUI.Label(new Rect(0, 0, SKILLBGSIZE, SKILLBGSIZE), m_SkillBackGround);
            // 文字列
            // 誰を選択しているか判定する
            int character = m_nowSelectCharacter;
            // キャラクターのレベルを取得する(savingparameterの書き変わりが入っていないとアウト）
            int characterlv = savingparameter.GetNowLevel(character);
            // そのキャラクターの習得可能なスキルを取得する
            int skillmax = Character_Spec.cs[character].Length;
            // キャラクターごとに習得しているスキルを描画する
            for (int i = 0; i < skillmax; i++)
            {
                if (characterlv >= Character_Spec.cs[character][i].m_LearningLevel)
                {
                    string skill = DrawSkillName(Character_Spec.cs[character][i].m_Skilltype);
                    DrawText(new Vector2(SKILLWORDSPOS_X, SKILLWORDSPOS_Y+i*20), skill, "Japanese_S");
                    DrawText(new Vector2(SKILLNAMEPOS_X, SKILLWORDSPOS_Y + i * 20), Character_Spec.cs[character][i].m_SkillName, "Japanese_S");
                }
            }
            GUI.EndGroup();
        }
    }
    
    // スキルの種類から文章を生成
    // 第1引数：入力の種類
    // 出力   ：スキルの入力方法
    private string DrawSkillName(CharacterSkill.SkillType skilltype)
    {
        // 入力の種類
        string inputtype = "";
        switch (skilltype)
        {
            // 射撃
            case CharacterSkill.SkillType.SHOT:
            case CharacterSkill.SkillType.SHOT_M2:
                inputtype = "射撃";
                break;
            case CharacterSkill.SkillType.SUB_SHOT:
            case CharacterSkill.SkillType.SUB_SHOT_M2:
                inputtype = "サブ射撃（射撃＋格闘）";
                break;
            case CharacterSkill.SkillType.CHARGE_SHOT:
            case CharacterSkill.SkillType.CHARGE_SHOT_M2:
                inputtype = "チャージ射撃";
                break;
            case CharacterSkill.SkillType.EX_SHOT:
            case CharacterSkill.SkillType.EX_SHOT_M2:
                inputtype = "特殊射撃(射撃＋ブースト)";
                break;
            // 格闘
            case CharacterSkill.SkillType.WRESTLE_1:
            case CharacterSkill.SkillType.WRESTLE_1_M2:
                inputtype = "格闘1段目";
                break;
            case CharacterSkill.SkillType.WRESTLE_2:
            case CharacterSkill.SkillType.WRESTLE_2_M2:
                inputtype = "格闘2段目";
                break;
            case CharacterSkill.SkillType.WRESTLE_3:
            case CharacterSkill.SkillType.WRESTLE_3_M2:
                inputtype = "格闘3段目";
                break;
            case CharacterSkill.SkillType.FRONT_WRESTLE_1:
            case CharacterSkill.SkillType.FRONT_WRESTLE_1_M2:
                inputtype = "↑＋格闘1段目";
                break;
            case CharacterSkill.SkillType.FRONT_WRESTLE_2:
            case CharacterSkill.SkillType.FRONT_WRESTLE_2_M2:
                inputtype = "↑＋格闘2段目";
                break;
            case CharacterSkill.SkillType.FRONT_WRESTLE_3:
            case CharacterSkill.SkillType.FRONT_WRESTLE_3_M2:
                inputtype = "↑＋格闘3段目";
                break;
            case CharacterSkill.SkillType.LEFT_WRESTLE_1:
            case CharacterSkill.SkillType.LEFT_WRESTLE_1_M2:
                inputtype = "←＋格闘1段目";
                break;
            case CharacterSkill.SkillType.LEFT_WRESTLE_2:
            case CharacterSkill.SkillType.LEFT_WRESTLE_2_M2:
                inputtype = "←＋格闘2段目";
                break;
            case CharacterSkill.SkillType.LEFT_WRESTLE_3:
            case CharacterSkill.SkillType.LEFT_WRESTLE_3_M2:
                inputtype = "←＋格闘3段目";
                break;
            case CharacterSkill.SkillType.RIGHT_WRESTLE_1:
            case CharacterSkill.SkillType.RIGHT_WRESTLE_1_M2:
                inputtype = "→＋格闘1段目";
                break;
            case CharacterSkill.SkillType.RIGHT_WRESTLE_2:
            case CharacterSkill.SkillType.RIGHT_WRESTLE_2_M2:
                inputtype = "→＋格闘2段目";
                break;
            case CharacterSkill.SkillType.RIGHT_WRESTLE_3:
            case CharacterSkill.SkillType.RIGHT_WRESTLE_3_M2:
                inputtype = "→＋格闘3段目";
                break;
            case CharacterSkill.SkillType.BACK_WRESTLE:
            case CharacterSkill.SkillType.BACK_WRESTLE_M2:
                inputtype = "↓＋格闘";
                break;
            case CharacterSkill.SkillType.EX_WRESTLE_1:
            case CharacterSkill.SkillType.EX_WRESTLE_1_M2:
                inputtype = "特殊格闘（格闘＋ブースト）1段目";
                break;
            case CharacterSkill.SkillType.EX_WRESTLE_2:
            case CharacterSkill.SkillType.EX_WRESTLE_2_M2:
                inputtype = "特殊格闘（格闘＋ブースト）2段目";
                break;
            case CharacterSkill.SkillType.EX_WRESTLE_3:
            case CharacterSkill.SkillType.EX_WRESTLE_3_M2:
                inputtype = "特殊格闘（格闘＋ブースト）3段目";
                break;
            case CharacterSkill.SkillType.EX_FRONT_WRESTLE_1:
            case CharacterSkill.SkillType.EX_FRONT_WRESTLE_1_M2:
                inputtype = "↑＋特殊格闘（格闘＋ブースト）";
                break;
            case CharacterSkill.SkillType.EX_FRONT_WRESTLE_2:
            case CharacterSkill.SkillType.EX_FRONT_WRESTLE_2_M2:
                inputtype = "↑＋特殊格闘（格闘＋ブースト）2段目";
                break;
            case CharacterSkill.SkillType.EX_FRONT_WRESTLE_3:
            case CharacterSkill.SkillType.EX_FRONT_WRESTLE_3_M2:
                inputtype = "↑＋特殊格闘（格闘＋ブースト）3段目";
                break;
            case CharacterSkill.SkillType.EX_LEFT_WRESTLE_1:
            case CharacterSkill.SkillType.EX_LEFT_WRESTLE_1_M2:
                inputtype = "←＋特殊格闘（格闘＋ブースト）1段目";
                break;
            case CharacterSkill.SkillType.EX_LEFT_WRESTLE_2:
            case CharacterSkill.SkillType.EX_LEFT_WRESTLE_2_M2:
                inputtype = "←＋特殊格闘（格闘＋ブースト）2段目";
                break;
            case CharacterSkill.SkillType.EX_LEFT_WRESTLE_3:
            case CharacterSkill.SkillType.EX_LEFT_WRESTLE_3_M2:
                inputtype = "←＋特殊格闘（格闘＋ブースト）3段目";
                break;
            case CharacterSkill.SkillType.BACK_EX_WRESTLE:
            case CharacterSkill.SkillType.BACK_EX_WRESTLE_M2:
                inputtype = "↓＋特殊格闘（格闘＋ブースト）";
                break;
            case CharacterSkill.SkillType.AIRDASH_WRESTLE:
            case CharacterSkill.SkillType.AIRDASH_WRESTLE_M2:
                inputtype = "空中ダッシュ格闘";
                break;
            // アビリティ
            case CharacterSkill.SkillType.DISABLE_BLUNT_FOOT:
                inputtype = "鈍足無効";
                break;
            case CharacterSkill.SkillType.DISABLE_ROCKON_IMPOSSIBLE:
                inputtype = "ロックオン不可無効";
                break;
            case CharacterSkill.SkillType.DISABLE_DESTRUCTION_MAGIC:
                inputtype = "魔力破壊無効";
                break;
            case CharacterSkill.SkillType.DISABLE_POISON:
                inputtype = "毒無効";
                break;
            case CharacterSkill.SkillType.DISABLE_MENTAL_CONTAMINATION:
                inputtype = "精神汚染無効";
                break;
            case CharacterSkill.SkillType.DISABLE_HALLUCINATION:
                inputtype = "幻覚無効";
                break;
            case CharacterSkill.SkillType.DISABLE_MENTALS:
                inputtype = "精神系ST異常無効";
                break;
            case CharacterSkill.SkillType.AROUSAL_ATTACK:
                inputtype = "覚醒必殺技";
                break;
            default:
                inputtype = "";
                break;
        }
        return inputtype;
    }

    //・スキルを選択するとSkillへ飛ぶ
    //ステートと顔グラフィックをスライドアウト
    // 移動完了後trueを返す
    private bool SlideOutStateAndFace()
    {
        // プレイヤーキャラのパネル
        m_statusPlayerPanelPos.x += MENUSELECTMOVESPEED + 9.2f;
        // 各ステートの基準座標
        m_statusLevelPos.x += MENUSELECTMOVESPEED + 8.2f;
        // 初期位置までたどり着いたら次へ
        if(m_statusPlayerPanelPos.x >= MENUROOT_CHARADATA_XFIRST)
        {
            return true;
        }
        return false;
    }
    
    // ステートと顔グラフィックをスライドイン
    private bool SlideInStateAndFace()
    {
        // プレイヤーキャラのパネル
        m_statusPlayerPanelPos.x -= MENUSELECTMOVESPEED + 9.2f;
        // 各ステートの基準座標
        m_statusLevelPos.x -= MENUSELECTMOVESPEED + 8.2f;
        // 初期位置までたどり着いたら次へ
        if (m_statusPlayerPanelPos.x <= MENUROOT_CHARADATA_X)
        {
            return true;
        }
        return false;
    }
    
    

    // 文字列「STATUS]
    // キャラの全身画像
    // パーツ「Skill Point」
    // パーツ「スキル」
    // パーツ「強化」
    // パーツ「終了」
    // プレイヤーキャラの顔パネル
    // プレイヤーキャラのパネル
    // プレイヤーキャラの名前を表示
    // プレイヤーキャラの現在HPを取得して表示		
    // プレイヤーキャラの現在覚醒ゲージ量を取得して表示	
    // プレイヤーキャラのソウルジェム汚染率を取得して表示
    
}
