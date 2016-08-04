using System;

namespace AntiBlock
{
    public class ContentException : Exception
    {
        public ContentException(string message)
            : this(message, null, null, null)
        {
        }

        public ContentException(string message, Exception innerException)
            : this(message, null, null, innerException)
        {
        }

        public ContentException(string message, string url, string content)
            : this(message, url, content, null)
        {
        }

        public ContentException(string message, string url, string content, Exception innerException) 
            : base(message, innerException)
        {
            Url = url;
            Content = content;
        }

        public string Url { get; }
        public string Content { get; }
    }
}