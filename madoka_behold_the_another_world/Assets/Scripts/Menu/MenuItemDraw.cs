using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class MenuItemDraw : MonoBehaviour 
{
	/// <summary>
	/// 現在何を選択したか
	/// </summary>
	public int NowSelect;

	/// <summary>
	/// アイテム名
	/// </summary>
	public Text[] ItemName;

	/// <summary>
	/// アイテムの個数
	/// </summary>
	public Text[] ItemNum;

	[SerializeField]
	private GameObject []Cursor;

	// Use this for initialization
	void Start () 
	{
		this.UpdateAsObservable().Subscribe(_ => 
		{ 
			for(int i=0; i<Cursor.Length; i++)
			{
				if(NowSelect == i)
				{
					CursorErase(i);
				}
			}			
		});
	}
	
	/// <summary>
	/// 出現位置以外のカーソルを消す
	/// </summary>
	/// <param name="appear"></param>
	private void CursorErase(int appear)
	{
		for (int i = 0; i<10; i++)
		{
			if (i == appear)
			{
				Cursor[i].SetActive(true);
			}
			else
			{
				Cursor[i].SetActive(false);
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
