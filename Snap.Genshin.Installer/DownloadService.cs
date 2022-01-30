using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Genshin.Installer
{
    internal static class DownloadService
    {
        private static readonly HttpClient client = new ();
        public static async Task<string> DownloadNet6Async(IProgress<(long, long)> progress)
        {
            var result = await client.GetAsync("https://download.visualstudio.microsoft.com/download/pr/bf058765-6f71-4971-aee1-15229d8bfb3e/c3366e6b74bec066487cd643f915274d/windowsdesktop-runtime-6.0.1-win-x64.exe",
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();

            var tempPath = Path.GetTempPath();
            var filePath = Path.Combine(tempPath, "dotnet6.exe");
            using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192);

            var totalBytes = result.Content.Headers.ContentLength ?? throw new HttpRequestException("no content");
            var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[8192];

            var totalReadBytes = 0L;
            var reportBytes = 0L;
            while (totalReadBytes < totalBytes)
            {
                var readBytes = await stream.ReadAsync(buffer).ConfigureAwait(false);
                totalReadBytes += readBytes;
                reportBytes += readBytes;
                file.Write(buffer, 0, readBytes);
                if (reportBytes >= totalBytes / 100 || totalReadBytes == totalBytes)
                {
                    progress.Report(new(totalReadBytes, totalBytes!));
                    reportBytes = 0;
                }
            }

            file.Flush();
            file.Close();

            return filePath;
        }

        public static async Task<string> DownloadWebView2Async(IProgress<(long, long)> progress)
        {
            var result = await client.GetAsync("https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();

            var tempPath = Path.GetTempPath();
            var filePath = Path.Combine(tempPath, "mswebview2.exe");
            using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192);

            var totalBytes = result.Content.Headers.ContentLength ?? throw new HttpRequestException("no content");
            var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[8192];

            var totalReadBytes = 0L;
            var reportBytes = 0L;
            while (totalReadBytes < totalBytes)
            {
                var readBytes = await stream.ReadAsync(buffer).ConfigureAwait(false);
                totalReadBytes += readBytes;
                reportBytes += readBytes;
                file.Write(buffer, 0, readBytes);
                if (reportBytes >= totalBytes / 100 || totalReadBytes == totalBytes)
                {
                    progress.Report(new(totalReadBytes, totalBytes!));
                    reportBytes = 0;
                }
            }

            file.Flush();
            file.Close();

            return filePath;
        }

        public static async Task<string> DownloadSGFromFastGitAsync(string version, IProgress<(long, long)> progress)
        {
            var result = await client.GetAsync($"https://download.fastgit.org/DGP-Studio/Snap.Genshin/releases/download/{version}/Publish.zip",
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();

            var tempPath = Path.GetTempPath();
            var filePath = Path.Combine(tempPath, $"SnapGenshin_build_{version}.zip");
            using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192);

            var totalBytes = result.Content.Headers.ContentLength ?? throw new HttpRequestException("no content");
            var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[8192];

            var totalReadBytes = 0L;
            var reportBytes = 0L;
            while (totalReadBytes < totalBytes)
            {
                var readBytes = await stream.ReadAsync(buffer).ConfigureAwait(false);
                totalReadBytes += readBytes;
                reportBytes += readBytes;
                file.Write(buffer, 0, readBytes);
                if (reportBytes >= totalBytes / 100 || totalReadBytes == totalBytes)
                {
                    progress.Report(new(totalReadBytes, totalBytes!));
                    reportBytes = 0;
                }
            }

            file.Flush();
            file.Close();

            return filePath;
        }

        public static async Task<string> DownloadSGFromGithubAsync(string version, IProgress<(long, long)> progress)
        {
            var result = await client.GetAsync($"https://github.com/DGP-Studio/Snap.Genshin/releases/download/{version}/Publish.zip",
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            result.EnsureSuccessStatusCode();

            var tempPath = Path.GetTempPath();
            var filePath = Path.Combine(tempPath, $"SnapGenshin_build_{version}.zip");
            using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192);

            var totalBytes = result.Content.Headers.ContentLength ?? throw new HttpRequestException("no content");
            var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[8192];

            var totalReadBytes = 0L;
            var reportBytes = 0L;
            while (totalReadBytes < totalBytes)
            {
                var readBytes = await stream.ReadAsync(buffer).ConfigureAwait(false);
                totalReadBytes += readBytes;
                reportBytes += readBytes;
                file.Write(buffer, 0, readBytes);
                if (reportBytes >= totalBytes / 100 || totalReadBytes == totalBytes)
                {
                    progress.Report(new(totalReadBytes, totalBytes!));
                    reportBytes = 0;
                }
            }

            file.Flush();
            file.Close();

            return filePath;
        }
    }
}
