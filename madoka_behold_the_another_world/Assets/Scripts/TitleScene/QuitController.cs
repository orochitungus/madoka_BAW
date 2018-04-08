using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class QuitController : MonoBehaviour 
{
	/// <summary>
	/// 各ボタン
	/// </summary>
	public Image OKButton;
	public Image CancelButton;

	/// <summary>
	/// 非選択状態のスプライト
	/// </summary>
	public Sprite OKButtonDefault;
	public Sprite CancelButtonDefault;
	/// <summary>
	/// 選択状態のスプライト
	/// </summary>
	public Sprite Hilight;

	public int NowSelect;

	/// <summary>
	/// モードチェンジしたばかりか否か
	/// </summary>
	public bool ModeChangeDone;



	public TitleCanvas Titlecanvas;

	// Use this for initialization
	void Start () 
	{
		ModeChangeDone = true;
		this.UpdateAsObservable().Subscribe(_ => 
		{
			switch (NowSelect)
			{
				case 0:
					OKButton.sprite = Hilight;
					CancelButton.sprite = CancelButtonDefault;
					break;
				case 1:
					OKButton.sprite = OKButtonDefault;
					CancelButton.sprite = Hilight;
					break;
			}

			// 左
			if (ControllerManager.Instance.Left)
			{
				if (NowSelect == 1)
				{
					NowSelect--;
				}
			}
			// 右
			if (ControllerManager.Instance.Right)
			{
				if (NowSelect == 0)
				{
					NowSelect++;
				}
			}

			// 選択確定時の処理
			if (!ModeChangeDone && ControllerManager.Instance.Shot)
			{
				if (NowSelect == 0)
				{
					Application.Quit();
				}
				else
				{
					AudioManager.Instance.PlaySE("cursor");
					Titlecanvas.TitleCanvasAnimator.Play(Titlecanvas.QuitClose);
				}
			}

			// ボタンを離すとモードチェンジ瞬間フラグを折る(一旦ボタンを離さないと次へ行かせられない）
			if (!ControllerManager.Instance.Shot)
			{
				ModeChangeDone = false;
			}
		});

		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
