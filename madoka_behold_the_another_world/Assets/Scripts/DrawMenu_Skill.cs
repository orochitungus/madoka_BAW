using UnityEngine;
using System.Collections;

// スキル画面を描画
public partial class DrawMenu
{
    // スキル画面のステート
    private enum SkillState
    {
        SKILL_INITIALISE,       // 各ステートを初期化
        TARGETSELECT,           // パーティーメンバーの誰を選択するか
        SLIDEOUT_ROOT,          // 選択を確定し、ルート画面のパーツをスライドアウトする
        SLIDEIN_ROOT,           // スキルからパーティーメンバー選択に戻ってきたとき、ルート画面のパーツをスライドインする
        SLIDEIN,                // スキル画面のパーツをスライドインする
        EXIT_SKILL,             // スキル表示を抜けてTARGETSELECTへ戻る
        SKILLDRAW,              // スキルを表示
        MAX_STATE               // 最大数
    };

    // 現在のモード
    private SkillState m_nowSkillState;

    // スキル画面の背景
    public Texture2D m_SkillmodeBack;

    // スキル画面背景の位置
    private Vector2 m_skillBackGroundPos;

    // 同初期位置
    private const float SKILL_BACKGROUNDPOS_X = 1025.0f;
    private const float SKILL_BACKGROUNDPOS_Y = 2.0f;

    // スキル画面を描画
    private void DrawSkill()
    {
        DrawSkillParts();
        switch (m_nowSkillState)
        {
            case SkillState.SKILL_INITIALISE:
                m_nowSelectCursor = 0;
                m_nowSkillState = SkillState.TARGETSELECT;
                break;
            case SkillState.TARGETSELECT:   // 対象を選択する
                // カーソルを描画する
                DrawPartyCursor(m_nowSelectCursor);
                // カーソルの移動範囲を選択する
                int maxcursormove = 3;
                for (int i = 0; i < MadokaDefine.MAXPARTYMEMBER; i++)
                {
                    if (savingparameter.GetNowParty(i) == (int)Character_Spec.CHARACTER_NAME.MEMBER_NONE)
                    {
                        maxcursormove = i + 1;
                    }
                }
                // カーソルを制御する
                if (ControlCurosr(ref m_nowSelectCursor, maxcursormove) && savingparameter.GetNowParty(m_nowSelectCursor) != (int)Character_Spec.CHARACTER_NAME.MEMBER_NONE)
                {
                    // 選択確定時の処理
                    m_nowSkillState = SkillState.SLIDEOUT_ROOT;
                    // 各パーツをスライドイン開始前位置に配置する
                    
                    // 現在の選択キャラを確定する
                    m_nowSelectCharacter = savingparameter.GetNowParty(m_nowSelectCursor);
                }
                // 戻るボタンを押したらスキル画面を抜ける
                if (CheckESC())
                {
                    // ルート画面へ戻る
                    m_nowSelectMenu = nowSelect_Menu.ROOT;
                }
                break;
            case SkillState.SLIDEOUT_ROOT:
                m_menuSelectpos.x -= MENUSELECTMOVESPEED;               // 選択
                m_menuRootCharacterpos.x += MENUSELECTMOVESPEED * 2;    // キャラのステート                
                if (m_menuSelectpos.x < MENUSELECTFIRSTPOS_X)
                {
                    m_nowSkillState = SkillState.SLIDEIN;
                }
                break;
            case SkillState.SLIDEIN_ROOT:   // 選択画面に戻るときに再度各パーツをスライドインさせる
                m_menuSelectpos.x += MENUSELECTMOVESPEED;               // 選択
                m_menuRootCharacterpos.x -= MENUSELECTMOVESPEED * 2;    // キャラのステート                
                if (m_menuSelectpos.x > 0)
                {
                    m_nowSkillState = SkillState.TARGETSELECT;
                }
                break;
            case SkillState.SLIDEIN:        // 必要なパーツをスライドイン
                // 文字列「SKILL]
                m_statusTitlePos.x += MENUSELECTMOVESPEED;
                // キャラの全身画像
                m_statusCharacterbodyPos.x += MENUSELECTMOVESPEED + 2.8f;
                // スキル画面の背景
                m_skillBackGroundPos.x -= MENUSELECTMOVESPEED + 7.475f;
                if (m_statusTitlePos.x >= 0)
                {
                    m_nowSkillState = SkillState.SKILLDRAW;
                }
                break;
            case SkillState.SKILLDRAW:
                // キャンセルでキャラ選択へ戻る
                if (CheckESC())
                {
                    m_nowSkillState = SkillState.EXIT_SKILL;
                }
                break;
            case SkillState.EXIT_SKILL:     // スキル画面のパーツをスライドアウト
                // 文字列「SKILL]
                m_statusTitlePos.x -= MENUSELECTMOVESPEED;
                // キャラの全身画像
                m_statusCharacterbodyPos.x -= MENUSELECTMOVESPEED + 2.8f;
                // スキル画面の背景
                m_skillBackGroundPos.x += MENUSELECTMOVESPEED + 7.475f;
                if (m_skillBackGroundPos.x >= 1025)
                {
                    m_nowSkillState = SkillState.SLIDEIN_ROOT;
                }
                break;
            default:
                break;
        }
    }

    // 必要な変数を初期化する
    private void SkillInitialize()
    {
        m_nowSkillState = SkillState.SKILL_INITIALISE;
        SkillSetInitPosition();
    }

    
    // 各変数の必要な位置を初期化する
    private void SkillSetInitPosition()
    {
        // 各パーツをスライドイン開始前位置に配置する
        // 文字列とキャラ画像はSTATUSと共通
        // 文字列（SKILL)
        m_statusTitlePos.x = STATUS_STATUSFIRSTPOS_X;
        m_statusTitlePos.y = STATUS_STATUSFIRSTPOS_Y;
        // キャラクター全身画像
        m_statusCharacterbodyPos.x = STATUS_CHARACTERBODYPOS_X;
        m_statusCharacterbodyPos.y = STATUS_CHARACTERBODYPOS_Y;
        // スキル背景
        m_skillBackGroundPos.x = SKILL_BACKGROUNDPOS_X;
        m_skillBackGroundPos.y = SKILL_BACKGROUNDPOS_Y;
    }

    // 必要なパーツを描画する
    private void DrawSkillParts()
    {
        // 文字列「STATUS]
        GUI.Label(new Rect(m_statusTitlePos.x, m_statusTitlePos.y, 1500, 100), "SKILL", "RootMenu_L");
        // キャラの全身画像
        int nowcharacter = savingparameter.GetNowParty(m_nowSelectCursor);
        GUI.Label(new Rect(m_statusCharacterbodyPos.x, m_statusCharacterbodyPos.y, STATUS_CHARACTERBODYSIZE, STATUS_CHARACTERBODYSIZE), m_StatusCharacterGraphic_All[nowcharacter]);
        // スキルの背景画像と文字列
        GUI.BeginGroup(new Rect(m_skillBackGroundPos.x, m_skillBackGroundPos.y, 1024, 512));
        GUI.Label(new Rect(0, 0, 1024, 512), m_SkillmodeBack);
        // 文字列
        // 誰を選択しているか判定する
        int character = m_nowSelectCharacter;
        // 入力
        DrawText(new Vector2(SKILL_INPUTPOS_X, SKILL_INPUTPOS_Y), "入力", "Japanese_S");
        // 名称
        DrawText(new Vector2(SKILL_NAMEPOS_X, SKILL_INPUTPOS_Y), "名称", "Japanese_S");
        // 攻撃力
        DrawText(new Vector2(SKILL_STR_X, SKILL_INPUTPOS_Y), "攻撃力", "Japanese_S");
        // 使用回数
        DrawText(new Vector2(SKILL_USENUM_X, SKILL_INPUTPOS_Y), "回数", "Japanese_S");
        // キャラクターのレベルを取得する(savingparameterの書き変わりが入っていないとアウト）
        int characterlv = savingparameter.GetNowLevel(character);
        // そのキャラクターの習得可能なスキルを取得する
        int skillmax = Character_Spec.cs[character].Length;
        // キャラクターの攻撃力レベルを算出
        int nowstr = savingparameter.GetStrLevel(character);
        // キャラクターの残弾数レベルを算出
        int nowbul = savingparameter.GetBulLevel(character);
        // 各スキルを描画
        for (int i = 0; i < skillmax; i++)
        {
            if (characterlv >= Character_Spec.cs[character][i].m_LearningLevel)
            {
                string skill = DrawSkillName(Character_Spec.cs[character][i].m_Skilltype);
                DrawText(new Vector2(SKILLWORDSPOS_X, SKILL_WORDY + i * 20), skill, "Japanese_S");
                DrawText(new Vector2(SKILLNAMEPOS_X, SKILL_WORDY + i * 20), Character_Spec.cs[character][i].m_SkillName, "Japanese_S");
                // 攻撃力と使用回数                
                // 射撃系
                if (Character_Spec.cs[character][i].m_OriginalBulletNum > 0)
                {
                    // 攻撃力を計算
                    int str = Character_Spec.cs[character][i].m_OriginalStr + Character_Spec.cs[character][i].m_GrowthCoefficientStr * (nowstr - 1);
                    DrawText(new Vector2(SKILL_STR_X, SKILL_WORDY + i * 20), str.ToString("D4"), "Japanese_S");
                    // 残弾数を計算
                    int bul = Character_Spec.cs[character][i].m_OriginalBulletNum + Character_Spec.cs[character][i].m_GrowthCoefficientBul * (nowbul - 1);
                    DrawText(new Vector2(SKILL_USENUM_X, SKILL_WORDY + i * 20), bul.ToString("D3"), "Japanese_S");
                }
                // アビリティ系
                else if (Character_Spec.cs[character][i].m_OriginalBulletNum == -1)
                {
                    
                }
                // 格闘系
                else
                {
                    // 攻撃力を計算
                    int str = Character_Spec.cs[character][i].m_OriginalStr + Character_Spec.cs[character][i].m_GrowthCoefficientStr * (nowstr - 1);
                    DrawText(new Vector2(SKILL_STR_X, SKILL_WORDY + i * 20), str.ToString("D4"), "Japanese_S");
                }
            }
        }
        GUI.EndGroup();
    }
	// スキル描画時の文字配置位置(背景左上からの相対座標）
    // 入力
    private const float SKILL_INPUTPOS_X = 22.0f;
    private const float SKILL_INPUTPOS_Y = 15.0f;
    // 名称
    private const float SKILL_NAMEPOS_X = 355.0f;
    // 攻撃力
    private const float SKILL_STR_X = 545.0f;
    // 使用回数
    private const float SKILL_USENUM_X = 627.0f;
    // スキル描画基準Y座標
    private const float SKILL_WORDY = 40.0f;
}
