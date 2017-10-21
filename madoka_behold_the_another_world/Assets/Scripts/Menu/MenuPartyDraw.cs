using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class MenuPartyDraw : MonoBehaviour 
{
	/// <summary>
	/// 表示されるキャラクターのグラフィック
	/// </summary>
	public MenuPartyCharCursor[] Menupartycharcursor;

	/// <summary>
	/// 現在選択中のキャラクター
	/// </summary>
	public int Nowselect;

	/// <summary>
	/// 選択されたパーティーメンバー
	/// </summary>
	public int []SelectedParty = new int[3];

	/// <summary>
	/// 表示されるキャラクターの名前
	/// </summary>
	public Text CharacterName;

	/// <summary>
	/// 表示されるキャラクターのレベル
	/// </summary>
	public Text CharacterLevel;

	/// <summary>
	/// 表示されるキャラクターの最大HP
	/// </summary>
	public Text CharacterMaxHP;

	/// <summary>
	/// 表示されるキャラクターの現在HP
	/// </summary>
	public Text CharacterNowHP;

	/// <summary>
	/// 表示されるキャラクターの最大Magic
	/// </summary>
	public Text CharacterMaxMagic;

	/// <summary>
	/// 表示されるキャラクターの現在Magic
	/// </summary>
	public Text CharacterNowMagic;

	/// <summary>
	/// 使用不可状態の文字列
	/// </summary>
	public GameObject UnSelectText;

	/// <summary>
	/// パーティーキャラセレクト画面
	/// </summary>
	public GameObject Select;

	/// <summary>
	/// 最終確認画面
	/// </summary>
	public GameObject FinalCheck;

	/// <summary>
	/// 最終確認画面に出てくるキャラクターの画像
	/// </summary>
	public Image []FinalCharacterGraphic;

	/// <summary>
	/// 最終確認画面に出てくるキャラクターの名前
	/// </summary>
	public Text []FinalCharacterNames;

	/// <summary>
	/// 最終確認画面に出てくるキャラクターの現在HP
	/// </summary>
	public Text []FinalCharacterNowHPs;

	/// <summary>
	/// 最終確認画面に出てくるキャラクターの最大HP
	/// </summary>
	public Text []FinalCharacterMaxHPs;

	/// <summary>
	/// 最終確認画面に出てくるキャラクターのレベル
	/// </summary>
	public Text []FinalCharacterLevels;

	/// <summary>
	/// 最終確認画面に出てくるキャラクターの現在MP
	/// </summary>
	public Text []FinalCharacterNowMagics;

	/// <summary>
	/// 最終確認画面に出てくるキャラクターの最大MP
	/// </summary>
	public Text []FinalCharacterMaxMagics;

	public Sprite MadokaFace;
	public Sprite SayakaFace;
	public Sprite HomuraFace;
	public Sprite MamiFace;
	public Sprite KyokoFace;
	public Sprite YumaFace;
	public Sprite KirikaFace;
	public Sprite OrikoFace;
	public Sprite SconosciutoFace;
	public Sprite HomuraBowFace;
	public Sprite UltimateMadokaFace;
	public Sprite DevilHomuraFace;
	public Sprite NagisaFace;
	public Sprite SayakaGodsibbFace;
	public Sprite MichelFace;

	// Use this for initialization
	void Start () 
	{
		this.UpdateAsObservable().Subscribe(_ =>
		{
			for(int i=0; i<Menupartycharcursor.Length; i++)
			{
				if(Nowselect == i)
				{
					// カーソルを赤くする
					Menupartycharcursor[i].Cursor.color = new Color(1, 0, 0);
					// ディアクティブでなければキャラクターの名前と現在HPを表示する
					if(Menupartycharcursor[i].Useable)
					{
						// 表示するキャラクターのインデックス
						int characterindex = Menupartycharcursor[i].CharacterCode;
						// 現在レベルを表示
						CharacterLevel.text = "Level-" +  savingparameter.GetNowLevel(characterindex).ToString("d2");
						// 名前を表示
						CharacterName.text = Character_Spec.Name[characterindex];
						// 現在HPを表示
						CharacterNowHP.text = savingparameter.GetNowHP(characterindex).ToString("d4");
						// 最大HPを表示
						CharacterMaxHP.text = savingparameter.GetMaxHP(characterindex).ToString("d4");
						// 現在MPを表示
						CharacterNowMagic.text = ((int)savingparameter.GetNowArousal(characterindex)).ToString("d4");
						// 最大MPを表示
						CharacterMaxMagic.text = ((int)savingparameter.GetMaxArousal(characterindex)).ToString("d4");
					}
					// ディアクティブの場合キャラクターの画像を消す
					else
					{
						// 名前を消す
						CharacterName.text = "";
						// 現在レベルを非表示
						CharacterLevel.text = "";
						// 現在HPを非表示
						CharacterNowHP.text = "----";
						// 現在MPを非表示
						CharacterNowMagic.text = "----";
						// 最大MPを非表示
						CharacterMaxMagic.text = "----";
						// 最大MPを非表示
						CharacterMaxHP.text = "----";
					}
				}
				else
				{
					Menupartycharcursor[i].Cursor.color = new Color(1, 1, 1);
				}
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// 入るときにセットアップする
	/// </summary>
	public void Setup()
	{
		// パーティーが選択不可なら選択不可を表示
		if(!savingparameter.UseableCharacterSelect)
		{
			UnSelectText.SetActive(true);
			Select.SetActive(false);
		}
		else 
		{
			UnSelectText.SetActive(false);
			Select.SetActive(true);
		}

		// 最終確認を非表示
		FinalCheck.SetActive(false);

		// 各キャラの選択の可否に応じてキャラグラフィックを表示非表示する
		// 鹿目まどか
		SetUpCursor(PartyCursorNumber.MADOKA, savingparameter.UseableMadoka);
		// 美樹さやか
		SetUpCursor(PartyCursorNumber.SAYAKA, savingparameter.UseableSayaka);
		// 暁美ほむら（銃）
		SetUpCursor(PartyCursorNumber.HOMURA, savingparameter.UseableHomura);
		// 巴マミ
		SetUpCursor(PartyCursorNumber.MAMI, savingparameter.UseableMami);
		// 佐倉杏子
		SetUpCursor(PartyCursorNumber.KYOKO, savingparameter.UseableKyoko);
		// 千歳ゆま
		SetUpCursor(PartyCursorNumber.YUMA, savingparameter.UseableYuma);
		// 呉キリカ
		SetUpCursor(PartyCursorNumber.KIRIKA, savingparameter.UseableKirika);
		// 美国織莉子
		SetUpCursor(PartyCursorNumber.ORIKO, savingparameter.UseableOriko);
		// スコノシュート
		SetUpCursor(PartyCursorNumber.SCONOSCIUTO, savingparameter.UseableSconosciuto);
		// 暁美ほむら（弓）
		SetUpCursor(PartyCursorNumber.HOMURABOW, savingparameter.UseableHomuraBow);
		// アルティメットまどか
		SetUpCursor(PartyCursorNumber.ULTIMATEMADOKA, savingparameter.UseableUltimateMadoka);
		// 悪魔ほむら
		SetUpCursor(PartyCursorNumber.DEVILHOMURA, savingparameter.UseableDevilHomura);
		// 百江なぎさ
		SetUpCursor(PartyCursorNumber.NAGISA, savingparameter.UseableNagisa);
		// 円環のさやか
		SetUpCursor(PartyCursorNumber.SAYAKAGODSIBB, savingparameter.UseableSayakaGodsibb);
		// ミッチェル・ノートルダム
		SetUpCursor(PartyCursorNumber.MICHEL, savingparameter.UseableMichel);

		for(int i=0; i< SelectedParty.Length; i++)
		{
			SelectedParty[i] = 0;
		}
	}

	/// <summary>
	/// キャラグラフィックをセットアップする
	/// </summary>
	/// <param name="num"></param>
	private void SetUpCursor(PartyCursorNumber num, bool useable)
	{
		if (useable)
		{
			Menupartycharcursor[(int)num].CharacterImage.gameObject.SetActive(true);
			Menupartycharcursor[(int)num].Useable = true;
			Menupartycharcursor[(int)num].CharacterImage.color = new Color(1, 1, 1, 1);
		}
		else
		{
			Menupartycharcursor[(int)num].CharacterImage.gameObject.SetActive(false);
			Menupartycharcursor[(int)num].Useable = false;
		}
	}

	/// <summary>
	/// 選択中のキャラを確定する
	/// </summary>
	///  <param name="selectednum">何番目に選択されたか</param>
	public void SelectDone(int selectednum)
	{
		SelectedParty[selectednum] = Menupartycharcursor[Nowselect].CharacterCode;
		Menupartycharcursor[Nowselect].Useable = false;
		Menupartycharcursor[Nowselect].CharacterImage.color = new Color(1, 1, 1, 0.5f);
	}

	/// <summary>
	/// 最終確認画面を表示する
	/// </summary>
	public void FinalConfirmDone()
	{
		for(int i=0; i< SelectedParty.Length; i++)
		{
			FinalCharacterGraphic[i].gameObject.SetActive(true);
			// 画像セット
			switch (SelectedParty[i])
			{
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA:
					FinalCharacterGraphic[i].sprite = MadokaFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA:
					FinalCharacterGraphic[i].sprite = SayakaFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA:
					FinalCharacterGraphic[i].sprite = HomuraFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_MAMI:
					FinalCharacterGraphic[i].sprite = MamiFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_KYOKO:
					FinalCharacterGraphic[i].sprite = KyokoFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_YUMA:
					FinalCharacterGraphic[i].sprite = YumaFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA:
					FinalCharacterGraphic[i].sprite = KirikaFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_ORIKO:
					FinalCharacterGraphic[i].sprite = OrikoFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO:
					FinalCharacterGraphic[i].sprite = SconosciutoFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B:
					FinalCharacterGraphic[i].sprite = HomuraBowFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA:
					FinalCharacterGraphic[i].sprite = UltimateMadokaFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_DEVIL_HOMURA:
					FinalCharacterGraphic[i].sprite = DevilHomuraFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_NAGISA:
					FinalCharacterGraphic[i].sprite = NagisaFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA_GODSIBB:
					FinalCharacterGraphic[i].sprite = SayakaGodsibbFace;
					break;
				case (int)Character_Spec.CHARACTER_NAME.MEMBER_MICHEL:
					FinalCharacterGraphic[i].sprite = MichelFace;
					break;
				default:
					FinalCharacterGraphic[i].gameObject.SetActive(false);
					break;
			}
			// 名前表示
			FinalCharacterNames[i].text = Character_Spec.Name[SelectedParty[i]];
			// レベル表示
			FinalCharacterLevels[i].text = "Level-" + savingparameter.GetNowLevel(SelectedParty[i]).ToString("d2");
			// HP表示
			FinalCharacterNowHPs[i].text = savingparameter.GetNowHP(SelectedParty[i]).ToString("d4");
			// 最大HP表示
			FinalCharacterMaxHPs[i].text = savingparameter.GetMaxHP(SelectedParty[i]).ToString("d4");
			// Magic表示
			FinalCharacterNowMagics[i].text = ((int)savingparameter.GetNowArousal(SelectedParty[i])).ToString("d4");
			// 最大Magic表示
			FinalCharacterMaxMagics[i].text = ((int)savingparameter.GetMaxArousal(SelectedParty[i])).ToString("d4");
		}
	}


	/// <summary>
	/// カーソルの何番目に誰を割り振ったか
	/// </summary>
	public enum PartyCursorNumber
	{
		MADOKA,
		SAYAKA,
		HOMURA,
		MAMI,
		KYOKO,
		YUMA,
		KIRIKA,
		ORIKO,
		SCONOSCIUTO,
		ULTIMATEMADOKA,
		DEVILHOMURA,
		NAGISA,
		SAYAKAGODSIBB,
		MICHEL,
		HOMURABOW
	}

}
