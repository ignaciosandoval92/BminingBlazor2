using BminingBlazor.Resources;
using BminingBlazor.ViewModels.Report;
using Microsoft.JSInterop;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class GenerateReport
    {
        public List<ReportViewModel> _reports;
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

                workSheet.Cells[1, 3].Value = Resource.ProjectCode;
                workSheet.Cells[1, 3].Style.Font.Size = 12;
                workSheet.Cells[1, 3].Style.Font.Bold = true;
                workSheet.Cells[1, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[1, 4].Value = Resource.ProjectName;
                workSheet.Cells[1, 4].Style.Font.Size = 12;
                workSheet.Cells[1, 4].Style.Font.Bold = true;
                workSheet.Cells[1, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[1, 5].Value = Resource.TypeHours;
                workSheet.Cells[1, 5].Style.Font.Size = 12;
                workSheet.Cells[1, 5].Style.Font.Bold = true;
                workSheet.Cells[1, 5].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[1, 6].Value = Resource.Hours;
                workSheet.Cells[1, 6].Style.Font.Size = 12;
                workSheet.Cells[1, 6].Style.Font.Bold = true;
                workSheet.Cells[1, 6].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[1, 7].Value = Resource.Date;
                workSheet.Cells[1, 7].Style.Font.Size = 12;
                workSheet.Cells[1, 7].Style.Font.Bold = true;
                workSheet.Cells[1, 7].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                workSheet.Cells[1, 8].Value = Resource.WorkArea;
                workSheet.Cells[1, 8].Style.Font.Size = 12;
                workSheet.Cells[1, 8].Style.Font.Bold = true;
                workSheet.Cells[1, 8].Style.Border.Top.Style = ExcelBorderStyle.Hair;
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

                    workSheet.Cells[line, 3].Value = report.MyCodProject;
                    workSheet.Cells[line, 3].Style.Font.Size = 12;
                    workSheet.Cells[line, 3].Style.Font.Bold = true;
                    workSheet.Cells[line, 3].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[line, 4].Value = report.MyNameProject;
                    workSheet.Cells[line, 4].Style.Font.Size = 12;
                    workSheet.Cells[line, 4].Style.Font.Bold = true;
                    workSheet.Cells[line, 4].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                     switch(report.MyTypeHours)
                    {
                        case Models.TimeTracking.TimeTrackingTypeEnum.Ordinary:
                            workSheet.Cells[line, 5].Value = Resource.OrdinaryHours;
                                        break;
                            case Models.TimeTracking.TimeTrackingTypeEnum.Extraordinary:
                            workSheet.Cells[line, 5].Value = Resource.ExtraordinaryHours;
                                        break;                        
                    };
                    workSheet.Cells[line, 5].Style.Font.Size = 12;
                    workSheet.Cells[line, 5].Style.Font.Bold = true;
                    workSheet.Cells[line, 5].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[line, 6].Value = report.MyTrackedHours;
                    workSheet.Cells[line, 6].Style.Font.Size = 12;
                    workSheet.Cells[line, 6].Style.Font.Bold = true;
                    workSheet.Cells[line, 6].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                    workSheet.Cells[line, 7].Value = report.MyDateTracked;
                    workSheet.Cells[line, 7].Style.Font.Size = 12;
                    workSheet.Cells[line, 7].Style.Font.Bold = true;
                    workSheet.Cells[line, 7].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                    workSheet.Cells[line, 7].Style.Numberformat.Format = "dd/MM/yyyy";


                    switch (report.MyWorkArea)
                    {
                        case Models.User.WorkAreaModelEnum.Unknown:
                            workSheet.Cells[line, 8].Value = Resource.Unknown;
                                        break;
                        case Models.User.WorkAreaModelEnum.Administration:
                            workSheet.Cells[line, 8].Value = Resource.Administration;
                                        break;
                        case Models.User.WorkAreaModelEnum.Engineering:
                            workSheet.Cells[line, 8].Value = Resource.Engeneering;
                                        break;
                        case Models.User.WorkAreaModelEnum.Thesis:
                            workSheet.Cells[line, 8].Value = Resource.Thesist;
                                        break;
                        case Models.User.WorkAreaModelEnum.Strategic_communication:
                            workSheet.Cells[line, 8].Value = Resource.StrategicCommunication;
                                        break;
                        case Models.User.WorkAreaModelEnum.ExternalConsultant:
                            workSheet.Cells[line, 8].Value = Resource.ExternalConsultant;
                                        break;
                    }                 
                    workSheet.Cells[line, 8].Style.Font.Size = 12;
                    workSheet.Cells[line, 8].Style.Font.Bold = true;
                    workSheet.Cells[line, 8].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                    workSheet.Cells[line, 8].Style.Numberformat.Format = "dd/MM/yyyy";
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
