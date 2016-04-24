using UnityEngine;
using System.Collections;

public class OPCamera : MonoBehaviour 
{
	public TitleController Titlecontroller;

	public ControllerManager Controllermanager;

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
			Titlecontroller.TitleCanvas.gameObject.SetActive(true);
			Titlecontroller.TitleCanvas.GetComponent<TitleCanvas>().AppearDone();
		}
	}

	/// <summary>
	/// タイトル画面へ移行する
	/// </summary>
	public void TitleSetup()
	{
		Titlecontroller.TitleCanvas.gameObject.SetActive(true);
	}
}
