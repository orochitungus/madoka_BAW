using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPartyCharCursor : MonoBehaviour 
{
	/// <summary>
	/// 使用可能であるか否か
	/// </summary>
	public bool Useable;

	/// <summary>
	/// 表示されるキャラクターの顔
	/// </summary>
	public Image CharacterImage;

	/// <summary>
	/// セーブデータにおけるキャラクターのコード
	/// </summary>
	public int CharacterCode;

	/// <summary>
	/// カーソル部分
	/// </summary>
	public Image Cursor;
}
