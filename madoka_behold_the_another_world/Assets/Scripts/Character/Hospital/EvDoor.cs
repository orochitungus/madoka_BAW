using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EvDoor : Selector
{

	public Animation EVDoorL;
	public Animation EVDoorR;
	
	// Update is called once per frame
	void Update () 
	{
		if(Select == SelectMode.SELECT && ForCode[NowSelect] != -1)
		{
			// 選択確定時SEを鳴らしてドアを開く
			AudioManager.Instance.PlaySE("crrect_answer1");
			EVDoorL.Play("mitakiharahospital_evdoor_l_open");
			EVDoorR.Play("mitakiharahospital_evdoor_r_open");

			// フラグを変える
			Select = SelectMode.OPEN;
			StartCoroutine(NextFloorGo(ForCode[NowSelect]));
		}
		else if(Select == SelectMode.SELECT)
		{
			CancelDone();
		}
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		int selected = 0;
		// 文字列を表示する
		for(int i=0; i< MaxSelect - 1; i++)
		{
			if(savingparameter.story >= StoryConditionMin[i] && savingparameter.story <= StoryConditionMax[i])
			{
				SelectText[i].text = SelectName[i];
				selected++;
			}
			else
			{
				break;
			}
		}
		// 最後の選択肢はキャンセルにする
		SelectText[selected].text = "キャンセル";
		ForCode[selected] = -1;
		// 残りの選択肢は消しておく
		for(int i=selected+1; i<4; i++)
		{
			SelectText[i].text = "";
			ForCode[i] = -2;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private IEnumerator NextFloorGo(int nextFloor)
	{
		// 扉が開くまで１秒待ってから次のフロアへ移動
		yield return new WaitForSeconds(1.0f);
		savingparameter.beforeField = FromCode;
		savingparameter.nowField = nextFloor;
		FadeManager.Instance.LoadLevel(NextScene[NowSelect], 1.0f);
	}
}
