﻿// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;
using System.Collections;

namespace Utage
{

	/// <summary>
	/// メッセージウィドウ処理のサンプル
	/// </summary>
	[AddComponentMenu("Utage/ADV/UguiMessageWindowFaceIcon")]
	public class AdvUguiMessageWindowFaceIcon : MonoBehaviour
	{
		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>()); } }
		[SerializeField]
		protected AdvEngine engine;

		AdvGraphicObject targetObject;
		GameObject iconRoot;

		void Awake()
		{
			Engine.Page.OnChangeText.AddListener(OnChangeText);
			Engine.MessageWindowManager.OnReset.AddListener(OnReset);
		}

		void Update()
		{
			if (targetObject == null)
			{
				HideIcon();
			}
		}

		void OnReset(AdvMessageWindowManager window )
		{
			if (iconRoot != null)
			{
				GameObject.Destroy(iconRoot);
				iconRoot = null;
			}
		}

		//テキストに変更があった場合
		void OnChangeText(AdvPage page)
		{
			if (!TyrSetIcon(page))
			{
				targetObject = null;
				HideIcon();
			}
		}

		void HideIcon()
		{
			if (iconRoot != null && iconRoot.activeSelf)
			{
				iconRoot.SetActive(false);
			}
		}


		//テキストに変更があった場合
		bool TyrSetIcon(AdvPage page)
		{
			this.targetObject = null;
			AdvCharacterInfo info = page.CharacterInfo;
			if (info == null || info.Graphic == null || info.Graphic.Main == null )
			{
				return false;
			}

			AdvGraphicInfo graphic = info.Graphic.Main;
			AdvCharacterSettingData settingData = graphic.SettingData as AdvCharacterSettingData;
			if (settingData == null) return false;

			AdvCharacterSettingData.IconInfo iconInfo = settingData.Icon;
			if (iconInfo.IconType == AdvCharacterSettingData.IconInfo.Type.None) return false;

			this.targetObject = Engine.GraphicManager.FindObject(info.Label);

			switch (iconInfo.IconType)
			{
				case AdvCharacterSettingData.IconInfo.Type.IconImage:
					SetIconImage(iconInfo.File);
					return true;
				case AdvCharacterSettingData.IconInfo.Type.DicingPattern:
					SetIconDicingPattern(iconInfo.File, iconInfo.IconSubFileName);
					return true;
				case AdvCharacterSettingData.IconInfo.Type.RectImage:
					IconGraphicType type = ParseIconGraphicType(graphic, info.Label);
					switch (type)
					{
						case IconGraphicType.Default:
							SetIconRectImage(graphic, iconInfo.IconRect);
							return true;
						case IconGraphicType.Dicing:
							SetIconDicing(graphic, iconInfo.IconRect);
							return true;
						case IconGraphicType.RenderTexture:
							SetIconRenderTexture(iconInfo.IconRect);
							return true;
						case IconGraphicType.NotSupport:
						default:
							return false;
					}
				case AdvCharacterSettingData.IconInfo.Type.None:
				default:
					return false;
			}
		}

		void SetIconImage(AssetFile file)
		{
			AssetFileManager.Load(file,
				x=>
				{
					RawImage rawImage = ChangeIconComponent<RawImage>();
					rawImage.material = null;
					Texture2D texture = x.Texture;
					rawImage.texture = texture;
					rawImage.uvRect = new Rect(0, 0, 1, 1);
					ChangeReference(file, rawImage.gameObject);
				});
		}

		void SetIconDicingPattern(AssetFile file, string pattern)
		{
			DicingImage dicing = ChangeIconComponent<DicingImage>();
			DicingTextures dicingTexture = file.UnityObject as DicingTextures;

			dicing.DicingData = dicingTexture;
			dicing.ChangePattern(pattern);
			dicing.UvRect = new Rect(0, 0, 1, 1);
			ChangeReference(file, dicing.gameObject);
		}


		enum IconGraphicType
		{
			Default,
			Dicing,
			RenderTexture,
			NotSupport,
		};

		IconGraphicType ParseIconGraphicType(AdvGraphicInfo info, string characterLabel)
		{
			switch (info.FileType)
			{
				case AdvGraphicInfo.FileTypeDicing:
					return IconGraphicType.Dicing;
				case AdvGraphicInfo.FileType2D:
				case "":
					return IconGraphicType.Default;
				default:
					AdvGraphicObject obj = Engine.GraphicManager.FindObject(characterLabel);
					if (obj != null && obj.EnableRenderTexture)
					{
						return IconGraphicType.RenderTexture;
					}
					else
					{
						return IconGraphicType.NotSupport;
					}
			}
		}


		//アイコンのイメージを表示
		void SetIconRectImage(AdvGraphicInfo graphic, Rect rect)
		{
			RawImage rawImage = ChangeIconComponent<RawImage>();
			rawImage.material = null;
			Texture2D texture = graphic.File.Texture;
			rawImage.texture = texture;
			float w = texture.width;
			float h = texture.height;
			rawImage.uvRect = rect.ToUvRect(w, h);

			ChangeReference(graphic.File, rawImage.gameObject);
		}

		//ダイシングで表示する
		void SetIconDicing(AdvGraphicInfo graphic, Rect rect)
		{
			DicingImage dicing = ChangeIconComponent<DicingImage>();
			DicingTextures dicingTexture = graphic.File.UnityObject as DicingTextures;
			string pattern = graphic.SubFileName;

			dicing.DicingData = dicingTexture;
			dicing.ChangePattern(pattern);

			float w = dicing.PatternData.Width;
			float h = dicing.PatternData.Height;
			dicing.UvRect = rect.ToUvRect(w, h);

			ChangeReference(graphic.File, dicing.gameObject);
		}


		//RenderTextureの一部を表示
		void SetIconRenderTexture(Rect rect)
		{
			AdvGraphicObject obj = targetObject;
			if (obj.RenderTextureSpace == null)
			{
				return;
			}

			RawImage rawImage = ChangeIconComponent<RawImage>();
			if (obj.RenderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image)
			{
				rawImage.material = new Material(ShaderManager.DrawByRenderTexture);
			}
			Texture texture = obj.RenderTextureSpace.RenderTexture;
			rawImage.texture = texture;
			float w = texture.width;
			float h = texture.height;

			Transform t = obj.TargetObject.transform;
//			RectTransform rectTransform = obj.TargetObject.transform as RectTransform;
//			Rect clipRect = new Rect( w/2+ imageRect.xMin, h/2-imageRect.yMax, imageRect.width, imageRect.height);

			float scaleX = t.localScale.x;
			float scaleY = t.localScale.y;
			rect.position = new Vector2(rect.position.x * scaleX, rect.position.y * scaleY);
			rect.size = new Vector2(rect.size.x*scaleX, rect.size.y * scaleY);
//			clipRect = new Rect(clipRect.xMin + rect.xMin, clipRect.yMin + rect.yMin, rect.width, rect.height);
			rawImage.uvRect = rect.ToUvRect(w, h);
		}


		T ChangeIconComponent<T>() where T : Component
		{
			T component = null;
			if (iconRoot != null)
			{
				component = iconRoot.GetComponent<T>();
				if (component != null)
				{
					iconRoot.SetActive(true);
					return component;
				}
			}
			if (iconRoot != null)
			{
				GameObject.Destroy(iconRoot);
			}

			component = this.transform.AddChildGameObjectComponent<T>("Icon");
			iconRoot = component.gameObject;
			RectTransform rect = iconRoot.transform as RectTransform;
			if (rect != null)
			{
				rect.SetStretch();
			}
			return component;
		}

		void ChangeReference( AssetFile file, GameObject go )
		{
			//直前のファイルがあればそれを削除
			foreach (var item in go.GetComponents<AssetFileReference>())
			{
				Destroy(item);
			}

			file.AddReferenceComponent(go);
		}
	}
}
