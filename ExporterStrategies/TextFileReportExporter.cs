namespace ExporterStrategies;

public sealed class TextFileReportExporter : IReportExporter
{
    private readonly string _path;

    public TextFileReportExporter(string path)
    {
        // TODO 4: Проверка пути
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым или состоять только из пробелов.", nameof(path));
        }
        _path = path;
    }

    public void Export(ReportDocument document)
    {
        // TODO 5: Проверка документа
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document), "Документ не может быть null");
        }

        // TODO 6: Запись отчёта. 
        string safeTitle = document.Title ?? string.Empty;
        string safeBody = document.Body ?? string.Empty;

        File.WriteAllText(_path, $"{safeTitle}{Environment.NewLine}{safeBody}");
    }
}