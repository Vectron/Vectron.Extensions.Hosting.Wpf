using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// A hosted service for running a WPF application.
/// </summary>
/// <typeparam name="TApplication">The type of the Wpf application entry point.</typeparam>
/// <typeparam name="TMainWindow">The main window to show.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="WpfHostedService{TApplication, TMainWindow}"/> class.
/// </remarks>
/// <param name="serviceProvider">A <see cref="IServiceProvider"/> for resolving services.</param>
/// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
/// <param name="options">Options for configuring the <see cref="ResourceDictionary"/>.</param>
/// <param name="logger">A <see cref="ILogger"/>.</param>
public sealed class WpfHostedService<TApplication, TMainWindow>(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    IOptions<ResourceDictionaryOptions> options,
    ILogger<WpfHostedService<TApplication, TMainWindow>> logger) : BackgroundService
    where TApplication : Application, IComponentConnector
    where TMainWindow : Window, IComponentConnector
{
    private readonly ResourceDictionaryOptions settings = options.Value;

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskCompletionSource = new TaskCompletionSource();
        var thread = new Thread(() =>
        {
            var application = serviceProvider.GetRequiredService<TApplication>();
            using var cancellationTokenRegistration = stoppingToken.Register(
                () => _ = application.Dispatcher.InvokeAsync(application.Shutdown, DispatcherPriority.Send, CancellationToken.None));
            application.InitializeComponent();
            SetupResourceDictionary(application);
            _ = application.Dispatcher.InvokeAsync(StartMainWindow, DispatcherPriority.Send, CancellationToken.None);
            _ = application.Run();
            taskCompletionSource.SetResult();
            hostApplicationLifetime.StopApplication();
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return taskCompletionSource.Task;
    }

    private void SetupResourceDictionary(TApplication application)
    {
        foreach (var dataTemplateDescriptor in settings.DataTemplates)
        {
            try
            {
                var dataTemplate = DataTemplateUtilities.CreateDataTemplate(dataTemplateDescriptor.DataType, dataTemplateDescriptor.ViewType);
                application.Resources.Add(dataTemplate.DataTemplateKey, dataTemplate);
            }
            catch (Exception ex)
            {
                logger.FailedToAddKeyToResourceDictionary(dataTemplateDescriptor, ex);
            }
        }

        foreach (var source in settings.Sources)
        {
            if (!Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out var uri))
            {
                logger.InvalidUriFormat(source);
                continue;
            }

            application.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        }
    }

    private void StartMainWindow()
    {
        var mainWindow = serviceProvider.GetRequiredService<TMainWindow>();
        Application.Current.MainWindow = mainWindow;
        if (mainWindow.Visibility != Visibility.Visible)
        {
            mainWindow.Show();
        }
    }
}
