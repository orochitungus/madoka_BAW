using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour 
{
	public CutinSystem m_CutinSystem;

	public void Awake()
	{
		m_CutinSystem.ShowCutin();
	}
}
