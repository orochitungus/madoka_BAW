using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class LoadManager : SingletonMonoBehaviour<LoadManager>
{
    /// <summary>
    /// 遷移先のシーン名
    /// </summary>
    public string NextScene;

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
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SceneChange()
    {
        StartCoroutine(GotoNextScene());
    }

    /// <summary>
    /// 次のシーンに遷移しつつフェードを解除する
    /// </summary>
    /// <returns></returns>
    private IEnumerator GotoNextScene()
    {
        // 一応fフレームが終わるまで待つ
        yield return new WaitForEndOfFrame();
        // 次のシーンへ遷移させる
        SceneManager.LoadScene(NextScene);
        // 一応NextSceneは解除
        NextScene = "";
        // フェードを解除する
        StartCoroutine(FadeManager.Instance.FadeOutDone(1.0f));
    }
}
