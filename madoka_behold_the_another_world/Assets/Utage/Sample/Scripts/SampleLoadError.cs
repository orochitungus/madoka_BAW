// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using Utage;
using System.Collections;


/// <summary>
/// Sample LoadErrorのコールバック関数を書き換え
/// </summary>
[AddComponentMenu("Utage/ADV/Examples/SampleLoadError")]
public class SampleLoadError : MonoBehaviour
{
	void Awake()
	{
		AssetFileManager.SetLoadErrorCallBack(CustomCallbackFileLoadError);
	}

	void CustomCallbackFileLoadError(AssetFile file)
	{
		string errorMsg = "インターネットに接続した状況でプレイしてください";
		if (SystemUi.GetInstance() != null)
		{
			//リロードを促すダイアログを表示
			SystemUi.GetInstance().OpenDialog1Button(
				errorMsg, LanguageSystemText.LocalizeText(SystemText.Retry),
				()=>OnRetry(file));
			this.gameObject.SetActive(false);
		}
		else
		{
			OnRetry(file);
		}
	}

	void OnRetry(AssetFile file)
	{
		AssetFileManager.ReloadFile(file);
	}
}

/// Sample LoadErrorのコールバック関数を書き換え処理を追加
namespace Utage
{
}
