using MathApp.Services.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MathApp.Models.BusinessModels;
using MathApp.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words;
using Aspose.Words.Saving;

namespace MathApp.Services
{
    public class FileService:IFileService
    {
        public FileService()
        {
        }
        public (MemoryStream stream, string contentType, string filename) GenerateTopicFile(Topic topic)
        {
            var stream = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                DocumentFormat.OpenXml.Wordprocessing.Body body = new DocumentFormat.OpenXml.Wordprocessing.Body();
                mainPart.Document.Append(body);

                string title = "";
                if (topic.Name.IndexOf("Урок", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    title = topic.Name;
                }
                else
                {
                    if (topic.Name.IndexOf("Състезание", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        title = topic.Name;
                    }
                    else
                    {
                        if (topic.Type == "lesson") title = "Урок " + topic.Name;
                        else title = "Състезание " + topic.Name;
                    }
                }
                title = title + " — " + topic.EventDate.ToString("dd.MM.yyyy г.");
                body.Append(CreateCenteredParagraph(title, true));
                body.Append(CreateParagraph($" "));

                string answers = "";

                for (int i = 0; i < topic.MathProblems.Count; i++)
                {
                    body.Append(CreateParagraph($"{i + 1}. {topic.MathProblems.ElementAt(i).Conditions}"));
                    for (int a = 0; a < topic.MathProblems.ElementAt(i).Answers.Count; a++)
                    {
                        answers = answers + GetCyrillicLetter(a + 1, isUppercase: false) + ") " + topic.MathProblems.ElementAt(i).Answers.ElementAt(a).Name;
                        if(a< topic.MathProblems.ElementAt(i).Answers.Count-1) answers = answers + "           ";
                    }
                    body.Append(CreateParagraph($"{answers}"));
                    
                    answers = "";
                }

                mainPart.Document.Save();
            }

            stream.Seek(0, SeekOrigin.Begin);

            string filename = topic.Name + ".docx";
            return (stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);

        }

        public (MemoryStream stream, string contentType, string filename) CreateAndEncryptTopicFile(Topic topic, string password)
        {

            var (stream, contentType, filename) = GenerateTopicFile(topic);


            string tempFilePath = Path.Combine(Path.GetTempPath(), filename);
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(tempFilePath);

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
            saveOptions.Password = password;

            foreach (Section section in doc.Sections)
            {
                section.HeadersFooters.Clear();
            }

            NodeCollection paragraphs = doc.FirstSection.Body.Paragraphs;
            if (paragraphs.Count > 0)
            {
                paragraphs[0].Remove();
            }

            var encryptedStream = new MemoryStream();
            doc.Save(encryptedStream, saveOptions);

            File.Delete(tempFilePath);

            encryptedStream.Seek(0, SeekOrigin.Begin); 
            return (encryptedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);
        }

        public (MemoryStream stream, string contentType, string filename) CreateAndEncryptTopicFileWithAnswers(Topic topic, string password)
        {
            var (stream, contentType, filename) = GenerateTopicFileWithAnswers(topic);

            string tempFilePath = Path.Combine(Path.GetTempPath(), filename);
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(tempFilePath);

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
            saveOptions.Password = password;

            var encryptedStream = new MemoryStream();
            doc.Save(encryptedStream, saveOptions);

            File.Delete(tempFilePath);
            encryptedStream.Seek(0, SeekOrigin.Begin); 
            return (encryptedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);
        }

        public (MemoryStream stream, string contentType, string filename) GenerateTopicFileWithAnswers(Topic topic)
        {
            
            var stream = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                DocumentFormat.OpenXml.Wordprocessing.Body body = new DocumentFormat.OpenXml.Wordprocessing.Body();
                mainPart.Document.Append(body);

                string title = "";
                if (topic.Name.IndexOf("Урок", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    title = topic.Name;

                }
                else
                {
                    if (topic.Name.IndexOf("Състезание", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        title = topic.Name;
                    }
                    else
                    {
                        if (topic.Type == "lesson") title = "Урок " + topic.Name;
                        else title = "Състезание " + topic.Name;
                    }
                }
                title = title + " — " + topic.EventDate.ToString("dd.MM.yyyy г.");

                body.Append(CreateCenteredParagraph(title, true));
                body.Append(CreateParagraph($" "));
                string answers = "";
                for (int i = 0; i < topic.MathProblems.Count; i++)
                {
                    body.Append(CreateParagraph($"{i + 1}. {topic.MathProblems.ElementAt(i).Conditions}"));
                    body.Append(CreateParagraph($"Решение: {topic.MathProblems.ElementAt(i).Solution}"));
                    for (int a = 0; a < topic.MathProblems.ElementAt(i).Answers.Count; a++)
                    {
                        answers = answers + GetCyrillicLetter(a + 1, isUppercase: false) + ") " + topic.MathProblems.ElementAt(i).Answers.ElementAt(a).Name;
                        if (a < topic.MathProblems.ElementAt(i).Answers.Count - 1) answers = answers + "           ";
                    }
                    body.Append(CreateParagraph($"{answers}"));
                    
                    answers = "";
                }

                body.Append(CreateCenteredParagraph("Отговори:",true));
                body.Append(CreateTableOfAnswers(topic));


                mainPart.Document.Save();
            }

            stream.Seek(0, SeekOrigin.Begin);

            string filename = topic.Name + ".docx";
            return (stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);

        }
        private DocumentFormat.OpenXml.Wordprocessing.Paragraph CreateCenteredParagraph(string text, bool isBold, string fontSize="36")
        {
            ParagraphProperties paragraphProperties = new ParagraphProperties
            {
                Justification = new Justification { Val = JustificationValues.Center }
            };

            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" });
            if (isBold)
            {
                runProperties.Append(new Bold());
            }
            if (!string.IsNullOrEmpty(fontSize))
            {
                runProperties.Append(new FontSize { Val = fontSize });
            }

            DocumentFormat.OpenXml.Wordprocessing.Run run = new DocumentFormat.OpenXml.Wordprocessing.Run();
            run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            run.RunProperties = runProperties;

            DocumentFormat.OpenXml.Wordprocessing.Paragraph para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
            para.PrependChild(paragraphProperties);
            para.Append(run);

            return para;
        }
        private Table CreateTableOfAnswers(Topic topic)
        {
            Table table = new Table();
            TableProperties tblProperties = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                ),
                new TableJustification { Val = TableRowAlignmentValues.Center }
            );
            table.AppendChild(tblProperties);

            TableRow headerRow = new TableRow();
            headerRow.Append(CreateTableCell("  Задача  "));
            headerRow.Append(CreateTableCell("  Отговор  "));
            table.Append(headerRow);


            for (int i = 0; i < topic.MathProblems.Count; i++)
            {
                var correctAnswers = topic.MathProblems.ElementAt(i).Answers.Where(x => x.Validity == 1);
                string answers = "";
                if (correctAnswers != null)
                {
                    for (int a = 0; a < correctAnswers.Count(); a++)
                    {
                        answers = answers + correctAnswers.ElementAt(a).Name;
                        if (a < correctAnswers.Count() - 1) answers = answers + ", ";
                    }
                }

                table.Append(CreateTableRow(topic.MathProblems.ElementAt(i).Position.ToString(), answers));

            }
            return table;

        }
        private DocumentFormat.OpenXml.Wordprocessing.Paragraph CreateParagraph(string text, bool isBold = false, string fontSize="28")
        {
            RunProperties runProperties = new RunProperties();
            runProperties.Append(new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" });
            if (isBold)
            {
                runProperties.Append(new Bold());
            }
            if (!string.IsNullOrEmpty(fontSize))
            {
                runProperties.Append(new FontSize { Val = fontSize });
            }

            DocumentFormat.OpenXml.Wordprocessing.Run run = new DocumentFormat.OpenXml.Wordprocessing.Run();
            run.Append(runProperties, new Text(text) { Space = SpaceProcessingModeValues.Preserve });

            DocumentFormat.OpenXml.Wordprocessing.Paragraph para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
            para.Append(run);

            return para;
        }
        private char GetCyrillicLetter(int number, bool isUppercase)
        {
            if (number < 1 || number > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 32.");
            }

            int baseUnicode = isUppercase ? 0x0410 : 0x0430; // U+0410 for uppercase, U+0430 for lowercase

            // Skip ё and Ё
            if (number == 6)
            {
                baseUnicode += 1; // ё is at position 6
            }
            else if (number > 6)
            {
                baseUnicode += 2;
            }

            return (char)(baseUnicode + number - 1);
        }
        private TableRow CreateTableRow(string field, string value)
        {
            TableRow tableRow = new TableRow();
            tableRow.Append(CreateTableCell(field));
            tableRow.Append(CreateTableCell(value));
            return tableRow;
        }

        private TableCell CreateTableCell(string text)
        {
            TableCell cell = new TableCell();
            TableCellProperties cellProperties = new TableCellProperties();
            string paddingHorizontal = "100";
            string paddingVertical = "30";
            cellProperties.Append(new TableCellMargin
            {
                TopMargin = new TopMargin { Width = paddingVertical, Type = TableWidthUnitValues.Dxa },
                BottomMargin = new BottomMargin { Width = paddingVertical, Type = TableWidthUnitValues.Dxa },
                LeftMargin = new LeftMargin { Width = paddingHorizontal, Type = TableWidthUnitValues.Dxa },
                RightMargin = new RightMargin { Width = paddingHorizontal, Type = TableWidthUnitValues.Dxa }
            });

            cell.Append(cellProperties);

            DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
            DocumentFormat.OpenXml.Wordprocessing.Run run = new DocumentFormat.OpenXml.Wordprocessing.Run();
            run.Append(new RunProperties(new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" }, new FontSize { Val = "28" }), new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            paragraph.Append(run);
            cell.Append(paragraph);

            return cell;
        }
    
    }
}
