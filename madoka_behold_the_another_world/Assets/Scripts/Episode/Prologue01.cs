using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utage;

public class Prologue01 : MonoBehaviour 
{
	/// <summary>
	/// カメラ１
	/// </summary>
	public Camera Camera1;

	/// <summary>
	/// ストーリーの進行度
	/// </summary>
	public int Xstory;

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
	/// キュゥべえのアニメーター
	/// </summary>
	public Animator KyubeyAnimator;

    /// <summary>
    /// スカイボックスのマテリアル
    /// </summary>
    public Material Sky;

	/// <summary>
	/// 制御を受け付けるか否か
	/// </summary>
	public bool IsControllable;

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


        // QBとほむらのモーションを初期化
        HomuraAnimator.SetTrigger("neutral");
		KyubeyAnimator.SetTrigger("neutral");

		// Xstory初期化
		Xstory = 0;

        // スカイボックス変更
        RenderSettings.skybox = Sky;

        // カメラを初期配置
        Camera1.transform.localPosition = new Vector3(-35, -103, -650);
        Camera1.fieldOfView = 60;
        Camera1.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    // Use this for initialization
    void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(Xstory)
        {
            case 0:
                // カメラをほむらたちの後ろまで下げる
                if(Camera1.transform.localPosition.z > -690)
                {
                    Camera1.transform.localPosition = new Vector3(Camera1.transform.localPosition.x, Camera1.transform.localPosition.y, Camera1.transform.localPosition.z - 0.5f); 
                }
                else
                {
                    // 切り替わりまでちょっと待つ
                    StartCoroutine(XStoryWait(1.0f));
                }
                break;
            case 1:
                // カメラをの向きを変える
                Camera1.transform.localPosition = new Vector3(-34.6f, -105.2179f, -670.6f);
                Camera1.transform.localRotation = Quaternion.Euler(new Vector3(0,180,0));
                Camera1.fieldOfView = 44;
                // シナリオを再生する
                StartCoroutine(CoTalk("Prologue01_1"));
                break;
            default:

                break;
        }

        // ショットキーの入力受け取り
        if(ControllerManager.Instance.Shot)
        {
            // タッチの疑似情報を作る 
        }
	}

    private IEnumerator XStoryWait(float time)
    {
        yield return new WaitForSeconds(time);
        Xstory++;
    }

    /// <summary>
    /// 「宴」のシナリオを再生する
    /// </summary>
    /// <param name="scenarioLabel"></param>
    /// <returns></returns>
    private IEnumerator CoTalk(string scenarioLabel)
    {
        //「宴」のシナリオを呼び出す
        Engine.JumpScenario(scenarioLabel);

        //「宴」のシナリオ終了待ち
        while (!Engine.IsEndScenario)
        {
            yield return 0;
        }
        Xstory++;
    }

}
