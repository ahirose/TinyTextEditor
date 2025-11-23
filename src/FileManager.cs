// ============================================================================
// FileManager.cs - ファイル操作を担当するクラス
// ============================================================================
//
// 【このファイルの役割】
// - ファイルを開く（読み込む）
// - ファイルを保存する（書き込む）
// - ファイルダイアログ（開く/保存）の表示
//
// 【学習ポイント】
// 1. ファイルの読み書きには System.IO 名前空間を使う
// 2. 日本語ファイルには文字エンコーディング（UTF-8）が重要
// 3. ファイル操作はエラーが起きやすいので try-catch で囲む
// 4. OpenFileDialog / SaveFileDialog でユーザーにファイルを選ばせる
// ============================================================================

using System.Text;

namespace TinyTextEditor;

/// <summary>
/// ファイルの読み書きを担当するクラス。
/// ファイル操作をMainFormから分離することで、コードの見通しが良くなる。
/// </summary>
public class FileManager
{
    // ========================================================================
    // 定数（変更されない値）
    // ========================================================================

    /// <summary>
    /// ファイルダイアログで使用するフィルター文字列。
    /// 形式: "表示名|拡張子パターン|表示名2|拡張子パターン2|..."
    /// </summary>
    private const string FileFilter =
        "テキストファイル (*.txt)|*.txt|" +
        "すべてのファイル (*.*)|*.*";

    // ========================================================================
    // 公開メソッド（外部から呼び出せるメソッド）
    // ========================================================================

    /// <summary>
    /// 「ファイルを開く」ダイアログを表示する。
    /// </summary>
    /// <returns>ダイアログの結果（成功/失敗とファイルパス）</returns>
    public DialogResultInfo ShowOpenDialog()
    {
        // OpenFileDialog: ファイルを選択するためのダイアログ
        using var dialog = new OpenFileDialog
        {
            Title = "ファイルを開く",           // ダイアログのタイトル
            Filter = FileFilter,                // ファイルの種類フィルター
            FilterIndex = 1,                    // デフォルトで1番目（テキストファイル）を選択
            RestoreDirectory = true,            // 前回のディレクトリを記憶
        };

        // ダイアログを表示し、ユーザーがOKを押したら成功
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            return new DialogResultInfo(true, dialog.FileName);
        }

        return new DialogResultInfo(false, null);
    }

    /// <summary>
    /// 「名前を付けて保存」ダイアログを表示する。
    /// </summary>
    /// <returns>ダイアログの結果（成功/失敗とファイルパス）</returns>
    public DialogResultInfo ShowSaveDialog()
    {
        // SaveFileDialog: 保存先を選択するためのダイアログ
        using var dialog = new SaveFileDialog
        {
            Title = "名前を付けて保存",
            Filter = FileFilter,
            FilterIndex = 1,
            RestoreDirectory = true,
            DefaultExt = "txt",                 // デフォルトの拡張子
            AddExtension = true,                // 拡張子がなければ自動で追加
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            return new DialogResultInfo(true, dialog.FileName);
        }

        return new DialogResultInfo(false, null);
    }

    /// <summary>
    /// ファイルを読み込む。
    /// </summary>
    /// <param name="filePath">読み込むファイルのパス</param>
    /// <returns>読み込み結果（成功/失敗、内容、エラーメッセージ）</returns>
    public FileOperationResult LoadFile(string filePath)
    {
        try
        {
            // ファイルを読み込む
            // UTF-8エンコーディングを使用（日本語を正しく読み込むため）
            // detectEncodingFromByteOrderMarks: true で BOM を自動検出
            string content = File.ReadAllText(filePath, Encoding.UTF8);

            return new FileOperationResult(true, content, null);
        }
        catch (FileNotFoundException)
        {
            // ファイルが見つからない場合
            return new FileOperationResult(false, null, "ファイルが見つかりません。");
        }
        catch (UnauthorizedAccessException)
        {
            // アクセス権限がない場合
            return new FileOperationResult(false, null, "ファイルへのアクセスが拒否されました。");
        }
        catch (IOException ex)
        {
            // その他のIO関連エラー
            return new FileOperationResult(false, null, $"ファイルの読み込みに失敗しました: {ex.Message}");
        }
        catch (Exception ex)
        {
            // 予期しないエラー
            return new FileOperationResult(false, null, $"予期しないエラーが発生しました: {ex.Message}");
        }
    }

    /// <summary>
    /// ファイルに保存する。
    /// </summary>
    /// <param name="filePath">保存先のファイルパス</param>
    /// <param name="content">保存するテキスト内容</param>
    /// <returns>保存結果（成功/失敗、エラーメッセージ）</returns>
    public FileOperationResult SaveFile(string filePath, string content)
    {
        try
        {
            // ファイルに書き込む
            // UTF-8エンコーディング（BOM付き）を使用
            // BOM (Byte Order Mark) があると、他のエディタでも文字化けしにくい
            File.WriteAllText(filePath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            return new FileOperationResult(true, null, null);
        }
        catch (UnauthorizedAccessException)
        {
            return new FileOperationResult(false, null, "ファイルへの書き込みが拒否されました。");
        }
        catch (DirectoryNotFoundException)
        {
            return new FileOperationResult(false, null, "保存先のディレクトリが見つかりません。");
        }
        catch (IOException ex)
        {
            return new FileOperationResult(false, null, $"ファイルの保存に失敗しました: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new FileOperationResult(false, null, $"予期しないエラーが発生しました: {ex.Message}");
        }
    }
}

// ============================================================================
// 結果を返すための構造体
// ============================================================================
//
// 【学習ポイント】
// - メソッドの結果を複数の値で返したい場合、専用の型を作ると分かりやすい
// - record 構文を使うと、シンプルに不変（immutable）な型を定義できる
// ============================================================================

/// <summary>
/// ダイアログの結果を表す。
/// </summary>
/// <param name="Success">ユーザーがOKを押したかどうか</param>
/// <param name="FilePath">選択されたファイルパス（キャンセル時はnull）</param>
public record DialogResultInfo(bool Success, string? FilePath);

/// <summary>
/// ファイル操作の結果を表す。
/// </summary>
/// <param name="Success">操作が成功したかどうか</param>
/// <param name="Content">読み込んだ内容（保存時や失敗時はnull）</param>
/// <param name="ErrorMessage">エラーメッセージ（成功時はnull）</param>
public record FileOperationResult(bool Success, string? Content, string? ErrorMessage);
