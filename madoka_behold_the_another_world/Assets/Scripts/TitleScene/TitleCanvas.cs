using UnityEngine;
using System.Collections;

public class TitleCanvas : MonoBehaviour 
{
	/// <summary>
	/// TitleCanvasのAnimator.主にステート管理に使う
	/// </summary>
	public Animator TitleCanvasAnimator;

	/// <summary>
	/// 各ステートの登録番号
	/// </summary>
	public int LogoStandby;
	public int LogoAppear;
	public int LogoFullAppear;
	public int LoadSelect;
	public int LoadFileSelectAppear;
	public int LoadFileSelectClose;

	public AnimatorStateInfo Animatorstate;  

	public void Awake()
	{
		// ステート登録
		LogoStandby = Animator.StringToHash("Base Layer.LogoStandby");
		LogoAppear = Animator.StringToHash("Base Layer.LogoAppear");
		LogoFullAppear = Animator.StringToHash("Base Layer.LogoFullAppear");
		LoadSelect = Animator.StringToHash("Base Layer.LoadSelect");
		LoadFileSelectAppear = Animator.StringToHash("Base Layer.LoadFileSelectAppear");
		LoadFileSelectClose = Animator.StringToHash("Base Layer.LoadFileSelectClose");
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Animatorstate = TitleCanvasAnimator.GetCurrentAnimatorStateInfo(0);
	}

	public void OnEnable()
	{
		
	}

	/// <summary>
	/// タイトル画面を表示する
	/// </summary>
	public void AppearDone()
	{
		TitleCanvasAnimator.SetBool("Appear", true);
	}

	/// <summary>
	/// メニュー画面に移行する
	/// </summary>
	public void ModeSelectSetup()
	{
		TitleCanvasAnimator.Play("Base Layer.LogoFullAppear");
	}

	/// <summary>
	/// ニューゲーム・ロードゲーム選択画面に遷移する
	/// </summary>
	public void LoadSelectSetup()
	{
		TitleCanvasAnimator.Play("Base Layer.LoadSelect");
	}
}
