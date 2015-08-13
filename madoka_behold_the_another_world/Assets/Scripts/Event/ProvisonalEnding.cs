using UnityEngine;
using System.Collections;

public class ProvisonalEnding : EventBase 
{

	// Use this for initialization
	void Start () 
	{
		
		EventInitialize();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (xstory)
        {
			case 0:
				keybreak = false;
				m_serif[0] = "プレイありがとうございました！";
				m_serif[1] = "今回はここまでとなります";
				m_serif[2] = "ご意見、ご感想は「http://page.freett.com/tungus/index.htm」";
				m_serif[3] = "までお願いします！";
				break;
			default:
				// タイトルへ飛ばす
				m_facetype = 0;
				m_drawname_jp = "";
				m_drawname_en = "";
				for (int i = 0; i < m_serif.Length; i++)
				{
					m_serif[i] = "";
				}
				savingparameter.beforeField = 0;
				Application.LoadLevel("title");
				break;
		}
		EventUpdate();
	}
}
