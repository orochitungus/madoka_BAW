using UnityEngine;
using System.Collections;

namespace UnityChan
{
	public class FaceUpdate : MonoBehaviour
	{
		public AnimationClip[] animations;
		Animator anim;
		public float delayWeight;
		public bool isKeepFace = false;

        public bool isNormal = true;
        public bool isEdge = true;
        public bool isSphere = true;
        public bool isRimLight = true;
        public bool isSpecular = true;

		void Start ()
		{

            anim = GetComponent<Animator> ();
		}

		void OnGUI ()
		{
            /*
			GUILayout.Box ("-BMToon-", GUILayout.Width (170), GUILayout.Height (25 * (animations.Length + 2)));
			Rect screenRect = new Rect (10, 25, 150, 25 * (animations.Length + 1));
			GUILayout.BeginArea (screenRect);
            isEdge = GUILayout.Toggle (isEdge, "Edge Enable");
            isNormal = GUILayout.Toggle(isNormal, "NormalMap Enable");
            isSpecular = GUILayout.Toggle(isSpecular, "Specular Enable");
            isSphere = GUILayout.Toggle(isSphere, "Sphere Enable");
            isRimLight = GUILayout.Toggle(isRimLight, "RimLight Enable");
            GUILayout.EndArea ();
            */
		}

		float current = 0;

		void Update ()
		{

			if (Input.GetMouseButton (0)) {
				current = 1;

                // 全ての子を取得
                Transform[] transformList;
                transformList = this.transform.GetComponentsInChildren<Transform>();

                // リストから子を取り出す
                foreach (Transform transform in transformList)
                {
                    Renderer rnd = transform.GetComponent<Renderer>();
                    if (rnd != null)
                    {
                        if(isEdge) rnd.material.SetFloat("_UseEdge", 1.0f);
                        else rnd.material.SetFloat("_UseEdge", 0.0f);
                    }
                }





			} else if (!isKeepFace) {
				current = Mathf.Lerp (current, 0, delayWeight);
			}
			anim.SetLayerWeight (1, current);
		}
	 

		//アニメーションEvents側につける表情切り替え用イベントコール
		public void OnCallChangeFace (string str)
		{   
			int ichecked = 0;
			foreach (var animation in animations) {
				if (str == animation.name) {
					ChangeFace (str);
					break;
				} else if (ichecked <= animations.Length) {
					ichecked++;
				} else {
					//str指定が間違っている時にはデフォルトで
					str = "default@unitychan";
					ChangeFace (str);
				}
			} 
		}

		void ChangeFace (string str)
		{
			isKeepFace = true;
			current = 1;
			anim.CrossFade (str, 0);
		}
	}
}
