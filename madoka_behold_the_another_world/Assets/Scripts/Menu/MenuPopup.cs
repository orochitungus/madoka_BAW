using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class MenuPopup : MonoBehaviour 
{
	public Text PopupText;
	
	[SerializeField]
	private Image OKButton;

	[SerializeField]
	public Image CancelButton;

	public int NowSelect;

	// Use this for initialization
	void Start () 
	{
		this.UpdateAsObservable().Subscribe(_ => 
		{
			if(NowSelect == 0)
			{
				OKButton.color = new Color(1, 0, 0);
				CancelButton.color = new Color(1, 1, 1);
			}
			else
			{
				OKButton.color = new Color(1, 1, 1);
				CancelButton.color = new Color(1, 0, 0);
			}
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
