using BminingBlazor.Resources;
using BminingBlazor.ViewModels.Report;
using BminingBlazor.ViewModels.User;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class GenerateReportNotCharged
    {
        public List<UserViewModel> _reports;
        public int line = 1;
        public void GenerateExcel(IJSRuntime iJSRuntime)
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("sheet");
                #region Header Row
                workSheet.Cells[1, 1].Value = Resource.Name;
                workSheet.Cells[1, 1].Style.Font.Size = 12;
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[1, 2].Value = Resource.Surname;
                workSheet.Cells[1, 2].Style.Font.Size = 12;
                workSheet.Cells[1, 2].Style.Font.Bold = true;
                workSheet.Cells[1, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;               

                workSheet.Cells[1, 3].Value = Resource.WorkArea;
                workSheet.Cells[1, 3].Style.Font.Size = 12;
                workSheet.Cells[1, 3].Style.Font.Bold = true;
                workSheet.Cells[1, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                #endregion
                foreach (var report in _reports)
                {
                    line = line + 1;
                    #region Body
                    workSheet.Cells[line, 1].Value = report.MyName;
                    workSheet.Cells[line, 1].Style.Font.Size = 12;
                    workSheet.Cells[line, 1].Style.Font.Bold = true;
                    workSheet.Cells[line, 1].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[line, 2].Value = report.MyPaternalSurname;
                    workSheet.Cells[line, 2].Style.Font.Size = 12;
                    workSheet.Cells[line, 2].Style.Font.Bold = true;
                    workSheet.Cells[line, 2].Style.Border.Top.Style = ExcelBorderStyle.Hair;                   

                    workSheet.Cells[line, 3].Value = report.MyWorkArea;
                    workSheet.Cells[line, 3].Style.Font.Size = 12;
                    workSheet.Cells[line, 3].Style.Font.Bold = true;
                    workSheet.Cells[line, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                    workSheet.Cells[line, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    #endregion

                }
                fileContents = package.GetAsByteArray();

            }
            iJSRuntime.InvokeAsync<GenerateReport>(
                    "saveAsFile",
                    "Reporte.xlsx",
                    Convert.ToBase64String(fileContents));
        }
    }
}
