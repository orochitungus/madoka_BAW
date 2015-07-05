using UnityEngine;
using System.Collections;

public class CutinSystem : MonoBehaviour 
{

	/// <summary>
	/// カットインとして表示されるグラフィック(Image)
	/// </summary>
	public GameObject m_CutinImage;

	/// <summary>
	/// カットインを表示する
	/// </summary>
	/// <param name="cutinname"></param>
	public void ShowCutin()
	{
		m_CutinImage.SetActive(true);
	}
}
