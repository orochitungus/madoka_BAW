using UnityEngine;
using System.Collections;

public partial class DrawMenu
{
    //・現在のモード（初期化・元からあったものをスライドアウト・スライドイン・選択・選択確認・ロード・スライドアウト）
    private enum LoadState
    {
        INITIALIZE,                 // 初期化
        SLIDEOUT_BEFORE,            // 元からあったものをスライドアウト
        SLIDEIN,                    // スライドイン
        SELECT,                     // 選択
        CHECKSELECT,                // 選択確認
        LOAD,                       // ロード実行処理
        SLIDEOUT,                   // スライドアウト
    };
    private LoadState m_nowloadState;

    // ロード画面のステートを初期化
    private void LoadInitialize()
    {
        m_savewordpos = new Vector2(SAVE_XFIRST, SAVE_Y);
        m_saveloadpos = new Vector2(SAVELOADBACK_X, SAVELOADBACK_YFIRST);
        m_nowloadState = LoadState.INITIALIZE;
        m_fileposTop = new Vector2(SAVEFILEPOSITION_X, SAVEFILEPOSITION_YFIRST);
    }

    private void DrawLoad()
    {
        // 文字列「LOAD」
        GUI.Label(new Rect(m_savewordpos.x, m_savewordpos.y, 1500, 100), "LOAD", "RootMenu_L");
        // セーブ画面背景
        GUI.Label(new Rect(m_saveloadpos.x, m_saveloadpos.y, 1024, 1024), saveloadbg);
        // セーブファイル
        for (int i = 0; i < SAVEFILEPERPAGE; ++i)
        {
            DrawText(new Vector2(m_fileposTop.x, m_fileposTop.y + i * SPACEBETWEENLINES), m_date[i + m_nowpage * SAVEFILEPERPAGE], "Japanese_S");
        }
        // カーソル
        if (m_nowloadState == LoadState.SELECT || m_nowloadState == LoadState.CHECKSELECT)
        {
            DrawText(new Vector2(CURSORPOS_X, m_fileposTop.y + m_savecursorpos * SPACEBETWEENLINES), ">", "Japanese_S");
        }
        // ポップアップ
        if (m_nowloadState == LoadState.CHECKSELECT)
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
            DrawText(new Vector2(POPUPSTRING_X - 20, POPUPSTRING_Y), m_popupwords, "Japanese_S");
            GUI.EndGroup();
        }

        switch (m_nowloadState)
        {
            case LoadState.INITIALIZE:
                SaveLoadInitialize();
                m_nowloadState = LoadState.SLIDEOUT_BEFORE;
                break;
            case LoadState.SLIDEOUT_BEFORE:
                m_menuSelectpos.x -= MENUSELECTMOVESPEED;               // 選択
                m_menuRootCharacterpos.x += MENUSELECTMOVESPEED * 2;    // キャラのステート                
                if (m_menuSelectpos.x < MENUSELECTFIRSTPOS_X)
                {
                    m_nowloadState = LoadState.SLIDEIN;
                }
                break;
            case LoadState.SLIDEIN:
                // LOADの文字列
                m_savewordpos.x += MENUSELECTMOVESPEED;
                // 背景
                m_saveloadpos.y += MENUSELECTMOVESPEED;
                // ファイル名
                m_fileposTop.y += MENUSELECTMOVESPEED;
                if (m_savewordpos.x > SAVE_X)
                {
                    m_savewordpos.x = SAVE_X;
                }
                if (m_saveloadpos.y >= SAVELOADBACK_Y)
                {
                    m_saveloadpos.y = SAVELOADBACK_Y;
                    m_fileposTop.y = SAVEFILEPOSITION_Y;
                }
                if (m_savewordpos.x >= SAVE_X && m_saveloadpos.y >= SAVELOADBACK_Y)
                {
                    m_nowloadState = LoadState.SELECT;
                }
                break;
            case LoadState.SELECT:
                // ページ送り
                ControlPage(ref m_nowpage, 10);
                // 選択時の処理
                if (ControlCurosr(ref m_savecursorpos, SAVEFILEPERPAGE))
                {
                    // 対象のファイルが存在するか確認する
                    string savefilename = @"save\" + (m_nowpage * 10 + m_savecursorpos + 1).ToString("D3") + ".sav";
                    // 存在した場合、確認画面へ
                    if (System.IO.File.Exists(savefilename))
                    {
                        m_nowloadState = LoadState.CHECKSELECT;
                        m_PopupSelect = 0;
                    }
                }
                // キャンセル時の処理
                if (CheckESC())
                {
                    m_nowloadState = LoadState.SLIDEOUT;
                }
                break;
            case LoadState.CHECKSELECT:
                // ポップアップを出す
                // ロードしますか？（OK/Cancel)
                m_popupwords = "ロードしますか？";
                if (ControlCursorHorizon(ref m_PopupSelect, 2))
                {
                    // OK選択
                    if (m_PopupSelect == 0)
                    {
                        m_nowloadState = LoadState.LOAD;                            
                    }
                    // cancel選択
                    else
                    {
                        m_nowloadState = LoadState.SELECT;
                    }
                }
                break;
            case LoadState.LOAD:
                {
                    // ロードする
                    string savefilename = @"save\" + (m_nowpage * 10 + m_savecursorpos + 1).ToString("D3") + ".sav";
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
                        savingparameter.SetNowParty(i, sd.nowparty[i]);
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
                    Application.LoadLevel(SceneName.sceneName[savingparameter.nowField]);
                }
                break;
            case LoadState.SLIDEOUT:
                // LOADの文字列
                m_savewordpos.x -= MENUSELECTMOVESPEED;
                // 背景
                m_saveloadpos.y -= MENUSELECTMOVESPEED;
                // ファイル名
                m_fileposTop.y -= MENUSELECTMOVESPEED;
                if (m_savewordpos.x < SAVE_XFIRST)
                {
                    m_savewordpos.x = SAVE_XFIRST;
                }
                if (m_saveloadpos.y < SAVELOADBACK_YFIRST)
                {
                    m_saveloadpos.y = SAVELOADBACK_YFIRST;
                    m_fileposTop.y = SAVEFILEPOSITION_YFIRST;
                }
                // スライドアウトしきったらルートへ戻る
                if (m_savewordpos.x <= SAVE_XFIRST && m_saveloadpos.y <= SAVELOADBACK_YFIRST)
                {
                    MenuInitialize(false);
                    m_nowSelectMenu = nowSelect_Menu.ROOT;
                }
                break;
        }

    }
}
