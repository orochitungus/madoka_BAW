// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：フェードイン処理
	/// </summary>
	internal abstract class AdvCommandFadeBase: AdvCommandEffectBase
	{
		float time;
		bool inverse;
		Color color;

		public AdvCommandFadeBase(StringGridRow row, bool inverse)
			: base(row)
		{
			this.inverse = inverse;
		}

		protected override void OnParse()
		{
			this.color = ParseCellOptional<Color>(AdvColumnName.Arg1, Color.white);
			if (IsEmptyCell(AdvColumnName.Arg2))
			{
				this.targetName = "SpriteCamera";
			}
			else
			{
				//第2引数はターゲットの設定
				this.targetName = ParseCell<string>(AdvColumnName.Arg2);
			}

			this.time = ParseCellOptional<float>(AdvColumnName.Arg6,0.2f);

			this.targetType = AdvEffectManager.TargetType.Camera;

			ParseWait(AdvColumnName.WaitType);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			Camera camera = target.GetComponentInChildren<Camera>(true);

			ImageEffectBase imageEffect;
			ImageEffectUtil.TryGetComonentCreateIfMissing(ImageEffectType.ColorFade.ToString(), out imageEffect, camera.gameObject);
			imageEffect.enabled = true;

			ColorFade colorFade = imageEffect as ColorFade;

			float start = inverse ? colorFade.color.a : 0;
			float end = inverse ? 0 : this.color.a;
			colorFade.color = color;

			Timer timer = camera.gameObject.AddComponent<Timer>();
			timer.AutoDestroy = true;
			timer.StartTimer(
				engine.Page.ToSkippedTime(this.time),
				(x) =>
				{
					colorFade.Strength = x.GetCurve(start, end);
				},
				(x) =>
				{
					OnComplete(thread);
					if (inverse)
					{
						imageEffect.enabled = false;
					}
				});
		}
	}
}
