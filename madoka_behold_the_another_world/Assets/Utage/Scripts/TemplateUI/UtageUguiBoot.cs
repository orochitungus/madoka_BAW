﻿// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.UI;
using Utage;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// タイトル表示のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/Title")]
public class UtageUguiBoot : UguiView
{
	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	
	public UguiFadeTextureStream fadeTextureStream;
	
	public UtageUguiTitle title;
	public UtageUguiLoadWait loadWait;

	public bool isWaitBoot;
	public bool isWaitDownLoad;
	public bool isWaitSplashScreen = true;

	///最初の画面なので自分でオープンする
	public void Start()
	{
		title.gameObject.SetActive(false);
		StartCoroutine(CoUpdate());
	}

	///
	IEnumerator CoUpdate()
	{
#if UNITY_5_3_OR_NEWER
		if (isWaitSplashScreen)
		{
			while (!WrapperUnityVersion.IsFinishedSplashScreen()) yield return null;
		}
#endif
		if (fadeTextureStream)
		{
			fadeTextureStream.gameObject.SetActive(true);
			fadeTextureStream.Play();
			while (fadeTextureStream.IsPlaying) yield return null;
		}
		if (isWaitBoot)
		{
			while (Engine.IsWaitBootLoading) yield return null;
		}
		this.Close();
		if (isWaitDownLoad && loadWait != null)
		{
			loadWait.OpenOnBoot();
		}
		else
		{
			title.Open();
		}
	}
}
