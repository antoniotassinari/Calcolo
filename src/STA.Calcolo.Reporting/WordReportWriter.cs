using System.IO.Compression;
using System.Security;
using System.Text;
namespace STA.Calcolo.Reporting;

public static class WordReportWriter
{
    private const string W="http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    private static string E(string s)=>SecurityElement.Escape(s)??"";
    private static string Run(string s,int size=18,bool bold=false,string color="192845")=>$"<w:r><w:rPr><w:rFonts w:ascii=\"Arial\" w:hAnsi=\"Arial\"/><w:sz w:val=\"{size}\"/>{(bold?"<w:b/>":"")}<w:color w:val=\"{color}\"/></w:rPr><w:t xml:space=\"preserve\">{E(s)}</w:t></w:r>";
    private static string P(string text,int size=18,bool bold=false,string background="",string color="192845",int after=60)=>$"<w:p><w:pPr><w:spacing w:after=\"{after}\" w:line=\"240\" w:lineRule=\"auto\"/>{(background.Length>0?$"<w:shd w:fill=\"{background}\"/>":"")}</w:pPr>{Run(text,size,bold,color)}</w:p>";
    public static byte[] Write(ReportDocument document,byte[] logo)
    {
        using var memory=new MemoryStream();
        using(var zip=new ZipArchive(memory,ZipArchiveMode.Create,true))
        {
            void Entry(string name,string text){using var writer=new StreamWriter(zip.CreateEntry(name).Open(),new UTF8Encoding(false));writer.Write(text);}
            Entry("[Content_Types].xml","<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Default Extension=\"png\" ContentType=\"image/png\"/><Override PartName=\"/word/document.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"/><Override PartName=\"/word/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml\"/><Override PartName=\"/word/footer.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml\"/></Types>");
            Entry("_rels/.rels","<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rDoc\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"word/document.xml\"/></Relationships>");
            Entry("word/_rels/document.xml.rels","<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rLogo\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" Target=\"media/logo.png\"/><Relationship Id=\"rStyles\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/><Relationship Id=\"rFooter\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer\" Target=\"footer.xml\"/></Relationships>");
            using(var stream=zip.CreateEntry("word/media/logo.png").Open())stream.Write(logo);
            Entry("word/styles.xml",$"<w:styles xmlns:w=\"{W}\"><w:docDefaults><w:rPrDefault><w:rPr><w:rFonts w:ascii=\"Arial\" w:hAnsi=\"Arial\"/><w:sz w:val=\"18\"/><w:lang w:val=\"it-IT\"/></w:rPr></w:rPrDefault></w:docDefaults><w:style w:type=\"paragraph\" w:styleId=\"Title\"><w:name w:val=\"Title\"/><w:rPr><w:b/><w:sz w:val=\"32\"/><w:color w:val=\"192845\"/></w:rPr></w:style></w:styles>");
            Entry("word/footer.xml",$"<w:ftr xmlns:w=\"{W}\"><w:p><w:pPr><w:jc w:val=\"right\"/></w:pPr>{Run("TA-Calcolo · Tassinari Associati · pagina ",16,false,"526778")}<w:fldSimple w:instr=\"PAGE\"/></w:p></w:ftr>");
            var body=new StringBuilder();int index=0;
            foreach(var page in document.Pages)
            {
                if(index>0)body.Append("<w:p><w:r><w:br w:type=\"page\"/></w:r></w:p>");
                body.Append(Logo(++index));
                body.Append($"<w:p><w:pPr><w:pStyle w:val=\"Title\"/><w:spacing w:after=\"80\"/></w:pPr>{Run(page.Title,30,true)}</w:p>");
                body.Append(P(document.Title+" · "+document.Created.ToString("dd/MM/yyyy"),17,false,"","526778",100));
                foreach(var table in page.Tables){body.Append(P(table.Title,22,true,"","192845",80));body.Append(Table(table));body.Append(P("",4,false,"","192845",100));}
                foreach(var equation in page.Equations??[])
                {
                    body.Append(P(equation.Title,22,true,"EAF0F1"));
                    string MathRun(string value)=>"<m:r><m:t>"+E(value)+"</m:t></m:r>";
                    string expression=equation.Denominator.Length==0?MathRun(equation.Numerator):"<m:f><m:num>"+MathRun(equation.Numerator)+"</m:num><m:den>"+MathRun(equation.Denominator)+"</m:den></m:f>";
                    body.Append("<w:p><m:oMathPara xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\"><m:oMath>"+MathRun(equation.Left+" = ")+expression+"</m:oMath></m:oMathPara></w:p>");
                    body.Append(P(equation.Explanation,18));
                    if(equation.Substitution.Length>0)body.Append(P(equation.Substitution,18,true));
                    body.Append(P("",8));
                }
                if(page.Cards.Count>0)
                {
                    body.Append("<w:tbl><w:tblPr><w:tblW w:w=\"10206\" w:type=\"dxa\"/><w:tblLayout w:type=\"fixed\"/><w:tblCellMar><w:top w:w=\"100\" w:type=\"dxa\"/><w:left w:w=\"100\" w:type=\"dxa\"/><w:bottom w:w=\"100\" w:type=\"dxa\"/><w:right w:w=\"100\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr><w:tblGrid><w:gridCol w:w=\"5103\"/><w:gridCol w:w=\"5103\"/></w:tblGrid><w:tr><w:trPr><w:cantSplit/></w:trPr>");
                    foreach(var card in page.Cards)
                    {
                        body.Append("<w:tc><w:tcPr><w:tcW w:w=\"5103\" w:type=\"dxa\"/><w:tcBorders><w:top w:val=\"single\" w:sz=\"4\" w:color=\"D6E0E2\"/><w:left w:val=\"single\" w:sz=\"4\" w:color=\"D6E0E2\"/><w:bottom w:val=\"single\" w:sz=\"4\" w:color=\"D6E0E2\"/><w:right w:val=\"single\" w:sz=\"4\" w:color=\"D6E0E2\"/></w:tcBorders><w:vAlign w:val=\"top\"/></w:tcPr>");
                        body.Append(P(card.Title,23,true,page.Accent,"FFFFFF",80));
                        foreach(var group in card.Groups)
                        {
                            body.Append(P(group.Title.ToUpperInvariant(),16,true,"EAF0F1","192845",40));
                            body.Append(Table(new("",[],group.Lines.Select(l=>new[]{l.Label,l.Value}).ToArray()),4903,true));
                            body.Append(P("",2,false,"","192845",30));
                        }
                        body.Append("<w:p/></w:tc>");
                    }
                    if(page.Cards.Count==1)body.Append("<w:tc><w:tcPr><w:tcW w:w=\"5103\" w:type=\"dxa\"/></w:tcPr><w:p/></w:tc>");
                    body.Append("</w:tr></w:tbl>");
                }
                foreach(var note in page.Notes)body.Append(P(note,18,false,"","526778",120));
            }
            body.Append("<w:sectPr><w:footerReference w:type=\"default\" r:id=\"rFooter\"/><w:pgSz w:w=\"11906\" w:h=\"16838\"/><w:pgMar w:top=\"850\" w:right=\"850\" w:bottom=\"850\" w:left=\"850\" w:header=\"350\" w:footer=\"350\"/></w:sectPr>");
            Entry("word/document.xml",$"<w:document xmlns:w=\"{W}\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><w:body>{body}</w:body></w:document>");
        }
        return memory.ToArray();
    }
    private static string Table(ReportTable table,int width=10206,bool compact=false)
    {
        int columns=table.Headers.Length>0?table.Headers.Length:table.Rows[0].Length;int[] widths=Enumerable.Range(0,columns).Select(i=>columns>2?(i==0?width*2/(columns+1):width/(columns+1)):width/columns).ToArray();
        var s=new StringBuilder($"<w:tbl><w:tblPr><w:tblW w:w=\"{width}\" w:type=\"dxa\"/><w:tblLayout w:type=\"fixed\"/><w:tblCellMar><w:top w:w=\"{(compact?10:60)}\" w:type=\"dxa\"/><w:bottom w:w=\"{(compact?10:60)}\" w:type=\"dxa\"/><w:left w:w=\"45\" w:type=\"dxa\"/><w:right w:w=\"45\" w:type=\"dxa\"/></w:tblCellMar></w:tblPr><w:tblGrid>");
        foreach(int w in widths)s.Append($"<w:gridCol w:w=\"{w}\"/>");s.Append("</w:tblGrid>");
        var rows=table.Headers.Length>0?new[]{table.Headers}.Concat(table.Rows):table.Rows;int rowIndex=0;
        foreach(var row in rows)
        {
            bool header=table.Headers.Length>0&&rowIndex==0;s.Append("<w:tr><w:trPr><w:cantSplit/>");if(header)s.Append("<w:tblHeader/>");s.Append("</w:trPr>");
            for(int i=0;i<row.Length;i++)s.Append($"<w:tc><w:tcPr><w:tcW w:w=\"{widths[i]}\" w:type=\"dxa\"/><w:shd w:fill=\"{(header?"192845":compact?"FFFFFF":rowIndex%2==0?"F3F6F6":"FFFFFF")}\"/></w:tcPr>{P(row[i],compact?16:17,header,"",header?"FFFFFF":"192845",0)}</w:tc>");
            s.Append("</w:tr>");rowIndex++;
        }
        return s.Append("</w:tbl>").ToString();
    }
    private static string Logo(int id)=>$"<w:p><w:pPr><w:spacing w:after=\"100\"/></w:pPr><w:r><w:drawing><wp:inline xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\"><wp:extent cx=\"2000000\" cy=\"427700\"/><wp:docPr id=\"{id}\" name=\"Tassinari Associati\"/><a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\"><pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\"><pic:nvPicPr><pic:cNvPr id=\"{id}\" name=\"Logo.png\"/><pic:cNvPicPr/></pic:nvPicPr><pic:blipFill><a:blip r:embed=\"rLogo\"/><a:stretch><a:fillRect/></a:stretch></pic:blipFill><pic:spPr><a:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"2000000\" cy=\"427700\"/></a:xfrm><a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom></pic:spPr></pic:pic></a:graphicData></a:graphic></wp:inline></w:drawing></w:r></w:p>";
}
