# VRTest
## このプロジェクトについて
このプロジェクトはVRをUnityで利用してゲームを作るために作成しているテスト用プロジェクトです
私のデバイス環境に大きく依存した作りになっています。
また、以下のリポジトリのゲームを作るための学習目的です。
https://github.com/TaguchiRei/MeshCut
環境は以下の通りです
 - unity6.3
 - MetaQuest3s

# セットアップ

## Unityセットアップ

1. **XR Plugin Management をインストール**
   - Unityプロジェクトを開き、**Project Settings** を開く
   - **XR Plugin Management** をインストールする

2. **使用する機器を選択**
   - セットアップ開始後、使用するデバイス（今回はMetaQuestなのでOpenXR）を選択する
<img width="464" height="514" alt="スクリーンショット 2026-02-22 181919" src="https://github.com/user-attachments/assets/429d2c64-2540-44c4-8b68-31deea1d68bb" />

3. **エラーの修正**
   - エラーが表示された場合は、表示される **Fix** をすべて実行する
<img width="539" height="511" alt="スクリーンショット 2026-02-22 182149" src="https://github.com/user-attachments/assets/75e02f94-386a-4ef4-bbab-25cb4864149a" />

4. **OpenXR の設定（今回の設定）**
   - **Project Settings → XR Plug-in Management** を開く
   - **OpenXR** を選択する
   - **Enabled Interaction Profiles** から、使用する機器のコントローラープロファイルを追加する(今回の場合はMeta Quest Touch Plus Contoller Profile)
<img width="929" height="661" alt="スクリーンショット 2026-02-22 183027" src="https://github.com/user-attachments/assets/eda4a277-34f9-40dd-8dc8-b064e57668e4" />

5.**XR Interaction Toolkitをインストール **
   -　**Package Manager → XR Interaction Toolkit**を探してインストール
