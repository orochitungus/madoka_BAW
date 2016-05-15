using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Test2 : MonoBehaviour 
{
	public Text TestWindow;

	// Use this for initialization
	void Start () 
	{
	
	}

	// Update is called once per frame
	void Update()
	{
		// キー入力
		if (Input.anyKeyDown)
		{
			Array k = Enum.GetValues(typeof(KeyCode));
			for (int i = 0; i < k.Length; i++)
			{
				// キー取得
				if (Input.GetKeyDown((KeyCode)k.GetValue(i)))
				{
					TestWindow.text = k.GetValue(i).ToString();
				}
			}
		}
	}
}
