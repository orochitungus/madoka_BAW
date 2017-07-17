using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		iTween.MoveTo(gameObject, iTween.Hash(
						// 移動先指定
						"position", new Vector3(793, 183, 0),
						// 移動時間指定
						"time", 0.5f,
						// 終了時Status呼び出し
						"oncomplete", "Return"
					));
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void Return()
	{
		iTween.MoveTo(gameObject, new Vector3(0, 0, 0), 0.5f);
	}
}
