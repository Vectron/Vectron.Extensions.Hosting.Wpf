using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// A hosted service for running a WPF application.
/// </summary>
/// <typeparam name="TApplication">The type of the Wpf application entry point.</typeparam>
/// <typeparam name="TMainWindow">The main window to show.</typeparam>
public sealed class WpfHostedService<TApplication, TMainWindow> : BackgroundService
    where TApplication : Application, IComponentConnector
    where TMainWindow : Window, IComponentConnector
{
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfHostedService{TApplication, TMainWindow}"/> class.
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> for resolving services.</param>
    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
    public WpfHostedService(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
    {
        this.serviceProvider = serviceProvider;
        this.hostApplicationLifetime = hostApplicationLifetime;
    }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskCompletionSource = new TaskCompletionSource();
        var thread = new Thread(() =>
        {
            var application = serviceProvider.GetRequiredService<TApplication>();
            var mainWindow = serviceProvider.GetRequiredService<TMainWindow>();
            using var cancellationTokenRegistration = stoppingToken.Register(application.Shutdown);
            application.InitializeComponent();
            _ = application.Run(mainWindow);
            taskCompletionSource.SetResult();
            hostApplicationLifetime.StopApplication();
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return taskCompletionSource.Task;
    }
}
