namespace Unosquare.Labs.EmbedIO.Command
{
    using Constants;
    using Swan;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class StaticFilesLiteModule
        : WebModuleBase
    {
        private readonly Lazy<Dictionary<string, string>> _mimeTypes =
            new Lazy<Dictionary<string, string>>(
                () =>
                    new Dictionary<string, string>(MimeTypes.DefaultMimeTypes.Value,
                        StringComparer.InvariantCultureIgnoreCase));

        private const int MaxEntryLength = 50;

        private const int SizeIndent = 20;

        private const string DefaultDocument = "index.html";

        private readonly string _fullPath;
        private readonly string _jsPayload;

        public StaticFilesLiteModule(string fileSystemPath)
        {
            if (!Directory.Exists(fileSystemPath))
                throw new ArgumentException($"Path '{fileSystemPath}' does not exist.");

            _fullPath = Path.GetFullPath(fileSystemPath);
            _jsPayload =
                $"<script>var ws=new WebSocket(\'ws://\'+document.location.hostname+\':{Program.WsPort}/watcher\');ws.onmessage=function(){{document.location.reload()}};</script>";

            AddHandler(ModuleMap.AnyPath, HttpVerbs.Get, HandleGet);
        }

        public override string Name => nameof(StaticFilesLiteModule);

        private static Task<bool> HandleDirectory(IHttpContext context, string localPath, CancellationToken ct)
        {
            var entries = new[] {context.Request.RawUrl == "/" ? string.Empty : "<a href='../'>../</a>"}
                .Concat(
                    Directory.GetDirectories(localPath)
                        .Select(path =>
                        {
                            var name = path.Replace(
                                localPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar,
                                string.Empty);
                            return new
                            {
                                Name = (name + Path.DirectorySeparatorChar).Truncate(MaxEntryLength, "..>"),
                                Url = Uri.EscapeDataString(name) + Path.DirectorySeparatorChar,
                                ModificationTime = new DirectoryInfo(path).LastWriteTimeUtc,
                                Size = "-"
                            };
                        })
                        .OrderBy(x => x.Name)
                        .Union(Directory.GetFiles(localPath, "*", SearchOption.TopDirectoryOnly)
                            .Select(path =>
                            {
                                var fileInfo = new FileInfo(path);
                                var name = Path.GetFileName(path);

                                return new
                                {
                                    Name = name.Truncate(MaxEntryLength, "..>"),
                                    Url = Uri.EscapeDataString(name),
                                    ModificationTime = fileInfo.LastWriteTimeUtc,
                                    Size = fileInfo.Length.FormatBytes()
                                };
                            })
                            .OrderBy(x => x.Name))
                        .Select(y => $"<a href='{y.Url}'>{WebUtility.HtmlEncode(y.Name)}</a>" +
                                     new string(' ', MaxEntryLength - y.Name.Length + 1) +
                                     y.ModificationTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                         CultureInfo.InvariantCulture) +
                                     new string(' ', SizeIndent - y.Size.Length) +
                                     y.Size))
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var content = "<html><head></head><body>{0}</body></html>".Replace(
                "{0}",
                $"<h1>Index of {WebUtility.HtmlEncode(context.RequestPathCaseSensitive())}</h1><hr/><pre>{string.Join("\n", entries)}</pre><hr/>");

            return context.HtmlResponseAsync(content, cancellationToken: ct);
        }

        private Task<bool> HandleGet(IHttpContext context, CancellationToken ct)
        {
            var urlPath = context.Request.Url.LocalPath.Replace('/', Path.DirectorySeparatorChar);
            var basePath = Path.Combine(_fullPath, urlPath.TrimStart(new[] {Path.DirectorySeparatorChar}));

            if (urlPath.Last() == Path.DirectorySeparatorChar)
                urlPath = urlPath + DefaultDocument;

            urlPath = urlPath.TrimStart(new[] {Path.DirectorySeparatorChar});

            var path = Path.Combine(_fullPath, urlPath);

            if (File.Exists(path))
                return HandleFile(context, path, ct);

            return Directory.Exists(basePath) ? HandleDirectory(context, basePath, ct) : Task.FromResult(false);
        }

        private async Task<bool> HandleFile(IHttpContext context, string localPath, CancellationToken ct)
        {
            Stream buffer = null;

            try
            {
                var fileExtension = Path.GetExtension(localPath);

                if (_mimeTypes.Value.ContainsKey(fileExtension))
                    context.Response.ContentType = _mimeTypes.Value[fileExtension];

                buffer = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                if (Path.GetExtension(localPath).StartsWith(".htm"))
                    buffer = WriteJsWebSocket(localPath);

                context.Response.ContentLength64 = buffer.Length;

                await context.Response.WriteToOutputStream(buffer, 0, ct);
            }
            catch (HttpListenerException)
            {
                // Connection error, nothing else to do
            }
            finally
            {
                buffer?.Dispose();
            }

            return true;
        }

        private Stream WriteJsWebSocket(string path)
        {
            var file = File.ReadAllText(path, Encoding.UTF8);

            if (file.IndexOf("</body>", StringComparison.Ordinal) == -1)
                return new MemoryStream(Encoding.UTF8.GetBytes(file));

            var newFile = file.Insert(file.IndexOf("</body>", StringComparison.Ordinal), _jsPayload);

            return new MemoryStream(Encoding.UTF8.GetBytes(newFile));
        }
    }
}