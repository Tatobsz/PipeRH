using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Pipe.RH.Application.Services
{
    public class HoleriteService
    {
        public void ExtrairTextoPDF(string caminho)
        {
            List<String> Holerites = new List<string>();

            using (PdfReader pdf = new PdfReader(caminho + "Holerites - Modelo.pdf"))
            {
                for (int numeroPagina = 1; numeroPagina <= pdf.NumberOfPages; numeroPagina++)
                {
                    var conteudoDaPagina = pdf.GetPageContent(numeroPagina);
                    var pagina = PdfTextExtractor.GetTextFromPage(pdf, numeroPagina);

                    var ocorrenciaAtual = 1;
                    var tentativas = 1;

                    String linhaDoNome = null;

                    while (tentativas != 6)
                    {
                        if (tentativas > 1)
                            ocorrenciaAtual = pagina.IndexOf("\n", ocorrenciaAtual + 2);

                        else
                            ocorrenciaAtual = pagina.IndexOf("\n", ocorrenciaAtual);

                        var stringAtual = pagina.Substring(ocorrenciaAtual, pagina.IndexOf("\n", ocorrenciaAtual + 2));
                        tentativas++;
                        linhaDoNome = stringAtual;
                    }

                    var index = linhaDoNome.IndexOf("\n", 2);
                    linhaDoNome = linhaDoNome.Substring(1, index - 2);

                    Regex regex = new Regex("[0-9]");
                    String nome = regex.Replace(linhaDoNome, "").TrimEnd().TrimStart();

                    Holerites.Add(pagina);

                    Document doc = new Document(PageSize.A4);
                    doc.AddCreationDate();

                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream($"{caminho}Holerites.{nome.Trim()}.pdf", FileMode.Create));

                    doc.Open();

                    doc.Add(new Paragraph("   "));
                    doc.Close();

                    using (var stream = new FileStream($"{caminho}Holerites.{nome.Trim()}.pdf", FileMode.Append))
                    {
                        stream.Write(conteudoDaPagina, 0, conteudoDaPagina.Length);
                    }
                }
            }
        }
    }
}
