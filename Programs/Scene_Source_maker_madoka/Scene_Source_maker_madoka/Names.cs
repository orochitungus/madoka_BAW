using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scene_Source_maker_madoka
{
    class Names
    {
        public Names()
        {
        }
        // 名前リスト(ここのリストを検索して引っかからなかったキャラはそのまま使う)
        // 第1次元：キャラクター
        // 第2次元1：シナリオ上の名前
        // 第2次元2：表示する名前（本名）
        // 第2次元3：表示する名前（英語）
        // 第2次元4：ノーマル顔のインデックス
        public string[][] CharacterNameList = new string[][]
        {
            new string[] {""},      // ダミー
            new string[] {"まどか","鹿目まどか","KANAME MADOKA", "CharacterFace.MADOKA_NORMAL"},     // まどか
            new string[] {"さやか", "美樹さやか","MIKI SAYAKA","CharacterFace.SAYAKA_NORMAL"},       // さやか
            new string[] {"マミ","巴マミ","TOMOE MAMI","CharacterFace.MAMI_NORMAL"},               // マミ
            new string[] {"ほむら","暁美ほむら","AKEMI HOMURA","CharacterFace.HOMURA_NORMAL"},       // ほむら
            new string[] {"杏子","佐倉杏子","SAKURA KYOKO","CharacterFace.KYOKO_NORMAL"},           // 杏子
            new string[] {"ゆま","千歳ゆま","CHITOSE YUMA","CharacterFace.YUMA_NORMAL"},           // ゆま
            new string[] {"織莉子","美国織莉子","MIKUNI ORIKO","CharacterFace.ORIKO_NORMAL"},       // 織莉子
            new string[] {"キリカ","呉キリカ","KURE KIRIKA","CharacterFace.KIRIKA_NORMAL"},          // キリカ
            new string[] {"スコノシュート","SCONOSCIUTO","SCONOSCIUTO","CharacterFace.SCONOSCIUTO_NORMAL"},// スコノシュート
            new string[] {"アルティメットまどか","アルティメットまどか","ULTIMATE MADOKA","CharacterFace.ULTIMATE_MADOKA_NORMAL"},
            new string[] {"さやかゴッドシブ","さやかゴッドシブ","SAYAKA GODSIBB","CharacterFace.SAYAKA_GODSIBB_NORMAL"},
            new string[] {"恭介","上条恭介","KAMIJOU KYOUSUKE","CharacterFace.KYOSUKE_NORMAL"},
            new string[] {"仁美","志筑仁美","SHIZUKI HITOMI","CharacterFace.HITOMI_NORMAL"},
            new string[] {"詢子","鹿目詢子","KANAME JUNKO","CharacterFace.JUNKO_NORMAL"},
            new string[] {"知久","鹿目知久","KANAME TOMOHISA","CharacterFace.TOMOHISA_NORMAL"},
            new string[] {"タツヤ","鹿目タツヤ","KANAME TATSUYA","CharacterFace.TATSUYA_NORMAL"},
            new string[] {"和子","早乙女和子","SAOTOME KAZUKO","CharacterFace.KAZUKO_NORMAL"},
            new string[] {"中沢","中沢","NAKAZAWA","CharacterFace.NAKAZAWA_NORMAL"},
            new string[] {"ミッチェル","ミッチェル・ノートルダム","MICHELLE NOSTREDAME","CharacterFace.MICHEL_NORMAL"},
            new string[] {"キュゥべえ","キュゥべえ","KYUBEY","CharacterFace.KYUBEY_NORMAL"}
        };        
    }
}
