using UnityEngine;
using System.Collections;

/// <summary>
/// 入手したアイテムを取得するクラス
/// </summary>
public static class FieldItemGetManagement
{
	/// <summary>
	/// どのアイテムであるか（Item.csのitemspec参照.-1を入れると金になる.初期値は-2）
	/// </summary>
	public static int ItemKind;

	/// <summary>
	/// アイテムの入手個数。金の場合は金額
	/// </summary>
	public static int ItemNum;
}
