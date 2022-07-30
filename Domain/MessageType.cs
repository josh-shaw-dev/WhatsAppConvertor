namespace WhatsAppConvertor.Models
{
    /*
     * Reverse engineered from the message_view table
     *  SELECT DISTINCT(message_type)
     *  FROM message_view;
     */

    public enum MessageType
    {
        Text = 0,
        Image = 1,
        Video = 3,
        Contact = 4,
        Document = 9,
        Gif = 13
    }
}
