using BookStore.Infrastructure.Interfaces;
using BookStore.Domain.Models;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;

namespace BookStore.Infrastructure.Services
{
    public class CsvExportService : ICsvExportService
    {
        public string ExportToCsv<T>(IEnumerable<T> data, Region region)
        {
            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, GetCsvConfiguration(region)))
                {
                    csv.WriteRecords(data);
                    return writer.ToString();
                }
            }
        }

        private CsvConfiguration GetCsvConfiguration(Region region) =>
            new CsvConfiguration(CultureInfo.GetCultureInfo(region.CultureCode))
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                Quote = '"',
                Escape = '"'
            };
        
    }
}
