using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;

public class NPCControlBase : MonoBehaviour 
{
	protected int xstory;
	protected TalkSystem Talksystem;

		
	/// <summary>
	/// 選択肢に使うときの現在の選択
	/// </summary>
	protected int NowSelect;
	/// <summary>
	/// 選択肢であるか否か
	/// </summary>
	protected bool IsSelectMode;

	/// <summary>
	/// 表示中の文字数
	/// </summary>
	protected int AppearWords;

	/// <summary>
	/// 表示台詞
	/// </summary>
	public string [] Serif;
	public string [] CharacterName_JP;
	public string [] CharacterName_EN;

	

	/// <summary>
	/// カーソル移動音
	/// </summary>
 	public AudioClip MoveCursor;
	
	/// <summary>
	/// 選択確定音
	/// </summary>
	public AudioClip Enter;

	/// <summary>
	/// 接触したプレイヤーキャラ
	/// </summary>
	protected CharacterControl_Base_Quest characterControl_Base_Quest;

	/// <summary>
	/// 使用状態であるか否か
	/// </summary>
	protected bool UseMode;

	void Awake()
	{
		UseMode = false;
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	// 接触時の処理
	protected virtual void OnCollisionEnter(Collision collision)
	{
		// TalkSystemを取得する
		GameObject talksystem = GameObject.Find("TalkSystem");
		Talksystem = talksystem.GetComponent<TalkSystem>();

		// 接触対象の移動機能を切る(処理が終わったら切る）
		characterControl_Base_Quest = collision.gameObject.GetComponent<CharacterControl_Base_Quest>();
		characterControl_Base_Quest.Moveable = false;
		// 背景を出しておく
		Talksystem.Fukidashi.gameObject.SetActive(true);

		// xstoryをリセットする
		xstory = 0;
		// ApperWordsをリセットする
		AppearWords = 0;
		// 選択肢をリセットする
		NowSelect = 0;
		// 使用可能にする
		UseMode = true;
	}
}
