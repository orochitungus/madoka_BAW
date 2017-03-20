using UnityEngine;
using System.Collections;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
	/// <summary>
	/// ゲームそのもののポーズを制御
	/// </summary>
	public PauseControllerInputDetector GamePauseController;

	/// <summary>
	/// 覚醒関係のポーズを制御
	/// </summary>
	public PauseControllerInputDetector ArousalPauseController;

	/// <summary>
	/// ラウンドコール関係のポーズを制御
	/// </summary>
	public PauseControllerInputDetector RoundCallPauseController;

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
