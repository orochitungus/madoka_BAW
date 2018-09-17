using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPGauge : MonoBehaviour 
{
	/// <summary>
	/// ゲージ背景
	/// </summary>
	public Image GaugeBG;

	/// <summary>
	/// ゲージ
	/// </summary>
	public Image Gauge;

	/// <summary>
	/// 1000単位の星（10000オーバーは割るまで減らさない）
	/// </summary>
	public Image []OverHPGauge;

	public Text EnemyName;

	/// <summary>
	/// Target表示部分
	/// </summary>
	public GameObject IsTarget;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// HPゲージ更新
	/// </summary>
	/// <param name="nowHP">対象のHP</param>
	/// <param name="maxHP">対象の最大HP</param>
	/// <param name="name">対象の名前</param>
	/// <param name="istarget">対象が撃墜目標であるか否か</param>
	public void HPGaugeUpudate(int nowHP,int maxHP, string name, bool istarget)
	{
		EnemyName.text = name;
		if(istarget)
		{
			IsTarget.SetActive(true);
		}
		else
		{
			IsTarget.SetActive(false);
		}
		// 対象のHPはいくつ？
		// 10000越え
		if(nowHP > 10000)
		{
			for(int i=0; i<OverHPGauge.Length; i++)
			{
				OverHPGauge[i].gameObject.SetActive(true);
			}
			GaugeBG.fillAmount = 1.0f;
			Gauge.fillAmount = 1.0f;
		}
		// 9000越え
		else if(nowHP > 9000)
		{
			GaugeGontroller(maxHP, nowHP, 10000, 9);			
		}
		// 8000越え
		else if(nowHP > 8000)
		{
			GaugeGontroller(maxHP, nowHP, 9000, 8);
		}
		// 7000越え
		else if(nowHP > 7000)
		{
			GaugeGontroller(maxHP, nowHP, 8000, 7);
		}
		// 6000越え
		else if(nowHP > 6000)
		{
			GaugeGontroller(maxHP, nowHP, 7000, 6);
		}
		// 5000越え
		else if(nowHP > 5000)
		{
			GaugeGontroller(maxHP, nowHP, 6000, 5);
		}
		// 4000越え
		else if(nowHP > 4000)
		{
			GaugeGontroller(maxHP, nowHP, 5000, 4);
		}
		// 3000越え
		else if(nowHP > 3000)
		{
			GaugeGontroller(maxHP, nowHP, 4000, 3);
		}
		// 2000越え
		else if(nowHP > 2000)
		{
			GaugeGontroller(maxHP, nowHP, 3000, 2);
		}
		// 1000越え
		else if(nowHP > 1000)
		{
			GaugeGontroller(maxHP, nowHP, 2000, 1);
		}
		// 1000以下
		else
		{
			GaugeBG.fillAmount = (float)maxHP / 1000;
			Gauge.fillAmount = (float)nowHP / 1000;
			for(int i=0; i<OverHPGauge.Length; i++)
			{
				OverHPGauge[i].gameObject.SetActive(false);
			}
		}
	}

	public void GaugeGontroller(int maxHP, int nowHP, int overmaxhp, int starcount)
	{
		int now = nowHP - overmaxhp + 1000;
		Gauge.fillAmount = (float)now / 1000;
		
		for (int i = 0; i<OverHPGauge.Length; i++)
		{
			if (i < starcount)
			{
				OverHPGauge[i].gameObject.SetActive(true);
			}
			else
			{
				OverHPGauge[i].gameObject.SetActive(false);
			}
		}
	}
}
