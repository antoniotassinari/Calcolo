using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace STA.Calcolo.Updates;
public sealed class GitHubReleaseClient(HttpClient http)
{
    public const string Repository = "studiotassinari/Calcolo";
    public const string ReleasesUrl = "https://github.com/" + Repository + "/releases/latest";
    public async Task<ReleasePackage?> LatestAsync(Version current, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/" + Repository + "/releases/latest");
        request.Headers.UserAgent.ParseAdd("STA-Calcolo-Updater/1.0");
        using var response = await http.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        if(bytes.Length > 4*1024*1024) throw new InvalidDataException("Risposta GitHub troppo grande.");
        return Parse(bytes, current);
    }
    public static ReleasePackage? Parse(byte[] json, Version current)
    {
        using var doc = JsonDocument.Parse(json); var release = doc.RootElement;
        if(release.GetProperty("draft").GetBoolean() || release.GetProperty("prerelease").GetBoolean()) return null;
        var tag=release.GetProperty("tag_name").GetString() ?? "";
        if(!Regex.IsMatch(tag,@"^v\d+\.\d+\.\d+$") || !Version.TryParse(tag[1..]+".0",out var latest)) throw new InvalidDataException("Versione GitHub non valida.");
        var normalized=new Version(current.Major,current.Minor,Math.Max(0,current.Build),Math.Max(0,current.Revision));
        if(latest<=normalized) return null;
        var version=tag[1..]; var name=$"STA-Calcolo-Update-{version}-win-x64.zip";
        var assets=release.GetProperty("assets").EnumerateArray().Where(a=>a.GetProperty("name").GetString()==name).ToArray();
        if(assets.Length!=1) throw new InvalidDataException("Pacchetto di aggiornamento assente o ambiguo.");
        var asset=assets[0];var url=asset.GetProperty("browser_download_url").GetString()??"";
        var digest=asset.GetProperty("digest").GetString()??"";var size=asset.GetProperty("size").GetInt64();
        if(url!=$"https://github.com/{Repository}/releases/download/{tag}/{name}" || !Regex.IsMatch(digest,"^sha256:[a-fA-F0-9]{64}$") || size<=0 || size>1024L*1024*1024)
            throw new InvalidDataException("Origine, dimensione o impronta del pacchetto non valida.");
        return new(version,url,digest[7..],size);
    }
    public async Task DownloadAsync(ReleasePackage package,string destination,CancellationToken ct=default)
    {
        using var response=await http.GetAsync(package.Url,HttpCompletionOption.ResponseHeadersRead,ct);
        response.EnsureSuccessStatusCode();
        if(response.RequestMessage?.RequestUri?.Scheme!="https")throw new InvalidDataException("Download non cifrato rifiutato.");
        await using(var input=await response.Content.ReadAsStreamAsync(ct))
        await using(var output=new FileStream(destination,FileMode.CreateNew,FileAccess.Write,FileShare.None))
        {
            byte[] buffer=new byte[131072];long total=0;int count;
            while((count=await input.ReadAsync(buffer,ct))>0){total+=count;if(total>package.Size)throw new InvalidDataException("Download oltre la dimensione prevista.");await output.WriteAsync(buffer.AsMemory(0,count),ct);}
            if(total!=package.Size)throw new InvalidDataException("Download incompleto.");
        }
        if(!UpdatePackage.Hash(destination).Equals(package.Sha256,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Impronta SHA-256 errata: aggiornamento rifiutato.");
    }
}
