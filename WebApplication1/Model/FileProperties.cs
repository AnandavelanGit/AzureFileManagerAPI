using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerAPI
{
    public class FileProperties
    {
        public string Container { get; set; }
        public string Name { get; set; }

        public DateTimeOffset ? UploadedDate { get; set; }

        public string UploadedBy { get; set; }

        public long ? Size { get; set; }

    }
}
