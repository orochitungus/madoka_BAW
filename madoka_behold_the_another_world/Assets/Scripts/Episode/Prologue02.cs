using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utage;
using UniRx;
using UniRx.Triggers;

public class Prologue02 : MonoBehaviour
{
    /// <summary>
	/// カメラ１
	/// </summary>
	public Camera Camera1;

    /// <summary>
	/// 宴呼び出し
	/// </summary>
	AdvEngine Engine { get { return engine ?? (engine = FindObjectOfType<AdvEngine>()); } }
    public AdvEngine engine;

    /// <summary>
	/// ほむらのアニメーター
	/// </summary>
	public Animator HomuraAnimator;

    /// <summary>
    /// スコノシュートのアニメーター
    /// </summary>
    public Animator SconosciutoAnimator;

    /// <summary>
    /// スコノシュート白衣のアニメーター
    /// </summary>
    public Animator SconosciutoWhiteCortAnimator;

    /// <summary>
    /// スコノシュート
    /// </summary>
    public GameObject Sconosciuto;

    /// <summary>
    /// スコノシュート白衣
    /// </summary>
    public GameObject SconosciutoWhiteCort;

    /// <summary>
	/// スカイボックスのマテリアル
	/// </summary>
	public Material Sky;

    /// <summary>
	/// 宴の表示部分を呼び出す
	/// </summary>
	public AdvUguiManager2 Advuguimanager2;

    void Awake()
    {
        // ADVEngine呼び出し
        engine = GameObject.Find("AdvEngine").GetComponent<AdvEngine>();

        // ControllerManagerがあるか判定
        if (GameObject.Find("ControllerManager") == null)
        {
            // 無ければ作る
            GameObject loadManager = (GameObject)Instantiate(Resources.Load("ControllerManager"));
            loadManager.name = "ControllerManager";
        }

        // AudioManagerがあるか判定
        if (GameObject.Find("AudioManager") == null)
        {
            // なければ作る
            GameObject loadManager = (GameObject)Instantiate(Resources.Load("AudioManager"));
            loadManager.name = "AudioManager";
        }

        // モーションを初期化する
        HomuraAnimator.SetTrigger("neutral");
        SconosciutoWhiteCortAnimator.SetTrigger("neutral");
        SconosciutoAnimator.SetTrigger("fall");

        // スカイボックス変更
        RenderSettings.skybox = Sky;

        // カメラを初期配置
        Camera1.transform.localPosition = new Vector3(-23.10001f, -27.6f, 257.5f);
        Camera1.fieldOfView = 18;
        Camera1.transform.localRotation = Quaternion.Euler(new Vector3(-1.117f,0,0));
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        // ショットキーの入力受け取り
        if (ControllerManager.Instance.Shot)
        {
            Advuguimanager2.OnInput();
        }
    }

    /// <summary>
	/// 「宴」のシナリオを再生する
	/// </summary>
	/// <param name="scenarioLabel"></param>
	/// <returns></returns>
	private IEnumerator CoTalk(string scenarioLabel)
    {
        iTween.RotateTo(Camera1.gameObject, iTween.Hash("x", 35,"islocal", true));
        yield return new WaitForSeconds(3.0f);
    }
}
