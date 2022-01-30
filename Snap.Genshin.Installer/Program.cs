using Microsoft.Win32;
using Snap.Genshin.Installer;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

var desktopRuntimeExsists = false;
var webview2Exsists = false;
Console.WriteLine("欢迎使用SnapGensin! 用户QQ群:910780153");

#region 环境检查

Console.WriteLine("正在执行安装前环境检查:");
Console.WriteLine();

// 检查dotnet6桌面运行时
var process = new Process()
{
    StartInfo = new()
    {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        FileName = "dotnet",
        Arguments = "--list-runtimes"
    }
};
try
{
    process.Start();
    process.WaitForExit();
    var dotnetRuntimeInfo = process.StandardOutput.ReadToEnd();
    desktopRuntimeExsists = dotnetRuntimeInfo.Contains("Microsoft.WindowsDesktop.App 6.0");
}
catch { desktopRuntimeExsists = false; }

// 检查WebView2运行时
var regKey = @"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients";
using (var edgeKey = Registry.LocalMachine.OpenSubKey(regKey))
{
    if (edgeKey is not null)
    {
        var productKeys = edgeKey.GetSubKeyNames();
        if (productKeys is not null && productKeys.Any())
        {
            webview2Exsists = true;
        }
    }
}

Console.Write("dotnet6桌面应用运行环境: \t");
Console.WriteLine(desktopRuntimeExsists ? "已安装" : "未安装");
Console.Write("MS Edge WebView2: \t\t");
Console.WriteLine(webview2Exsists ? "已安装" : "未安装");

#endregion

#region 环境下载及安装
if (!desktopRuntimeExsists)
{
    var progress = new Progress<(long, long)>();
    progress.ProgressChanged += (s, e) =>
    {
        Console.WriteLine($"正在下载dotnet6桌面运行时：{((double)e.Item1 / e.Item2 * 100):0.00}%");
    };
    var runtimePath = await DownloadService.DownloadNet6Async(progress);
    Console.WriteLine("正在安装...");
    var p = Process.Start(runtimePath);
    p.WaitForExit();
    Console.WriteLine("dotnet6运行环境安装" + (p.ExitCode == 0 ? "成功。" : "失败。"));
}
if (!webview2Exsists)
{
    var progress = new Progress<(long, long)>();
    progress.ProgressChanged += (s, e) =>
    {
        Console.WriteLine($"正在下载webview2：{((double)e.Item1 / e.Item2 * 100):0.00}%");
    };
    var runtimePath = await DownloadService.DownloadWebView2Async(progress);
    Console.WriteLine("正在安装...");
    var p = Process.Start(runtimePath);
    p.WaitForExit();
    Console.WriteLine("webview2安装完毕");
}
#endregion

#region 下载最新版本SG
Console.WriteLine("开始下载SnapGenshin...");
var client = new HttpClient(new HttpClientHandler
{
    AllowAutoRedirect = false
});
// 优先fastgit
var fastgit = true;
var networkProblem = false;
HttpResponseMessage? resp = null;
try
{
    resp = await client.GetAsync("https://hub.fastgit.org/DGP-Studio/Snap.Genshin/releases/latest", HttpCompletionOption.ResponseHeadersRead);
    if (resp.StatusCode != System.Net.HttpStatusCode.Redirect) throw new Exception();
}
catch
{
    fastgit = false;
}
// 然后github
if (!fastgit)
{
    try
    {
        resp = await client.GetAsync("https://github.com/DGP-Studio/Snap.Genshin/releases/latest", HttpCompletionOption.ResponseHeadersRead);
        if (resp.StatusCode != System.Net.HttpStatusCode.Redirect) throw new Exception();
    }
    catch
    {
        networkProblem = true;
    }
}
if (networkProblem || resp is null)
{
    Console.WriteLine("检测到网络问题，无法下载SG本体。");
    Console.WriteLine("但安装器已为你准备好所有运行环境，你可以移步QQ群910780153进行本体下载。");
    Console.WriteLine("按回车键结束程序...");
    Console.ReadKey();
    return;
}

var version = Regex.Match(resp.Headers.Location?.ToString() ?? string.Empty, ".+/tag/(.+)").Groups[1].Value;
var sgdnPath = "";
if (fastgit)
{
    var progress = new Progress<(long, long)>();
    progress.ProgressChanged += (s, e) =>
    {
        Console.WriteLine($"正在下载最新版SG：{((double)e.Item1 / e.Item2 * 100):0.00}%");
    };
    sgdnPath = await DownloadService.DownloadSGFromFastGitAsync(version, progress);
}
else
{
    var progress = new Progress<(long, long)>();
    progress.ProgressChanged += (s, e) =>
    {
        Console.WriteLine($"正在下载最新版SG：{((double)e.Item1 / e.Item2 * 100):0.00}%");
    };
    sgdnPath = await DownloadService.DownloadSGFromFastGitAsync(version, progress);

}

if (string.IsNullOrEmpty(sgdnPath))
{
    Console.WriteLine("SG本体下载失败，请移步QQ群:910780153");
    return;
}
#endregion

#region 解压并打开
Console.WriteLine("SnapGenshin将被解压在安装器同级目录，解压完毕后您可以随意移动。");
ZipFile.ExtractToDirectory(sgdnPath, "SnapGenshin", true);
Console.WriteLine("解压完毕");
Console.WriteLine("SnapGenshin安装成功。");

var di = new DirectoryInfo("SnapGenshin");
Process.Start("Explorer.exe", di.FullName);

Console.WriteLine("按回车结束程序...");
Console.ReadKey();
#endregion