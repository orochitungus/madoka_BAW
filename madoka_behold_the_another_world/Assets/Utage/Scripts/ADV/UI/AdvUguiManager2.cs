using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utage
{
	/// <summary>
	/// コントローラーやキーボードの入力を受け取れるようにAdvUguiManagerを拡張する
	/// </summary>
	public class AdvUguiManager2 : AdvUguiManager
	{
		
		public override void OnInput(BaseEventData data = null)
		{
			//文字送り
			Engine.Page.InputSendMessage();
		}

	
	}
}
