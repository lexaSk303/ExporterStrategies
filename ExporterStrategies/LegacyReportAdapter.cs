namespace ExporterStrategies;

public sealed class LegacyReportAdapter : IReportExporter
{
    private readonly LegacyReportPrinter _printer;

    public LegacyReportAdapter(LegacyReportPrinter printer)
    {
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
    }

    public void Export(ReportDocument document)
    {
        // TODO 3: Согласование проверки входных данных
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document), "Документ не может быть null");
        }

        _printer.PrintReport(document.Title ?? string.Empty, document.Body ?? string.Empty);
    }
}