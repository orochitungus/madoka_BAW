using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TalkSystem : MonoBehaviour 
{
	/// <summary>
	/// 吹き出し背景
	/// </summary>
	public Image Fukidashi;
	/// <summary>
	/// キャラ名日本語
	/// </summary>
	public Image CharacterNameJP;
	/// <summary>
	/// キャラ名英語
	/// </summary>
	public Image CharacterNameEn;
	/// <summary>
	/// キャラ顔（0のみNoface)
	/// </summary>
	public Image[] CharacterFace;

	/// <summary>
	/// 吹き出しの中身
	/// </summary>
	public Text FukidashiText;
	/// <summary>
	/// キャラ名日本語の中身
	/// </summary>
	public Text CharacterNameJPText;
	/// <summary>
	/// キャラ名英語の中身
	/// </summary>
	public Text CharacterNameENText;
	/// <summary>
	/// Noface時に出現するテキスト
	/// </summary>
	public Text NofaceText;
	/// <summary>
	/// 選択肢の横に現れるカーソル
	/// </summary>
	public Text[] Cursor;
	/// <summary>
	/// 選択肢の内容
	/// </summary>
	public Text[] Choices;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
