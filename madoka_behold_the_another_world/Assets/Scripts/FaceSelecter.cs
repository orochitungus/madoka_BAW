using UnityEngine;
using System.Collections;

public class FaceSelecter : MonoBehaviour 
{
    // 通常顔のテクスチャ
    public Texture original;

    // 通常顔のテクスチャの名前
    public string originalfacetexture;

    // 差し替え対象のテクスチャのインデックス
    int m_changetarget_index;

    // 差し替え対象テクスチャ
    [SerializeField] Texture[] m_ReplacementTextures;

	// Use this for initialization
	void Start () 
	{
        // 変更対象のテクスチャのインデックスを取得する
        for (int i = 0; i < renderer.materials.GetLength(0); i++)
        {
            Texture tex = renderer.materials[i].GetTexture("_MainTex");
            if (tex)
            {
                // ということで、差し替えたいテクスチャーの名前と等しいテクスチャーを、normalfaceとする(この場合はmadoka_normalface)
                if (tex.name == originalfacetexture)
                {
                    original = renderer.materials[i].mainTexture; // i番目のテクスチャーを差し替え対象と定義
                    m_changetarget_index = i;     // そのときのインデックスを保持しておく（差し替えの時何番目のテクスチャーを差し替えるかの判定に使う）
                }
            }
        }
	}

    // Update is called once per frame
    void Update() 
	{
	
	}

    // テクスチャーの切り替えを実行する
    // 第1引数：切り替え先のテクスチャの名前
    public void ChangeFaceTexture(string texturename)
    {        
        // 取得した情報を基にテクスチャを笑顔のものに差し替える
        renderer.materials[m_changetarget_index].mainTexture = (Texture)Instantiate(Resources.Load(texturename));
    }
    // 上記のResourceを使わずに、予め登録しておいたテクスチャーを使うバージョン
    // index[in]    :切り替え対象のテクスチャーのインデックス
    public void ChangeFaceTexture(int index)
    {
        renderer.materials[m_changetarget_index].mainTexture = m_ReplacementTextures[index];
    }
}
