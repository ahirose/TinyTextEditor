# TinyTextEditor

Windows用の最も基本的なテキストエディタです。学習目的で設計されており、コードの理解しやすさを最優先にしています。

## 特徴

- **シンプルな構造**: 4つのファイルだけで構成
- **日本語対応**: UTF-8エンコーディング、日本語フォント
- **豊富なコメント**: すべてのコードに日本語で詳細な説明
- **単一責任の原則**: 各クラスが1つの責務だけを担当

## 機能一覧

| カテゴリ | 機能 | ショートカット |
|----------|------|----------------|
| ファイル | 新規作成 | Ctrl+N |
| ファイル | 開く | Ctrl+O |
| ファイル | 保存 | Ctrl+S |
| ファイル | 名前を付けて保存 | Ctrl+Shift+S |
| 編集 | 元に戻す | Ctrl+Z |
| 編集 | やり直し | Ctrl+Y |
| 編集 | 切り取り | Ctrl+X |
| 編集 | コピー | Ctrl+C |
| 編集 | 貼り付け | Ctrl+V |
| 編集 | すべて選択 | Ctrl+A |

## 必要な環境

- Windows 10/11
- .NET 8.0 SDK

## ビルドと実行

```bash
# ビルド
dotnet build

# 実行
dotnet run

# リリースビルド（配布用）
dotnet publish -c Release -o ./publish
```

## プロジェクト構造

```
TinyTextEditor/
├── TinyTextEditor.csproj   # プロジェクト設定ファイル
├── README.md               # このファイル
├── LEARNING_GUIDE.md       # 詳細な学習ガイド
└── src/
    ├── Program.cs          # アプリケーションの入口
    ├── MainForm.cs         # メインウィンドウ（UI全体）
    ├── FileManager.cs      # ファイルの読み書き
    └── EditManager.cs      # 編集操作
```

---

## ソースコードの読み方ガイド

### 推奨する読む順序

学習効果を最大化するため、以下の順序でコードを読むことをお勧めします：

### Step 1: `src/Program.cs`（10行）

**最初に読むべきファイル**。アプリケーションの開始地点です。

```csharp
static void Main()
{
    ApplicationConfiguration.Initialize();
    Application.Run(new MainForm());  // ここからMainFormが起動
}
```

**学べること**:
- C#プログラムの開始地点（`Main`メソッド）
- Windows Formsアプリの起動方法

### Step 2: `src/EditManager.cs`（50行）

**最もシンプルなクラス**。1つの責務（編集操作）だけを持ちます。

```csharp
public void Cut()
{
    if (_textBox.SelectionLength > 0)
    {
        _textBox.Cut();
    }
}
```

**学べること**:
- クラスの基本構造
- TextBoxコントロールの操作
- クリップボードの使い方

### Step 3: `src/FileManager.cs`（100行）

**ファイル操作を担当**。読み書きとダイアログ処理。

```csharp
// ファイル読み込み
string content = File.ReadAllText(filePath, Encoding.UTF8);

// ファイル保存（BOM付きUTF-8）
File.WriteAllText(filePath, content, new UTF8Encoding(true));
```

**学べること**:
- ファイルの読み書き（`System.IO`）
- 文字エンコーディング（UTF-8）
- エラー処理（try-catch）
- ファイルダイアログの使い方

### Step 4: `src/MainForm.cs`（300行）

**中心となるファイル**。すべてを統合します。

```csharp
public class MainForm : Form
{
    private readonly TextBox _textBox;
    private readonly FileManager _fileManager;
    private readonly EditManager _editManager;

    public MainForm()
    {
        // ウィンドウの設定
        this.Text = "TinyTextEditor";
        this.Size = new Size(800, 600);

        // コントロールの追加
        _textBox = new TextBox { Multiline = true, Dock = DockStyle.Fill };
        this.Controls.Add(_textBox);

        // メニューの作成
        CreateMenuBar();
    }
}
```

**学べること**:
- Formクラスの継承
- コントロールの配置（TextBox, MenuStrip）
- イベントハンドラ（ユーザー操作への反応）
- メニューバーの作成

---

## 重要な概念

### 1. イベント駆動プログラミング

```
ユーザーがメニューをクリック
    ↓
イベントが発生（Click）
    ↓
イベントハンドラが呼ばれる（MenuItem_Open）
    ↓
処理を実行（ファイルを開く）
```

### 2. 単一責任の原則

各クラスは1つの責務だけを持ちます：

| クラス | 責務 |
|--------|------|
| `Program` | アプリの起動 |
| `MainForm` | UIの管理 |
| `FileManager` | ファイル操作 |
| `EditManager` | 編集操作 |

### 3. Dockプロパティ

```csharp
_textBox.Dock = DockStyle.Fill;  // ウィンドウ全体に広がる
```

| 値 | 動作 |
|----|------|
| `Fill` | 親全体に広がる |
| `Top` | 上端に固定 |
| `Bottom` | 下端に固定 |

---

## 発展課題

このエディタを拡張してみましょう：

| レベル | 課題 | ヒント |
|--------|------|--------|
| 初級 | 行番号を表示 | 左側にLabelを配置 |
| 初級 | ステータスバー追加 | StatusStripコントロール |
| 中級 | 検索機能（Ctrl+F） | `String.IndexOf()` |
| 中級 | 置換機能 | `String.Replace()` |
| 上級 | 複数Undo/Redo | スタックで履歴管理 |
| 上級 | タブで複数ファイル | TabControl |

---

## 詳細ガイド

より詳しい解説は [LEARNING_GUIDE.md](./LEARNING_GUIDE.md) を参照してください。

## ライセンス

MIT License
