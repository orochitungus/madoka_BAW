using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponGauge : MonoBehaviour 
{
	/// <summary>
	/// 武装の画像
	/// </summary>
	public Image WeaponGraphic;

	/// <summary>
	/// チャージゲージ
	/// </summary>
	public Image Charge;

	/// <summary>
	/// チャージを使うか否か
	/// </summary>
	public bool UseChargeGauge;

	/// <summary>
	/// リロードゲージ
	/// </summary>
	public Image Reload;

	/// <summary>
	/// 残弾数表示
	/// </summary>
	public Text BulletNumber;

	/// <summary>
	/// 最大弾数
	/// </summary>
	public float MaxBulletNumber;

	/// <summary>
	/// 現在の弾数
	/// </summary>
	public float NowBulletNumber;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
