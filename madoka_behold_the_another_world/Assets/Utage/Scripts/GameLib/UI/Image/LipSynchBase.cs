// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Utage
{
	//リップシンクのタイプ
	[System.Flags]
	public enum LipSynchType
	{
		Text,               //テキストのみ
		Voice,              //ボイスが鳴っている場合は、そのボイスに合わせてリップシンク
		TextAndVoice,       //テキストとボイス
	};

	[System.Serializable]
	public class LipSynchEvent : UnityEvent<LipSynchBase> { }
	/// <summary>
	/// まばたき処理の基底クラス
	/// </summary>
	public abstract class LipSynchBase : MonoBehaviour
	{
		public LipSynchType Type { get { return type; } set { type = value; } }
		[SerializeField]
		LipSynchType type = LipSynchType.TextAndVoice;

		public float Duration { get { return duration; } set { duration = value; } }
		[SerializeField]
		float duration = 0.2f;

		public float Interval { get { return interval; } set { interval = value; } }
		[SerializeField]
		float interval = 0.2f;

		//ボイス音量に合わせて口パクする際のスケール値
		public float ScaleVoiceVolume { get { return scaleVoiceVolume; } set { scaleVoiceVolume = value; } }
		[SerializeField]
		float scaleVoiceVolume = 1;

		//口のパターンタグ
		public string LipTag { get { return lipTag; } set { lipTag = value; } }
		[SerializeField]
		string lipTag = "lip";

		//アニメーションデータ
		public MiniAnimationData AnimationData { get { return animationData; } set { animationData = value; } }
		[SerializeField]
		MiniAnimationData animationData = new MiniAnimationData();

		//リップシンクのボリューム(0～1。0以下の場合は無効)
		public float LipSyncVolume { get; set; }
		

		public GameObject Target
		{
			get { if (target == null) { target = this.gameObject; } return target; }
			set { target = value; }
		}
		[SerializeField]
		GameObject target;

		//テキストのリップシンクを現在有効になっているか
		//外部から変更する
		public bool EnableTextLipSync
		{
			get { return enableTextLipSync; }
			set { enableTextLipSync = value; }
		}
		[SerializeField]
		bool enableTextLipSync;

		//テキストのリップシンクチェック
		public LipSynchEvent OnCheckTextLipSync = new LipSynchEvent();

		//ターゲットのキャラクターラベルを取得
		public string CharacterLabel
		{
			get
			{
				if (string.IsNullOrEmpty(characterLabel))
				{
					return this.gameObject.name;
				}
				else
				{
					return characterLabel;
				}
			}
			set
			{
				characterLabel = value;
			}
		}
		string characterLabel;

		//有効か
		public bool IsEnable { get; set; }

		//再生中か
		public bool IsPlaying { get; set; }

		//再生
		public void Play()
		{
			IsEnable = true;
		}

		//強制終了
		public void Cancel()
		{
			IsEnable = false;
			IsPlaying = false;
			if (coLypSync != null)
			{
				StopCoroutine(coLypSync);
				coLypSync = null;
			}
		}

		//更新
		void Update()
		{
			bool enableLipSync = IsEnable && (CheckVoiceLipSync() || CheckTextLipSync() );
			if (enableLipSync)
			{
				if (!IsPlaying)
				{
					IsPlaying = true;
					StartLipSync();
				}
				UpdateLipSync();
			}
			else
			{
				if (IsPlaying)
				{
					IsPlaying = false;
				}
			}
		}

		bool CheckVoiceLipSync()
		{
			switch (Type)
			{
				case LipSynchType.Voice:
				case LipSynchType.TextAndVoice:
					SoundManager soundManager = SoundManager.GetInstance();
					if (soundManager != null)
					{
						if (soundManager.IsPlayingVoice(CharacterLabel))
						{
							return true;
						}
					}
					break;
				default:
					break;
			}
			return false;
		}

		bool CheckTextLipSync()
		{
			switch (Type)
			{
				case LipSynchType.Text:
				case LipSynchType.TextAndVoice:
					{
						OnCheckTextLipSync.Invoke(this);
						return EnableTextLipSync;
					}
				default:
					break;
			}
			return false;
		}

		void UpdateLipSync()
		{
			if (CheckVoiceLipSync())
			{
				LipSyncVolume = (SoundManager.GetInstance().GetVoiceSamplesVolume(CharacterLabel) * ScaleVoiceVolume);
			}
			else
			{
				LipSyncVolume = -1;
			}
		}

		protected Coroutine coLypSync;

		protected void StartLipSync()
		{
			if (coLypSync == null)
			{
				coLypSync = StartCoroutine(CoUpdateLipSync());
			}
		}

		protected abstract IEnumerator CoUpdateLipSync();
	}
}
