using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameSystem : MonoBehaviour 
{
    [SerializeField] private InputField nameField;
    [SerializeField] private Image playerImage;
    [SerializeField] private Slider playerImageScaleSlider;

    [SerializeField] private Sprite monsterSprite;
    [SerializeField] private Sprite mermanSprite;
    [SerializeField] private Sprite robotSprite;

    public enum PlayerType
    {
        Monster,
        Merman,
        Robot
    }

    PlayerType playerType = PlayerType.Monster;

    // アバター画像を反転させるプロパティ
    // チェックボックスからイベント設定をする
    bool isFlipPlayerImage;
    public bool IsFlipPlayerImage
    {
        set
        {
            isFlipPlayerImage = value;
            UpdatePlayerImageScale();
        }
    }

    // 画像の表示倍率。スライダーからイベント設定をする
    float playerImageScale = 1.0f;
    public void ChangePlayerImageScale(float scale)
    {
        playerImageScale = scale;
        UpdatePlayerImageScale();
    }

    void UpdatePlayerImageScale()
    {
        Vector3 scale = Vector3.one * (playerImageScale * 0.4f + 0.6f);
        if (isFlipPlayerImage) scale.x *= -1.0f;
        playerImage.transform.localScale = scale;
    }

    // バックボタンの処理
    public void Back()
    {
        Debug.Log("Back");
    }

    // シーンを読み込み直す
    public void Reset()
    {
        Application.LoadLevel(0);
    }

    // Submitボタン
    public void Submit()
    {
        Debug.Log("Player Name:" + nameField.text);
        Debug.Log("Player Name:" + playerType.ToString());
        Debug.Log("Main Image Scale: " + playerImage.transform.localScale);
    }

    // 表示アバターのタイプを設定。ラジオボタンから設定する
    public void ChangePlayerType(int index)
    {
        ChangePlayerType((PlayerType)index);
    }
    // 上記関数のオーバーロード
    public void ChangePlayerType(PlayerType type)
    {
        if (playerType == type) return;
        playerType = type;
        switch (type)
        {
            case PlayerType.Monster:
                playerImage.sprite = monsterSprite;
                break;
            case PlayerType.Merman:
                playerImage.sprite = mermanSprite;
                break;
            case PlayerType.Robot:
                playerImage.sprite = robotSprite;
                break;
            default:
                break;
        }
        playerImage.SetNativeSize();
    }
}
