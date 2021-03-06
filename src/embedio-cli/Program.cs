namespace Unosquare.Labs.EmbedIO.Command
{
    using Swan;
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Entry point
    /// </summary>
    internal class Program
    {
        public static int WsPort { get; set; }

        /// <summary>
        /// Load WebServer instance
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Runtime.WriteWelcomeBanner();

            var options = new Options();

            if (!Runtime.ArgumentParser.ParseArguments(args, options)) return;

            var currentDirectory = Directory.GetCurrentDirectory();

            var url = $"http://localhost:{options.Port}/";
            WsPort = options.Port + 1;

            using var server = new WebServer(url);
            server.WithLocalSession();
                
            // Static files
            if (options.RootPath != null || options.ApiAssemblies == null)
                server.RegisterModule(new StaticFilesLiteModule(options.RootPath ?? SearchForWwwRootFolder(currentDirectory)));

            // Watch Files
            if (!options.NoWatch)
                Watcher.Instance.WatchFiles(options.RootPath ?? SearchForWwwRootFolder(currentDirectory));

            // Assemblies
            $"Registering Assembly {options.ApiAssemblies}".Debug(nameof(Program));
            LoadApi(server, options.ApiAssemblies ?? currentDirectory);

            // start the server
            server.RunAsync();
                
            var browser = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }
            };

            browser.Start();
            "Press any key to stop the server.".Info(nameof(Program));

            Console.ReadKey();
        }

        /// <summary>
        /// Load an Assembly
        /// </summary>
        /// <param name="apiPath"></param>
        /// <param name="server"></param>
        private static void LoadApi(IWebServer server, string apiPath)
        {
            try
            {
                var fullPath = Path.GetFullPath(apiPath);

                if (Path.GetExtension(apiPath).Equals(".dll"))
                {
                    var assembly = Assembly.LoadFile(fullPath);
                    server.LoadApiControllers(assembly).LoadWebSockets(assembly);
                }
                else
                {
                    var files = Directory.GetFiles(fullPath, "*.dll");

                    foreach (var file in files)
                    {
                        var assembly = Assembly.LoadFile(file);
                        server.LoadApiControllers(assembly).LoadWebSockets(assembly);
                    }
                }
            }
            catch (FileNotFoundException fileEx)
            {
                $"Assembly FileNotFoundException {fileEx.Message}".Debug(nameof(Program));
            }
            catch (Exception ex)
            {
                $"Assembly Exception {ex.Message}".Debug(nameof(Program));
                ex.Log(nameof(Program));
            }
        }

        private static string SearchForWwwRootFolder(string rootPath)
        {
            var path = Path.Combine(rootPath, "wwwroot");
            return Directory.Exists(path) ? path : rootPath;
        }
    }
}
