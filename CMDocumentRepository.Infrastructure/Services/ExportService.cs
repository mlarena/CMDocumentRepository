using CMDocumentRepository.Application.DTOs;
using ClosedXML.Excel;

namespace CMDocumentRepository.Infrastructure.Services;

public interface IExportService
{
    byte[] ExportDocumentsToExcel(List<DocumentDto> documents);
}

public class ExportService : IExportService
{
    public byte[] ExportDocumentsToExcel(List<DocumentDto> documents)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Документы");

        worksheet.Cell(1, 1).Value = "Номер";
        worksheet.Cell(1, 2).Value = "Название";
        worksheet.Cell(1, 3).Value = "Тип";
        worksheet.Cell(1, 4).Value = "Категория";
        worksheet.Cell(1, 5).Value = "Версия";
        worksheet.Cell(1, 6).Value = "Статус";
        worksheet.Cell(1, 7).Value = "Автор";
        worksheet.Cell(1, 8).Value = "Дата создания";

        var headerRange = worksheet.Range(1, 1, 1, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < documents.Count; i++)
        {
            var doc = documents[i];
            worksheet.Cell(i + 2, 1).Value = doc.DocumentNumber;
            worksheet.Cell(i + 2, 2).Value = doc.Title;
            worksheet.Cell(i + 2, 3).Value = doc.DocumentTypeName;
            worksheet.Cell(i + 2, 4).Value = doc.CategoryName;
            worksheet.Cell(i + 2, 5).Value = doc.Version.ToString("F1");
            worksheet.Cell(i + 2, 6).Value = GetStatusName(doc.Status);
            worksheet.Cell(i + 2, 7).Value = doc.CreatorName;
            worksheet.Cell(i + 2, 8).Value = doc.CreatedAt.ToString("dd.MM.yyyy");
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string GetStatusName(CMDocumentRepository.Domain.Enums.DocumentStatus status)
    {
        return status switch
        {
            CMDocumentRepository.Domain.Enums.DocumentStatus.Draft => "Черновик",
            CMDocumentRepository.Domain.Enums.DocumentStatus.PendingApproval => "На согласовании",
            CMDocumentRepository.Domain.Enums.DocumentStatus.Approved => "Согласован",
            CMDocumentRepository.Domain.Enums.DocumentStatus.Rejected => "Отклонён",
            CMDocumentRepository.Domain.Enums.DocumentStatus.Rework => "На доработке",
            CMDocumentRepository.Domain.Enums.DocumentStatus.Active => "Действует",
            CMDocumentRepository.Domain.Enums.DocumentStatus.Archived => "Архив",
            _ => status.ToString()
        };
    }
}
