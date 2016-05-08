using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class OptionButtonController : MonoBehaviour 
{
	/// <summary>
	/// 背景テクスチャー
	/// </summary>
	public Image BGTexture;

	/// <summary>
	/// 選択時の背景
	/// </summary>
	public Sprite SelectedTexture;

	/// <summary>
	/// 非選択時の背景
	/// </summary>
	public Sprite UnSelectedTexture;

	public bool IsSelect;

	// Use this for initialization
	void Start () 
	{
		this.UpdateAsObservable().Subscribe(_ => 
		{
			if(IsSelect)
			{
				BGTexture.sprite = SelectedTexture;
			}
			else
			{
				BGTexture.sprite = UnSelectedTexture;
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
