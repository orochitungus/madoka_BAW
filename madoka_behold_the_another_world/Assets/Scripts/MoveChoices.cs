using UnityEngine;
using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 移動先選択
/// </summary>
public class MoveChoices : NPCControlBase 
{
	// 条件1の時（インスペクターは2次元配列使えないので一応条件設定できるようにしておく。とりあえずstoryで）
	// 条件1最小story
	public int MinStory1;
	// 条件1最大story
	public int MaxStory1;
	// 移動元コード
	public int Fromcode1;
	// 移動先コード
	public int[] Forcode1;
	// 移動先シーン名（cancelを入れるとキャンセル扱いの選択肢になる）
	public string[] ForScene1;
	/// <summary>
	/// 選択肢
	/// </summary>
	public string [] Choices1;
	public int MaxSelect1;

	// 条件2の時
	// 条件2最小story
	public int MinStory2;
	// 条件2最大story
	public int MaxStory2;
	// 移動元コード
	public int Fromcode2;
	// 移動先コード
	public int[] Forcode2;
	// 移動先シーン名（cancelを入れるとキャンセル扱いの選択肢になる）
	public string[] ForScene2;
	/// <summary>
	/// 選択肢
	/// </summary>
	public string [] Choices2;

	// 選択肢の総数
	public int MaxSelect2;
	// Use this for initialization
	void Start () 
	{
		// 選択モードにする
		IsSelectMode = true;
		// xstoryが0のときは選択モードとし、決定ボタンを押してxstoryを1にしたときに選択処理を行う
		// 条件1の時
		this.UpdateAsObservable()
		.Where(_=> xstory == 1 && savingparameter.story >= MinStory1 && savingparameter.story <= MaxStory1)
		.Subscribe(_ =>
		{
			// 選択肢がcancelだった場合
			if(ForScene1[NowSelect] == "cancel")
			{
				SelectedCancel();
			}
			else
			{
				SelectedNextStage(Fromcode1, Forcode1[NowSelect], ForScene1[NowSelect]);
			}
		});
		// 条件2のとき
		this.UpdateAsObservable()
		.Where(_ => xstory == 1 && savingparameter.story >= MinStory2 && savingparameter.story <= MaxStory2)
		.Subscribe(_ =>
		{
			// 選択肢がcancelだった場合
			if (ForScene2[NowSelect] == "cancel")
			{
				SelectedCancel();
			}
			else
			{
				SelectedNextStage(Fromcode2, Forcode1[NowSelect], ForScene2[NowSelect]);
			}
		});

		// 条件1の選択肢表示
		this.UpdateAsObservable().Where(_ => UseMode && savingparameter.story >= MinStory1 && savingparameter.story <= MaxStory1).Subscribe(_ =>
		{
			// 念押しでTalkSystemが無かったら戻しておく
			if (Talksystem == null)
			{
				Debug.Log("TalkSystem Null");
				return;
			}
			Talksystem.CharacterFace[0].gameObject.SetActive(true);
			Talksystem.NofaceText.text = "Select";
			for (int i=0; i<MaxSelect1; i++)
			{
				// 選択肢表示
				if (Choices1[i] != "")
				{
					Talksystem.Choices[i].gameObject.SetActive(true);
					Talksystem.Choices[i].text = Choices1[i];
				}
				// カーソル表示
				if (NowSelect == i)
				{
					Talksystem.Cursor[i].gameObject.SetActive(true);
				}
				else
				{
					Talksystem.Cursor[i].gameObject.SetActive(false);
				}
			}
		});
		// 条件2の選択肢表示
		this.UpdateAsObservable().Where(_ => UseMode && savingparameter.story >= MinStory2 && savingparameter.story <= MaxStory2).Subscribe(_ =>
		{
			// 念押しでTalkSystemが無かったら戻しておく
			if (Talksystem == null)
			{
				Debug.Log("TalkSystem Null");
				return;
			}
			Talksystem.CharacterFace[0].gameObject.SetActive(true);
			Talksystem.NofaceText.text = "Select";
			for (int i=0; i<MaxSelect2; i++)
			{
				// 選択肢表示
				if (Choices2[i] != "")
				{
					Talksystem.Choices[i].gameObject.SetActive(true);
					Talksystem.Choices[i].text = Choices2[i];
				}
				// カーソル表示
				if (NowSelect == i)
				{
					Talksystem.Cursor[i].gameObject.SetActive(true);
				}
				else
				{
					Talksystem.Cursor[i].gameObject.SetActive(false);
				}
			}
		});

		// 選択肢ストリーム(上）
		this.UpdateAsObservable().Where(_ => UseMode && Input.GetAxis("Vertical") > 0.5f && NowSelect > 0).
		ThrottleFirst(TimeSpan.FromSeconds(0.5f)).
		Subscribe(_ =>
		{
			AudioSource.PlayClipAtPoint(MoveCursor, transform.position);
			NowSelect--;
		});

		// 選択肢ストリーム(下）
		int MaxSelect = 0;
		if (savingparameter.story >= MinStory1 && savingparameter.story <= MaxStory1)
		{
			MaxSelect = MaxSelect1;
		}
		else if (savingparameter.story >= MinStory2 && savingparameter.story <= MaxStory2)
		{
			MaxSelect = MaxSelect2;
		}
		this.UpdateAsObservable().Where(_ => UseMode && Input.GetAxis("Vertical") < -0.5f && NowSelect < MaxSelect).
		ThrottleFirst(TimeSpan.FromSeconds(0.5f)).		// 連続押しされたら困るので、0.5秒間は入力カット
		Subscribe(_ =>
		{
			AudioSource.PlayClipAtPoint(MoveCursor, transform.position);
			NowSelect++;
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void SelectedCancel()
	{
		// 再度行動可能にする
		characterControl_Base_Quest.Moveable = true;
		// 表示物を切る
		UseMode = false;
		int MaxSelect = 0;
		if (savingparameter.story >= MinStory1 && savingparameter.story <= MaxStory1)
		{
			MaxSelect = MaxSelect1;
		}
		else if (savingparameter.story >= MinStory2 && savingparameter.story <= MaxStory2)
		{
			MaxSelect = MaxSelect2;
		}
		for (int i=0; i<MaxSelect; i++)
		{
			// カーソル
			Talksystem.Cursor[i].gameObject.SetActive(false);
			// 選択肢
			Talksystem.Choices[i].gameObject.SetActive(false);
		}
		// 背景
		Talksystem.Fukidashi.gameObject.SetActive(false);
	}

	void SelectedNextStage(int fromcode, int forcode, string forscene)
	{
		savingparameter.beforeField = fromcode;
		savingparameter.nowField = forcode;
		FadeManager.Instance.LoadLevel(forscene, 1.0f);
	}
}
