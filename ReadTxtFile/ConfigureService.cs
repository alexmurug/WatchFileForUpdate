using Topshelf;

namespace ReadTxtFile
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<MyService>(service =>
                {
                    service.ConstructUsing(s => new MyService());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.  
                configure.RunAsLocalSystem();
                configure.SetServiceName("NowPlayingService");
                configure.SetDisplayName("NowPlayingService");
                configure.SetDescription("NowPlayingService by Alex M.");
            });
        }
    }
}
