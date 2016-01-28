using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;

public class NPCContol : NPCControlBase 
{
	/// <summary>
	/// 会話時のアニメーションの名前
	/// </summary>
	public string TalkAnimationName;

	/// <summary>
	/// 通常時のアニメーションの名前
	/// </summary>
	public string NormalAnimationName;

	// Use this for initialization
	void Start () 
	{
		// 台詞送りストリーム(決定ボタンを押すと台詞が出切っていなかった場合全部出し、出切っていたらxstoryが進む）
		this.UpdateAsObservable()
		.Where(_ => UseMode && (Input.GetButtonDown("Shot") || Input.GetButtonDown("Enter")))
		.Subscribe(_ =>
		{
			if (!IsSelectMode && Serif[xstory].Length > AppearWords)
			{
				AppearWords = Serif[xstory].Length;
			}
			else
			{
				AppearWords = 0;
				// 終わったら解除
				if (xstory >= Serif.Length - 1)
				{
					// 再度行動可能にする
					characterControl_Base_Quest.Moveable = true;
					// 表示物を切る
					UseMode = false;
					// 台詞
					Talksystem.FukidashiText.text = "";
					// 背景
					Talksystem.Fukidashi.gameObject.SetActive(false);
					// 顔
					Talksystem.CharacterFace[0].gameObject.SetActive(false);
					// キャラ名
					Talksystem.CharacterNameJP.gameObject.SetActive(false);
					Talksystem.CharacterNameEn.gameObject.SetActive(false);
					
					// アニメーションを戻す
					this.GetComponent<Animation>().Play(NormalAnimationName);
				}
				else
				{ 
					xstory++;
				}
			}
		});



		// 台詞流しストリーム（台詞が出きっていないときに限り0.1秒ごとにAppearWordsをインクリメント)
		this.UpdateAsObservable().
		Where(_ => UseMode && !IsSelectMode && Serif[xstory].Length > AppearWords).
		ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => AppearWords++);

		// 台詞があるときは台詞を出す
		this.UpdateAsObservable().Where(_ => UseMode && Serif.Length > 0)
		.Subscribe(_ =>
		{
			Talksystem.FukidashiText.gameObject.SetActive(true);
			Talksystem.FukidashiText.text = Serif[xstory].Substring(0, AppearWords);
		});
		
		
		
		this.UpdateAsObservable().
		Where(_ => UseMode)
		.Subscribe(_ =>
		{
			// キャラ名があるときはキャラ名を出す(日本語）
			if (CharacterName_JP[xstory] != "")
			{ 
				Talksystem.CharacterNameJP.gameObject.SetActive(true);
				Talksystem.CharacterNameJPText.text = CharacterName_JP[xstory];
			}
			// キャラ名が無いときはキャラ名を消す
			else
			{
				Talksystem.CharacterNameJP.gameObject.SetActive(false);
			}
			// キャラ名があるときはキャラ名を出す(英語）
			if(CharacterName_EN[xstory] != "")
			{
				Talksystem.CharacterNameEn.gameObject.SetActive(true);
				Talksystem.CharacterNameENText.text = CharacterName_EN[xstory];
			}
			// キャラ名が無いときはキャラ名を消す
			else
			{
				Talksystem.CharacterNameEn.gameObject.SetActive(false);
			}
		});

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		// 本体をプレイヤーキャラの方向に向ける
		// プレイヤーキャラの座標
		Vector3 playerPos = collision.transform.position;
		// 自分の位置
		Vector3 mypos = this.transform.position;
		// 方向を算出
		Quaternion looklot = Quaternion.LookRotation(playerPos - mypos);
		// 方向を変える
		this.transform.rotation = looklot;
		// アニメーションをさせる
		this.GetComponent<Animation>().Play(TalkAnimationName);

		// 顔を出す
		Talksystem.CharacterFace[0].gameObject.SetActive(true);
		Talksystem.NofaceText.text = "Talk";
		// 背景を出しておく
		Talksystem.Fukidashi.gameObject.SetActive(true);
		
	}
}
