// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{

	/// <summary>
	/// フェード切り替え機能つきのスプライト表示
	/// </summary>
	public abstract class AdvGraphicObjectUguiBase : AdvGraphicBase
	{
		//初期化処理
		public override void Init(AdvGraphicObject parentObject)
		{
			base.Init(parentObject);

			AddGraphicComponentOnInit();

			if (GetComponent<IAdvClickEvent>() == null)
			{
				this.gameObject.AddComponent<AdvClickEvent>();
			}
		}

		//初期化時のコンポーネント追加処理
		protected abstract void AddGraphicComponentOnInit();
		protected abstract Material Material { get; set; }

		//拡大縮小の設定
		internal override void Scale(AdvGraphicInfo graphic)
		{
			RectTransform rectTransform = this.transform as RectTransform;
			rectTransform.localScale = graphic.Scale;
		}

		//配置
		internal override void Alignment(Utage.Alignment alignment, AdvGraphicInfo graphic)
		{
			RectTransform rectTransform = this.transform as RectTransform;
			rectTransform.pivot = graphic.Pivot;
			rectTransform.Alignment(alignment, graphic.Position);
		}
	}
}
