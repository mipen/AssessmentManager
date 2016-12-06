using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.IO;
using System.Windows.Forms;

namespace AssessmentManager
{
    public class HandoutWriter
    {
        private AssessmentSession session;
        private string outputPath, rulesPath;

        public HandoutWriter(AssessmentSession session, string outputPath, string rulesPath)
        {
            this.session = session;
            this.outputPath = outputPath;
            this.rulesPath = rulesPath;
        }

        #region Alignments

        private const string Center = "Center";
        private const string Left = "Left";
        private const string Right = "Right";
        private const string Justify = "Justify";

        #endregion

        #region Fonts

        private const string FontName = "Calibri";

        private static readonly Font TitleFont = FontFactory.GetFont(FontName, 14f, Font.BOLD);
        private static readonly Font BodyFont = FontFactory.GetFont(FontName, 12f, Font.NORMAL);
        private static readonly Font BodyFont_Bold = FontFactory.GetFont(FontName, 12f, Font.BOLD);
        private static readonly Font TopFont_Bold = FontFactory.GetFont(FontName, 11f, Font.BOLD);
        private static readonly Font TopFont = FontFactory.GetFont(FontName, 11f, Font.NORMAL);

        #endregion

        public bool MakePdf()
        {
            bool successful = true;
            Document doc = new Document();
            try
            {
                FileStream fs = new FileStream(outputPath, FileMode.Create);

                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                foreach (var s in session.StudentData)
                {
                    //Do student name
                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100f;
                    table.AddCell(GetCell(s.StudentID, TopFont, PdfPCell.ALIGN_LEFT));
                    table.AddCell(GetCell("", TopFont, PdfPCell.ALIGN_CENTER));
                    table.AddCell(GetCell($"{s.FirstName} {s.LastName}", TopFont, PdfPCell.ALIGN_RIGHT));
                    table.SpacingAfter = 5f;
                    doc.Add(table);

                    //Do title
                    Paragraph titlePara = new Paragraph("Manukau Institute of Technology", TitleFont);
                    titlePara.SetAlignment(Center);
                    doc.Add(titlePara);

                    //Do course
                    Paragraph coursePara = new Paragraph(CourseManager.Instance.FindCourseByID(session.CourseID).CourseTitle, TopFont_Bold);
                    coursePara.SpacingAfter = 5f;
                    coursePara.SetAlignment(Center);
                    doc.Add(coursePara);

                    //Do assessment name
                    Paragraph assessmentPara = new Paragraph(session.AssessmentInfo.AssessmentName, TopFont_Bold);
                    assessmentPara.SetAlignment(Center);
                    assessmentPara.SpacingAfter = -7f;
                    doc.Add(assessmentPara);

                    //Do line
                    Paragraph linePara = new Paragraph(new Chunk(new LineSeparator(0.0f, 80f, Color.BLACK, Element.ALIGN_CENTER, 1)));
                    linePara.SpacingAfter = 10f;
                    doc.Add(linePara);

                    //Do time stuff
                    PdfPTable timeTable = new PdfPTable(3);
                    timeTable.WidthPercentage = 100f;
                    timeTable.AddCell(GetCell("Date: " + session.StartTime.ToShortDateString(), TopFont, PdfPCell.ALIGN_LEFT));
                    timeTable.AddCell(GetCell("|", TopFont, PdfPCell.ALIGN_CENTER));
                    timeTable.AddCell(GetCell("Time: " + session.StartTime.ToString("hh:mm:ss tt"), TopFont, PdfPCell.ALIGN_RIGHT));
                    timeTable.SpacingAfter = 5f;
                    doc.Add(timeTable);

                    Paragraph readingTimePara = new Paragraph("Reading time (mins): " + session.ReadingTime.ToString(), TopFont_Bold);
                    readingTimePara.IndentationLeft = 30f;
                    doc.Add(readingTimePara);

                    Paragraph finishTimePara = new Paragraph("Finish time: " + session.FinishTime.ToString("hh:mm:ss tt"), TopFont_Bold);
                    finishTimePara.SpacingAfter = 5f;
                    finishTimePara.IndentationLeft = 30f;
                    doc.Add(finishTimePara);

                    //Do account name and pass
                    Paragraph accountNamePara = new Paragraph("Test username: " + s.AccountName, TopFont_Bold);
                    accountNamePara.IndentationLeft = 30f;
                    doc.Add(accountNamePara);

                    Paragraph accountPasswordPara = new Paragraph("Test password: " + s.AccountPassword, TopFont_Bold);
                    accountPasswordPara.SpacingAfter = 10f;
                    accountPasswordPara.IndentationLeft = 30f;
                    doc.Add(accountPasswordPara);

                    //Do rules
                    if (File.Exists(rulesPath))
                    {
                        string rules = File.ReadAllText(rulesPath);
                        Paragraph instructionsTitlePara = new Paragraph("Instructions: ", BodyFont_Bold);
                        instructionsTitlePara.SpacingAfter = 5f;
                        doc.Add(instructionsTitlePara);

                        Paragraph rulesPara = new Paragraph(rules, BodyFont);
                        rulesPara.IndentationLeft = 10f;
                        doc.Add(rulesPara);
                    }

                    doc.NewPage();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creating handout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                successful = false;
            }
            finally
            {
                doc.Close();
            }
            return successful;
        }

        /*public static void MakeTestHandout(string filePath)
        {
            Document doc = new Document();
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);

                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                //Do student name
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100f;
                table.AddCell(GetCell("130001111", TopFont, PdfPCell.ALIGN_LEFT));
                table.AddCell(GetCell("", TopFont, PdfPCell.ALIGN_CENTER));
                table.AddCell(GetCell("John Smith", TopFont, PdfPCell.ALIGN_RIGHT));
                table.SpacingAfter = 5f;
                doc.Add(table);

                //Do title
                Paragraph titlePara = new Paragraph("Manukau Institute of Technology", TitleFont);
                titlePara.SetAlignment(Center);
                doc.Add(titlePara);

                //Do course
                Paragraph coursePara = new Paragraph("Test Course", TopFont_Bold);
                coursePara.SpacingAfter = 5f;
                coursePara.SetAlignment(Center);
                doc.Add(coursePara);

                //Do assessment name
                Paragraph assessmentPara = new Paragraph("Assessment", TopFont_Bold);
                assessmentPara.SetAlignment(Center);
                assessmentPara.SpacingAfter = -7f;
                doc.Add(assessmentPara);

                //Do line
                Paragraph linePara = new Paragraph(new Chunk(new LineSeparator(0.0f, 80f, Color.BLACK, Element.ALIGN_CENTER, 1)));
                linePara.SpacingAfter = 10f;
                doc.Add(linePara);

                //Do time stuff
                PdfPTable timeTable = new PdfPTable(3);
                timeTable.WidthPercentage = 100f;
                timeTable.AddCell(GetCell("Date: " + DateTime.Now.ToShortDateString(), TopFont, PdfPCell.ALIGN_LEFT));
                timeTable.AddCell(GetCell("|", TopFont, PdfPCell.ALIGN_CENTER));
                timeTable.AddCell(GetCell("Time: " + "12:00:00 PM", TopFont, PdfPCell.ALIGN_RIGHT));
                timeTable.SpacingAfter = 5f;
                doc.Add(timeTable);

                Paragraph readingTimePara = new Paragraph("Reading time (mins): " + "10", TopFont_Bold);
                readingTimePara.IndentationLeft = 30f;
                doc.Add(readingTimePara);

                Paragraph finishTimePara = new Paragraph("Finish time: " + "01:00:00 PM", TopFont_Bold);
                finishTimePara.SpacingAfter = 5f;
                finishTimePara.IndentationLeft = 30f;
                doc.Add(finishTimePara);

                //Do account name and pass
                Paragraph accountNamePara = new Paragraph("Test username: " + "MIT_01", TopFont_Bold);
                accountNamePara.IndentationLeft = 30f;
                doc.Add(accountNamePara);

                Paragraph accountPasswordPara = new Paragraph("Test password: " + "password1", TopFont_Bold);
                accountPasswordPara.SpacingAfter = 10f;
                accountPasswordPara.IndentationLeft = 30f;
                doc.Add(accountPasswordPara);

                //Do rules
                if (File.Exists(CONSTANTS.RULES_FILE_PATH))
                {
                    string rules = File.ReadAllText(CONSTANTS.RULES_FILE_PATH);
                    Paragraph instructionsTitlePara = new Paragraph("Instructions: ", BodyFont_Bold);
                    instructionsTitlePara.SpacingAfter = 5f;
                    doc.Add(instructionsTitlePara);

                    Paragraph rulesPara = new Paragraph(rules, BodyFont);
                    rulesPara.IndentationLeft = 10f;
                    doc.Add(rulesPara);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creating handout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                doc.Close();
            }
        }*/

        private static PdfPCell GetCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Padding = 0f;
            cell.HorizontalAlignment = alignment;
            cell.Border = PdfPCell.NO_BORDER;
            return cell;
        }
    }
}
