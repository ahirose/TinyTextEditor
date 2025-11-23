// ============================================================================
// MainForm.cs - メインウィンドウ（フォーム）
// ============================================================================
//
// 【このファイルの役割】
// - テキストエディタのメインウィンドウを定義
// - メニューバーの作成と配置
// - テキスト入力エリアの配置
// - 各コンポーネント（FileManager, EditManager）の連携
//
// 【学習ポイント】
// 1. Form クラスを継承してウィンドウを作る
// 2. コントロール（部品）をフォームに追加する方法
// 3. イベント（クリックなど）の処理方法
// 4. 日本語表示のためのエンコーディング設定
// ============================================================================

using System.Text;

namespace TinyTextEditor;

/// <summary>
/// テキストエディタのメインウィンドウ。
/// Form クラスを継承することで、ウィンドウとしての機能を持つ。
/// </summary>
public class MainForm : Form
{
    // ========================================================================
    // フィールド（このクラスが持つデータ）
    // ========================================================================

    /// <summary>
    /// テキストを入力・編集するためのテキストボックス。
    /// 複数行のテキストを扱える。
    /// </summary>
    private readonly TextBox _textBox;

    /// <summary>
    /// ファイルの読み書きを担当するマネージャー
    /// </summary>
    private readonly FileManager _fileManager;

    /// <summary>
    /// 編集操作（元に戻す、やり直しなど）を担当するマネージャー
    /// </summary>
    private readonly EditManager _editManager;

    /// <summary>
    /// 現在開いているファイルのパス（新規の場合はnull）
    /// </summary>
    private string? _currentFilePath;

    /// <summary>
    /// テキストが変更されたかどうか（保存の確認に使用）
    /// </summary>
    private bool _isTextChanged;

    // ========================================================================
    // コンストラクタ（初期化処理）
    // ========================================================================

    /// <summary>
    /// MainFormのコンストラクタ。
    /// ウィンドウの初期設定とコントロールの配置を行う。
    /// </summary>
    public MainForm()
    {
        // ----------------------------------------------------------------
        // ウィンドウの基本設定
        // ----------------------------------------------------------------
        this.Text = "TinyTextEditor - 新規ファイル";  // タイトルバーの文字
        this.Size = new Size(800, 600);               // ウィンドウサイズ（幅, 高さ）
        this.StartPosition = FormStartPosition.CenterScreen;  // 画面中央に表示

        // ----------------------------------------------------------------
        // テキストボックスの作成と設定
        // ----------------------------------------------------------------
        _textBox = new TextBox
        {
            // 複数行入力を有効にする（これがないと1行しか入力できない）
            Multiline = true,

            // ウィンドウ全体に広がるようにする
            Dock = DockStyle.Fill,

            // スクロールバーを両方向に表示
            ScrollBars = ScrollBars.Both,

            // 自動的な折り返しを無効にする（横スクロールを使う）
            WordWrap = false,

            // 日本語フォントを設定（MSゴシックは等幅フォント）
            Font = new Font("MS Gothic", 11),

            // Tabキーでスペースではなくタブ文字を入力
            AcceptsTab = true,

            // Enterキーで改行を入力
            AcceptsReturn = true,
        };

        // テキストが変更されたときのイベントハンドラを登録
        _textBox.TextChanged += TextBox_TextChanged;

        // テキストボックスをフォームに追加
        this.Controls.Add(_textBox);

        // ----------------------------------------------------------------
        // マネージャークラスの初期化
        // ----------------------------------------------------------------
        _fileManager = new FileManager();
        _editManager = new EditManager(_textBox);

        // ----------------------------------------------------------------
        // メニューバーの作成
        // ----------------------------------------------------------------
        CreateMenuBar();

        // ----------------------------------------------------------------
        // 初期状態の設定
        // ----------------------------------------------------------------
        _currentFilePath = null;
        _isTextChanged = false;
    }

    // ========================================================================
    // メニューバーの作成
    // ========================================================================

    /// <summary>
    /// メニューバーを作成してフォームに追加する。
    /// メニュー構造: ファイル(F) | 編集(E)
    /// </summary>
    private void CreateMenuBar()
    {
        // メニューバー（メニュー全体を入れる容器）を作成
        var menuStrip = new MenuStrip();

        // ================================================================
        // 「ファイル」メニューの作成
        // ================================================================
        var fileMenu = new ToolStripMenuItem("ファイル(&F)");  // &Fはアクセスキー（Alt+F）

        // 各メニュー項目を作成
        var newMenuItem = new ToolStripMenuItem("新規作成(&N)", null, MenuItem_New);
        newMenuItem.ShortcutKeys = Keys.Control | Keys.N;  // Ctrl+N

        var openMenuItem = new ToolStripMenuItem("開く(&O)...", null, MenuItem_Open);
        openMenuItem.ShortcutKeys = Keys.Control | Keys.O;  // Ctrl+O

        var saveMenuItem = new ToolStripMenuItem("保存(&S)", null, MenuItem_Save);
        saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;  // Ctrl+S

        var saveAsMenuItem = new ToolStripMenuItem("名前を付けて保存(&A)...", null, MenuItem_SaveAs);
        saveAsMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;  // Ctrl+Shift+S

        var exitMenuItem = new ToolStripMenuItem("終了(&X)", null, MenuItem_Exit);
        exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;  // Alt+F4

        // ファイルメニューに項目を追加
        fileMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            newMenuItem,
            openMenuItem,
            new ToolStripSeparator(),  // 区切り線
            saveMenuItem,
            saveAsMenuItem,
            new ToolStripSeparator(),
            exitMenuItem
        });

        // ================================================================
        // 「編集」メニューの作成
        // ================================================================
        var editMenu = new ToolStripMenuItem("編集(&E)");

        var undoMenuItem = new ToolStripMenuItem("元に戻す(&U)", null, MenuItem_Undo);
        undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;

        var redoMenuItem = new ToolStripMenuItem("やり直し(&R)", null, MenuItem_Redo);
        redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;

        var cutMenuItem = new ToolStripMenuItem("切り取り(&T)", null, MenuItem_Cut);
        cutMenuItem.ShortcutKeys = Keys.Control | Keys.X;

        var copyMenuItem = new ToolStripMenuItem("コピー(&C)", null, MenuItem_Copy);
        copyMenuItem.ShortcutKeys = Keys.Control | Keys.C;

        var pasteMenuItem = new ToolStripMenuItem("貼り付け(&P)", null, MenuItem_Paste);
        pasteMenuItem.ShortcutKeys = Keys.Control | Keys.V;

        var selectAllMenuItem = new ToolStripMenuItem("すべて選択(&A)", null, MenuItem_SelectAll);
        selectAllMenuItem.ShortcutKeys = Keys.Control | Keys.A;

        // 編集メニューに項目を追加
        editMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            undoMenuItem,
            redoMenuItem,
            new ToolStripSeparator(),
            cutMenuItem,
            copyMenuItem,
            pasteMenuItem,
            new ToolStripSeparator(),
            selectAllMenuItem
        });

        // ================================================================
        // メニューバーをフォームに追加
        // ================================================================
        menuStrip.Items.Add(fileMenu);
        menuStrip.Items.Add(editMenu);
        this.MainMenuStrip = menuStrip;
        this.Controls.Add(menuStrip);
    }

    // ========================================================================
    // イベントハンドラ（ユーザーの操作に反応するメソッド）
    // ========================================================================

    /// <summary>
    /// テキストが変更されたときに呼ばれる。
    /// </summary>
    private void TextBox_TextChanged(object? sender, EventArgs e)
    {
        // 変更フラグを立てる
        if (!_isTextChanged)
        {
            _isTextChanged = true;
            UpdateTitle();  // タイトルに「*」を付けて変更を示す
        }
    }

    // ----------------------------------------------------------------
    // ファイルメニューのイベントハンドラ
    // ----------------------------------------------------------------

    /// <summary>
    /// 「新規作成」が選択されたとき
    /// </summary>
    private void MenuItem_New(object? sender, EventArgs e)
    {
        // 保存されていない変更があれば確認
        if (!ConfirmSaveChanges()) return;

        // テキストをクリア
        _textBox.Clear();
        _currentFilePath = null;
        _isTextChanged = false;
        UpdateTitle();
    }

    /// <summary>
    /// 「開く」が選択されたとき
    /// </summary>
    private void MenuItem_Open(object? sender, EventArgs e)
    {
        // 保存されていない変更があれば確認
        if (!ConfirmSaveChanges()) return;

        // ファイルを開くダイアログを表示
        var result = _fileManager.ShowOpenDialog();

        if (result.Success && result.FilePath != null)
        {
            // ファイルを読み込む
            var loadResult = _fileManager.LoadFile(result.FilePath);

            if (loadResult.Success)
            {
                _textBox.Text = loadResult.Content;
                _currentFilePath = result.FilePath;
                _isTextChanged = false;
                UpdateTitle();
            }
            else
            {
                // エラーメッセージを表示
                MessageBox.Show(
                    $"ファイルを開けませんでした。\n\n{loadResult.ErrorMessage}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }

    /// <summary>
    /// 「保存」が選択されたとき
    /// </summary>
    private void MenuItem_Save(object? sender, EventArgs e)
    {
        // ファイルパスがなければ「名前を付けて保存」
        if (_currentFilePath == null)
        {
            MenuItem_SaveAs(sender, e);
            return;
        }

        // ファイルに保存
        var result = _fileManager.SaveFile(_currentFilePath, _textBox.Text);

        if (result.Success)
        {
            _isTextChanged = false;
            UpdateTitle();
        }
        else
        {
            MessageBox.Show(
                $"ファイルを保存できませんでした。\n\n{result.ErrorMessage}",
                "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    /// <summary>
    /// 「名前を付けて保存」が選択されたとき
    /// </summary>
    private void MenuItem_SaveAs(object? sender, EventArgs e)
    {
        // 保存ダイアログを表示
        var result = _fileManager.ShowSaveDialog();

        if (result.Success && result.FilePath != null)
        {
            // ファイルに保存
            var saveResult = _fileManager.SaveFile(result.FilePath, _textBox.Text);

            if (saveResult.Success)
            {
                _currentFilePath = result.FilePath;
                _isTextChanged = false;
                UpdateTitle();
            }
            else
            {
                MessageBox.Show(
                    $"ファイルを保存できませんでした。\n\n{saveResult.ErrorMessage}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }

    /// <summary>
    /// 「終了」が選択されたとき
    /// </summary>
    private void MenuItem_Exit(object? sender, EventArgs e)
    {
        this.Close();  // ウィンドウを閉じる（OnFormClosingが呼ばれる）
    }

    // ----------------------------------------------------------------
    // 編集メニューのイベントハンドラ
    // ----------------------------------------------------------------

    private void MenuItem_Undo(object? sender, EventArgs e) => _editManager.Undo();
    private void MenuItem_Redo(object? sender, EventArgs e) => _editManager.Redo();
    private void MenuItem_Cut(object? sender, EventArgs e) => _editManager.Cut();
    private void MenuItem_Copy(object? sender, EventArgs e) => _editManager.Copy();
    private void MenuItem_Paste(object? sender, EventArgs e) => _editManager.Paste();
    private void MenuItem_SelectAll(object? sender, EventArgs e) => _editManager.SelectAll();

    // ========================================================================
    // ヘルパーメソッド
    // ========================================================================

    /// <summary>
    /// ウィンドウのタイトルを更新する。
    /// ファイル名と変更状態（*）を表示。
    /// </summary>
    private void UpdateTitle()
    {
        // ファイル名を取得（ない場合は「新規ファイル」）
        string fileName = _currentFilePath != null
            ? Path.GetFileName(_currentFilePath)
            : "新規ファイル";

        // 変更されている場合は「*」を付ける
        string modified = _isTextChanged ? " *" : "";

        this.Text = $"TinyTextEditor - {fileName}{modified}";
    }

    /// <summary>
    /// 保存されていない変更がある場合、保存するか確認する。
    /// </summary>
    /// <returns>処理を続行してよい場合はtrue、キャンセルの場合はfalse</returns>
    private bool ConfirmSaveChanges()
    {
        if (!_isTextChanged) return true;  // 変更がなければそのまま続行

        // 確認ダイアログを表示
        var result = MessageBox.Show(
            "変更を保存しますか？",
            "確認",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question
        );

        switch (result)
        {
            case DialogResult.Yes:
                // 保存してから続行
                MenuItem_Save(null, EventArgs.Empty);
                return !_isTextChanged;  // 保存が成功したら続行

            case DialogResult.No:
                // 保存せずに続行
                return true;

            case DialogResult.Cancel:
            default:
                // キャンセル
                return false;
        }
    }

    /// <summary>
    /// フォームが閉じられようとしているときに呼ばれる。
    /// 保存の確認を行う。
    /// </summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!ConfirmSaveChanges())
        {
            e.Cancel = true;  // 閉じるのをキャンセル
        }
        base.OnFormClosing(e);
    }
}
