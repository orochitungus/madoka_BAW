using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class MenuStatus : MonoBehaviour 
{
	/// <summary>
	/// キャラクター全体画像
	/// </summary>
	public Image CharacterImage;

	/// <summary>
	/// 日本語名
	/// </summary>
	public Text NameJp;

	/// <summary>
	/// 英語名
	/// </summary>
	public Text NameEn;

	/// <summary>
	/// レベル
	/// </summary>
	public Text Level;

	/// <summary>
	/// 現在HP
	/// </summary>
	public Text NowHP;

	/// <summary>
	/// 最大HP
	/// </summary>
	public Text MaxHP;

	/// <summary>
	/// 現在Magic
	/// </summary>
	public Text NowMagic;

	/// <summary>
	/// 最大Magic
	/// </summary>
	public Text MaxMagic;

	/// <summary>
	/// スキルポイント
	/// </summary>
	public Text SkillPoint;

	/// <summary>
	/// 
	/// </summary>
	public Text Str;

	public GameObject StrArrow;

	/// <summary>
	/// 
	/// </summary>
	public Text Con;

	public GameObject ConArrow;

	/// <summary>
	/// 
	/// </summary>
	public Text Vit;

	public GameObject VitArrow;

	/// <summary>
	/// 
	/// </summary>
	public Text Dex;

	public GameObject DexArrow;

	/// <summary>
	/// 
	/// </summary>
	public Text Agi;

	public GameObject AgiArrow;

	/// <summary>
	/// 現在選択中の項目
	/// </summary>
	public int NowSelect;

	/// <summary>
	/// 全身像
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

	// Use this for initialization
	void Start () 
	{
		this.UpdateAsObservable().Subscribe(_ =>
		{
			if(NowSelect == (int)StatusKind.STR)
			{
				StrArrow.SetActive(true);
				ConArrow.SetActive(false);
				VitArrow.SetActive(false);
				DexArrow.SetActive(false);
				AgiArrow.SetActive(false);
			}
			else if(NowSelect == (int)StatusKind.CON)
			{
				StrArrow.SetActive(false);
				ConArrow.SetActive(true);
				VitArrow.SetActive(false);
				DexArrow.SetActive(false);
				AgiArrow.SetActive(false);
			}
			else if(NowSelect == (int)StatusKind.VIT)
			{
				StrArrow.SetActive(false);
				ConArrow.SetActive(false);
				VitArrow.SetActive(true);
				DexArrow.SetActive(false);
				AgiArrow.SetActive(false);
			}
			else if(NowSelect == (int)StatusKind.DEX)
			{
				StrArrow.SetActive(false);
				ConArrow.SetActive(false);
				VitArrow.SetActive(false);
				DexArrow.SetActive(true);
				AgiArrow.SetActive(false);
			}
			else if(NowSelect == (int)StatusKind.AGI)
			{
				StrArrow.SetActive(false);
				ConArrow.SetActive(false);
				VitArrow.SetActive(false);
				DexArrow.SetActive(false);
				AgiArrow.SetActive(true);
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// キャラごとに初期化を行う
	/// </summary>
	/// <param name="selectedCharacter"></param>
	public void Initialize(int selectedCharacter)
	{
		// キャラグラフィック
		// まどか
		if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
		{
			CharacterImage.sprite = Madoka;
		}
		// さやか
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA)
		{
			CharacterImage.sprite = Sayaka;
		}
		// ほむら
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA)
		{
			CharacterImage.sprite = Homura;
		}
		// マミ
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_MAMI)
		{
			CharacterImage.sprite = Mami;
		}
		// 杏子
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_KYOKO)
		{
			CharacterImage.sprite = Kyoko;
		}
		// ゆま
		else if (selectedCharacter ==(int)Character_Spec.CHARACTER_NAME.MEMBER_YUMA)
		{
			CharacterImage.sprite = Yuma;
		}
		// キリカ
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA)
		{
			CharacterImage.sprite = Kirika;
		}
		// 織莉子
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_ORIKO)
		{
			CharacterImage.sprite = Oriko;
		}
		// 弓ほむら
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
		{
			CharacterImage.sprite = HomuraB;
		}
		// スコノシュート
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO)
		{
			CharacterImage.sprite = Sconosciuto;
		}
		// アルティメットまどか
		else if (selectedCharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA)
		{
			CharacterImage.sprite = UMadoka;
		}
		// 円環のさやか

		// デビルほむら

		// なぎさ

		// ミッチェル

		// 上記以外は消す(テスト用の魔獣など）
		else
		{
			CharacterImage.sprite = null;
		}

		// 名前を書き換える
		NameJp.text = Character_Spec.Name[selectedCharacter];
		NameEn.text = Character_Spec.NameEn[selectedCharacter];

		// レベルを書き換える
		Level.text = "Level-" + savingparameter.GetNowLevel(selectedCharacter);

		// HPを書き換える
		NowHP.text = savingparameter.GetNowHP(selectedCharacter).ToString("d4");
		MaxHP.text = savingparameter.GetMaxHP(selectedCharacter).ToString("d4");

		// Magicを書き換える
		NowMagic.text = ((int)savingparameter.GetNowArousal(selectedCharacter)).ToString("d4");
		MaxMagic.text = ((int)savingparameter.GetMaxArousal(selectedCharacter)).ToString("d4");

		// SkillPointを書き換える
		SkillPoint.text = savingparameter.GetSkillPoint(selectedCharacter).ToString("d2");

		// Strを書き換える
		Str.text = savingparameter.GetStrLevel(selectedCharacter).ToString("d2");
		
		// Conを書き換える
		Con.text = savingparameter.GetArousalLevel(selectedCharacter).ToString("d2");
		

		// Vitを書き換える
		Vit.text = savingparameter.GetArousalLevel(selectedCharacter).ToString("d2");
		
		// Dexを書き換える
		Dex.text = savingparameter.GetArousalLevel(selectedCharacter).ToString("d2");
		
		// Agiを書き換える
		Agi.text = savingparameter.GetArousalLevel(selectedCharacter).ToString("d2");

		// 選択項目を初期化する
		NowSelect = 0;
		
	}

	
}
public enum StatusKind
{
	STR,
	CON,
	VIT,
	DEX,
	AGI,
	TOTALSTATUSNUM
}