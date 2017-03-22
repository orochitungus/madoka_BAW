// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UtageExtensions;

namespace Utage
{

	/// <summary>
	/// クロスフェード可能なRawImage表示
	/// </summary>
	[AddComponentMenu("Utage/Lib/UI/CrossFadeRawImage")]
	public class UguiCrossFadeRawImage : MonoBehaviour, IMeshModifier, IMaterialModifier
	{
		public Texture FadeTexture
		{
			get
			{
				return fadeTexture;
			}
			set
			{
				if (fadeTexture == value)
					return;

				fadeTexture = value;
				Target.SetVerticesDirty();
				Target.SetMaterialDirty();
			}
		}
		[SerializeField]
		Texture fadeTexture;


		float Strengh
		{
			get { return strengh; }
			set
			{
				strengh = value;
				Target.SetMaterialDirty();
			}
		}


		[SerializeField, Range(0, 1.0f)]
		float strengh = 1;

		public virtual Graphic Target { get { return target ?? (target = GetComponent<RawImage>()); } }
		protected Graphic target;

		Timer Timer
		{
			get
			{
				if (timer == null)
				{
					timer = this.gameObject.AddComponent<Timer>();
				}
				return timer;
			}
		}
		Timer timer;

		Material lastMaterial;
		public Material Material
		{
			get
			{
				return Target.material;
			}
			set
			{
				Target.material = value;
			}
		}
		Material corssFadeMaterial;

		void Awake()
		{
			lastMaterial = Target.material;
			corssFadeMaterial = new Material(ShaderManager.CrossFade);
			Material = corssFadeMaterial;
		}

		void OnDestroy()
		{
			Material = lastMaterial;
			Destroy(corssFadeMaterial);
			Destroy(timer);
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			Target.SetVerticesDirty();
			Target.SetMaterialDirty();
		}
#endif

		public void ModifyMesh(Mesh mesh)
		{
			using (var helper = new VertexHelper(mesh))
			{
				ModifyMesh(helper);
				helper.FillMesh(mesh);
			}
		}

		public void ModifyMesh(VertexHelper vh)
		{
			Texture tex = Target.mainTexture;
			if (tex == null) return;

			RebuildVertex(vh);
		}

		public virtual void RebuildVertex(VertexHelper vh)
		{
			vh.Clear();
			var r = Target.GetPixelAdjustedRect();
			var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

			var color32 = Target.color;

			Rect uvRect = (Target as RawImage).uvRect;

			vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uvRect.xMin, uvRect.yMin), new Vector2(uvRect.xMin, uvRect.yMin), Vector3.zero, Vector4.zero);
			vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uvRect.xMin, uvRect.yMax), new Vector2(uvRect.xMin, uvRect.yMax), Vector3.zero, Vector4.zero);
			vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uvRect.xMax, uvRect.yMax), new Vector2(uvRect.xMax, uvRect.yMax), Vector3.zero, Vector4.zero);
			vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uvRect.xMax, uvRect.yMin), new Vector2(uvRect.xMax, uvRect.yMin), Vector3.zero, Vector4.zero);

			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}


		public Material GetModifiedMaterial(Material baseMaterial)
		{
			baseMaterial.SetFloat("_Strength", Strengh);
			baseMaterial.SetTexture("_FadeTex", FadeTexture);
			return baseMaterial;
		}

		internal void CrossFade(Texture fadeTexture, float time, Action onComplete)
		{
			this.FadeTexture = fadeTexture;
			Target.material.EnableKeyword("CROSS_FADE");

			Timer.StartTimer(
				time,
				x => Strengh = x.Time01Inverse,
				x =>
				{
					Target.material.DisableKeyword("CROSS_FADE");
					onComplete();
				});
		}
	}
}