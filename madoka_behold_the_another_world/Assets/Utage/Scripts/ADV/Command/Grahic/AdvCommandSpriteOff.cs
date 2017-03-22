// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura

namespace Utage
{

	/// <summary>
	/// コマンド：スプライト表示
	/// </summary>
	internal class AdvCommandSpriteOff : AdvCommand
	{
		public AdvCommandSpriteOff(StringGridRow row)
			: base(row)
		{
			this.name = ParseCellOptional<string>(AdvColumnName.Arg1, "");
			this.fadeTime = ParseCellOptional<float>(AdvColumnName.Arg6, fadeTime);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(name))
			{
				engine.GraphicManager.SpriteManager.FadeOutAll(fadeTime);
			}
			else
			{
				engine.GraphicManager.SpriteManager.FadeOut(name, fadeTime);
			}
		}

		string name;
		float fadeTime = 0.2f;
	}
}