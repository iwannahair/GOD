using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;

/// <summary>
/// Excel/CSV 读取器：
/// - 支持 .xlsx（基于解压与XML解析，避免第三方依赖）
/// - 支持 .csv（支持引号转义与逗号分隔）
/// 注意：此实现以“读取首个工作表”为目标，适合游戏项目中的常见数据表处理场景。
/// </summary>
public static class ExcelTableReader
{
    /// <summary>
    /// 读取指定路径的表格文件，根据扩展名自动选择解析方式。
    /// 支持 .xlsx / .csv。
    /// </summary>
    public static TableData Read(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("文件不存在", filePath);

        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        switch (ext)
        {
            case ".xlsx":
                return ReadXlsx(filePath);
            case ".csv":
                return ReadCsv(filePath, Encoding.UTF8); // 可根据需要调整编码
            default:
                throw new NotSupportedException($"不支持的文件格式: {ext}，请使用 .xlsx 或 .csv");
        }
    }

    #region CSV
    /// <summary>
    /// 读取 CSV 文件，支持基本的引号与逗号转义处理。
    /// </summary>
    private static TableData ReadCsv(string filePath, Encoding encoding)
    {
        var data = new TableData();
        using (var reader = new StreamReader(filePath, encoding, true))
        {
            string line;
            var rowIndex = 0;
            while ((line = reader.ReadLine()) != null)
            {
                var cells = ParseCsvLine(line);
                if (rowIndex == 0)
                {
                    // 第一行认为是表头
                    data.Headers = NormalizeHeaders(cells);
                }
                else
                {
                    data.Rows.Add(cells);
                }
                rowIndex++;
            }
        }
        return data;
    }

    /// <summary>
    /// 解析单行 CSV，支持双引号内包含逗号与双引号转义（"" -> ").
    /// </summary>
    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        if (line == null) return result;

        var sb = new StringBuilder();
        bool inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    // 处理转义双引号
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++; // 跳过下一个引号
                    }
                    else
                    {
                        inQuotes = false; // 引号结束
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            else
            {
                if (c == ',')
                {
                    result.Add(sb.ToString());
                    sb.Length = 0;
                }
                else if (c == '"')
                {
                    inQuotes = true; // 引号开始
                }
                else
                {
                    sb.Append(c);
                }
            }
        }
        result.Add(sb.ToString());
        return result;
    }
    #endregion

    #region XLSX
    /// <summary>
    /// 读取 XLSX 文件：解析 sharedStrings 与首个工作表的 sheetData。
    /// </summary>
    private static TableData ReadXlsx(string filePath)
    {
        using (var fs = File.OpenRead(filePath))
        using (var zip = new ZipArchive(fs, ZipArchiveMode.Read))
        {
            var sharedStrings = LoadSharedStrings(zip);
            var sheetPath = ResolveFirstWorksheetPath(zip);
            if (string.IsNullOrEmpty(sheetPath))
                throw new Exception("未能解析到工作表路径（workbook.xml 未包含 sheets 信息）");

            return ParseWorksheet(zip, sheetPath, sharedStrings);
        }
    }

    /// <summary>
    /// 读取共享字符串表（xl/sharedStrings.xml）。
    /// </summary>
    private static List<string> LoadSharedStrings(ZipArchive zip)
    {
        var entry = zip.GetEntry("xl/sharedStrings.xml");
        if (entry == null) return null; // 可能没有共享字符串

        using (var stream = entry.Open())
        {
            var xdoc = XDocument.Load(stream);
            XNamespace ns = xdoc.Root?.Name.Namespace ?? XNamespace.None;
            var list = new List<string>();
            foreach (var si in xdoc.Descendants(ns + "si"))
            {
                // 组合 si 下可能分段的 t 文本
                var text = string.Concat(si.Descendants(ns + "t").Select(t => (string)t));
                list.Add(text);
            }
            return list;
        }
    }

    /// <summary>
    /// 从 workbook 与 rels 解析首个工作表的路径。
    /// </summary>
    private static string ResolveFirstWorksheetPath(ZipArchive zip)
    {
        // 解析 workbook.xml 获取第一个 sheet 的 r:id
        var workbookEntry = zip.GetEntry("xl/workbook.xml");
        if (workbookEntry == null) return null;

        string firstRid = null;
        using (var stream = workbookEntry.Open())
        {
            var xdoc = XDocument.Load(stream);
            XNamespace ns = xdoc.Root?.Name.Namespace ?? XNamespace.None;
            var sheet = xdoc.Descendants(ns + "sheet").FirstOrDefault();
            if (sheet == null) return null;
            var ridAttr = sheet.Attribute(XName.Get("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships"));
            firstRid = ridAttr?.Value;
        }

        if (string.IsNullOrEmpty(firstRid)) return null;

        // 解析 workbook.xml.rels 将 r:id 映射到实际目标路径（例如 worksheets/sheet1.xml）
        var relsEntry = zip.GetEntry("xl/_rels/workbook.xml.rels");
        if (relsEntry == null) return null;

        using (var stream = relsEntry.Open())
        {
            var xdoc = XDocument.Load(stream);
            XNamespace ns = xdoc.Root?.Name.Namespace ?? XNamespace.None;
            foreach (var rel in xdoc.Descendants(ns + "Relationship"))
            {
                var id = (string)rel.Attribute("Id");
                var target = (string)rel.Attribute("Target");
                var type = (string)rel.Attribute("Type");
                if (id == firstRid && !string.IsNullOrEmpty(target) && type != null && type.EndsWith("/worksheet"))
                {
                    // 目标相对于 xl/ 目录
                    var normalized = target.Replace("\\", "/");
                    if (!normalized.StartsWith("xl/"))
                        normalized = "xl/" + normalized;
                    return normalized;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 解析工作表 XML，构建 TableData（首行作为表头）。
    /// </summary>
    private static TableData ParseWorksheet(ZipArchive zip, string sheetPath, List<string> sharedStrings)
    {
        var entry = zip.GetEntry(sheetPath);
        if (entry == null)
            throw new FileNotFoundException($"未找到工作表XML: {sheetPath}");

        using (var stream = entry.Open())
        {
            var xdoc = XDocument.Load(stream);
            XNamespace ns = xdoc.Root?.Name.Namespace ?? XNamespace.None;
            var rows = xdoc.Descendants(ns + "sheetData").Descendants(ns + "row").ToList();
            var data = new TableData();

            int maxCol = 0;
            var parsedRows = new List<Dictionary<int, string>>();

            foreach (var row in rows)
            {
                var dict = new Dictionary<int, string>();
                foreach (var c in row.Elements(ns + "c"))
                {
                    // 单元格引用例如：A1、B2 -> 解析列号
                    var rRef = (string)c.Attribute("r");
                    int colIndex = 0;
                    if (!string.IsNullOrEmpty(rRef))
                    {
                        var colLetters = new string(rRef.TakeWhile(ch => char.IsLetter(ch)).ToArray());
                        colIndex = ColumnLettersToIndex(colLetters);
                    }

                    var t = (string)c.Attribute("t"); // 数据类型：s=共享字符串
                    var v = c.Element(ns + "v");
                    string value = string.Empty;
                    if (v != null)
                    {
                        var raw = (string)v;
                        if (t == "s")
                        {
                            // 共享字符串索引 -> 字符串值
                            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int sIdx) && sharedStrings != null && sIdx >= 0 && sIdx < sharedStrings.Count)
                                value = sharedStrings[sIdx];
                            else
                                value = raw; // 兜底
                        }
                        else
                        {
                            // 其他类型：按原值作为字符串返回
                            value = raw;
                        }
                    }
                    dict[colIndex] = value ?? string.Empty;
                    if (colIndex + 1 > maxCol) maxCol = colIndex + 1;
                }
                parsedRows.Add(dict);
            }

            // 将第一行作为表头，其余作为数据
            if (parsedRows.Count > 0)
            {
                var headerRow = new List<string>(new string[maxCol]);
                foreach (var kv in parsedRows[0])
                    headerRow[kv.Key] = kv.Value;
                data.Headers = NormalizeHeaders(headerRow);

                for (int i = 1; i < parsedRows.Count; i++)
                {
                    var line = new List<string>(new string[maxCol]);
                    foreach (var kv in parsedRows[i])
                        line[kv.Key] = kv.Value;
                    data.Rows.Add(line);
                }
            }
            return data;
        }
    }

    /// <summary>
    /// 列字母转下标（A->0, B->1, ..., Z->25, AA->26 ...）。
    /// </summary>
    private static int ColumnLettersToIndex(string letters)
    {
        int result = 0;
        foreach (char c in letters)
        {
            result *= 26;
            result += (char.ToUpperInvariant(c) - 'A' + 1);
        }
        return result - 1; // 从0开始
    }
    #endregion

    /// <summary>
    /// 规范化表头：为空或重复时自动填充，如 Column1、Column2 ...
    /// </summary>
    private static List<string> NormalizeHeaders(IList<string> headers)
    {
        var list = new List<string>();
        var seen = new HashSet<string>();
        for (int i = 0; i < headers.Count; i++)
        {
            var h = headers[i];
            if (string.IsNullOrWhiteSpace(h)) h = $"Column{i + 1}";
            var baseName = h.Trim();
            var name = baseName;
            int suffix = 1;
            while (seen.Contains(name))
            {
                suffix++;
                name = $"{baseName}_{suffix}";
            }
            seen.Add(name);
            list.Add(name);
        }
        return list;
    }
}