using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssessmentManager
{
    public class AssessmentSessionResultsWriter
    {
        private AssessmentSession session = null;
        private string filePath = "";

        public AssessmentSessionResultsWriter(AssessmentSession session, string filePath)
        {
            this.session = session;
            this.filePath = filePath;
        }

        #region Fonts

        private const string FontName = "Calibri";

        private readonly Font TitleFont = FontFactory.GetFont(FontName, 14f, Font.BOLD);
        private readonly Font AuthorFont = FontFactory.GetFont(FontName, 11f, Font.ITALIC);
        private readonly Font WeightingFont = FontFactory.GetFont(FontName, 11f, Font.NORMAL);

        private readonly Font TextFont = FontFactory.GetFont(FontName, 12f, Font.NORMAL);
        private readonly Font ColumnHeaderFont = FontFactory.GetFont(FontName, 13f, Font.BOLD);

        #endregion

        #region Alignments

        private const string Center = "Center";
        private const string Left = "Left";
        private const string Right = "Right";
        private const string Justify = "Justify";

        #endregion

        public bool MakePdf()
        {
            Document doc = new Document();
            bool successful = true;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);

                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                //Do author
                if (session.AssessmentInfo != null && !session.AssessmentInfo.Author.NullOrEmpty())
                {
                    string authorText = $"Author: {session.AssessmentInfo.Author}";
                    Paragraph authorPara = new Paragraph(authorText, AuthorFont);
                    authorPara.SetAlignment("Left");
                    doc.Add(authorPara);
                }

                //Do title
                string titleStr = "";
                if (session.AssessmentInfo != null)
                    titleStr = session.AssessmentInfo.AssessmentName;
                else
                    titleStr = "Assessment";
                Paragraph titlePara = new Paragraph(titleStr, TitleFont);
                titlePara.SetAlignment(Center);
                titlePara.SpacingAfter = 5f;
                doc.Add(titlePara);

                //Do weighting
                if (session.AssessmentInfo != null)
                {
                    Paragraph weightingPara = new Paragraph($"{session.AssessmentInfo.AssessmentWeighting}%", WeightingFont);
                    weightingPara.SetAlignment(Center);
                    weightingPara.SpacingAfter = 15f;
                    doc.Add(weightingPara);
                }

                //Do table
                List<StudentMarkingData> sData = session.StudentMarkingData.OrderByDescending(s => s.FinalMark).ThenBy(s => s.StudentData.LastName).ToList();

                PdfPTable table = new PdfPTable(3);
                table.SetWidths(new float[] { 3f, 5f, 2f });
                table.WidthPercentage = 100f;
                table.AddCell(GetCell("StudentID", ColumnHeaderFont, PdfPCell.ALIGN_CENTER));
                table.AddCell(GetCell("Name", ColumnHeaderFont, PdfPCell.ALIGN_CENTER));
                table.AddCell(GetCell("Result", ColumnHeaderFont, PdfPCell.ALIGN_CENTER));

                foreach(var s in sData)
                {
                    table.AddCell(GetCell(s.StudentData.StudentID, TextFont, PdfPCell.ALIGN_CENTER));
                    table.AddCell(GetCell(s.StudentData.FirstName + " " + s.StudentData.LastName, TextFont, PdfPCell.ALIGN_CENTER));
                    table.AddCell(GetCell(s.FinalMark.ToString(), TextFont, PdfPCell.ALIGN_CENTER));
                }

                doc.Add(table);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error creating PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                successful = false;
            }
            finally
            {
                doc.Close();
            }
            return successful;
        }

        private PdfPCell GetCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Padding = 4f;
            cell.HorizontalAlignment = alignment;
            cell.Border = PdfPCell.BOX;
            return cell;
        }
    }
}
