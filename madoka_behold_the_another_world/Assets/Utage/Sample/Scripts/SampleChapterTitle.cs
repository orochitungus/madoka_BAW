// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using Utage;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 章別のDLサンプル
/// DLするかどうかでボタンを変える（実際には併用することはないと思われる）
/// </summary>
[AddComponentMenu("Utage/ADV/Examples/ChapterTitle")]
public class SampleChapterTitle : MonoBehaviour
{

	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>()); } }
	[SerializeField]
	protected AdvEngine engine;

	public UtageUguiTitle title;
	public UtageUguiLoadWait loadWait;
	public UtageUguiMainGame mainGame;
	public List<string> chapterUrlList = new List<string>();
	public List<string> startLabel = new List<string>();


	void Start()
	{
	}


	public void OnClickDownLoadChpater(int chapterIndex)
	{
		StartCoroutine(LoadChaptersAsync(chapterIndex));
	}

	IEnumerator LoadChaptersAsync(int chapterIndex)
	{ 
		//指定した章よりも前の章はロードする必要がある
		for (int i = 0; i < chapterIndex + 1; ++i )
		{
			string url = chapterUrlList[i];
			//もう設定済みならロードしない
			if (this.Engine.ExitsChapter(url)) continue;

			//ロード自体はこれだけ
			//ただし、URLは
			// http://madnesslabo.net/Utage3Chapter/Windows/chapter2.chapter.asset
			//のように、Windowsなどのプラットフォーム別にフォルダわけなどを終えた絶対URLが必要
			yield return this.Engine.LoadChapterAsync(url);
		}
		//設定データを反映
		this.Engine.GraphicManager.Remake(this.Engine.DataManager.SettingDataManager.LayerSetting);

		//パラメーターをデフォルト値でリセット
		//これは場合によってはリセットしたくない場合もあるので、あえて外にだす
		this.Engine.Param.InitDefaultAll(this.Engine.DataManager.SettingDataManager.DefaultParam);

		//リソースファイルのダウンロードを進めておく
		this.Engine.DataManager.DownloadAll();

		//ロード待ちのための画面遷移
		title.Close();
		loadWait.OpenOnChapter();
		loadWait.onClose.RemoveAllListeners();
		loadWait.onClose.AddListener(
			() =>
			{
				mainGame.Open();
				this.Engine.StartGame(startLabel[chapterIndex]);
			}
			);
	}
}
