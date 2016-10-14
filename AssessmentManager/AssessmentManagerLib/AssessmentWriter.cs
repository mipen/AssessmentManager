using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AssessmentManager
{
    public class AssessmentWriter
    {
        private Assessment a;
        private AssessmentInformation info;
        private string filePath = "";

        public AssessmentWriter(Assessment assessment, AssessmentInformation info, string filePath)
        {
            a = assessment;
            this.info = info;
            this.filePath = filePath;
        }

        #region Fonts

        private const string FontName = "Calibri";

        private readonly Font AuthorFont = FontFactory.GetFont(FontName, 11f, Font.ITALIC);
        private readonly Font TitleFont = FontFactory.GetFont(FontName, 14f, Font.BOLD);
        private readonly Font WeightingFont = FontFactory.GetFont(FontName, 11f, Font.NORMAL);
        private readonly Font TotalMarksFont = FontFactory.GetFont(FontName, 12f, Font.BOLDITALIC);

        private readonly Font QuestionHeaderFont = FontFactory.GetFont(FontName, 12f, Font.BOLD);
        private readonly Font QuestionTextFont = FontFactory.GetFont(FontName, 12f, Font.NORMAL);
        private readonly Font ModelAnswerHeaderFont = FontFactory.GetFont(FontName, 11f, Font.ITALIC);
        private readonly Font MarksHeaderFont = FontFactory.GetFont(FontName, 12, Font.BOLD);
        private readonly Font MarksHeaderSecondaryFont = FontFactory.GetFont(FontName, 11f, Font.NORMAL);

        #endregion

        #region Alignments

        private const string Center = "Center";
        private const string Left = "Left";
        private const string Right = "Right";
        private const string Justify = "Justify";

        #endregion

        private const float SubQuestionIndent = 20f;
        private const float SubSubQuestionIndent = 40f;

        public bool MakePdf(bool withAnswers)
        {
            Document doc = new Document();
            bool successful = true;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);

                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                //Do author
                if (!info.Author.NullOrEmpty())
                {
                    string authorText = $"Author: {info.Author}";
                    Paragraph authorPara = new Paragraph(authorText, AuthorFont);
                    authorPara.SetAlignment("Left");
                    doc.Add(authorPara);
                }

                //Do title
                Paragraph titlePara = new Paragraph(info.AssessmentName, TitleFont);
                titlePara.SetAlignment(Center);
                titlePara.SpacingAfter = 5f;
                doc.Add(titlePara);

                //Do weighting
                if (!withAnswers)
                {
                    Paragraph weightingPara = new Paragraph($"{info.AssessmentWeighting}%", WeightingFont);
                    weightingPara.SetAlignment(Center);
                    weightingPara.SpacingAfter = 5f;
                    doc.Add(weightingPara);
                }
                else
                {
                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100f;
                    table.AddCell(GetCell("Includes model answers", ModelAnswerHeaderFont, PdfPCell.ALIGN_LEFT));
                    table.AddCell(GetCell($"{info.AssessmentWeighting}%", WeightingFont, PdfPCell.ALIGN_CENTER));
                    table.AddCell(GetCell("", AuthorFont, PdfPCell.ALIGN_RIGHT));
                    doc.Add(table);
                }

                Paragraph linePara = new Paragraph(new Chunk(new LineSeparator(0.0f, 100f, Color.BLACK, Element.ALIGN_LEFT, 1)));
                linePara.SpacingAfter = 15f;
                doc.Add(linePara);

                //Do the questions
                DoQuestions(a.Questions, doc, withAnswers);

                //Show total marks for assessment
                Paragraph totalMarksPara = new Paragraph("");
                totalMarksPara.Add(new Chunk(new VerticalPositionMark()));
                Paragraph totalPhrase = new Paragraph($"Total marks for assessment: {a.TotalMarks}", TotalMarksFont);
                totalPhrase.SetAlignment(Right);
                totalMarksPara.Add(totalPhrase);

                totalMarksPara.SpacingBefore = 10f;
                
                doc.Add(totalMarksPara);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error creating pdf");
                successful = false;
            }
            finally
            {
                doc.Close();
            }
            return successful;
        }

        private void DoQuestions(List<Question> questions, Document doc, bool withAnswers)
        {
            foreach (Question q in questions)
            {
                //Draw the question
                DrawQuestion(q, doc, withAnswers);

                //If the question has subquestions, draw those
                if (q.HasSubQuestions)
                {
                    DoQuestions(q.SubQuestions, doc, withAnswers);
                }
            }
        }

        private void DrawQuestion(Question q, Document doc, bool withAnswers)
        {
            Paragraph mainPara = new Paragraph();
            mainPara.SpacingBefore = 20f;
            mainPara.IndentationLeft = GetIndent(q);

            //Question name
            Phrase questionHeader = new Phrase(q.Name, QuestionHeaderFont);
            mainPara.Add(questionHeader);

            //Question marks
            mainPara.Add(new Chunk(new VerticalPositionMark()));
            Chunk marksChunk = null;
            Chunk secondaryMarksChunk = null;
            if (q.AnswerType != AnswerType.None)
            {
                marksChunk = new Chunk($"Marks: {q.Marks}", MarksHeaderFont);
                if (q.HasSubQuestions)
                    secondaryMarksChunk = new Chunk($"Total marks: {q.TotalMarks}", MarksHeaderSecondaryFont);
            }
            else if (q.HasSubQuestions)
            {
                marksChunk = new Chunk($"Total marks: {q.TotalMarks}", MarksHeaderFont);
            }
            else
            {
                marksChunk = new Chunk("");
            }
            mainPara.Add(marksChunk);
            mainPara.Add("\n");
            if (secondaryMarksChunk != null)
            {
                mainPara.Add("");
                mainPara.Add(new Chunk(new VerticalPositionMark()));
                mainPara.Add(secondaryMarksChunk);
                mainPara.Add("\n");
            }

            //Question text
            Paragraph questionTextPara = new Paragraph(q.QuestionTextRaw, QuestionTextFont);
            questionTextPara.FirstLineIndent = 15f;
            mainPara.Add(questionTextPara);
            mainPara.Add("\n");

            //Multi choice options (if applicable)
            if(q.AnswerType==AnswerType.Multi)
            {
                Phrase optA = new Phrase($"A) {q.OptionA}\n", QuestionTextFont);
                Phrase optB = new Phrase($"B) {q.OptionB}\n", QuestionTextFont);
                Phrase optC = new Phrase($"C) {q.OptionC}\n", QuestionTextFont);
                Phrase optD = new Phrase($"D) {q.OptionD}\n", QuestionTextFont);
                Paragraph optionsPara = new Paragraph();
                optionsPara.Add(optA);
                optionsPara.Add(optB);
                optionsPara.Add(optC);
                optionsPara.Add(optD);
                mainPara.Add(optionsPara);
            }

            //Model answers
            if(withAnswers)
            {
                Paragraph answerPara = null;
                if(q.AnswerType==AnswerType.Multi)
                {
                    answerPara = new Paragraph($"Correct option is: ({q.CorrectOption})",QuestionTextFont);
                }
                else if(q.AnswerType==AnswerType.Open || q.AnswerType==AnswerType.Single)
                {
                    answerPara = new Paragraph(q.ModelAnswer, QuestionTextFont);
                }

                if(answerPara!=null)
                {
                    mainPara.Add(new Phrase("Model answer: \n", ModelAnswerHeaderFont));
                    mainPara.Add(answerPara);
                }
            }

            doc.Add(mainPara);
        }

        private float GetIndent(Question q)
        {
            int count = q.Name.Count(c => c == '.');
            if (count == 0)
                return 0f;
            else if (count == 1)
                return SubQuestionIndent;
            else if (count == 2)
                return SubSubQuestionIndent;
            return 0f;
        }

        private PdfPCell GetCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Padding = 0f;
            cell.HorizontalAlignment = alignment;
            cell.Border = PdfPCell.NO_BORDER;
            return cell;
        }
    }
}
