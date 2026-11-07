using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using STA.Calcolo.Updates;
internal static class ReleaseChecks
{
    public static async Task Run()
    {
        var payload=Encoding.UTF8.GetBytes("verified package");var hash=Convert.ToHexString(SHA256.HashData(payload));
        var url="https://github.com/studiotassinari/Calcolo/releases/download/v1.1.0/STA-Calcolo-Update-1.1.0-win-x64.zip";
        byte[] Json(string tag="v1.1.0",string? link=null,string? digest=null,bool draft=false)=>JsonSerializer.SerializeToUtf8Bytes(new{tag_name=tag,draft,prerelease=false,assets=new[]{new{name="STA-Calcolo-Update-1.1.0-win-x64.zip",browser_download_url=link??url,size=payload.Length,digest=digest??"sha256:"+hash}}});
        void Reject(byte[] data){try{GitHubReleaseClient.Parse(data,new Version(1,0,0));throw new Exception("Metadata accepted");}catch(InvalidDataException){}}
        if(GitHubReleaseClient.Parse(Json(),new Version(1,1,0))!=null || GitHubReleaseClient.Parse(Json(draft:true),new Version(1,0,0))!=null)throw new Exception("Version selection failed");
        Reject(Json(link:"https://example.test/unsafe.zip"));Reject(Json(digest:"sha256:123"));Reject(Json(tag:"../bad"));
        var package=GitHubReleaseClient.Parse(Json(),new Version(1,0,0))!;
        var path=Path.Combine(Path.GetTempPath(),"calcolo-download-"+Guid.NewGuid().ToString("N")+".zip");
        try
        {
            using var http=new HttpClient(new Handler(payload));var client=new GitHubReleaseClient(http);
            await client.DownloadAsync(package,path);if(File.ReadAllBytes(path).Length!=payload.Length)throw new Exception("Download failed");File.Delete(path);
            try{await client.DownloadAsync(package with {Sha256=new string('0',64)},path);throw new Exception("Bad hash accepted");}catch(InvalidDataException){}File.Delete(path);
            try{await client.DownloadAsync(package with {Size=payload.Length-1},path);throw new Exception("Oversize accepted");}catch(InvalidDataException){}
        }
        finally{if(File.Exists(path))File.Delete(path);}
        Console.WriteLine("PASS: release stabili, versione, provenienza GitHub, download, SHA-256 e dimensione");
    }
    private sealed class Handler(byte[] data):HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)=>Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK){Content=new ByteArrayContent(data),RequestMessage=request});
    }
}

