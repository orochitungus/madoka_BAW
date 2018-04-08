using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

// ゲーム全般に絡む設定はここで設定する
[Serializable()]        // セーブを有効化する
public class gamesetting 
{
    // 変数

    // 画面解像度
    public int width;
    public int height;

    // セリフ配置位置
    public Vector2 []serifpos = new Vector2 [5];

    // セリフの文字数
    public int selif_length;

    // セリフの行数
    public int selif_lines;

    // セリフ背景の配置位置
    public Vector2 serifboard_pos;
    // キャラクター名背景（日本語）の配置位置
    public Vector2 characterboard_pos_jp;

    // キャラクター名（日本語）の配置位置
    public Vector2 characternamePos_jp;
    
    // キャラクター名背景（英語）の配置位置
    public Vector2 characterboard_pos_en;
    // キャラクター名（英語）の配置位置
    public Vector2 characternamePos_en;

    // キャラクター顔の配置位置
    public Vector2 characterfacePos;

    // コンストラクタ
    public gamesetting()
    {
    }

    // 設定用関数
    public void setupgamesetting()
    {
        this.width = 1024;
        this.height = 576;
        this.selif_length = 30;
        this.selif_lines = 4;

        this.serifboard_pos.x = 100;
        this.serifboard_pos.y = 400;
        this.characterboard_pos_jp.x = 100;
        this.characterboard_pos_jp.y = 370;
        this.characterboard_pos_en.x = 507;
        this.characterboard_pos_en.y = 370;
        this.characternamePos_jp.x = 105;
        this.characternamePos_jp.y = 375;
        this.characternamePos_en.x = 512;
        this.characternamePos_en.y = 380;
        this.characterfacePos.x = 640;
        this.characterfacePos.y = 70;
        this.serifpos[0].x = 145;
        this.serifpos[0].y = 410;
        this.serifpos[1].x = 145;
        this.serifpos[1].y = 432;
        this.serifpos[2].x = 145;
        this.serifpos[2].y = 454;
        this.serifpos[3].x = 145;
        this.serifpos[3].y = 478;
        this.serifpos[4].x = 145;
        this.serifpos[4].y = 502;
    }
	
}
