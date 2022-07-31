using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhatsAppConvertor.Configuration;
using WhatsAppConvertor.Models;

namespace WhatsAppConvertor.Data
{
    public class MessageDataRepository : IMessageDataRepository
    {
        private readonly ILogger<MessageDataRepository> _logger;
        private readonly MessageDatabaseOptions _options;

        public MessageDataRepository(
            ILogger<MessageDataRepository> logger,
            IOptions<MessageDatabaseOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<IEnumerable<ChatMessage>> GetChats(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Attempting to open connection to db at path {Path}", _options.ConnectionString);

            using SqliteConnection connection = new(_options.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            SqliteCommand command = connection.CreateCommand();
            const string commandText =
            @"
                SELECT 
	                cv.raw_string_jid AS RawStringJid,
	                mv._id AS MessageId,
	                mv.sort_id AS MessageSortId,
	                mv.from_me AS MessageFromMe,
	                mv.chat_row_id as ChatId,
	                mv.received_timestamp AS MessageRecievedTime,
	                mv.message_type AS MessageType,
	                mv.text_data AS MessageText,
	                mm.file_path AS FilePath,
	                mt.thumbnail AS Thumbnail
                FROM message_view mv
                LEFT JOIN message_media mm ON mm.message_row_id = mv._id 
                LEFT JOIN chat_view cv ON cv._id = mv.chat_row_id
                LEFT JOIN message_thumbnail mt ON mt.message_row_id = mv._id
            ";

            CommandDefinition commandDefinition = new(commandText, cancellationToken: cancellationToken);

            IEnumerable<ChatMessage> chats = await connection.QueryAsync<ChatMessage>(commandDefinition);

            await connection.CloseAsync();

            _logger.LogInformation("Retrieved {MessageCount} messages from db", chats.Count());

            return chats;
        }
    }
}
