using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvDoor : Selector
{

	public Animation EVDoorL;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Select == SelectMode.SELECT)
		{
			// 選択確定時ドアを開く

			// フラグを変える
			Select = SelectMode.OPEN;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	private IEnumerator NextFloorGo()
	{
		// 扉が開くまで１秒待ってから次のフロアへ移動
		yield return new WaitForSeconds(1.0f);
	}
}
