using UnityEngine;
using System.Collections;

/// <summary>
/// 覚醒技発動中のロックオンカーソルを出し入れする
/// </summary>
public class ArousalAttackRockonCursorController : MonoBehaviour 
{
	// 覚醒技を発動しているキャラクター
	public CharacterControl_Base MainCharacter;
		
	// ロックオンカーソルを描画しているカメラ
	public Player_Camera_Controller PlayerCameraController;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(MainCharacter.ArousalAttackProduction)
		{
			PlayerCameraController.IsArousalAttack = true;
		}
		else
		{
			PlayerCameraController.IsArousalAttack = false;
		}
	}
}
