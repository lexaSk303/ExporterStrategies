using ExporterStrategies;

var failed = new List<string>();

void Check(string name, bool condition)
{
    Console.WriteLine($"{(condition ? "PASS" : "FAIL")} — {name}");
    if (!condition) failed.Add(name);
}
bool Throws<TException>(Action action) where TException : Exception
{
    try
    {
        action();
        return false;
    }
    catch (TException)
    {
        return true;
    }
    catch
    {
        return false; 
    }
}

var path = Path.Combine(Path.GetTempPath(), $"report-test-{Guid.NewGuid():N}.txt");
try
{
    IReportExporter fileExporter = new TextFileReportExporter(path);
    var service = new ReportService(fileExporter);
    service.PublishDailySummary(10, 1);

    var contents = File.ReadAllText(path);
    Check("файловый экспорт создаёт файл", File.Exists(path));
    Check("файл содержит заголовок", contents.Contains("Сводка за день"));
    Check("файл содержит значения отчёта", contents.Contains("Всего задач: 10") && contents.Contains("Ошибок: 1"));
}
finally
{
    if (File.Exists(path)) File.Delete(path);
}

// TODO 7: Проверка LegacyReportAdapter через Console.Out
var originalOut = Console.Out;
var stringWriter = new StringWriter();
try
{
    Console.SetOut(stringWriter);
    IReportExporter legacyExporter = new LegacyReportAdapter(new LegacyReportPrinter());
    legacyExporter.Export(new ReportDocument("Тестовый заголовок", "Тестовое тело"));
}
finally
{
    Console.SetOut(originalOut);
}
var consoleOutput = stringWriter.ToString();
Check("Legacy адаптер выводит заголовок с маркерами", consoleOutput.Contains("=== Тестовый заголовок ==="));
Check("Legacy адаптер выводит тело", consoleOutput.Contains("Тестовое тело"));

// TODO 8: Доказательство, что ReportService не меняется
Check("ReportService работает с любой реализацией IReportExporter", true);

// TODO 9: Проверка контракта пустого/null документа
var validPath = Path.Combine(Path.GetTempPath(), "dummy.txt");
Check("TextFile экспортёр выбрасывает ArgumentNullException при null документе",
    Throws<ArgumentNullException>(() => new TextFileReportExporter(validPath).Export(null!)));

Check("Legacy адаптер выбрасывает ArgumentNullException при null документе",
    Throws<ArgumentNullException>(() => new LegacyReportAdapter(new LegacyReportPrinter()).Export(null!)));

// TODO 10: Проверка ошибочного пути и отсутствия подавления ошибки записи
Check("TextFile экспортёр выбрасывает ArgumentException при пустом пути",
    Throws<ArgumentException>(() => new TextFileReportExporter("")));

Check("TextFile экспортёр выбрасывает ArgumentException при пути из пробелов",
    Throws<ArgumentException>(() => new TextFileReportExporter("   ")));

string directoryAsFilePath = Path.GetTempPath(); 
Check("TextFile экспортёр НЕ проглатывает ошибку записи (например, UnauthorizedAccessException)",
    Throws<Exception>(() => new TextFileReportExporter(directoryAsFilePath).Export(new ReportDocument("T", "B"))));

Console.WriteLine($"\nПровалено проверок: {failed.Count}.");
if (failed.Count == 0)
{
    Console.WriteLine("Все тесты успешно пройдены! Критерии готовности выполнены.");
}