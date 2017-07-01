// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ムービー再生(Unity5.6以降のVideoClip版)
	/// </summary>
	internal class AdvCommandVideo : AdvCommand
	{
		public AdvCommandVideo(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.label = ParseCell<string>(AdvColumnName.Arg1);
			this.cameraName = ParseCell<string>(AdvColumnName.Arg2);
			this.loop = ParseCellOptional<bool>(AdvColumnName.Arg3, false);
			this.cancel = ParseCellOptional<bool>(AdvColumnName.Arg4, true);

			string path = FilePathUtil.Combine(dataManager.BootSetting.ResourceDir, "Video");
			path = FilePathUtil.Combine(path, label);
			this.file = AddLoadFile(path,null);

		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.GraphicManager.VideoManager.Play(label, cameraName, file, loop, cancel);
		}

		public override bool Wait(AdvEngine engine)
		{
			if (engine.UiManager.IsInputTrig)
			{
				engine.GraphicManager.VideoManager.Cancel(label);
			}
			bool isPlaying = engine.GraphicManager.VideoManager.IsPlaying(label);
			if (!isPlaying)
			{
				engine.GraphicManager.VideoManager.Remove(label);
			}
			return isPlaying;
		}

		AssetFile file;
		string label;
		bool loop;
		bool cancel;
		string cameraName;
	}
}
