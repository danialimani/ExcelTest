using DocumentFormat.OpenXml.Bibliography;
using ExcelExam.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelExam.Helper
{
    public static class Utility
    {

        public static void GetDataTableFromExcel(Stream fileStream,ApplicationDbContext context, bool hasHeader = true)
        {
            try
            {
                List<ExcelExam.Data.DatabaseModel.City> c = new List<ExcelExam.Data.DatabaseModel.City>();
                List<ExcelExam.Data.DatabaseModel.Sale> s = new List<ExcelExam.Data.DatabaseModel.Sale>();
                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                   // using (var stream = File.OpenRead(path))
                    using (var stream = fileStream)
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets.First();
                    var startRow = hasHeader ? 2 : 1;

                        for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {                         
                                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                                c.Add(new ExcelExam.Data.DatabaseModel.City()
                                {
                                    CityName = wsRow[rowNum, ws.Dimension.End.Column].Text,
                                });
                        }
                    context.Cities.AddRange(c);
                    context.SaveChanges();
                    c = null;
                    ws = pck.Workbook.Worksheets[1];
                    startRow = hasHeader ? 2 : 1;
                        for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                                var ss = new ExcelExam.Data.DatabaseModel.Sale();
                                ss.Price = float.Parse(wsRow[rowNum, ws.Dimension.End.Column].Text);
                                ss.PersonFullName = wsRow[rowNum, ws.Dimension.End.Column - 1].Text;
                                ss.ProductCode = wsRow[rowNum, ws.Dimension.End.Column - 2].Text;
                                ss.NameProduct = wsRow[rowNum, ws.Dimension.End.Column - 3].Text;
                                ss.CityName =  wsRow[rowNum, ws.Dimension.End.Column - 4].Text;
                                ss.Id = int.Parse(wsRow[rowNum, ws.Dimension.End.Column - 5].Text);
                                s.Add(ss);
                        }
                        context.Sales.AddRange(s);
                        context.SaveChanges();
                    s = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("***********************************************************************");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("***********************************************************************");

            }

        }
    }
}
