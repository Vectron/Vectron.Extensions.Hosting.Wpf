using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vectron.Extensions.Hosting.Wpf;

/// <summary>
/// Extension methods for adding services to an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add a <see cref="DataTemplate"/> to the <see cref="ResourceDictionaryOptions"/>.
    /// </summary>
    /// <param name="options">The <see cref="ResourceDictionaryOptions"/> to add the item to.</param>
    /// <param name="dataTemplate">The <see cref="DataTemplate"/> to add.</param>
    /// <returns>The updated <see cref="ResourceDictionaryOptions"/>.</returns>
    public static ResourceDictionaryOptions Add(this ResourceDictionaryOptions options, DataTemplate dataTemplate)
    {
        options.Entries.Add(dataTemplate.DataTemplateKey, dataTemplate);
        return options;
    }

    /// <summary>
    /// Add a source to an other <see cref="ResourceDictionary"/> to include in this application.
    /// </summary>
    /// <param name="options">The <see cref="ResourceDictionaryOptions"/> to add the item to.</param>
    /// <param name="source">The <see cref="Uri"/> to the <see cref="ResourceDictionary"/> location.</param>
    /// <returns>The updated <see cref="ResourceDictionaryOptions"/>.</returns>
    public static ResourceDictionaryOptions Add(this ResourceDictionaryOptions options, Uri source)
    {
        options.Sources.Add(source);
        return options;
    }

    /// <summary>
    /// Add a <see cref="DataTemplate"/> to the <see cref="ResourceDictionary"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDataTemplateResource<TData, TView>(this IServiceCollection services)
        where TData : class
        where TView : UserControl
    {
        var template = CreateTemplate<TData, TView>();
        _ = services.ConfigureResourceDictionary(options => options.Add(template));
        return services;
    }

    /// <summary>
    /// Add a <see cref="ResourceDictionary"/> to the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="uri">The location of the resource dictionary to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddResourceDictionary(this IServiceCollection services, string uri)
    {
        var creationOptions = new UriCreationOptions()
        {
            DangerousDisablePathAndQueryCanonicalization = false,
        };

        if (Uri.TryCreate(uri, creationOptions, out var result))
        {
            _ = services.ConfigureResourceDictionary(options => options.Add(result));
        }

        return services;
    }

    /// <summary>
    /// Adds a scoped WPF window service of the type specified in <typeparamref name="TWindow"/> to
    /// the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window service to add.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWindowScoped<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        _ = services.AddScoped(WindowFactory<TWindow, TViewModel>);
        _ = services.AddScoped<TViewModel>();
        return services;
    }

    /// <summary>
    /// Adds a singleton WPF window service of the type specified in <typeparamref name="TWindow"/>
    /// to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window service to add.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWindowSingleton<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        _ = services.AddSingleton(WindowFactory<TWindow, TViewModel>);
        _ = services.AddSingleton<TViewModel>();
        return services;
    }

    /// <summary>
    /// Adds a transient WPF window service of the type specified in <typeparamref name="TWindow"/>
    /// to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window service to add.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWindowTransient<TWindow, TViewModel>(this IServiceCollection services)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        _ = services.AddTransient(WindowFactory<TWindow, TViewModel>);
        _ = services.AddTransient<TViewModel>();
        return services;
    }

    /// <summary>
    /// Adds services required for using wpf.
    /// </summary>
    /// <typeparam name="TApplication">The main WPF entry point.</typeparam>
    /// <typeparam name="TMainWindow">The main window to show.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWpf<TApplication, TMainWindow>(this IServiceCollection services)
        where TApplication : Application, IComponentConnector
        where TMainWindow : Window, IComponentConnector
    {
        _ = services.AddHostedService<WpfHostedService<TApplication, TMainWindow>>();
        services.TryAddSingleton<TApplication>();
        services.TryAddSingleton<TMainWindow>();
        return services;
    }

    /// <summary>
    /// Adds services required for using wpf.
    /// </summary>
    /// <typeparam name="TApplication">The main WPF entry point.</typeparam>
    /// <typeparam name="TMainWindow">The main window to show.</typeparam>
    /// <typeparam name="TMainWindowViewModel">The main window view model.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWpf<TApplication, TMainWindow, TMainWindowViewModel>(this IServiceCollection services)
        where TApplication : Application, IComponentConnector
        where TMainWindow : Window, IComponentConnector
        where TMainWindowViewModel : class
    {
        _ = services.AddHostedService<WpfHostedService<TApplication, TMainWindow>>();
        services.TryAddSingleton<TApplication>();
        _ = services.AddWindowSingleton<TMainWindow, TMainWindowViewModel>();
        return services;
    }

    private static IServiceCollection ConfigureResourceDictionary(this IServiceCollection services, Action<ResourceDictionaryOptions> configureOptions)
    {
        _ = services.Configure(configureOptions);
        return services;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "MA0101:String contains an implicit end of line character", Justification = "Idea is to use new lines.")]
    private static DataTemplate CreateTemplate<TData, TView>()
    {
        var dataType = typeof(TData);
        var viewType = typeof(TView);

        var xaml = $$"""
            <DataTemplate DataType="{x:Type vm:{{dataType.Name}}}">
                <v:{{viewType.Name}} />
            </DataTemplate>
            """;

        var context = new ParserContext
        {
            XamlTypeMapper = new XamlTypeMapper(Array.Empty<string>()),
        };
        context.XamlTypeMapper.AddMappingProcessingInstruction("vm", dataType.Namespace, dataType.Assembly.FullName);
        context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

        context.XmlnsDictionary.Add(string.Empty, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
        context.XmlnsDictionary.Add("vm", "vm");
        context.XmlnsDictionary.Add("v", "v");

        var template = (DataTemplate)XamlReader.Parse(xaml, context);
        return template;
    }

    private static TWindow WindowFactory<TWindow, TViewModel>(IServiceProvider serviceProvider)
        where TWindow : Window, IComponentConnector
        where TViewModel : class
    {
        var window = ActivatorUtilities.CreateInstance<TWindow>(serviceProvider);
        window.DataContext = serviceProvider.GetRequiredService<TViewModel>();
        window.InitializeComponent();
        return window;
    }
}
