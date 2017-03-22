// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Utage;


/// <summary>
/// チャプター起動のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/StartChapter")]
public class UtageUguiStartChapter : MonoBehaviour
{
	public UtageUguiTitle title;
	public string chapterUrl;
	public string startLabel;

	public void OpenChapter()
	{
	}
}
