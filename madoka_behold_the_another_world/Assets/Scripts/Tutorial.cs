using UnityEngine;
using System.Collections;

public partial class titlecamera_control : MonoBehaviour
{
    // 各チュートリアル背景
    public Texture2D[] Tutorial;
    // チュートリアル左矢印
    public Texture2D TutorialArrowLeft;
    // チュートリアル右矢印
    public Texture2D TutorialArrowRight;

    // チュートリアル表示ページ
    private int tutorialpage;

    // チュートリアル表示位置
    private Vector2 mTutorialPos;

    // チュートリアル初期位置
    private const float TUTORIALXFIRST = 102.0f;
    private const float TUTORIALYFIRST = -722.0f;
}
