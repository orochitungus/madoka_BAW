using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSkillDraw : MonoBehaviour
{
    /// <summary>
    /// キャラクターの顔
    /// </summary>
    public Image CharacterFace;

    /// <summary>
    /// キャラクター名英語
    /// </summary>
    public Text CharacterNameEN;

    /// <summary>
    /// キャラクター名日本語
    /// </summary>
    public Text CharacterNameJP;

    /// <summary>
    /// キャラクターレベル
    /// </summary>
    public Text CharacterLevel;

    /// <summary>
    /// キャラクターの最大HP
    /// </summary>
    public Text CharacterMaxHP;

    /// <summary>
    /// キャラクターの現在HP
    /// </summary>
    public Text CharacterNowHP;

    /// <summary>
    /// キャラクターの最大Magic
    /// </summary>
    public Text CharacterMaxMagic;

    /// <summary>
    /// キャラクターの現在Magic;
    /// </summary>
    public Text CharacterNowMagic;

    /// <summary>
    /// キャラクターのスキル名
    /// </summary>
    public Text SkillName;

    /// <summary>
    /// キャラクターのスキルコマンド
    /// </summary>
    public Text SkillCommand;

    /// <summary>
    ///  顔グラフィック
    /// </summary>
    public Sprite Madoka;
    public Sprite Sayaka;
    public Sprite Homura;
    public Sprite Mami;
    public Sprite Kyoko;
    public Sprite Yuma;
    public Sprite Kirika;
    public Sprite Oriko;
    public Sprite HomuraB;
    public Sprite Sconosciuto;
    public Sprite UMadoka;
    public Sprite SayakaG;
    public Sprite DHomura;
    public Sprite Nagisa;
    public Sprite Michel;

	/// <summary>
	/// ページ矢印左
	/// </summary>
	public Text LeftArrow;

	/// <summary>
	/// ページ矢印右
	/// </summary>
	public Text RightArrow;

	/// <summary>
	/// 1Pあたりのスキル表示個数
	/// </summary>
	private int SKILLSHOWNUMBER = 15;

	/// <summary>
	/// 現在表示中のページ
	/// </summary>
	public int NowPage;

	/// <summary>
	/// 最大ページ数
	/// </summary>
	public int MaxPage;

    /// <summary>
    /// キャラクターステータスを書き込む
    /// </summary>
    public void Initiallize(int selectedCharacter)
    {
        // まどか
        if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
        {
            CharacterFace.sprite = Madoka;
        }
        // さやか
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA)
        {
            CharacterFace.sprite = Sayaka;
        }
        // ほむら
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA)
        {
            CharacterFace.sprite = Homura;
        }
        // マミ
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_MAMI)
        {
            CharacterFace.sprite = Mami;
        }
        // 杏子
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_KYOKO)
        {
            CharacterFace.sprite = Kyoko;
        }
        // ゆま
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_YUMA)
        {
            CharacterFace.sprite = Yuma;
        }
        // キリカ
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA)
        {
            CharacterFace.sprite = Kirika;
        }
        // 織莉子
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_ORIKO)
        {
            CharacterFace.sprite = Oriko;
        }
        // 弓ほむら
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
        {
            CharacterFace.sprite = HomuraB;
        }
        // スコノシュート
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO)
        {
            CharacterFace.sprite = Sconosciuto;
        }
        // アルティメットまどか
        else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA)
        {
            CharacterFace.sprite = UMadoka;
        }
        // 円環のさやか

        // デビルほむら

        // なぎさ

        // ミッチェル

        // 上記以外は消す(テスト用の魔獣など）
        else
        {
            CharacterFace.sprite = null;
        }

        // 名前を書き換える
        CharacterNameJP.text = Character_Spec.Name[selectedCharacter];
        CharacterNameEN.text = Character_Spec.NameEn[selectedCharacter];

        // レベルを書き換える
        CharacterLevel.text = "Level-" + savingparameter.GetNowLevel(selectedCharacter);

        // HPを書き換える
        CharacterNowHP.text = savingparameter.GetNowHP(selectedCharacter).ToString("d4");
        CharacterMaxHP.text = savingparameter.GetMaxHP(selectedCharacter).ToString("d4");

        // Magicを書き換える
        CharacterNowMagic.text = ((int)savingparameter.GetNowArousal(selectedCharacter)).ToString("d4");
        CharacterMaxMagic.text = ((int)savingparameter.GetMaxArousal(selectedCharacter)).ToString("d4");

		// 現在のページを0にする
		NowPage = 0;

		// 最大ページ数を定義(ラストページはファイナルマジックにする）
		MaxPage = Character_Spec.cs[selectedCharacter].Length / SKILLSHOWNUMBER + 2;

		// スキルを表示する
		// ファイナルマジック除き15個以下
		if (Character_Spec.cs[selectedCharacter].Length <= SKILLSHOWNUMBER)
		{
			for (int i = 0; i < Character_Spec.cs[selectedCharacter].Length; i++)
			{
				// スキル名
				SkillName.text += Character_Spec.cs[selectedCharacter][i].m_SkillName + "\n";
				// コマンド
				// N格闘2段目/3段目
				if(Character_Spec.cs[selectedCharacter][i].m_Skilltype == CharacterSkill.SkillType.WRESTLE_2 || Character_Spec.cs[selectedCharacter][i].m_Skilltype == CharacterSkill.SkillType.WRESTLE_3)
				{
					CommandDraw(Character_Spec.cs[selectedCharacter][i].m_Skilltype, "", Character_Spec.cs[selectedCharacter][i-1].m_SkillName);
				}
				// それ以外
				else
				{
					CommandDraw(Character_Spec.cs[selectedCharacter][i].m_Skilltype);
				}
			}
		}
		// 15個以上
		else
		{
			// とりあえず最初の15個を並べる
			for (int i = 0; i < SKILLSHOWNUMBER; i++)
			{
				// スキル名
				SkillName.text += Character_Spec.cs[selectedCharacter][i].m_SkillName + "\n";
				// コマンド
				// N格闘2段目/3段目
				if (Character_Spec.cs[selectedCharacter][i].m_Skilltype == CharacterSkill.SkillType.WRESTLE_2 || Character_Spec.cs[selectedCharacter][i].m_Skilltype == CharacterSkill.SkillType.WRESTLE_3)
				{
					CommandDraw(Character_Spec.cs[selectedCharacter][i].m_Skilltype, "", Character_Spec.cs[selectedCharacter][i - 1].m_SkillName);
				}
				// それ以外
				else
				{
					CommandDraw(Character_Spec.cs[selectedCharacter][i].m_Skilltype);
				}
			}
		}

	}

	/// <summary>
	/// コマンドを描画する
	/// </summary>
	/// <param name="skillType"></param>
	/// <param name="modeName">モードチェンジがあるキャラの場合、モードの名前</param>
	/// <param name="baseCommand">派生技の場合、元の技</param>
	public void CommandDraw(CharacterSkill.SkillType skillType, string modeName = "", string baseCommand = "")
	{
		switch (skillType)
		{
			case CharacterSkill.SkillType.SHOT:
				SkillCommand.text += "射撃" + "\n";
				break;
			case CharacterSkill.SkillType.SHOT_M2:
				SkillCommand.text += modeName + "中に射撃" + "\n";
				break;
			case CharacterSkill.SkillType.SUB_SHOT:
				SkillCommand.text += "サブ射撃(射撃+格闘)" + "\n";
				break;
			case CharacterSkill.SkillType.SUB_SHOT_M2:
				SkillCommand.text += modeName + "中にサブ射撃(射撃+格闘)" + "\n";
				break;
			case CharacterSkill.SkillType.EX_SHOT:
				SkillCommand.text += "特殊射撃(射撃+ジャンプ)" + "\n";
				break;
			case CharacterSkill.SkillType.EX_SHOT_M2:
				SkillCommand.text += modeName + "中に特殊射撃(射撃+ジャンプ)" + "\n";
				break;
			case CharacterSkill.SkillType.WRESTLE_1:
				SkillCommand.text += "格闘" + "\n";
				break;
			case CharacterSkill.SkillType.WRESTLE_1_M2:
				SkillCommand.text += modeName + "中に格闘" + "\n";
				break;
			case CharacterSkill.SkillType.WRESTLE_2:
				SkillCommand.text += baseCommand + "中に格闘" + "\n";
				break;
			case CharacterSkill.SkillType.WRESTLE_2_M2:
				SkillCommand.text += baseCommand + "中に格闘" + "\n";
				break;
			case CharacterSkill.SkillType.WRESTLE_3:
				SkillCommand.text += baseCommand + "中に格闘" + "\n";
				break;
			case CharacterSkill.SkillType.WRESTLE_3_M2:
				SkillCommand.text += baseCommand + "中に格闘" + "\n";
				break;
			case CharacterSkill.SkillType.FRONT_WRESTLE_1:
				SkillCommand.text += "↑+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.FRONT_WRESTLE_1_M2:
				SkillCommand.text += modeName + "中に↑+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.LEFT_WRESTLE_1:
				SkillCommand.text += "←+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.LEFT_WRESTLE_1_M2:
				SkillCommand.text += modeName + "中に←+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.RIGHT_WRESTLE_1:
				SkillCommand.text += "→+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.RIGHT_WRESTLE_1_M2:
				SkillCommand.text += modeName + "中に→+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.BACK_WRESTLE:
				SkillCommand.text += "↓+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.BACK_WRESTLE_M2:
				SkillCommand.text += modeName + "中に↓+格闘" + "\n";
				break;
			case CharacterSkill.SkillType.EX_WRESTLE_1:
				SkillCommand.text += "格闘+ジャンプ" + "\n";
				break;
			case CharacterSkill.SkillType.EX_WRESTLE_1_M2:
				SkillCommand.text += modeName + "中に格闘＋ジャンプ" + "\n";
				break;
			case CharacterSkill.SkillType.EX_FRONT_WRESTLE_1:
				SkillCommand.text += "↑+格闘+ジャンプ" + "\n";
				break;
			case CharacterSkill.SkillType.EX_FRONT_WRESTLE_1_M2:
				SkillCommand.text += modeName + "中に↑+格闘+ジャンプ" + "\n";
				break;
			case CharacterSkill.SkillType.BACK_EX_WRESTLE:
				SkillCommand.text += "空中で↓+格闘+ジャンプ" + "\n";
				break;
			case CharacterSkill.SkillType.AIRDASH_WRESTLE:
				SkillCommand.text += "空中ダッシュ中に格闘" + "\n";
				break;
			case CharacterSkill.SkillType.CHARGE_SHOT:
				SkillCommand.text += "射撃長押しして離す" + "\n";
				break;
			case CharacterSkill.SkillType.CHARGE_WRESTLE:
				SkillCommand.text += "格闘長押しして離す" + "\n";
				break;
		}
	}

	/// <summary>
	/// ページごとにスキルを描画
	/// </summary>
	/// <param name="nowpage"></param>
	public void SkillDraw(int nowpage, int selectedCharacter)
	{
		// いったん全クリア
		SkillName.text = "";
		SkillCommand.text = "";
		// 開始位置
		int start = nowpage * SKILLSHOWNUMBER;
		// 終了位置
		int end = Character_Spec.cs[selectedCharacter].Length - start;
		
		for(int i=start; i<end; i++)
		{
			// スキル名
			SkillName.text += Character_Spec.cs[selectedCharacter][i].m_SkillName + "\n";
			// コマンド
			// N格闘2段目/3段目
			if (Character_Spec.cs[selectedCharacter][i].m_Skilltype == CharacterSkill.SkillType.WRESTLE_2 || Character_Spec.cs[selectedCharacter][i].m_Skilltype == CharacterSkill.SkillType.WRESTLE_3)
			{
				CommandDraw(Character_Spec.cs[selectedCharacter][i].m_Skilltype, "", Character_Spec.cs[selectedCharacter][i - 1].m_SkillName);
			}
			// それ以外
			else
			{
				CommandDraw(Character_Spec.cs[selectedCharacter][i].m_Skilltype);
			}
		}

	}

	/// <summary>
	/// ファイナルマジックを描画
	/// </summary>
	/// <param name="selectedcharacter"></param>
	public void FinalMagicDraw(int selectedcharacter)
	{
		// 一旦全クリア
		SkillName.text = "";
		SkillCommand.text = "";

		SkillName.text = FinalMagicSpec.SkillName[selectedcharacter];
		SkillCommand.text = "マジックバースト発動中に射撃+格闘+ジャンプ";
	}

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
