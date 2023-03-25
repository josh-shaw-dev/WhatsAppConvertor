using System;

namespace WhatsAppConvertor.Domain.Dto
{
    public class ChatGroupDto
    {
        static private readonly char[] UnwantedFileSeperators = new char[] { ' ', '\\', '/' };
        private const string FilePathSeperatorReplacement = "-";

        public int ChatId { get; set; }

        public string? DisplayName { get; set; }

        public string FilePath
        {
            get
            {
                string unreplacedFilePath = $"{DisplayName ?? "No display name"}-{ChatId}";
                string[] temp = unreplacedFilePath.Split(UnwantedFileSeperators, StringSplitOptions.RemoveEmptyEntries);

                return string.Join(FilePathSeperatorReplacement, temp);
            }
        }
    }
}