using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 轻量级表格数据结构：包含表头与数据行。
/// 该结构用于在工具与导出之间传输解析后的表格数据，避免引入沉重的 DataTable 依赖。
/// </summary>
public class TableData
{
    /// <summary>
    /// 表头（第一行）。如果源表没有显式表头，可由解析器自动生成。
    /// </summary>
    public List<string> Headers = new List<string>();

    /// <summary>
    /// 数据行。每一行都是一个与列一一对应的字符串列表。
    /// </summary>
    public List<List<string>> Rows = new List<List<string>>();

    /// <summary>
    /// 列数（优先取表头列数，其次从数据行中推断最大列数）。
    /// </summary>
    public int ColumnCount => Headers != null && Headers.Count > 0
        ? Headers.Count
        : (Rows != null && Rows.Count > 0 ? Rows.Max(r => r?.Count ?? 0) : 0);
}