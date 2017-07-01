using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Camera/CameraRoot")]
	public class CameraRoot : MonoBehaviour
	{
		public LetterBoxCamera LetterBoxCamera
		{
			get
			{
				if (letterBoxCamera == null)
				{
					letterBoxCamera = this.gameObject.GetComponentInChildren<LetterBoxCamera>(true);
				}
				return letterBoxCamera;
			}
		}
		LetterBoxCamera letterBoxCamera;

		const int Version = 0;
		//セーブデータ用のバイナリ書き込み
		public void Write(BinaryWriter writer)
		{
			writer.Write(Version);
			writer.WriteLocalTransform(this.transform);
			writer.WriteBuffer(LetterBoxCamera.Write);
			ImageEffectBase[] list = LetterBoxCamera.GetComponents<ImageEffectBase>();
			writer.Write(list.Length);
			for ( int i = 0; i < list.Length; ++i)
			{
				ImageEffectBase effect = list[i];
				string type = ImageEffectUtil.ToImageEffectType(effect.GetType());
				writer.Write(type);
				writer.WriteBuffer(list[i].Write);
			}
		}

		//セーブデータ用のバイナリ読み込み
		public void Read(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version < 0 || version > Version)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
				return;
			}

			reader.ReadLocalTransform(this.transform);
			reader.ReadBuffer(LetterBoxCamera.Read);

			int count = reader.ReadInt32();
			for (int i = 0; i < count; ++i)
			{
				string imageEffectType = reader.ReadString();
				ImageEffectBase imageEffect;
				bool alreadyEnabled;
				if (!ImageEffectUtil.TryGetComonentCreateIfMissing(imageEffectType, out imageEffect, out alreadyEnabled, LetterBoxCamera.gameObject))
				{
					Debug.LogError("Unkonwo Image Effect Type [ " + imageEffectType  +" ]");
					reader.SkipBuffer();
				}
				else
				{
					reader.ReadBuffer(imageEffect.Read);
				}
			}
		}
	}
}