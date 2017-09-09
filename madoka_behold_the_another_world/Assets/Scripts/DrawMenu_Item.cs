using UnityEngine;
using System.Collections;

public partial class DrawMenu
{
    // 各ステートを初期化
    // Itemを選択後、一覧画面へ移動する
    // アイテムを使用するか装備するか選択する
    // 使用を選択すると、アイテム一覧の横にカーソルが出て、どれを使用するか選択できる
    // アイテムの横で選択を行うと、Informationに全員のHPとSG汚染率が表示される
    // その状態でカーソルが各キャラの横に現れる
    // 単独回復アイテムなら、カーソルを動かして誰に使うか決める
    // 全員回復アイテムなら、カーソルを全員の横に出す
    // 双方共通で、決定ボタンを押すとポップアップを出して使用の可否を決める
    // 使用すると、SavingParameterに設定している個数と回復した値を書き換える
    // 装備を選択すると、アイテム一覧の横にカーソルが出て、どれを使用するか選択できる
    // アイテムの横で選択を行うと、Equipが横に出て装備か使用の選択まで戻り、SavingParameterの選択が書き変わる

    // アイテム画面のステート
    private enum ItemState
    {
        ITEM_INITIALIZE,        // 各ステートを初期化        
        SLIDEOUT_ROOT,          // 選択を確定し、ルート画面のパーツをスライドアウトする
        SLIDEIN,                // アイテム画面のパーツをスライドインする
        SELECTITEMMODE,         // アイテムを使用するか装備するか選択する
        ITEMUSESELECT,          // アイテムの使用を選択した状態で、アイテムを選択する
        ITEMUSECHECK,           // アイテムを誰に使うか選ぶ
        ITEMUSECHECK_ALL,       // アイテムの使用対象が全体の場合
        ITEMUSECHECK_FINAL,     // ポップアップを出して最終確認を行う(選択の内容を問わずITEMUSESELECTへ戻る）
        EQUIPSELECT,            // どのアイテムを装備するか選択する
        EQUIPSELECT_CHECK,      // アイテム装備の最終確認をする
        EXIT_ITEMMODE,          // アイテムモードを終了する
        MAX_STATE               // 最大数
    };

    // 現在のモード
    private ItemState m_nowItemState;

    // スイッチと背景はステータススキル画面と共通

    // 使用・装備・終了選択のどれを選択しているか
    public enum ItemMode
    {
        USE,
        EQUIP,
        EXIT,

        STATE_TOTAL
    };
    private ItemMode m_nowItemMode;

    // アイテム画面を制御
    private void DrawItem()
    {
        DrawItemParts();
        switch (m_nowItemState)
        {
            case ItemState.ITEM_INITIALIZE:        // 各ステートを初期化
                m_nowItemMode = ItemMode.USE;
                m_nowItemState = ItemState.SLIDEOUT_ROOT;
                m_selectItemCursor = 0;
                break;
            case ItemState.SLIDEOUT_ROOT:          // 選択を確定し、ルート画面のパーツをスライドアウトする
                m_menuSelectpos.x -= MENUSELECTMOVESPEED;               // 選択
                m_menuRootCharacterpos.x += MENUSELECTMOVESPEED * 2;    // キャラのステート                
                if (m_menuSelectpos.x < MENUSELECTFIRSTPOS_X)
                {
                    m_nowItemState = ItemState.SLIDEIN;
                }
                break;
            case ItemState.SLIDEIN:                // アイテム画面のパーツをスライドインする
                // 文字列「Item]
                m_statusTitlePos.x += MENUSELECTMOVESPEED;
                // キャラの全身画像
                m_statusCharacterbodyPos.x += MENUSELECTMOVESPEED + 2.8f;
                // パーツ「使用」
                m_statusSkillPos.x += MENUSELECTMOVESPEED + 0.325f;
                // パーツ「装備」
                m_statusStrengthingPos.x += MENUSELECTMOVESPEED + 0.325f;
                // パーツ「終了」
                m_statusExitPos.x += MENUSELECTMOVESPEED + 0.325f;
                // アイテム背景
                m_SkillBGPos.x -= MENUSELECTMOVESPEED + 5.3f;
                if (m_statusTitlePos.x > 0)
                {
                    m_nowItemState = ItemState.SELECTITEMMODE;
                    m_statusCharacterbodyPos.x = 0.0f;
                }
                break;
            case ItemState.SELECTITEMMODE:         // アイテムを使用するか装備するか終了するか選択する
                 // 選択対象を変更
                int state = (int)m_nowItemMode;
                if (ControlCurosr(ref state, 3))
                {
                    // 使用を選択
                    if (m_nowItemMode == ItemMode.USE)
                    {
                        m_selectItemCursor = 0;
                        m_nowItemState = ItemState.ITEMUSESELECT;
                    }
                    // 装備を選択
                    else if (m_nowItemMode == ItemMode.EQUIP)
                    {
                        m_selectItemCursor = 0;
                        m_nowItemState = ItemState.EQUIPSELECT;
                    }
                    // 終了を選択
                    else
                    {
                        m_nowItemState = ItemState.EXIT_ITEMMODE;
                    }
                }
                m_nowItemMode = (ItemMode)state;
                break;
            case ItemState.ITEMUSESELECT:          // アイテムの使用を選択した状態で、アイテムを選択する
                // 選択対象を変更
                if (ControlCurosr(ref m_selectItemCursor, Item.itemspec.Length))
                {
                    // アイテムの効能が全体か否かで次の移動対象を選択
                    if (Item.itemspec[m_selectItemCursor].IsAll())
                    {
                        m_nowItemState = ItemState.ITEMUSECHECK_ALL;
                    }
                    else
                    {
                        m_nowItemState = ItemState.ITEMUSECHECK;
                    }
                    m_selectItemTargetCursor = 0;
                }
                // ESCで抜ける
                if (CheckESC())
                {
                    m_nowItemState = ItemState.SELECTITEMMODE;
                }
                break;
            case ItemState.ITEMUSECHECK:           // アイテムを誰に使うか選ぶ
                if (ControlCurosr(ref m_selectItemTargetCursor, MadokaDefine.MAXPARTYMEMBER) && savingparameter.GetItemNum(m_selectItemCursor) > 0)
                {
                    // いない場合は選択不可
                    if (savingparameter.GetNowParty(m_selectItemTargetCursor) == 0)
                    {
                        return;
                    }
                    if(!savingparameter.ItemUseCheck(m_selectItemCursor,m_selectItemTargetCursor))
                    {
                        return;
                    }
                    m_PopupSelect = 0;
                    m_nowItemState = ItemState.ITEMUSECHECK_FINAL;
                }
                // ESCで抜ける
                if (CheckESC())
                {                    
                    m_nowItemState = ItemState.ITEMUSESELECT;
                }
                break;
            case ItemState.ITEMUSECHECK_ALL:       // アイテムの使用対象が全員の場合
                // 選択確定
                if (ControlCurosr(ref m_selectItemTargetCursor, MadokaDefine.MAXPARTYMEMBER,false) && savingparameter.GetItemNum(m_selectItemCursor) > 0)
                {
                    if (!savingparameter.ItemUseCheckAll(m_selectItemCursor))
                    {
                        return;
                    }
                    // SE再生
                    AudioSource.PlayClipAtPoint(m_Enter, transform.position);
                    m_nowItemState = ItemState.ITEMUSECHECK_FINAL;
                    m_PopupSelect = 0;
                }
                // ESCで抜ける
                if (CheckESC())
                {
                    m_nowItemState = ItemState.ITEMUSESELECT;
                }
                break;
            case ItemState.ITEMUSECHECK_FINAL:     // ポップアップを出して最終確認を行う(選択の内容を問わずITEMUSESELECTへ戻る）
                if(ControlCursorHorizon(ref m_PopupSelect,2,false))
                {
                    // OK選択
                    if (m_PopupSelect == 0)
                    {
                        // 効果発生
                        savingparameter.ItemDone(m_selectItemCursor, m_selectItemTargetCursor);
                        // SE再生
                        // HP
                        if (Item.itemspec[m_selectItemCursor].ItemFuciton() == ItemSpec.ItemFunction.REBIRETH_HP)
                        {
                            AudioSource.PlayClipAtPoint(RebirthHP1, transform.position);
                        }
                        // 蘇生/蘇生HP両方
                        else if (Item.itemspec[m_selectItemCursor].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_DEATH || Item.itemspec[m_selectItemCursor].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_FULL)
                        {
                            AudioSource.PlayClipAtPoint(Regenaration, transform.position);
                        }
                        // 蘇生・HP・マジックゲージ回復
						else
                        {
                            AudioSource.PlayClipAtPoint(PurificationSG, transform.position);
                        }
                        // ITEMUSESELECTへ戻る
                        m_nowItemState = ItemState.ITEMUSESELECT;
                    }
                    // Cancel選択
                    else
                    {
                        m_nowItemState = ItemState.ITEMUSESELECT;
                    }
                }
                break;
            case ItemState.EQUIPSELECT:            // どのアイテムを装備するか選択する
                if (ControlCurosr(ref m_selectItemCursor, Item.itemspec.Length))
                {
                    // 0個のアイテムは選択不可
                    if (savingparameter.GetItemNum(m_selectItemCursor) == 0)
                    {
                        return;
                    }
                    // SE再生
                    AudioSource.PlayClipAtPoint(m_Enter, transform.position);
                    m_PopupSelect = 0;
                    m_nowItemState = ItemState.EQUIPSELECT_CHECK;
                }
                // ESCで抜ける
                if (CheckESC())
                {
                    m_nowItemState = ItemState.SELECTITEMMODE;
                }
                break;
            case ItemState.EQUIPSELECT_CHECK:      // アイテム装備の最終確認をする
                if (ControlCursorHorizon(ref m_PopupSelect, 2))
                {
                    // OK選択
                    if (m_PopupSelect == 0)
                    {
                        savingparameter.SetNowEquipItem(m_selectItemCursor);
                        m_nowItemState = ItemState.EQUIPSELECT;
                    }
                    // Cancel選択
                    else
                    {
                        m_nowItemState = ItemState.EQUIPSELECT;
                    }
                }
                break;
            case ItemState.EXIT_ITEMMODE:          // アイテムモードを終了する
                // 各パーツをスライドアウトさせる
                // 文字列「Item]
                m_statusTitlePos.x -= MENUSELECTMOVESPEED;
                // キャラの全身画像
                m_statusCharacterbodyPos.x -= MENUSELECTMOVESPEED + 2.8f;
                // パーツ「使用」
                m_statusSkillPos.x -= MENUSELECTMOVESPEED + 0.325f;
                // パーツ「装備」
                m_statusStrengthingPos.x -= MENUSELECTMOVESPEED + 0.325f;
                // パーツ「終了」
                m_statusExitPos.x -= MENUSELECTMOVESPEED + 0.325f;
                // アイテム背景
                m_SkillBGPos.x += MENUSELECTMOVESPEED + 5.3f;
                // スライドアウトしきったらルートへ戻る
                if (m_statusTitlePos.x < STATUS_STATUSFIRSTPOS_X)
                {
                    MenuInitialize(false);
                    m_nowSelectMenu = nowSelect_Menu.ROOT;
                }
                break;
        }
    }

    

    // 必要な変数を初期化
    private void ItemInitialize()
    {
        m_nowItemState = ItemState.ITEM_INITIALIZE;
        // 文字列（ITEM)
        m_statusTitlePos.x = STATUS_STATUSFIRSTPOS_X;
        m_statusTitlePos.y = STATUS_STATUSFIRSTPOS_Y;
        // キャラクター全身画像
        m_statusCharacterbodyPos.x = STATUS_CHARACTERBODYPOS_X;
        m_statusCharacterbodyPos.y = STATUS_CHARACTERBODYPOS_Y;
        // 使用
        m_statusSkillPos.x = STATUS_SKILLPOS_X;
        m_statusSkillPos.y = STATUS_SKILLPOS_Y;
        // 装備
        m_statusStrengthingPos.x = STATUS_STRENGPOS_X;
        m_statusStrengthingPos.y = STATUS_STRENGPOS_Y;
        // 終了
        m_statusExitPos.x = STATUS_EXITPOS_X;
        m_statusExitPos.y = STATUS_EXITPOS_Y;
        // アイテム背景（スキル背景と共通）
        m_SkillBGPos.x = SKILLBG_X;
        m_SkillBGPos.y = SKILLBG_Y;
    }

    // 必要な部品を描画
    private void DrawItemParts()
    {
        // 文字列「ITEM]
        GUI.Label(new Rect(m_statusTitlePos.x, m_statusTitlePos.y, 1500, 100), "ITEM", "RootMenu_L");
        // キャラの全身画像
        int nowcharacter = savingparameter.GetNowParty(0);
        GUI.Label(new Rect(m_statusCharacterbodyPos.x, m_statusCharacterbodyPos.y, STATUS_CHARACTERBODYSIZE, STATUS_CHARACTERBODYSIZE), m_StatusCharacterGraphic_All[nowcharacter]);
        // パーツ「使用」
        GUI.BeginGroup(new Rect(m_statusSkillPos.x, m_statusSkillPos.y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y));
        // 背景
        if (m_nowItemMode == ItemMode.USE)
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch_Enter);
        }
        else
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch);
        }
        GUI.Label(new Rect(STATUS_STRENGWORDSPOS_X, STATUS_SKILLWORDSPOS_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "使用", "Japanese");
        GUI.EndGroup();
        // パーツ「装備」
        GUI.BeginGroup(new Rect(m_statusStrengthingPos.x, m_statusStrengthingPos.y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y));
        // 背景
        if (m_nowItemMode == ItemMode.EQUIP)
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch_Enter);
        }
        else
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch);
        }
        GUI.Label(new Rect(STATUS_STRENGWORDSPOS_X, STATUS_STRENGWORDSPOS_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "装備", "Japanese");
        GUI.EndGroup();
        // パーツ「終了」
        GUI.BeginGroup(new Rect(m_statusExitPos.x, m_statusExitPos.y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y));
        // 背景
        if (m_nowItemMode == ItemMode.EXIT)
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch_Enter);
        }
        else
        {
            GUI.Label(new Rect(0, 0, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), m_StatusSwitch);
        }
        GUI.Label(new Rect(STATUS_STRENGWORDSPOS_X, STATUS_STRENGWORDSPOS_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "終了", "Japanese");
        GUI.EndGroup();
        // アイテム背景
        // アイテムの背景＆文字列
        GUI.BeginGroup(new Rect(m_SkillBGPos.x, m_SkillBGPos.y, SKILLBGSIZE, SKILLBGSIZE));
        // スキルの背景
        GUI.Label(new Rect(0, 0, SKILLBGSIZE, SKILLBGSIZE), m_SkillBackGround);
        GUI.Label(new Rect(ITEMNAME_X, ITEMNAME_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "名称", "Japanese_S");
        GUI.Label(new Rect(ITEMNUM_X, ITEMNUM_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "所持数", "Japanese_S");
        // 所持アイテムを描画
        for (int i = 0; i < Item.itemspec.Length; i++)
        {
            // あれば描画
            if (savingparameter.GetItemNum(i) > 0)
            {
                // 名称
                GUI.Label(new Rect(ITEMNAME_SETX, ITEMNAME_SETY + i * ITEMNAME_OFFSET, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), Item.itemspec[i].Name(), "Japanese_S");
                // 所持数
                GUI.Label(new Rect(ITEMNUM_SETX, ITEMNUM_SETY + i * ITEMNAME_OFFSET, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), savingparameter.GetItemNum(i).ToString("D2"), "Japanese_S");
            }
        }
        // カーソル
        if (m_nowItemState == ItemState.ITEMUSESELECT || m_nowItemState == ItemState.ITEMUSECHECK || m_nowItemState == ItemState.ITEMUSECHECK_FINAL || m_nowItemState == ItemState.EQUIPSELECT
            || m_nowItemState == ItemState.EQUIPSELECT_CHECK)
        {
            GUI.Label(new Rect(ITEMCURSOR_X, ITEMNUM_SETY + m_selectItemCursor * ITEMNAME_OFFSET, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), ">", "Japanese_S");
        }
        // Equip
        GUI.Label(new Rect(EQUIPPOS_X, ITEMNUM_SETY + savingparameter.GetNowEquipItem() * ITEMNAME_OFFSET, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "装備中", "Japanese_S");

        // アイテム使用選択時のキャラクタースペック
        //if (m_nowItemState == ItemState.ITEMUSECHECK || m_nowItemState == ItemState.ITEMUSECHECK_ALL || m_nowItemState == ItemState.ITEMUSECHECK_FINAL)
        {
            // HP
            GUI.Label(new Rect(CHARACTERINFO_HP_X, CHARACTERINFO_HP_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "HP", "Japanese_S");
            // ソウルジェム汚染率
            GUI.Label(new Rect(CHARACTERINFO_SG_X, CHARACTERINFO_SG_Y, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), "ソウルジェム汚染率", "Japanese_S");
            // 名前
            for (int i = 0; i < 3; i++)
            {
                if (savingparameter.GetNowParty(i) != 0)
                {
                    // 名前
                    GUI.Label(new Rect(CHARACTERINFO_NAME_X, CHARACTERINFO_NAME_Y + i * 20, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), Character_Spec.Name[savingparameter.GetNowParty(i)], "Japanese_S");
                    // HP
                    // HP最大値
                    int maxHP = savingparameter.GetMaxHP(savingparameter.GetNowParty(i));
                    // HP現在値
                    int nowHP = savingparameter.GetNowHP(savingparameter.GetNowParty(i));
                    GUI.Label(new Rect(CHARACTERINFO_HP_X - 40, CHARACTERINFO_NAME_Y + i * 20, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), nowHP.ToString("d4") + "/" + maxHP.ToString("d4"), "Japanese_S");
                    // ソウルジェム汚染率
                    float contamination_r = savingparameter.GetGemContimination(savingparameter.GetNowParty(i));
                    GUI.Label(new Rect(CHARACTERINFO_SG_X + 50, CHARACTERINFO_NAME_Y + i * 20, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), contamination_r.ToString("000.00") + "%", "Japanese_S");
                }
            }
            // 対象PC選択用カーソル
            // 全体使用の場合
            if (m_nowItemState == ItemState.ITEMUSECHECK_ALL)
            {
                for (int i = 0; i < 3; i++)
                {
                    GUI.Label(new Rect(ITEMCURSOR_X, CHARACTERINFO_NAME_Y + i * 20, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), ">", "Japanese_S");
                }
            }
            // 単体使用の場合
            else if (m_nowItemState == ItemState.ITEMUSECHECK)
            {
                GUI.Label(new Rect(ITEMCURSOR_X, CHARACTERINFO_NAME_Y + m_selectItemTargetCursor * 20, STATUS_SKILLPOINTSIZE_X, STATUS_SKILLPOINTSIZE_Y), ">", "Japanese_S");
            }
        }
        GUI.EndGroup();

        // ポップアップ出現時
        // アイテム使用
        if (m_nowItemState == ItemState.ITEMUSECHECK_FINAL || m_nowItemState == ItemState.EQUIPSELECT_CHECK)
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
            if (m_nowItemState == ItemState.ITEMUSECHECK_FINAL)
            {
                DrawText(new Vector2(POPUPSTRING_X, POPUPSTRING_Y), "使用しますか？", "Japanese_S");
            }
            else
            {
                DrawText(new Vector2(POPUPSTRING_X, POPUPSTRING_Y), "装備しますか？", "Japanese_S");
            }
            GUI.EndGroup();
        }        

    }

    // アイテム選択用のカーソルの位置
    private int m_selectItemCursor;
    // アイテム選択対象のカーソルの位置
    private int m_selectItemTargetCursor;

    // アイテム背景文字位置
    // 名称（という文字列）
    private const float ITEMNAME_X = 185.0f;
    private const float ITEMNAME_Y = 15.0f;
    // 所持数（という文字列）
    private const float ITEMNUM_X = 472.0f;
    private const float ITEMNUM_Y = 15.0f;
    // アイテム名描画開始位置
    private const float ITEMNAME_SETX = 130.0f;
    private const float ITEMNAME_SETY = 38.0f;
    // アイテム名文字サイズオフセット
    private const float ITEMNAME_OFFSET = 20.0f;
    // アイテム所持数描画開始位置
    private const float ITEMNUM_SETX = 490.0f;
    private const float ITEMNUM_SETY = 38.0f;
    // アイテムカーソルの位置
    private const float ITEMCURSOR_X = 30.0f; 
    // Equip（装備アイテム）の位置
    private const float EQUIPPOS_X = 50.0f;

    // アイテム使用時のキャラクタースペックの位置
    // 名前
    private const float CHARACTERINFO_NAME_X = 50.0f;
    private const float CHARACTERINFO_NAME_Y = 260.0f;
    // HP
    private const float CHARACTERINFO_HP_X = 240.0f;
    private const float CHARACTERINFO_HP_Y = 240.0f;
    // SG汚染率
    private const float CHARACTERINFO_SG_X = 400.0f;
    private const float CHARACTERINFO_SG_Y = 240.0f;

}
