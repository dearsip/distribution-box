# distribution-box

These packages are licensed under CC0.

[![CC0](http://i.creativecommons.org/p/zero/1.0/88x31.png "CC0")](http://creativecommons.org/publicdomain/zero/1.0/)

主にVRChatでの利用を想定しています。

## Polychoral Accessory

![Screenshot](images/polychoral.jpg)

4次元座標（と法線・閾値）を持つメッシュを3次元投影で表示し、ボーン位置を入力として回転させるシェーダーです。

`PolychoralAccessory.prefab`には、VRChatアバターの手の上に表示するためのPhysBone、[Modular Avatar](https://modular-avatar.nadena.dev/ja/)及び[Avatar Menu Creator for MA](https://avatar-menu-creator-for-ma.vrchat.narazaka.net/)設定が含まれます。
`PolyhedralAccessory.prefab`は、多面体の2次元投影の表示のためにPhysBoneの挙動を制限したものです。

アルファベットと数字の組からなるメッシュ及びマテリアルは、その表記が表す対称性に基づき一つの頂点座標から多胞体を生成するもので、ボーン位置を回転の代わりに頂点座標に割り当てています。

[こちらのワールド](https://vrchat.com/home/world/wrld_29bde305-ffb9-4b22-8369-1eccf7316fae)にサンプルアバターがあります。

## Polychora Viewer

![Screenshot](images/viewer.jpg)

VRChatワールドで4次元多胞体を描画するためのアセットです。モーションコントローラと設定ウィンドウが付属しており、ワールド内で回転操作や描画スタイル変更等ができます。状態は同期されます。

PC及びAndroid（Quest3以外未検証）に対応していますが、対応のためにはマテリアルが参照するシェーダーを変更する必要があります。[EasyQuestSwitch](https://github.com/vrchat-community/EasyQuestSwitch)の利用を推奨します。

| マテリアル | シェーダー (PC) | シェーダー (Android) |
| ---- | ---- | ---- |
| CalcDisplay | Face | FaceQuest |
| CalcEdge | Edge | EdgeQuest |
| Line | Line | EdgeQuest |

図形の情報は現状スクリプトに直接書き込まれており、プレハブの`Shape Num`で指定します。0～5が4次元正多胞体、6～10が正多面体（`PolyhedraViewer.prefab`で動作）です。

上記と同じく、アルファベットと数字の組からなるメッシュ及びマテリアルは頂点座標の変更に対応し、設定ウィンドウの反対側に表示される三角形または四面体に沿って白球を動かすことで、頂点座標を変更できます。こちらは現在色の変更及び同期には対応していません。

[上記と同じワールド](https://vrchat.com/home/world/wrld_29bde305-ffb9-4b22-8369-1eccf7316fae)に設置しています。

## 4D SDF Slice Stacker

![Screenshot](images/4S3.jpg)

4変数の陰関数（特に符号付き距離関数: Signed Distance Function, SDF）で表された3次元超曲面を、格子状に並んだ2次元平面との交差として描画するシェーダーです。

`4S3_Grabbable.prefab`には、Polychoral Accessoryと同様に、VRChatアバターの手の上に表示し掴んで回転させるためのPhysBone、[Modular Avatar](https://modular-avatar.nadena.dev/ja/)設定が含まれます。
`4S3_Static.prefab`は、デスクトップでの表示のためにPhysBone設定を除き、交差法立体視描画機能を追加したものです。

`4S3_Presets.prefab`および`4S3_Custom.prefab`は、関連するExpressions Menuを追加するためのアセットです。`4S3_Presets.prefab`では、上記ワールドの展示を再現した設定を纏めています。`4S3_Custom.prefab`では、ほぼ全ての設定項目を編集できます。使用には[Avatar Menu Creator for MA](https://avatar-menu-creator-for-ma.vrchat.narazaka.net/)が必要です。両者を同時に使用することはできません。

描画にDepthテクスチャを使用するため、リアルタイムシャドウのないワールドでは表示されません。アバターに別途影を生成するライトを追加することで常に描画できるようになります。（[参考資料](https://qiita.com/yuri_tsukimi/items/721c4f49e7228c0865db)）

[こちらのワールド](https://vrchat.com/home/world/wrld_08c252c2-fc4f-441f-93bc-c583e4054dca)に設定項目の説明及びサンプルアバターがあります。
