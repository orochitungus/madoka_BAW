using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public partial class DrawMenu
{
    public Texture2D saveloadbg;
    //・現在のモード（初期化・元からあったものをスライドアウト・スライドイン・選択・選択確認・上書き時上書き確認・セーブ・スライドアウト）
    private enum SaveState
    {
        INITIALIZE,                 // 初期化
        SLIDEOUT_BEFORE,            // 元からあったものをスライドアウト
        SLIDEIN,                    // スライドイン
        SELECT,                     // 選択
        CHECKSELECT,                // 選択確認
        CHECKSELECTFINAL,           // 上書き時上書き確認
        SAVE,                       // セーブ実行処理
        SLIDEOUT,                   // スライドアウト
    };
    private SaveState m_nowsavestate;
    private int m_nowpage;          // 現在のページ
    private Vector2 m_saveloadpos;  // セーブ・ロード背景の位置
    private Vector2 m_savewordpos;  // 文字列saveの位置
    private Vector2 m_fileposTop;   // 一番上のファイルの位置	
    private int m_savecursorpos;    // 現在のカーソルの位置		
    public static string[] m_date = new string[100];// date.savの中の配列
    private string m_popupwords;

    private const float SAVE_X = 0;                 // 文字列「save」の最終的な配置位置
    private const float SAVE_Y = 0;
    private const float SAVE_XFIRST = -400;         // 文字列「save」の最初の配置位置
    private const float SAVELOADBACK_X = 85;        // セーブロード背景の最終的な配置位置
    private const float SAVELOADBACK_Y = 60;
    private const float SAVELOADBACK_YFIRST = -400; // セーブロード背景の最初の配置位置
    private const int SAVEFILEPERPAGE = 10;         // 1ページあたりのファイル表示数
    private const float SAVEFILEPOSITION_X = 290;   // 一番上の行に表示されるファイルの位置
    private const float SAVEFILEPOSITION_Y = 65;
    private const float SAVEFILEPOSITION_YFIRST = -395;
    private const float SPACEBETWEENLINES = 35;     // ファイル名同士の幅
    private const float CURSORPOS_X = 260;          // カーソルの配置位置
    // セーブ画面のステートを初期化
    private void SaveInitialize()
    {
        m_savewordpos = new Vector2(SAVE_XFIRST,SAVE_Y);
        m_saveloadpos = new Vector2(SAVELOADBACK_X,SAVELOADBACK_YFIRST);
        m_nowsavestate = SaveState.INITIALIZE;
        m_fileposTop = new Vector2(SAVEFILEPOSITION_X, SAVEFILEPOSITION_YFIRST);
    }



    private void DrawSave()
    {
        // 文字列「SAVE」
        GUI.Label(new Rect(m_savewordpos.x, m_savewordpos.y, 1500, 100), "SAVE", "RootMenu_L");
        // セーブ画面背景
        GUI.Label(new Rect(m_saveloadpos.x, m_saveloadpos.y, 1024, 1024), saveloadbg);
        // セーブファイル
        for (int i = 0; i < SAVEFILEPERPAGE; ++i)
        {
            DrawText(new Vector2(m_fileposTop.x, m_fileposTop.y + i * SPACEBETWEENLINES), m_date[i + m_nowpage * SAVEFILEPERPAGE], "Japanese_S");
        }
        // カーソル
        if (m_nowsavestate == SaveState.SELECT || m_nowsavestate == SaveState.CHECKSELECT || m_nowsavestate == SaveState.CHECKSELECTFINAL)
        {
            DrawText(new Vector2(CURSORPOS_X, m_fileposTop.y + m_savecursorpos * SPACEBETWEENLINES), ">", "Japanese_S");
        }
        // ポップアップ
        if (m_nowsavestate == SaveState.CHECKSELECT || m_nowsavestate == SaveState.CHECKSELECTFINAL)
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

        switch (m_nowsavestate)
        {
            case SaveState.INITIALIZE:          // 初期化                
                SaveLoadInitialize();
                m_nowsavestate = SaveState.SLIDEOUT_BEFORE;                
                break;
            case SaveState.SLIDEOUT_BEFORE:     // 元からあったものをスライドアウト
                m_menuSelectpos.x -= MENUSELECTMOVESPEED;               // 選択
                m_menuRootCharacterpos.x += MENUSELECTMOVESPEED * 2;    // キャラのステート                
                if (m_menuSelectpos.x < MENUSELECTFIRSTPOS_X)
                {
                    m_nowsavestate = SaveState.SLIDEIN;
                }
                break;
            case SaveState.SLIDEIN:             // スライドイン
                // saveの文字列
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
                    m_nowsavestate = SaveState.SELECT;
                }
                break;
            case SaveState.SELECT:              // 選択
                // ページ送り
                ControlPage(ref m_nowpage, 10);
                // 選択時の処理
                if (ControlCurosr(ref m_savecursorpos, SAVEFILEPERPAGE))
                {
                    m_nowsavestate = SaveState.CHECKSELECT;
                    m_PopupSelect = 0;
                }
                // キャンセル時の処理
                if (CheckESC())
                {
                    m_nowsavestate = SaveState.SLIDEOUT;
                }
                break;
            case SaveState.CHECKSELECT:         // 選択確認
                {// ポップアップを出す
                    // ここにセーブしますか？（OK/Cancel)
                    m_popupwords = "ここにセーブしますか？";
                    if (ControlCursorHorizon(ref m_PopupSelect, 2))
                    {
                        // OK選択
                        if (m_PopupSelect == 0)
                        {
                            // セーブファイル名
                            string savefilename = @"save\" + (m_nowpage * 10 + m_savecursorpos + 1).ToString("D3") + ".sav";
                            // そのセーブファイルがすでに存在した場合はCHECKSELECTFINALへ
                            if (System.IO.File.Exists(savefilename))
                            {
                                m_nowsavestate = SaveState.CHECKSELECTFINAL;
                            }
                            // 存在しないのならSAVEへ
                            else
                            {
                                m_nowsavestate = SaveState.SAVE;
                            }
                        }
                        // cancel選択
                        else
                        {
                            m_nowsavestate = SaveState.SELECT;
                        }
                    }
                }
                break;
            case SaveState.CHECKSELECTFINAL:    // 上書き時、最終選択確認
                // ポップアップを出す
                // 上書きしてもよろしいですか？（OK/Cancel)
                m_popupwords = "上書きしますか？";
                if (ControlCursorHorizon(ref m_PopupSelect, 2))
                {
                    // OK選択
                    if (m_PopupSelect == 0)
                    {
                       m_nowsavestate = SaveState.SAVE;                       
                    }
                    // cancel選択
                    else
                    {
                        m_nowsavestate = SaveState.SELECT;
                    }
                }
                break;
            case SaveState.SAVE:                // セーブ実行処理し、m_dateの該当箇所を書き換える
                {
                    // 保存先のファイル名
                    string savefilename = @"save\" + (m_nowpage * 10 + m_savecursorpos + 1).ToString("D3") + ".sav";                    
                    // 表記するファイル名を作る
                    string drawfilename = "";
                    // ファイル名のインデックスを取得
                    drawfilename = (m_nowpage * 10 + m_savecursorpos + 1).ToString("D3") + ":";
                    // 現在の日付と時刻を取得
                    DateTime dt = DateTime.Now;
                    drawfilename = drawfilename + dt.Year.ToString("D4") + "/" + dt.Month.ToString("D2") + "/" + dt.Day + "/" + dt.Hour.ToString("D2") + ":" + dt.Minute.ToString("D2");
                    // 現在の場所を取得
                    drawfilename = drawfilename + " " + ParameterManager.Instance.Mapscenename.sheets[0].list[savingparameter.nowField].NAME;
					// 該当のセーブ名を書き換える
					m_date[m_nowpage * 10 + m_savecursorpos] = drawfilename;
                    // ファイル名一覧を書き換える
                    System.Text.Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                    //書き込むファイルを開く
                    System.IO.StreamWriter sr = new System.IO.StreamWriter(@"save\save.sav", false, enc);
                    for (int i = 0; i < MadokaDefine.SAVEDATANUM; ++i)
                    {
                        sr.WriteLine(m_date[i]);
                    }
                    sr.Close();
                    // セーブファイルとなるオブジェクト
                    SaveData sd = new SaveData();
                    // sdに保存する内容を記述する
                    // ストーリー進行度
                    sd.story = savingparameter.story;
                    // キャラクターの配置位置
                    sd.nowposition_x = savingparameter.nowposition.x;
                    sd.nowposition_y = savingparameter.nowposition.y;
                    sd.nowposition_z = savingparameter.nowposition.z;
                    // キャラクターの配置角度
                    sd.nowrotation_x = savingparameter.nowrotation.x;
                    sd.nowrotation_y = savingparameter.nowrotation.y;
                    sd.nowrotation_z = savingparameter.nowrotation.z;
                    // アイテムボックスの開閉フラグ
                    for(int i=0; i<MadokaDefine.NUMOFITEMBOX; ++i)
                    {
                        sd.itemboxopen[i] = savingparameter.itemboxopen[i];
                    }
                    // 現在の所持金
                    sd.nowmoney = savingparameter.nowmoney;
                    // 現在のパーティー(護衛対象も一応パーティーに含める)
                    for (int i = 0; i < 4; ++i)
                    {
                        sd.nowparty[i] = savingparameter.GetNowParty(i);
                    }
                    // キャラクター関連
                    for (int i = 0; i < (int)Character_Spec.CHARACTER_NAME.CHARACTER_ALL_NUM; ++i)
                    {
                        // 各キャラのレベル
                        sd.nowlevel[i] = savingparameter.GetNowLevel(i);
                        // 各キャラの現在の経験値
                        sd.nowExp[i] = savingparameter.GetNowExp(i);
                        // 各キャラのHP
                        sd.nowHP[i] = savingparameter.GetNowHP(i);
                        // 各キャラの最大HP
                        sd.nowMaxHP[i] = savingparameter.GetMaxHP(i);
                        // 各キャラの覚醒ゲージ
                        sd.nowArousal[i] = savingparameter.GetNowArousal(i);
                        // 各キャラの最大覚醒ゲージ
                        sd.nowMaxArousal[i] = savingparameter.GetMaxArousal(i);
                        // 各キャラの攻撃力レベル
                        sd.StrLevel[i] = savingparameter.GetStrLevel(i);
                        // 各キャラの防御力レベル
                        sd.DefLevel[i] = savingparameter.GetDefLevel(i);
                        // 各キャラの残弾数レベル
                        sd.BulLevel[i] = savingparameter.GetBulLevel(i);
                        // 各キャラのブースト量レベル
                        sd.BoostLevel[i] = savingparameter.GetBoostLevel(i);
                        // 覚醒ゲージレベル
                        sd.ArousalLevel[i] = savingparameter.GetArousalLevel(i);
                        // ソウルジェム汚染率
                        sd.GemContimination[i] = savingparameter.GetGemContimination(i);
                        // スキルポイント
                        sd.SkillPoint[i] = savingparameter.GetSkillPoint(i);
                    }
                    // 各アイテムの保持数
                    for (int i = 0; i < Item.itemspec.Length; ++i)
                    {
                        sd.m_numofItem[i] = savingparameter.GetItemNum(i);
                    }
                    // 現在装備中のアイテム
                    sd.m_nowequipItem = savingparameter.GetNowEquipItem();
                    // 現在の場所
                    sd.nowField = savingparameter.nowField;
                    // 前にいた場所
                    sd.beforeField = savingparameter.beforeField;
                    // オブジェクトの内容をファイルに保存する
                    savingparameter.SaveToBinaryFile(sd, savefilename);
                    // m_nowsavestateをSELECTに戻す
                    m_nowsavestate = SaveState.SELECT;
                }
                break;
            case SaveState.SLIDEOUT:            // スライドアウト
                // saveの文字列
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
            default:
                break;
        }
    }

    // 日付データのセーブデータを生成する
    private void SaveLoadInitialize()
    {
        // saveフォルダがない場合、saveフォルダを作る
        if (!System.IO.Directory.Exists("save"))
        {
            //System.IO.DirectoryInfo di = 
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

        m_nowpage = 0;
        m_savecursorpos = 0;
        m_popupwords = "";
    }
   
}
