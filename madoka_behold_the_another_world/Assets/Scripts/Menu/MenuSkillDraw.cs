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
