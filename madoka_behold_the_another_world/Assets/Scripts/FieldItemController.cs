using UnityEngine;
using System.Collections;

/// <summary>
/// フィールド上に配置されているアイテムの入手を行う
/// </summary>
public class FieldItemController : MonoBehaviour 
{
	/// <summary>
	/// どのアイテムであるか（Item.csのitemspec参照.-1を入れると金になる）
	/// </summary>
	public int ItemKind;

	/// <summary>
	/// アイテムの入手個数。金の場合は金額
	/// </summary>
	public int ItemNum;

	/// <summary>
	/// savingparameterに保持されるアイテムボックス回収フラグのインデックス（これがtrueだと破棄する）
	/// </summary>
	public int ItemBoxNumber;

	// アイテム入手時のSE
	public AudioClip ItemSE;

	// Use this for initialization
	void Start () 
	{
		if(savingparameter.itemboxopen[ItemBoxNumber])
		{
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	/// <summary>
	/// 接触時処理
	/// </summary>
	/// <param name="character">接触したキャラクター</param>
	void OnCollisionEnter(Collision character)
	{
		// 接触したキャラクターがCharacterControl_BaseかCharacterControl_Base_Questであり、プレイヤーの場合のみ処理開始
		CharacterControlBase hitBattleCharacter = character.gameObject.GetComponent<CharacterControlBase>();
		CharacterControlBaseQuest hitQuestCharacter = character.gameObject.GetComponent<CharacterControlBaseQuest>();

		// プレイヤーであれば処理開始
		if((hitBattleCharacter != null && hitBattleCharacter.IsPlayer == CharacterControlBase.CHARACTERCODE.PLAYER) || hitQuestCharacter != null)
		{
			// アイテム入手音を鳴らす
			AudioSource.PlayClipAtPoint(ItemSE, transform.position);
			// savingparameterに指定したアイテムを増やすか金を増やす
			savingparameter.SetItemNum(ItemKind,savingparameter.GetItemNum(ItemKind) + ItemNum);
			// インフォメーションボードに入手したアイテムを表示するようにFieldItemGetManagementの値を書き換える
			FieldItemGetManagement.ItemKind = ItemKind;
			FieldItemGetManagement.ItemNum = ItemNum;
			// アイテムボックス入手フラグを立てる
			savingparameter.itemboxopen[ItemBoxNumber] = true;
			// 入手後は自壊させる
			Destroy(gameObject);
		}
	}
}
