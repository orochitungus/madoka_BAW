using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 選択肢表示用のクラス.これを継承して選択肢を作ること。選択肢の背景はStageSettingQuestが持っているのでそこから用意
/// </summary>
public class Selector : MonoBehaviour
{
	/// <summary>
	/// 文字列表示の背景
	/// </summary>
	public Image BackGround;

	/// <summary>
	/// 選択肢の出現条件最小
	/// </summary>
	public int []StoryConditionMin;
	/// <summary>
	/// 選択肢の出現条件最大
	/// </summary>
	public int []StoryConditionMax;

	/// <summary>
	/// 選択肢のタイトル
	/// </summary>
	public Text SelectTitle;

	/// <summary>
	/// 選択肢のテキスト
	/// </summary>
	public Text[]SelectText;

	/// <summary>
	/// 選択肢の名前
	/// </summary>
	public string[]SelectName;

	/// <summary>
	/// 行き先のコード(-1ならキャンセル)
	/// </summary>
	public int[]ForCode;

	/// <summary>
	/// 行き先のシーンの名前
	/// </summary>
	public string []NextScene;

	/// <summary>
	/// 移動元のコード
	/// </summary>
	public int FromCode;

	/// <summary>
	/// カーソル
	/// </summary>
	public Text []Cursor;

	/// <summary>
	/// 現在の選択中の選択肢
	/// </summary>
	protected int NowSelect;

	/// <summary>
	/// 選択肢の最大値
	/// </summary>
	public int MaxSelect;

	// 1F前の上入力
	private bool _PreTopInput;
	// 1F前の下入力
	private bool _PreUnderInput;

	/// <summary>
	/// 接触したプレイヤーオブジェクト
	/// </summary>
	protected GameObject Player;

	/// <summary>
	/// 選択したか否か
	/// </summary>
	protected SelectMode Select;

	// Use this for initialization
	void Start()
	{
		CursorController();
	}

	// Update is called once per frame
	void Update()
	{

	}

	/// <summary>
	/// 接触時に選択肢を表示する
	/// </summary>
	/// <param name="collision"></param>
	protected virtual void OnCollisionEnter(Collision collision)
	{
		Select = SelectMode.STANDBY;
		// プレイヤーキャラクターの移動を封じる
		collision.transform.GetComponent<CharacterControlBaseQuest>().Moveable = false;
		// プレイヤーキャラクターのアニメを止める
		Animator animator = collision.transform.GetComponent<Animator>();
		animator.SetTrigger("Idle");
		NowSelect = 0;
		// 選択肢をアクティブにする
		BackGround.gameObject.SetActive(true);
		// プレイヤーを保持しておく
		Player = collision.gameObject;
	}

	/// <summary>
	/// キャンセル系の選択肢を選択したときに動作を戻す
	/// </summary>
	public void CancelDone()
	{
		// 選択肢を非アクティブにする
		BackGround.gameObject.SetActive(false);
		// プレイヤーキャラクターの移動を復帰させる
		Player.GetComponent<CharacterControlBaseQuest>().Moveable = true;
	}

	protected void CursorController()
	{
		this.UpdateAsObservable().Where(_ => BackGround.gameObject.activeSelf).Subscribe(_ =>
		{
			// 上
			if (!_PreTopInput && ControllerManager.Instance.Top)
			{
				if (NowSelect > 0)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect--;
				}
			}
			// 下
			if (!_PreUnderInput && ControllerManager.Instance.Under)
			{
				if (NowSelect < MaxSelect && ForCode[NowSelect + 1] != -2)
				{
					AudioManager.Instance.PlaySE("cursor");
					NowSelect++;
				}
			}

			// 入力更新
			// 上
			_PreTopInput = ControllerManager.Instance.Top;
			// 下
			_PreUnderInput = ControllerManager.Instance.Under;

			// カーソル位置更新
			for (int i = 0; i < MaxSelect; i++)
			{
				if (i == NowSelect)
				{
					Cursor[i].gameObject.SetActive(true);
				}
				else
				{
					Cursor[i].gameObject.SetActive(false);
				}
			}

			// 選択確定
			if (Select == SelectMode.STANDBY && ControllerManager.Instance.Shot)
			{
				Select = SelectMode.SELECT;
				AudioManager.Instance.PlaySE("OK");
			}
		});
	}
}

/// <summary>
/// 開く扉などのステート
/// </summary>
public enum SelectMode
{
	STANDBY,	// 開く前
	SELECT,		// 選択した後
	OPEN		// 開いて次ステージへ飛ばす処理
}
