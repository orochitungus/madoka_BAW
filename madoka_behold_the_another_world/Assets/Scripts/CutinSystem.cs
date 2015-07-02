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
	/// カットインとして表示されるグラフィック
	/// </summary>
	public GameObject[] m_CutinImages = new GameObject[(int)CUTINNAME.CUTINNUMBER];

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
