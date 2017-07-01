// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UtageExtensions;
#if UNITY_5_6_OR_NEWER
using UnityEngine.Video;
#endif

namespace Utage
{

	/// <summary>
	/// ビデオ表示の管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/VideoManager")]
	public class AdvVideoManager : MonoBehaviour
	{
		public AdvEngine Engine { get { return engine ?? (engine = this.GetComponentInParent<AdvEngine>()); } }
		AdvEngine engine;

#if UNITY_5_6_OR_NEWER
		class VideoInfo
		{
			public bool Cancel { get; set; }
			public VideoPlayer Player { get; set; }
		}

		Dictionary<string, VideoInfo> Videos { get { return videos; } }
		Dictionary<string, VideoInfo> videos = new Dictionary<string, VideoInfo>();

		internal void Play(string label, string cameraName, AssetFile file, bool loop, bool cancel)
		{
			VideoInfo info = new VideoInfo() { Cancel = cancel, };
			Videos.Add(label, info);
			GameObject go = this.transform.AddChildGameObject(label);
			VideoPlayer videoPlayer = go.AddComponent<VideoPlayer>();
			videoPlayer.isLooping = loop;
			videoPlayer.clip = file.UnityObject as VideoClip;
			videoPlayer.targetCamera = Engine.EffectManager.FindTarget(AdvEffectManager.TargetType.Camera, cameraName).GetComponentInChildren<Camera>();
			videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
			videoPlayer.Play();
			info.Player = videoPlayer;
		}

		internal void Cancel(string label)
		{
			if (!Videos[label].Cancel)
			{
				return;
			}
			Videos[label].Player.Stop();
		}

		internal bool IsPlaying(string label)
		{
			if (!Videos.ContainsKey(label)) return false;

			return Videos[label].Player.isPlaying;
		}

		internal void Remove(string label)
		{
			GameObject.Destroy( Videos[label].Player.gameObject);
			Videos.Remove(label);
		}
#else
		internal void Play(string label, string cameraName, AssetFile file, bool loop, bool cancel)
		{

		}

		internal void Cancel(string label)
		{
		}

		internal bool IsPlaying(string label)
		{
			return false;
		}

		internal void Remove(string label)
		{
		}
#endif
	}
}
