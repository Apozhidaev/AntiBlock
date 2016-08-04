using System;

namespace AntiBlock
{
    public class BlockingException : Exception
    {
        public BlockingException(string domain, string ip)
            : base($"The {domain} have blocked ip address: {ip}")
        {
        }
    }
}