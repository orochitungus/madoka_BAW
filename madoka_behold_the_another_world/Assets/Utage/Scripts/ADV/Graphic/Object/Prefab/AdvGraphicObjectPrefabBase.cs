// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{

	/// <summary>
	/// プレハブオブジェクト表示のスーパークラス
	/// </summary>
	public abstract class AdvGraphicObjectPrefabBase : AdvGraphicBase
	{
		protected GameObject currentObject;

		//初期化処理
		public override void Init(AdvGraphicObject parentObject)
		{
			base.Init(parentObject);
		}

		//********描画時にクロスフェードが失敗するであろうかのチェック********//
		internal override bool CheckFailedCrossFade(AdvGraphicInfo grapic)
		{
			return LastResource != grapic;
		}

		//********描画時のリソース変更********//
		internal override void ChangeResourceOnDraw(AdvGraphicInfo grapic, float fadeTime)
		{
			//新しくリソースを設定
			if (LastResource != grapic)
			{
				currentObject = GameObject.Instantiate(grapic.File.UnityObject) as GameObject;
				Vector3 localPostion = currentObject.transform.localPosition;
				Vector3 localEulerAngles = currentObject.transform.localEulerAngles;
				Vector3 localScale = currentObject.transform.localScale;
				currentObject.transform.SetParent(this.transform);
				currentObject.transform.localPosition = localPostion;
				currentObject.transform.localScale = localScale;
				currentObject.transform.localEulerAngles = localEulerAngles;
				currentObject.ChangeLayerDeep(this.gameObject.layer);
				currentObject.gameObject.SetActive(true);
				ChangeResourceOnDrawSub(grapic);
			}

			if (LastResource == null)
			{
				ParentObject.FadeIn(fadeTime, () => { });
			}
		}

		//********描画時のリソース変更********//
		protected abstract void ChangeResourceOnDrawSub(AdvGraphicInfo grapic);
		//		{
		//			this.sprite = currentObject.GetComponent<SpriteRenderer>();
		//		}

		//拡大縮小の設定
		internal override void Scale(AdvGraphicInfo graphic)
		{
			this.transform.localScale = graphic.Scale * Layer.Manager.PixelsToUnits;
		}

		//配置
		internal override void Alignment(Utage.Alignment alignment, AdvGraphicInfo graphic)
		{
			this.transform.localPosition = graphic.Position;
		}

		//上下左右の反転
		internal override void Flip(bool flipX, bool flipY)
		{
		}

		//********描画時の引数適用********//
		internal override void SetCommandArg(AdvCommand command)
		{
			string stateName = command.ParseCellOptional<string>(AdvColumnName.Arg2, "");
			float fadeTime = command.ParseCellOptional<float>(AdvColumnName.Arg6, 0.2f);
			if (!string.IsNullOrEmpty(stateName))
			{
				Animator animator = GetComponentInChildren<Animator>();
				if (animator)
				{
					animator.CrossFade(stateName, fadeTime);
				}
				else
				{
					Animation animation = GetComponentInChildren<Animation>();
					if (animation != null)
					{
						animation.CrossFade(stateName, fadeTime);
					}
				}
			}
		}

		//ルール画像つきのフェードイン（オブジェクト単位にかけるのでテクスチャ描き込み効果なし）
		public override void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			Debug.LogError(this.gameObject.name + " is not support RuleFadeIn", this.gameObject);
			if (onComplete != null) onComplete();
		}

		//ルール画像つきのフェードアウト（オブジェクト単位にかけるのでテクスチャ描き込み効果なし）
		public override void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			Debug.LogError(this.gameObject.name + " is not support RuleFadeOut", this.gameObject);
			if (onComplete != null) onComplete();
		}
	}
}
