using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerBG : MonoBehaviour 
{
	// 表示する顔グラフィック
	public Image BustupMadoka;
	public Image BustupSayaka;
	public Image BustupHomura;
	public Image BustupHomuraBow;
	public Image BustupMami;
	public Image BustupKyoko;
	public Image BustupYuma;
	public Image BustupKirika;
	public Image BustupOriko;
	public Image BustupUltiMadoka;
	public Image BustupScono;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	/// <summary>
	/// HPゲージの後ろのバストアップ
	/// </summary>
	/// <param name="select"></param>
	public void SelectPlayerBG(BustupSelect select)
	{
		switch (select)
		{
			case BustupSelect.MADOKA:
				BustupMadoka.gameObject.SetActive(true);
				break;
			case BustupSelect.SAYAKA:
				BustupSayaka.gameObject.SetActive(true);
				break;
			case BustupSelect.HOMURA:
				BustupHomura.gameObject.SetActive(true);
				break;
			case BustupSelect.HOMURA_B:
				BustupHomuraBow.gameObject.SetActive(true);
				break;
			case BustupSelect.MAMI:
				BustupMami.gameObject.SetActive(true);
				break;
			case BustupSelect.KYOKO:
				BustupKyoko.gameObject.SetActive(true);
				break;
			case BustupSelect.YUMA:
				BustupYuma.gameObject.SetActive(true);
				break;
			case BustupSelect.KIRIKA:
				BustupKirika.gameObject.SetActive(true);
				break;
			case BustupSelect.ORIKO:
				BustupOriko.gameObject.SetActive(true);
				break;
			case BustupSelect.SCONO:
				BustupScono.gameObject.SetActive(true);
				break;
			case BustupSelect.ULTIMADOKA:
				BustupUltiMadoka.gameObject.SetActive(true);
				break;
		}
	}
}

public enum BustupSelect
{
	NONE,
	MADOKA,
	SAYAKA,
	HOMURA,
	HOMURA_B,
	MAMI,
	KYOKO,
	YUMA,
	KIRIKA,
	ORIKO,
	ULTIMADOKA,
	SCONO
}
