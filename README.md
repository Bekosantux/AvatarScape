# AvatarScape

VRChatアバターを現実の写真と合成するUnity WebGLアプリケーション

## 概要

AvatarScapeは、VRChatアバターをインポートして、背景画像と合成し、ライティングやポージングを調整して撮影を行うWebアプリです。

## 機能

- **アバターインポート**: AssetBundle形式でビルドされたアバターを読み込み
- **ライティング調整**: 3点照明システムによるライティング制御
- **カメラ操作**: マウスでのオービット、ズーム、パン操作
- **スクリーンショット**: 高品質な画像として保存

## 必要環境

- Unity 2022.3 LTS 以上
- Universal Render Pipeline (URP)

## セットアップ手順

### 1. Unityプロジェクトの作成

1. Unity Hubで新しいプロジェクトを作成
2. テンプレートは「3D (URP)」を選択
3. プロジェクト名は任意（例: AvatarScape）

### 2. スクリプトのインポート

このリポジトリの `Assets/` フォルダの内容を、Unityプロジェクトの `Assets/` フォルダにコピーしてください。

### 3. 必要なパッケージ

#### TextMeshPro
初回起動時にインポートダイアログが表示されます。「Import TMP Essentials」を選択してください。

#### lilToon (オプション)
lilToonシェーダーを使用するアバターを読み込む場合は、lilToonをインポートしてください。

- [lilToon リリースページ](https://github.com/lilxyzw/lilToon/releases)

### 4. シーンのセットアップ

1. 新規シーンを作成
2. 以下のGameObjectを配置:

```
Hierarchy:
├── Main Camera (CameraController.cs)
├── Directional Light (Main Light)
├── Fill Light (Point Light)
├── Rim Light (Point Light)
├── AvatarScapeManager (AvatarScapeManager.cs, AvatarLoader.cs)
├── WebGLBridge (WebGLFileBridge.cs)
├── LightingController (LightingController.cs)
├── Canvas
│   ├── LoadingPanel
│   │   ├── ProgressSlider
│   │   ├── ProgressText (TMP)
│   │   └── StatusText (TMP)
│   └── MainPanel
│       ├── LoadAvatarButton
│       ├── AvatarNameText (TMP)
│       └── AvatarInfoText (TMP)
└── EventSystem
```

### 5. WebGLビルド設定

1. File > Build Settings
2. Platform を WebGL に切り替え
3. Player Settings で以下を設定:
   - Compression Format: Gzip または Brotli
   - Memory Size: 512MB 以上推奨
   - Exception Support: Full (デバッグ時)

### 6. GitHub Pagesへのデプロイ

1. WebGLビルドを実行
2. `Build` フォルダの内容をリポジトリの `docs/` フォルダにコピー
3. リポジトリの Settings > Pages で Source を `docs/` に設定

## アバターのビルド方法

別リポジトリ「AvatarBuilder」を参照してください。

### クイックガイド

1. BRPのUnityプロジェクトでアバターを用意
2. AvatarBuilderエディタウィンドウを開く
3. アバターPrefabを選択してビルド
4. 出力された `.unity3d` ファイルをAvatarScapeで読み込み

## 操作方法

- **左ドラッグ**: カメラ回転
- **ホイール**: ズーム
- **中ドラッグ**: パン
- **R + クリック**: カメラリセット

## プロジェクト構造

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── AvatarLoader.cs      # AssetBundle読み込み
│   │   ├── AvatarMetadata.cs    # メタデータクラス
│   │   └── AvatarScapeManager.cs
│   ├── UI/
│   │   └── UIManager.cs
│   ├── Camera/
│   │   └── CameraController.cs
│   ├── Lighting/
│   │   └── LightingController.cs
│   └── WebGL/
│       └── WebGLFileBridge.cs
└── Plugins/
    └── WebGL/
        └── FileLoader.jslib
```

## ライセンス

MIT License

## 関連リポジトリ

- [AvatarBuilder](https://github.com/yourusername/AvatarBuilder) - アバタービルド用エディタ拡張
