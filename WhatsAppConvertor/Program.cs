using System.IO.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Data;
using WhatsAppConvertor.Exporters;
using WhatsAppConvertor.Models;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<MessageDatabaseOptions>()
            .Bind(context.Configuration.GetSection(MessageDatabaseOptions.Position));
        services.AddOptions<WaDatabaseOptions>()
            .Bind(context.Configuration.GetSection(WaDatabaseOptions.Position));
        services.AddOptions<ExportOptions>()
            .Bind(context.Configuration.GetSection(ExportOptions.Position));

        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IMessageDataRepository, MessageDataRepository>();
        services.AddSingleton<IContactDataRepository, ContactDataRepository>();
        services.AddAutoMapper(typeof(Program));
        services.AddRazorTemplating();

        services.AddTransient<IExporter, HtmlExporter>();
        services.AddTransient<IExporter, TextExporter>();
        services.AddTransient<IExporter, JsonExporter>();
    })
    .Build();

ILogger logger = host.Services.GetRequiredService<ILogger<Program>>();    

IMessageDataRepository messageRepo = host.Services.GetRequiredService<IMessageDataRepository>();
IContactDataRepository contactRepo = host.Services.GetRequiredService<IContactDataRepository>();
IEnumerable<IExporter> exporters = host.Services.GetServices<IExporter>();

IEnumerable<ChatMessage> chats = await messageRepo.GetChats();
IEnumerable<Contact> contacts = await contactRepo.GetContacts();
IDictionary<string, Contact> contactsJidDict = contacts.ToDictionary(c => c.RawStringJid ?? string.Empty);

IList<ChatMessageAndContact> messagesWithContacts = new List<ChatMessageAndContact>();
foreach (ChatMessage chatMessage in chats)
{
    contactsJidDict.TryGetValue(chatMessage.RawStringJid ?? string.Empty, out Contact? contact);
    ChatMessageAndContact message = new()
    {
        Contact = contact,
        ChatMessage = chatMessage
    };

    messagesWithContacts.Add(message);
}

foreach (IExporter exporter in exporters)
{
    try
    {
        await exporter.ExportAsync(chats, contacts, messagesWithContacts);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to export");
    }
}
