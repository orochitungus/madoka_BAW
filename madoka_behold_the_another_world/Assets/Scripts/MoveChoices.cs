using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 移動先選択
/// </summary>
public class MoveChoices : NPCControlBase 
{
	// 移動元コード
	public int Fromcode;
	// 移動先コード
	public int[] Forcode;
	// 移動先シーン名（cancelを入れるとキャンセル扱いの選択肢になる）
	public string[] ForScene;

	// Use this for initialization
	void Start () 
	{
		// 選択モードにする
		IsSelectMode = true;
		// xstoryが0のときは選択モードとし、決定ボタンを押してxstoryを1にしたときに選択処理を行う
		this.UpdateAsObservable()
		.Where(_=> xstory == 1)
		.Subscribe(_ =>
		{ 
			// 選択肢がcancelだった場合
			if(ForScene[NowSelect] == "cancel")
			{
				// 再度行動可能にする
				characterControl_Base_Quest.Moveable = true;
				// 表示物を切る
				UseMode = false;
				for(int i=0; i<MaxSelect; i++)
				{ 
					// カーソル
					Talksystem.Cursor[i].gameObject.SetActive(false);
					// 選択肢
					Talksystem.Choices[i].gameObject.SetActive(false);
				}
				// 背景
				Talksystem.Fukidashi.gameObject.SetActive(false);

			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
