using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
namespace STA.Calcolo.Reporting;

public static class PdfReportWriter
{
    private static readonly object Gate=new();
    private static readonly XColor Navy=XColor.FromArgb(0x19,0x28,0x45),Muted=XColor.FromArgb(0x52,0x67,0x78),Pale=XColor.FromArgb(0xEA,0xF0,0xF1),Line=XColor.FromArgb(0xD6,0xE0,0xE2);
    private const double Margin=42.52,Width=510.24,Bottom=790;
    public static byte[] Write(ReportDocument report,byte[] logo,byte[] regularFont,byte[] boldFont)
    {
        lock(Gate)
        {
            GlobalFontSettings.FontResolver ??=new ReportFontResolver(regularFont,boldFont);
            using var document=new PdfDocument();document.Info.Title=report.Title;document.Info.Author="Tassinari Associati";document.Info.Creator="TA-Calcolo";
            using var logoStream=new MemoryStream(logo);using var picture=XImage.FromStream(logoStream);
            for(int index=0;index<report.Pages.Count;index++)
            {
                var content=report.Pages[index];var page=document.AddPage();page.Size=PdfSharp.PageSize.A4;
                using var g=XGraphics.FromPdfPage(page);double y=Margin;
                g.DrawImage(picture,Margin,y,150,150*picture.PixelHeight/picture.PixelWidth);y+=41;
                y+=Text(g,content.Title,Margin,y,Width,17,true,Navy)+6;
                y+=Text(g,report.Title+" · "+report.Created.ToString("dd/MM/yyyy"),Margin,y,Width,9,false,Muted)+14;
                foreach(var table in content.Tables){y+=Text(g,table.Title,Margin,y,Width,12,true,Navy)+8;y=Table(g,table,Margin,y,Width)+16;}
                foreach(var equation in content.Equations??[])
                {
                    y+=Text(g,equation.Title,Margin,y,Width,11,true,Navy)+12;
                    double leftWidth=90, formulaWidth=Width-leftWidth;
                    double numeratorHeight=Text(g,equation.Numerator,Margin+leftWidth,y,formulaWidth,10,false,Navy);
                    Text(g,equation.Left+" =",Margin,y+(equation.Denominator.Length>0?numeratorHeight:0),leftWidth-8,11,true,Navy);
                    y+=numeratorHeight+5;
                    if(equation.Denominator.Length>0){g.DrawLine(new XPen(Navy,.7),Margin+leftWidth,y,Margin+Width,y);y+=5;y+=Text(g,equation.Denominator,Margin+leftWidth,y,formulaWidth,10,false,Navy);}
                    y+=12;y+=Text(g,equation.Explanation,Margin,y,Width,9,false,Muted)+8;
                    if(equation.Substitution.Length>0)y+=Text(g,equation.Substitution,Margin,y,Width,9,true,Navy)+8;
                    y+=14;
                }
                if(content.Cards.Count>0)
                {
                    double cardWidth=(Width-12)/2,highest=0;
                    for(int i=0;i<content.Cards.Count;i++)highest=Math.Max(highest,Card(g,content.Cards[i],Margin+i*(cardWidth+12),y,cardWidth,content.Accent));
                    y+=highest+14;
                }
                foreach(var note in content.Notes){y+=Text(g,note,Margin,y,Width,9,false,Muted)+10;}
                Ensure(y);
                g.DrawLine(new XPen(Line,.5),Margin,805,Margin+Width,805);
                Text(g,$"TA-Calcolo · Tassinari Associati                         Pagina {index+1} di {report.Pages.Count}",Margin,811,Width,8,false,Muted);
            }
            using var output=new MemoryStream();document.Save(output,false);return output.ToArray();
        }
    }
    private static double Card(XGraphics g,ReportCard card,double x,double y,double width,string accent)
    {
        var color=XColor.FromArgb(Convert.ToInt32(accent.Substring(0,2),16),Convert.ToInt32(accent.Substring(2,2),16),Convert.ToInt32(accent.Substring(4,2),16));
        double start=y;g.DrawRectangle(new XSolidBrush(color),x,y,width,23);Text(g,card.Title,x+7,y+5,width-14,11,true,XColors.White);y+=28;
        foreach(var group in card.Groups)
        {
            g.DrawRectangle(new XSolidBrush(Pale),x+5,y,width-10,17);Text(g,group.Title.ToUpperInvariant(),x+9,y+4,width-18,8,true,Navy);y+=20;
            foreach(var row in group.Lines)
            {
                double left=(width-18)*.49,right=(width-18)*.51;
                double height=Math.Max(Text(g,row.Label,x+8,y,left-5,8,false,Muted),Text(g,row.Value,x+8+left,y,right,8.2,false,Navy));y+=height+3;
            }
            y+=5;
        }
        Ensure(y);g.DrawRectangle(new XPen(Line,.6),x,start,width,y-start+3);return y-start+3;
    }
    private static double Table(XGraphics g,ReportTable table,double x,double y,double width)
    {
        int n=table.Headers.Length;double[] widths=Enumerable.Range(0,n).Select(i=>n>2?(i==0?width*2/(n+1):width/(n+1)):width/n).ToArray();int number=0;
        foreach(var row in new[]{table.Headers}.Concat(table.Rows))
        {
            bool header=number==0;double size=8.3;
            double height=row.Select((value,i)=>Lines(g,value,widths[i]-12,new XFont("Arial",size,header?XFontStyleEx.Bold:XFontStyleEx.Regular)).Length*(size*1.25)).Max()+12;
            Ensure(y+height);double xx=x;
            for(int i=0;i<n;i++)
            {
                g.DrawRectangle(new XSolidBrush(header?Navy:number%2==0?Pale:XColors.White),xx,y,widths[i],height);
                Text(g,row[i],xx+6,y+6,widths[i]-12,size,header,header?XColors.White:Navy);xx+=widths[i];
            }
            g.DrawLine(new XPen(Line,.4),x,y+height,x+width,y+height);y+=height;number++;
        }
        return y;
    }
    private static double Text(XGraphics g,string text,double x,double y,double width,double size,bool bold,XColor color)
    {
        var font=new XFont("Arial",size,bold?XFontStyleEx.Bold:XFontStyleEx.Regular);var lines=Lines(g,text,width,font);double leading=size*1.25;
        var small=new XFont("Arial",size*.75,bold?XFontStyleEx.Bold:XFontStyleEx.Regular);
        for(int i=0;i<lines.Length;i++)
        {
            double xx=x;
            foreach(char ch in lines[i])
            {
                int sub="ₜᵧₚₛₓ₀ᵣₑᵤᵢ꜀".IndexOf(ch);bool lowered=sub>=0;
                string glyph=lowered?"typsx0reuic"[sub].ToString():ch.ToString();var face=lowered?small:font;
                g.DrawString(glyph,face,new XSolidBrush(color),new XPoint(xx,y+size+i*leading+(lowered?size*.22:0)));
                xx+=g.MeasureString(glyph,face).Width;
            }
        }
        return lines.Length*leading;
    }
    private static string[] Lines(XGraphics g,string text,double width,XFont font)
    {
        List<string> result=[];string line="";
        foreach(var paragraph in text.Split('\n'))
        {
            foreach(var word in paragraph.Split(' '))
            {
                if(g.MeasureString(MeasureText((line.Length>0?line+" ":"")+word),font).Width<=width){line+=(line.Length>0?" ":"")+word;continue;}
                if(line.Length>0){result.Add(line);line="";}
                foreach(char c in word){if(g.MeasureString(MeasureText(line+c),font).Width>width&&line.Length>0){result.Add(line);line="";}line+=c;}
            }
            result.Add(line);line="";
        }
        return result.ToArray();
    }
    private static string MeasureText(string text)
    {
        const string subs="ₜᵧₚₛₓ₀ᵣₑᵤᵢ꜀",normal="typsx0reuic";
        return new string(text.Select(c=>subs.IndexOf(c) is var i && i>=0?normal[i]:c).ToArray());
    }
    private static void Ensure(double y){if(y>Bottom)throw new InvalidOperationException("La relazione supera l’area A4 disponibile. Ridurre la lunghezza del titolo o dei testi prima di esportare.");}
}
