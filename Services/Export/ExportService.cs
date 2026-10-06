using CsvHelper;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Medtrix_PmTool_V2_Api.Services.Export
{
    public class ExportService : IExportService
    {
        public FileContentResult ExportCsv<T>(IEnumerable<T> data, string fileName)
        {
            var list = data.ToList();
            var properties = GetExportProperties<T>();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

            // Header
            writer.WriteLine(string.Join(",", properties.Select(p => EscapeCsv(GetHeaderName(p)))));

            // Data
            foreach (var item in list)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item);
                    return EscapeCsv(FormatValue(value));
                });

                writer.WriteLine(string.Join(",", values));
            }

            writer.Flush();

            return new FileContentResult(memoryStream.ToArray(), "text/csv")
            {
                FileDownloadName = $"{fileName}.csv"
            };
        }

        public FileContentResult ExportPdf<T>(IEnumerable<T> data, string fileName)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var list = data.ToList();
            var properties = GetExportProperties<T>();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header().Column(column =>
                    {
                        column.Item()
                              .Text(fileName)
                              .FontSize(20)
                              .Bold();

                        column.Item()
                              .Text($"Generated On : {DateTime.Now:dd-MMM-yyyy hh:mm tt}")
                              .FontSize(10)
                              .FontColor(Colors.Grey.Darken2);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var p in properties)
                                columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            foreach (var p in properties)
                            {
                                header.Cell()
                                    .Background(Colors.Blue.Lighten2)
                                    .Border(1)
                                    .Padding(5)
                                    .Text(GetHeaderName(p))
                                    .Bold()
                                    .FontColor(Colors.White);
                            }
                        });

                        foreach (var item in list)
                        {
                            foreach (var p in properties)
                            {
                                var value = FormatValue(p.GetValue(item));

                                table.Cell()
                                    .Border(1)
                                    .Padding(5)
                                    .Text(value);
                            }
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf();

            return new FileContentResult(pdf, "application/pdf")
            {
                FileDownloadName = $"{fileName}.pdf"
            };
        }

        #region Helpers

        private static List<PropertyInfo> GetExportProperties<T>()
        {
            return typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToList();
        }

        private static string GetHeaderName(PropertyInfo property)
        {
            return property.Name;
        }

        private static string FormatValue(object? value)
        {
            if (value == null)
                return "";

            return value switch
            {
                DateTime dt => dt.ToString("dd-MMM-yyyy"),
                decimal d => d.ToString("0.00"),
                double d => d.ToString("0.00"),
                float f => f.ToString("0.00"),
                bool b => b ? "Yes" : "No",
                _ => value.ToString() ?? ""
            };
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }

        #endregion
    }
}