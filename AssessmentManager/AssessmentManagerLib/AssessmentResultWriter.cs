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
    public class AssessmentResultWriter
    {

        private StudentMarkingData smd = null;
        private bool includeModelAnswers = false;

        public AssessmentResultWriter(StudentMarkingData smd, bool includeModelAnswers)
        {
            this.includeModelAnswers = includeModelAnswers;
            this.smd = smd;
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

        public bool MakePDF(string filePath)
        {
            Document doc = new Document();
            bool successful = true;
            AssessmentInformation info = smd.Scripts.First().Script.AssessmentInfo;

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
                    authorPara.Add(new Chunk(new VerticalPositionMark()));
                    authorPara.Add(new Chunk($"{smd.StudentData.FirstName} {smd.StudentData.LastName} - {smd.StudentData.StudentID}"));
                    doc.Add(authorPara);
                }


                //Do title
                Paragraph titlePara = new Paragraph(info.AssessmentName, TitleFont);
                titlePara.SetAlignment(Center);
                titlePara.SpacingAfter = 5f;
                doc.Add(titlePara);

                //Do weighting
                Paragraph weightingPara = new Paragraph($"{info.AssessmentWeighting}%", WeightingFont);
                weightingPara.SetAlignment(Center);
                weightingPara.SpacingAfter = 5f;
                doc.Add(weightingPara);

                Paragraph linePara = new Paragraph(new Chunk(new LineSeparator(0.0f, 100f, Color.BLACK, Element.ALIGN_LEFT, 1)));
                linePara.SpacingAfter = 15f;
                doc.Add(linePara);

                //Do each question
                DoQuestions(smd.MarkingQuestions, doc);

                //Show marks for assessment
                Paragraph totalMarksPara = new Paragraph("");
                totalMarksPara.Add(new Chunk(new VerticalPositionMark()));
                Paragraph totalPhrase = new Paragraph($"Final result: {smd.FinalMark.ToString("0.#")} / {smd.TotalAvailableMarks}", TotalMarksFont);
                totalPhrase.SetAlignment(Right);
                totalMarksPara.Add(totalPhrase);
                totalMarksPara.SpacingBefore = 10f;
                doc.Add(totalMarksPara);

                Paragraph percentPara = new Paragraph("");
                percentPara.Add(new Chunk(new VerticalPositionMark()));
                Paragraph percentPhrase = new Paragraph($"{((smd.FinalMark / smd.TotalAvailableMarks) * 100).ToString("00.0")}%", TotalMarksFont);
                percentPhrase.SetAlignment(Right);
                percentPara.Add(percentPhrase);
                doc.Add(percentPara);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creating pdf");
                successful = false;
            }
            finally
            {
                doc.Close();
            }

            return successful;
        }

        private void DoQuestions(List<MarkingQuestion> questions, Document doc)
        {
            foreach (var q in questions)
            {
                DrawQuestion(q, doc);

                if (q.HasSubQuestions)
                {
                    DoQuestions(q.SubMarkingQuestions, doc);
                }
            }
        }

        private void DrawQuestion(MarkingQuestion mq, Document doc)
        {
            //Get the question
            Question question = smd.Scripts.First().Script.FindQuestion(mq.QuestionName);

            Paragraph mainPara = new Paragraph();
            mainPara.SpacingBefore = 20f;
            mainPara.IndentationLeft = GetIndent(mq.QuestionName);

            //Question header text
            Phrase questionHeader = new Phrase(mq.QuestionName, QuestionHeaderFont);
            mainPara.Add(questionHeader);
            if (question.AnswerType != AnswerType.None)
            {
                //Marks
                mainPara.Add(new Chunk(new VerticalPositionMark()));
                mainPara.Add(new Chunk($"Marks: {mq.AssignedMarks.ToString("0.#")} / {question.Marks}", MarksHeaderFont));
            }
            mainPara.Add("\n");

            //Question text
            Paragraph questionTextPara = new Paragraph(question.QuestionTextRaw, QuestionTextFont);
            questionTextPara.FirstLineIndent = 15f;
            mainPara.Add(questionTextPara);
            mainPara.Add("\n");

            //Multi Choice options if applicable
            if (question.AnswerType == AnswerType.Multi)
            {
                Phrase optA = new Phrase($"A) {question.OptionA}\n", QuestionTextFont);
                Phrase optB = new Phrase($"B) {question.OptionB}\n", QuestionTextFont);
                Phrase optC = new Phrase($"C) {question.OptionC}\n", QuestionTextFont);
                Phrase optD = new Phrase($"D) {question.OptionD}\n", QuestionTextFont);
                Paragraph optionsPara = new Paragraph();
                optionsPara.Add(optA);
                optionsPara.Add(optB);
                optionsPara.Add(optC);
                optionsPara.Add(optD);
                mainPara.Add(optionsPara);
            }

            //Model answer
            if (question.AnswerType != AnswerType.None)
            {
                if (includeModelAnswers && !question.ModelAnswer.NullOrEmpty())
                {
                    mainPara.Add(new Phrase("Model answer: \n", ModelAnswerHeaderFont));
                    if (question.AnswerType == AnswerType.Multi)
                    {
                        Paragraph multiAnswerPara = new Paragraph($"The correct option was: ({question.CorrectOption}) {question.GetOptionText(question.CorrectOption)}", QuestionTextFont);
                        mainPara.Add(multiAnswerPara);
                    }
                    else
                    {
                        Paragraph longAnswerPara = new Paragraph(question.ModelAnswer, QuestionTextFont);
                        mainPara.Add(longAnswerPara);
                    }
                    mainPara.Add("\n");
                }

                //Student response
                mainPara.Add(new Phrase("Student answer: \n", ModelAnswerHeaderFont));
                Answer answer = smd.Scripts.First().Script.Answers[mq.QuestionName];
                if (answer.Attempted)
                {
                    if (question.AnswerType == AnswerType.Multi)
                    {
                        MultiChoiceOption opt = answer.SelectedOption;
                        mainPara.Add(new Paragraph($"Student answered option ({opt}) {question.GetOptionText(opt)}", QuestionTextFont));
                    }
                    else
                    {
                        string sAnswer = question.AnswerType == AnswerType.Open ? answer.LongAnswer : answer.ShortAnswer;
                        mainPara.Add(new Paragraph(sAnswer, QuestionTextFont));
                    }
                }
                else
                    mainPara.Add(new Paragraph("Student did not answer", QuestionTextFont));
                mainPara.Add("\n");

                //Marker response
                if (!mq.MarkerResponse.NullOrEmpty())
                {
                    mainPara.Add(new Phrase("Marker response: \n", ModelAnswerHeaderFont));
                    mainPara.Add(new Paragraph(mq.MarkerResponse, QuestionTextFont));
                    mainPara.Add("\n");
                }
            }

            doc.Add(mainPara);
        }

        private float GetIndent(string qName)
        {
            int count = qName.Count(c => c == '.');
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
