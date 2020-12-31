using BminingBlazor.Resources;
using BminingBlazor.ViewModels.ActivityRecord;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazored.TextEditor;
namespace BminingBlazor.Utility
{
    public class PdfActivityRecord
    {
        public BlazoredTextEditor Notes;
        public string Note { get; set; }
        public async Task<string> OnitianalizedAsync()
        {
            await Notes.LoadHTMLContent(_oActivityRecord.MyNotes);
            var Note = await Notes.GetText();
            return Note;
        }
        #region Declararion
        int _maxColumn = 3;
        Document _document;
        PdfPTable _pdfTable = new PdfPTable(3);
        PdfPTable _pdfTable2 = new PdfPTable(3);
        PdfPTable _pdfTable3 = new PdfPTable(4);
        PdfPTable _pdfTable4 = new PdfPTable(3);
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
            _pdfTable2.WidthPercentage = 100;
            _pdfTable2.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfTable3.WidthPercentage = 100;
            _pdfTable3.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfTable4.WidthPercentage = 100;
            _pdfTable4.HorizontalAlignment = Element.ALIGN_LEFT;
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
            this.ReportBody2();
            this.ReportBody3();
            this.ReportBody4();

            _pdfTable.HeaderRows = 2;
            //_pdfTable2.HeaderRows = 2;
            _document.Add(_pdfTable);
            _document.Add(_pdfTable2);
            _document.Add(_pdfTable3);
            _document.Add(_pdfTable4);
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
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 12f, 1);
            _pdfCell = new PdfPCell(new Phrase($"{Resource.Project}:{_oActivityRecord.MyProjectCode}-{_oActivityRecord.MyProjectName}", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 12f, 1);
            _pdfCell = new PdfPCell(new Phrase($"{Resource.Date}:{_oActivityRecord.MyDate}", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 12f, 1);
            _pdfCell = new PdfPCell(new Phrase($"{Resource.DurationHours}:{_oActivityRecord.MyDurationHours}", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 12f, 1);
            _pdfCell = new PdfPCell(new Phrase($"{Resource.Place}:{_oActivityRecord.MyPlace}", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 10f, 1);
            _pdfCell = new PdfPCell(new Phrase(Resource.Members, _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 1f, 1);
            _pdfCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfCell);
            _pdfTable.CompleteRow();
            _document.Add(Chunk.Newline);

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

            foreach (var oActivityRecord in _oActivityRecord.OurMembers)
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
        private void ReportBody2()
        {
            _fontStyle = FontFactory.GetFont("Arial", 1f, 1);
            _pdfCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable2.AddCell(_pdfCell);
            _pdfTable2.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 10f, 1);
            _pdfCell = new PdfPCell(new Phrase(Resource.Commitments, _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable2.AddCell(_pdfCell);
            _pdfTable2.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 1f, 1);
            _pdfCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable2.AddCell(_pdfCell);
            _pdfTable2.CompleteRow();
            _document.Add(Chunk.Newline);

        }
        private void ReportBody3()
        {
            _fontStyle = FontFactory.GetFont("Arial", 9f, 1);
            var fontStyle = FontFactory.GetFont("Arial", 9f, 0);

            #region Table Header
            _pdfCell = new PdfPCell(new Phrase(Resource.Responsible, _fontStyle));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfTable3.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(Resource.EndDate, _fontStyle));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfTable3.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(Resource.Commitment, _fontStyle));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfTable3.AddCell(_pdfCell);

            _pdfCell = new PdfPCell(new Phrase(Resource.Status, _fontStyle));
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfCell.BackgroundColor = BaseColor.Gray;
            _pdfTable3.AddCell(_pdfCell);

            _pdfTable.CompleteRow();
            #endregion

            #region Table Body

            foreach (var oActivityRecord in _oActivityRecord.OurCommitments)
            {
                _pdfCell = new PdfPCell(new Phrase(oActivityRecord.MyResponsible, fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfTable3.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(oActivityRecord.MyCommitmentDate.ToString("dd/MM/yyyy"), fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfTable3.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(oActivityRecord.MyCommitment, fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfTable3.AddCell(_pdfCell);

                _pdfCell = new PdfPCell(new Phrase(oActivityRecord.MyStatus.ToString(), fontStyle));
                _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfCell.BackgroundColor = BaseColor.White;
                _pdfTable3.AddCell(_pdfCell);
            }
            #endregion
        }
        private void ReportBody4()
        {
            _fontStyle = FontFactory.GetFont("Arial", 1f, 1);
            _pdfCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 10f, 1);
            _pdfCell = new PdfPCell(new Phrase(Resource.SecurityReflection, _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 1f, 1);
            _pdfCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 7f, 0);
            _pdfCell = new PdfPCell(new Phrase(_oActivityRecord.MySecurityReflection, _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 1f, 1);
            _pdfCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 10f, 1);
            _pdfCell = new PdfPCell(new Phrase(Resource.Notes, _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);

            _fontStyle = FontFactory.GetFont("Arial", 1f, 1);
            _pdfCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);

            
            _fontStyle = FontFactory.GetFont("Arial", 7f, 0);
            _pdfCell = new PdfPCell(new Phrase(_oActivityRecord.MyNotes, _fontStyle));
            _pdfCell.Colspan = _maxColumn;
            _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfCell.Border = 0;
            _pdfCell.ExtraParagraphSpace = 0;
            _pdfTable4.AddCell(_pdfCell);
            _pdfTable4.CompleteRow();
            _document.Add(Chunk.Newline);
        }
    }
}
