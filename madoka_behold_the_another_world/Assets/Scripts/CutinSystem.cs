using UnityEngine;
using System.Collections;

public class CutinSystem : MonoBehaviour 
{
	/// <summary>
	/// カットインの名前
	/// </summary>
	public enum CUTINNAME
	{
		CUTIN_MADOKA,
		CUTIN_SAYAKA,
		CUTIN_HOMURA_GUN,
		CUTIN_MAMI,
		CUTIN_KYOKO,
		CUTIN_YUMA,
		CUTIN_ORIKO,
		CUTIN_KIRIKA,
		CUTIN_SCONOSCIUTO,
		CUTIN_HOMURA_BOW,
		CUTIN_ULTIMATE_MADOKA,
		CUTINNUMBER
	}

	/// <summary>
	/// カットインとして表示されるグラフィック(Image)
	/// </summary>
	public GameObject[] m_CutinImages = new GameObject[(int)CUTINNAME.CUTINNUMBER];

	/// <summary>
	/// カットイン制御用アニメーター
	/// </summary>
	public CutinAnimation m_CutinAnimator;

	/// <summary>
	/// Canvasのルート
	/// </summary>
	public Canvas m_Canvas;

	public void Start()
	{
		m_Canvas.GetComponent<Canvas>().enabled = false;
	}

	/// <summary>
	/// カットインを表示する
	/// </summary>
	/// <param name="cutinname"></param>
	public void ShowCutin(CUTINNAME cutinname)
	{
		m_Canvas.GetComponent<Canvas>().enabled = true;
		m_CutinAnimator.StartAnimation();
		m_CutinImages[(int)cutinname].SetActive(true);
		
	}

	// カットインを消去する
	public void EraseCutin(CUTINNAME cutinname)
	{
		m_CutinImages[(int)cutinname].SetActive(false);
		m_CutinAnimator.EndAnimation();
		m_Canvas.GetComponent<Canvas>().enabled = false;
	}
}
