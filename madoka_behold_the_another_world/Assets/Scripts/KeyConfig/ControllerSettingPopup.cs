using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerSettingPopup : MonoBehaviour
{
    public KeyConfigController Keyconfingcontroller;

    /// <summary>
    /// OKボタンを押した場合、スティック設定に入る
    /// </summary>
    public void OnClickOKButton()
    {

    }

    /// <summary>
    /// キャンセルボタンを押した場合、キーコンフィグに移行する
    /// </summary>
    public void OnClickCancelButton()
    {
        // ついでに再出現しないように元のフラグも折っておく
        Keyconfingcontroller.Controllersetting.SetBool("SetRightStick", false);
        Keyconfingcontroller.Controllersetting.SetBool("", true);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
