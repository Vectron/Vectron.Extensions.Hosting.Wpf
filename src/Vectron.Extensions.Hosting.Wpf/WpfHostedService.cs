using System.Windows;
using System.Windows.Markup;
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
public sealed class WpfHostedService<TApplication, TMainWindow> : BackgroundService
    where TApplication : Application, IComponentConnector
    where TMainWindow : Window, IComponentConnector
{
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly ILogger<WpfHostedService<TApplication, TMainWindow>> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly ResourceDictionaryOptions settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfHostedService{TApplication, TMainWindow}"/> class.
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> for resolving services.</param>
    /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
    /// <param name="options">Options for configuring the <see cref="ResourceDictionary"/>.</param>
    /// <param name="logger">A <see cref="ILogger"/>.</param>
    public WpfHostedService(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime,
        IOptions<ResourceDictionaryOptions> options,
        ILogger<WpfHostedService<TApplication, TMainWindow>> logger)
    {
        this.serviceProvider = serviceProvider;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.logger = logger;
        settings = options.Value;
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
            SetupResourceDictionary(application);

            _ = application.Run(mainWindow);
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
}
