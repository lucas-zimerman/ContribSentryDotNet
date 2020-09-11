using System;
using System.Collections.Generic;
using System.IO;

namespace ContribSentry.Testing
{
    public class TempFolder : IDisposable
    {
        public string FolderName;

        public TempFolder()
        {
            FolderName = Guid.NewGuid().ToString();
            Directory.CreateDirectory($"./{FolderName}");
        }

        public void Dispose()
        {
            var files = Directory.EnumerateFiles($"./{FolderName}");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
            Directory.Delete($"./{FolderName}");
        }
    }
}
