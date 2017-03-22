// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimurausing UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ボイス再生
	/// </summary>
	internal class AdvCommandVoice : AdvCommand
	{

		public AdvCommandVoice(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			//名前
			this.characterLabel = ParseCell<string>(AdvColumnName.Arg1);

			//ボイス
			string voice = ParseCell<string>(AdvColumnName.Voice);
			//ボイスファイル設定
			if (AdvCommand.IsEditorErrorCheck)
			{
			}
			else
			{
				voiceFile = AddLoadFile(dataManager.BootSetting.GetLocalizeVoiceFilePath(voice), null);
			}
			this.isLoop = ParseCellOptional<bool>(AdvColumnName.Arg2, false);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (engine.Page.CheckSkip() && engine.Config.SkipVoiceAndSe) 
			{
				return;
			}
			engine.SoundManager.PlayVoice(characterLabel, voiceFile, isLoop);
		}

		protected string characterLabel;
		protected AssetFile voiceFile;
		bool isLoop;
	}
}