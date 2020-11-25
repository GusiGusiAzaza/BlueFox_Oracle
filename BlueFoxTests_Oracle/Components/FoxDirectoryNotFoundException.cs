using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace BlueFoxTests_Oracle.Components
{
    [Serializable]
    public class FoxDirectoryNotFoundException : DirectoryNotFoundException
    {
        public readonly List<string> InvalidPaths = new List<string>();

        public FoxDirectoryNotFoundException()
        {
        }

        public FoxDirectoryNotFoundException(string message) : base(message)
        {
        }

        public FoxDirectoryNotFoundException(string message, List<string> paths) : base(message)
        {
            InvalidPaths = paths;
        }

        public FoxDirectoryNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FoxDirectoryNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
