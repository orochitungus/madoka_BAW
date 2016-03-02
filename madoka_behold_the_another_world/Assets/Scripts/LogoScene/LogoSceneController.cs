using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LogoSceneController : MonoBehaviour 
{    
    /// <summary>
    /// Logo制御を行うアニメーション
    /// </summary>
    public Animator LogoAppear;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(AppearCompanyLogo());
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	/// <summary>
	/// 一定時間後にTungus-Villageの文字列を出し、さらに一定時間後やっていなかったらコントローラー設定に飛ばし、やっていたらタイトルに飛ばす
	/// </summary>
	/// <returns></returns>
	private IEnumerator AppearCompanyLogo()
	{
        LogoAppear.SetBool("PresentedAppear", true);
		yield return new WaitForSeconds(1.0f);
        LogoAppear.SetBool("CompanyLogoAppear", true);
		yield return new WaitForSeconds(2.0f);
		// TODO:コントローラー設定をやっていたらタイトルへ、していなかったらコントローラー設定へ

	}
}
