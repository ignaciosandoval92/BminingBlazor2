using BminingBlazor.Resources;
using BminingBlazor.ViewModels.ActivityRecord;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.Utility
{
    public class PdfActivityRecord
    {
        #region Declararion
        int _maxColumn = 3;
        Document _document;
        PdfPTable _pdfTable = new PdfPTable(3);
        PdfPCell _pdfCell;
        Font _fontStyle;
        MemoryStream _memoryStream = new MemoryStream();
        ActivityRecordViewModel _oActivityRecord = new ActivityRecordViewModel();
        #endregion
        public byte[] Report(ActivityRecordViewModel oActivityRecord)
        {
            _oActivityRecord = oActivityRecord;
            _document = new Document(PageSize.A4, 10f, 10f, 20f, 30f);
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _fontStyle = FontFactory.GetFont("Arial", 8f, 1);
            PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();
            float[] sizes = new float[_maxColumn];
            for (var i = 0; i < _maxColumn; i++)
            {
                if (i == 0) sizes[i] = 50;
                else sizes[i] = 100;
            }
            _pdfTable.SetWidths(sizes);

            this.ReportHeader();
            this.ReportBody();

            _pdfTable.HeaderRows = 2;
            _document.Add(_pdfTable);
            _document.Close();
            return _memoryStream.ToArray();
        }
        private void ReportHeader()
        {
            _fontStyle = FontFactory.GetFont("Arial", 18f, 1);
            _pdfCell = new PdfPCell(new Phrase(_oActivityRecord.MyTitle, _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Arial", 14f, 1);
            _pdfCell = new PdfPCell(new Phrase($"{Resource.Project}:{_oActivityRecord.MyProjectCode}-{_oActivityRecord.MyProjectName}", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();

            _fontStyle = FontFactory.GetFont("Arial", 14f, 1);
            _pdfCell = new PdfPCell(new Phrase($"{Resource.Date}:{_oActivityRecord.MyDate}", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
        }

        private void ReportBody()
        {
            _fontStyle = FontFactory.GetFont("Arial", 9f, 1);
            var fontStyle = FontFactory.GetFont("Arial", 9f, 0);

            #region Table Header
            _pdfCell = new PdfPCell(new Phrase(Resource.Name, _fontStyle));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(Resource.Email, _fontStyle));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(Resource.Enterprise, _fontStyle));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfTable.AddCell(_pdfCell);

            _pdfTable.CompleteRow();
            #endregion

            #region Table Body
           
            foreach(var oActivityRecord in _oActivityRecord.OurMembers)
            {
                _pdfCell = new PdfPCell(new Phrase(oActivityRecord.MyName, fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(oActivityRecord.MyEmail, fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(oActivityRecord.MyEnterprise, fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfTable.AddCell(_pdfCell);
            }
            #endregion
        }
    }
}
