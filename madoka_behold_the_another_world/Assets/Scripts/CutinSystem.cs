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
	/// カットインとして表示されるグラフィック(Canvas)
	/// </summary>
	public GameObject[] m_CutinImages = new GameObject[(int)CUTINNAME.CUTINNUMBER];

	public CutinAnimation[] m_CutinAnimations = new CutinAnimation[(int)CUTINNAME.CUTINNUMBER];

	/// <summary>
	/// カットインを表示する
	/// </summary>
	/// <param name="cutinname"></param>
	public void ShowCutin(CUTINNAME cutinname)
	{
		m_CutinImages[(int)cutinname].SetActive(true);
		m_CutinAnimations[(int)cutinname].StartAnimation();
	}

	// グラフィックを消去する
	public void EraseCutin(CUTINNAME cutinname)
	{
		m_CutinImages[(int)cutinname].SetActive(false);
	}
}
