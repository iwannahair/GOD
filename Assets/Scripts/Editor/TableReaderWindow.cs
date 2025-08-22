#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Excel/CSV 表格读取与预览导出工具（EditorWindow）：
/// - 选择文件（.xlsx/.csv）
/// - 预览表头与数据
/// - 导出为 CSV 或 JSON
/// </summary>
public class TableReaderWindow : EditorWindow
{
    private string _filePath = string.Empty;          // 选择的文件路径
    private TableData _data;                           // 解析后的数据
    private Vector2 _scroll;                           // 预览滚动

    [MenuItem("Tools/Table Reader")] // 在 Unity 菜单创建入口
    private static void Open()
    {
        var win = GetWindow<TableReaderWindow>(false, "Table Reader", true);
        win.minSize = new Vector2(720, 420);
        win.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Excel/CSV 表格读取工具", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("选择 .xlsx 或 .csv 文件，工具将自动解析首个工作表/首行作为表头，并提供数据预览与导出。", MessageType.Info);

        // 文件选择
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("文件路径", GUILayout.Width(60));
        _filePath = EditorGUILayout.TextField(_filePath);
        if (GUILayout.Button("浏览...", GUILayout.Width(80)))
        {
            var selected = EditorUtility.OpenFilePanel("选择表格文件", Application.dataPath, "xlsx,csv");
            if (!string.IsNullOrEmpty(selected))
            {
                _filePath = selected;
                _data = null; // 重置数据
            }
        }
        if (GUILayout.Button("读取", GUILayout.Width(60)))
        {
            TryRead();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 数据信息与导出
        if (_data != null)
        {
            EditorGUILayout.LabelField($"列数: {_data.ColumnCount}    行数: {_data.Rows.Count}");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出 CSV", GUILayout.Width(100)))
            {
                ExportCsv();
            }
            if (GUILayout.Button("导出 JSON", GUILayout.Width(100)))
            {
                ExportJson();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            DrawPreviewTable();
        }
    }

    /// <summary>
    /// 执行读取并捕获异常，输出到控制台。
    /// </summary>
    private void TryRead()
    {
        if (string.IsNullOrEmpty(_filePath) || !File.Exists(_filePath))
        {
            EditorUtility.DisplayDialog("错误", "请选择有效的 .xlsx 或 .csv 文件路径", "确定");
            return;
        }

        try
        {
            _data = ExcelTableReader.Read(_filePath);
            Repaint();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"读取失败: {ex.Message}\n{ex}");
            EditorUtility.DisplayDialog("读取失败", ex.Message, "确定");
        }
    }

    /// <summary>
    /// 绘制预览表格（简单网格）。
    /// </summary>
    private void DrawPreviewTable()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        // 绘制表头
        if (_data.Headers != null && _data.Headers.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var h in _data.Headers)
            {
                GUILayout.Label(h, EditorStyles.boldLabel, GUILayout.MinWidth(100));
            }
            EditorGUILayout.EndHorizontal();
        }

        // 绘制数据行
        for (int r = 0; r < _data.Rows.Count; r++)
        {
            var row = _data.Rows[r];
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < _data.ColumnCount; c++)
            {
                string cell = c < row.Count ? (row[c] ?? string.Empty) : string.Empty;
                GUILayout.Label(cell, GUILayout.MinWidth(100));
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 将数据导出为 CSV 文件。
    /// </summary>
    private void ExportCsv()
    {
        if (_data == null) return;
        string path = EditorUtility.SaveFilePanel("导出 CSV", Application.dataPath, "table_export", "csv");
        if (string.IsNullOrEmpty(path)) return;

        var sb = new StringBuilder();
        // 写表头
        WriteCsvLine(sb, _data.Headers);
        // 写数据
        foreach (var row in _data.Rows)
            WriteCsvLine(sb, row, _data.ColumnCount);

        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
        EditorUtility.RevealInFinder(path);
        EditorUtility.DisplayDialog("导出成功", "CSV 导出完成", "确定");
    }

    /// <summary>
    /// 将数据导出为 JSON 文件（结构：{"headers":[],"rows":[[]...]}）。
    /// </summary>
    private void ExportJson()
    {
        if (_data == null) return;
        string path = EditorUtility.SaveFilePanel("导出 JSON", Application.dataPath, "table_export", "json");
        if (string.IsNullOrEmpty(path)) return;

        var sb = new StringBuilder();
        sb.Append("{\n");
        sb.Append("  \"headers\": [");
        for (int i = 0; i < _data.Headers.Count; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(EscapeJsonString(_data.Headers[i] ?? string.Empty));
        }
        sb.Append("],\n");
        sb.Append("  \"rows\": [\n");
        for (int r = 0; r < _data.Rows.Count; r++)
        {
            var row = _data.Rows[r];
            sb.Append("    [");
            for (int c = 0; c < _data.ColumnCount; c++)
            {
                if (c > 0) sb.Append(", ");
                string cell = c < row.Count ? (row[c] ?? string.Empty) : string.Empty;
                sb.Append(EscapeJsonString(cell));
            }
            sb.Append("]");
            if (r < _data.Rows.Count - 1) sb.Append(",");
            sb.Append("\n");
        }
        sb.Append("  ]\n");
        sb.Append("}\n");

        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
        EditorUtility.RevealInFinder(path);
        EditorUtility.DisplayDialog("导出成功", "JSON 导出完成", "确定");
    }

    /// <summary>
    /// 写入一行 CSV（带引号转义）。
    /// </summary>
    private void WriteCsvLine(StringBuilder sb, IList<string> values, int columnCountOverride = -1)
    {
        int count = columnCountOverride > 0 ? columnCountOverride : (values?.Count ?? 0);
        for (int i = 0; i < count; i++)
        {
            if (i > 0) sb.Append(',');
            string v = (values != null && i < values.Count) ? (values[i] ?? string.Empty) : string.Empty;
            bool needQuote = v.Contains(",") || v.Contains("\"") || v.Contains("\n") || v.Contains("\r");
            if (needQuote)
            {
                sb.Append('"');
                sb.Append(v.Replace("\"", "\"\""));
                sb.Append('"');
            }
            else
            {
                sb.Append(v);
            }
        }
        sb.Append("\n");
    }

    /// <summary>
    /// 简单 JSON 字符串转义并带引号。
    /// </summary>
    private string EscapeJsonString(string s)
    {
        if (s == null) return "\"\"";
        var escaped = s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        return "\"" + escaped + "\"";
    }
}
#endif