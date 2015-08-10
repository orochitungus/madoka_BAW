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
	/// 選択肢に使うときの選択肢の数（最大4）
	/// </summary>
	public int SelectNum;
	/// <summary>
	/// 最大選択数
	/// </summary>
	protected int MaxSelect;
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
	public int AppearWords;

	/// <summary>
	/// 表示台詞
	/// </summary>
	public string [] Serif;
	public string [] CharacterName_JP;
	public string [] CharacterName_EN;

	/// <summary>
	/// 選択肢
	/// </summary>
	public string [] Choices;

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
		// 台詞送りストリーム(決定ボタンを押すと台詞が出切っていなかった場合全部出し、出切っていたらxstoryが進む）
		this.UpdateAsObservable()
		.Where(_=> UseMode && (Input.GetButtonDown("Shot") || Input.GetButtonDown("Enter")))
		.Subscribe(_ =>
		{ 
			if(!IsSelectMode && Serif[xstory].Length > AppearWords)
			{
				AppearWords = Serif[xstory].Length;
			}
			else
			{ 
				AppearWords = 0;
				xstory++;
			}
		});

		// 選択肢ストリーム(上）
		this.UpdateAsObservable().Where(_ => UseMode && Input.GetAxis("Vertical") > 0.5f && NowSelect > 0).
		ThrottleFirst(TimeSpan.FromSeconds(0.5f)).
		Subscribe(_=>
		{
			AudioSource.PlayClipAtPoint(MoveCursor, transform.position);
			NowSelect--;
		});

		// 選択肢ストリーム(下）
		this.UpdateAsObservable().Where(_=> UseMode && Input.GetAxis("Vertical") < -0.5f && NowSelect < MaxSelect).
		ThrottleFirst(TimeSpan.FromSeconds(0.5f)).		// 連続押しされたら困るので、0.5秒間は入力カット
		Subscribe(_=>
		{
			AudioSource.PlayClipAtPoint(MoveCursor, transform.position);
			NowSelect++;
		});
		
		// 台詞流しストリーム（台詞が出きっていないときに限り0.3秒ごとにAppearWordsをインクリメント)
		this.UpdateAsObservable().
		Where(_ => UseMode && !IsSelectMode && Serif[xstory].Length > AppearWords).
		ThrottleFirst(TimeSpan.FromSeconds(0.3f)).Subscribe(_=> AppearWords++);

		// 表示物制御
		this.UpdateAsObservable().Where(_=> UseMode).Subscribe(_=>
		{
			// 念押しでTalkSystemが無かったら戻しておく
			if(Talksystem == null)
			{
				Debug.Log("TalkSystem Null");
				return;
			}
			// 選択時
			if(IsSelectMode)
			{
				Talksystem.CharacterFace[0].gameObject.SetActive(true);
				Talksystem.NofaceText.text = "Select";
				for(int i=0; i<MaxSelect; i++)
				{
					// 選択肢表示
					if(Choices[i] != "")
					{
						Talksystem.Choices[i].gameObject.SetActive(true);
					}
					// カーソル表示
					if(NowSelect == i)
					{
						Talksystem.Cursor[i].gameObject.SetActive(true);
					}
					else
					{
						Talksystem.Cursor[i].gameObject.SetActive(false);
					}
				}
			}
			// 台詞表示時
			else
			{
				if(Serif[xstory] != "")
				{
					Talksystem.FukidashiText.text = Serif[xstory].Substring(0,AppearWords);
				}
			}
		});
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
	protected void OnCollisionEnter(Collision collision)
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
