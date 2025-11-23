# TinyTextEditor 学習ガイド

このドキュメントは、TinyTextEditorのコードを読んでWindowsアプリケーション開発を学ぶためのガイドです。

## 目次

1. [プロジェクト構造](#プロジェクト構造)
2. [推奨する学習順序](#推奨する学習順序)
3. [各ファイルの詳細解説](#各ファイルの詳細解説)
4. [重要な概念](#重要な概念)
5. [発展課題](#発展課題)

---

## プロジェクト構造

```
TinyTextEditor/
├── TinyTextEditor.csproj   # プロジェクト設定ファイル
├── LEARNING_GUIDE.md       # このファイル
└── src/
    ├── Program.cs          # エントリーポイント（最初に実行される）
    ├── MainForm.cs         # メインウィンドウ（UI全体を管理）
    ├── FileManager.cs      # ファイル操作を担当
    └── EditManager.cs      # 編集操作を担当
```

### なぜこの構造なのか？

**単一責任の原則** に従い、各クラスが1つの責務だけを持つように設計しています：

| クラス | 責務 |
|--------|------|
| `Program` | アプリケーションの起動のみ |
| `MainForm` | ウィンドウの表示とUI管理 |
| `FileManager` | ファイルの読み書き |
| `EditManager` | テキストの編集操作 |

---

## 推奨する学習順序

### ステップ 1: Program.cs を読む

**ファイル**: `src/Program.cs`

最初に読むべきファイルです。ここからアプリケーションが始まります。

**学ぶこと**:
- C#プログラムがどこから始まるか（`Main()` メソッド）
- Windows Formsアプリケーションの起動方法
- `Application.Run()` の役割

**コードのポイント**:
```csharp
[STAThread]  // Windows Formsに必要な属性
static void Main()
{
    ApplicationConfiguration.Initialize();
    Application.Run(new MainForm());  // ここでウィンドウが表示される
}
```

### ステップ 2: MainForm.cs を読む

**ファイル**: `src/MainForm.cs`

テキストエディタの中心となるファイルです。最も長いですが、重要です。

**学ぶこと**:
- `Form` クラスを継承してウィンドウを作る方法
- コントロール（TextBox, MenuStrip）の配置
- イベントハンドラ（ユーザー操作への反応）
- メニューバーの作成方法

**コードのポイント**:

1. **フォームの基本設定**:
```csharp
this.Text = "TinyTextEditor";           // タイトル
this.Size = new Size(800, 600);         // サイズ
this.StartPosition = FormStartPosition.CenterScreen;  // 位置
```

2. **コントロールの追加**:
```csharp
_textBox = new TextBox { Multiline = true, Dock = DockStyle.Fill };
this.Controls.Add(_textBox);  // フォームにテキストボックスを追加
```

3. **イベントハンドラの登録**:
```csharp
_textBox.TextChanged += TextBox_TextChanged;  // テキスト変更時に呼ばれる
```

4. **メニューの作成**:
```csharp
var openMenuItem = new ToolStripMenuItem("開く(&O)...", null, MenuItem_Open);
openMenuItem.ShortcutKeys = Keys.Control | Keys.O;  // Ctrl+O
```

### ステップ 3: FileManager.cs を読む

**ファイル**: `src/FileManager.cs`

ファイルの読み書きを学べます。

**学ぶこと**:
- ファイルの読み込み（`File.ReadAllText()`）
- ファイルの書き込み（`File.WriteAllText()`）
- 文字エンコーディング（UTF-8）
- エラー処理（try-catch）
- ファイルダイアログの使い方

**コードのポイント**:

1. **ファイル読み込み**:
```csharp
string content = File.ReadAllText(filePath, Encoding.UTF8);
```

2. **ファイル保存（BOM付きUTF-8）**:
```csharp
File.WriteAllText(filePath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
```

3. **エラー処理**:
```csharp
try {
    // ファイル操作
} catch (FileNotFoundException) {
    // ファイルが見つからない
} catch (UnauthorizedAccessException) {
    // アクセス権限がない
}
```

### ステップ 4: EditManager.cs を読む

**ファイル**: `src/EditManager.cs`

最もシンプルなファイルです。クリップボード操作を学べます。

**学ぶこと**:
- TextBoxの標準機能（Undo, Cut, Copy, Paste）
- クリップボードの操作
- クラスの責務分離

**コードのポイント**:
```csharp
public void Cut()
{
    if (_textBox.SelectionLength > 0)
    {
        _textBox.Cut();  // TextBoxの標準メソッド
    }
}
```

---

## 重要な概念

### 1. イベント駆動プログラミング

Windows Formsはイベント駆動型です。ユーザーの操作（クリック、入力など）が「イベント」として発生し、それに対応する「イベントハンドラ」が呼ばれます。

```
ユーザー操作 → イベント発生 → イベントハンドラ実行
```

**例**: メニューの「開く」をクリック
```csharp
// イベントハンドラの登録
var openMenuItem = new ToolStripMenuItem("開く", null, MenuItem_Open);
                                                       ↑ イベントハンドラ

// イベントハンドラの実装
private void MenuItem_Open(object? sender, EventArgs e)
{
    // 「開く」が選択されたときの処理
}
```

### 2. コントロールの階層構造

Windows Formsでは、コントロール（UI部品）を階層的に配置します：

```
Form（ウィンドウ）
├── MenuStrip（メニューバー）
│   ├── ファイルメニュー
│   │   ├── 新規作成
│   │   ├── 開く
│   │   └── ...
│   └── 編集メニュー
│       └── ...
└── TextBox（テキスト入力エリア）
```

### 3. Dock プロパティ

`Dock` プロパティでコントロールの配置方法を指定します：

| 値 | 説明 |
|----|------|
| `DockStyle.Fill` | 親コントロール全体に広がる |
| `DockStyle.Top` | 上端に固定 |
| `DockStyle.Bottom` | 下端に固定 |
| `DockStyle.Left` | 左端に固定 |
| `DockStyle.Right` | 右端に固定 |

### 4. 文字エンコーディング

日本語を正しく扱うためには文字エンコーディングが重要です：

- **UTF-8**: 現代の標準。日本語を含む多言語に対応
- **BOM (Byte Order Mark)**: ファイルの先頭に付けるマーク。エンコーディングを示す
- **Shift-JIS**: 古いWindows日本語環境で使われた

```csharp
// UTF-8で読み込み
File.ReadAllText(path, Encoding.UTF8);

// BOM付きUTF-8で保存
File.WriteAllText(path, content, new UTF8Encoding(true));
```

---

## 発展課題

このエディタを拡張して、より多くのことを学びましょう：

### 初級

1. **行番号の表示**
   - RichTextBoxを使うか、別のコントロールを横に配置

2. **ステータスバーの追加**
   - カーソル位置（行・列）を表示
   - StatusStripコントロールを使用

3. **フォント変更機能**
   - FontDialogを使ってフォントを選択

### 中級

4. **検索機能**
   - Ctrl+F で検索ダイアログを表示
   - `String.IndexOf()` でテキストを検索

5. **置換機能**
   - 検索して置換
   - `String.Replace()` を使用

6. **最近使ったファイル**
   - 開いたファイルの履歴を保存
   - 設定ファイルへの保存を学ぶ

### 上級

7. **複数段階のUndo/Redo**
   - 操作履歴をスタックで管理
   - コマンドパターンを学ぶ

8. **シンタックスハイライト**
   - RichTextBoxで文字色を変更
   - 正規表現でキーワードを検出

9. **タブによる複数ファイル編集**
   - TabControlを使用
   - 各タブに別々のテキストを表示

---

## ビルドと実行

### 必要な環境
- .NET 8.0 SDK
- Windows OS

### コマンド

```bash
# ビルド
dotnet build

# 実行
dotnet run

# リリースビルド
dotnet publish -c Release
```

---

## 参考資料

- [Microsoft Docs: Windows Forms](https://docs.microsoft.com/ja-jp/dotnet/desktop/winforms/)
- [Microsoft Docs: File and Stream I/O](https://docs.microsoft.com/ja-jp/dotnet/standard/io/)
- [Microsoft Docs: TextBox Class](https://docs.microsoft.com/ja-jp/dotnet/api/system.windows.forms.textbox)

---

## まとめ

このテキストエディタは以下の要素で構成されています：

1. **Program.cs**: アプリの入口
2. **MainForm.cs**: UIとイベント処理
3. **FileManager.cs**: ファイル操作
4. **EditManager.cs**: 編集操作

それぞれのファイルを順番に読み、コメントを参考にしながら理解を深めてください。
分からないことがあれば、コードにあるコメントや、このガイドを参照してください。

Happy Coding! 🎉
