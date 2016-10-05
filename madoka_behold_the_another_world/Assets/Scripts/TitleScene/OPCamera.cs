using UnityEngine;
using System.Collections;

public class OPCamera : MonoBehaviour 
{
	public TitleController Titlecontroller;


	/// <summary>
	/// TitleCanvasにくっついているタイトル画面制御用のAnimator
	/// </summary>
	Animator StateAnimator;
	int []stateCode = new int[4];

	void Awake()
	{
		// Animatorを取得する
		StateAnimator = Titlecontroller.TitleCanvas.GetComponent<TitleCanvas>().TitleCanvasAnimator;
		// モーション登録
		stateCode[0] = Animator.StringToHash("Base Layer.LogoStandby");
		stateCode[1] = Animator.StringToHash("Base Layer.LogoAppear");
		stateCode[2] = Animator.StringToHash("Base Layer.LogoFullAppear");
		stateCode[3] = Animator.StringToHash("BAse Layer.LoadSelect");
	}

	// Use this for initialization
	void Start () 
	{        
    }
	

	// Update is called once per frame
	void Update () 
	{
		// コントローラーのショットキーが押されたらタイトル画面へ移行する
		if(ControllerManager.Instance.Shot)
		{
			AnimatorStateInfo state = Titlecontroller.TitleCanvas.GetComponent<TitleCanvas>().TitleCanvasAnimator.GetCurrentAnimatorStateInfo(0);

			
			// タイトル表示中にショットキーが押されたら、SEを鳴らしてメニュー画面へ移行する
			if(state.fullPathHash == stateCode[1])
			{
				AudioManager.Instance.PlaySE("OK");	
				Titlecontroller.TitleCanvas.GetComponent<TitleCanvas>().ModeSelectSetup();
				Titlecontroller.Mainmanucontroller.NowSelect = 0;
				Titlecontroller.Mainmanucontroller.ModeChangeDone = true;
			}
			// メインメニュー表示中
			else if(state.fullPathHash == stateCode[2])
			{
				
			}
            // OPデモ中
            else            
            {
                Titlecontroller.TitleCanvas.gameObject.SetActive(true);
                Titlecontroller.TitleCanvas.GetComponent<TitleCanvas>().AppearDone();
            }
        }
	}

	/// <summary>
	/// タイトル画面へ移行する
	/// </summary>
	public void TitleSetup()
	{
		Titlecontroller.TitleCanvas.gameObject.SetActive(true);
        Titlecontroller.TitleCanvas.GetComponent<TitleCanvas>().AppearDone();
    }
}
