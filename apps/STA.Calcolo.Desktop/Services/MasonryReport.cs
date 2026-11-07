using System.Net;
using System.Text;
using STA.Calcolo.Application;
using STA.Muratura.Core;
namespace STA.Calcolo.Desktop.Services;
public static class MasonryReport
{
    public static string Create(MasonryProject p,IReadOnlyList<ResultRow> rows)
    {
        var html=new StringBuilder("<!doctype html><html lang='it'><meta charset='utf-8'><title>Confronto maschi murari</title><style>body{font:14px Arial;margin:35px}td,th{padding:6px;border-bottom:1px solid #ccc}pre{white-space:pre-wrap}tr{break-inside:avoid}</style><h1>");
        html.Append(WebUtility.HtmlEncode(p.Title)).Append("</h1><p>Modello del sito acquisito il 07/11/2026. Somma Vt per fessurazione diagonale; h efficace = h. Telaio: sole colonne, asse forte; esclusi traverso, nodi, ancoraggi, instabilità e interazione N-M. Confronto preliminare, non verifica completa. Profili OPPO del 06/11/2026, dati indicativi.</p><table><tr><th>Grandezza</th><th>Valore</th><th>Unità</th></tr>");
        foreach(var r in rows)html.Append($"<tr><td>{WebUtility.HtmlEncode(r.Grandezza)}</td><td>{r.Valore:0.######}</td><td>{WebUtility.HtmlEncode(r.Unità)}</td></tr>");
        return html.Append("</table><h2>Dati del progetto (unità del modulo)</h2><pre>").Append(WebUtility.HtmlEncode(MasonryFiles.Write(p))).Append("</pre></html>").ToString();
    }
}
