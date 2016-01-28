using UnityEngine;
using System.Collections;

public class Madoka_OP : MonoBehaviour 
{
    // タイトル用のテクスチャ差し替えクラス
    // この２つの変数はinspectorで指定すること（ここに直接描いても多分OK）
    public Texture normalface;  // 通常顔
    public Texture smileface;   // 笑顔
    public string changetargettexture;  // 差し替えられる対象のテクスチャーの名前
    public string changenexttexture;    // 差し替えられた後のテクスチャの名前

    int changetarget_index;     // 差し替えられる対象のテクスチャーのインデックス

	// Use this for initialization
    void Start()
    {
        // 変更対象のテクスチャのインデックスを取得する
        for (int i = 0; i < GetComponent<Renderer>().materials.GetLength(0); i++)
        {
            Texture tex = GetComponent<Renderer>().materials[i].GetTexture("_MainTex");
            if (tex)
            {
                // ということで、差し替えたいテクスチャーの名前と等しいテクスチャーを、normalfaceとする(この場合はmadoka_normalface)
                if (tex.name == changetargettexture)
                {
                    normalface = GetComponent<Renderer>().materials[i].mainTexture; // i番目のテクスチャーを差し替え対象と定義
                    changetarget_index = i;     // そのときのインデックスを保持しておく（差し替えの時何番目のテクスチャーを差し替えるかの判定に使う）
                }
            }
        }
        // 取得した情報を基にテクスチャを笑顔のものに差し替える
        //renderer.materials[changetarget_index].mainTexture = (Texture)Instantiate(Resources.Load("madoka_smile"));
        GetComponent<Renderer>().materials[changetarget_index].mainTexture = (Texture)Instantiate(Resources.Load(changenexttexture));
    }
	void Update () 
    {
	
	}
}
