using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 一定時間で消失させたいエフェクトなどに用いる
/// </summary>
public class Hiteffect : MonoBehaviour 
{
	// エフェクトの維持時間
	public float LifeTime;	

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(Eraser());
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private IEnumerator Eraser()
	{
		yield return new WaitForSeconds(LifeTime);
		Destroy(gameObject);
	}
}
