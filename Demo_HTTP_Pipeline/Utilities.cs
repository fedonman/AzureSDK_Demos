using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_HTTP_Pipeline
{
    public static class Utilities
    {
        const string SampleFileContent = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras dolor purus, interdum in turpis ut, ultrices ornare augue. Donec mollis varius sem, et mattis ex gravida eget. Duis nibh magna, ultrices a nisi quis, pretium tristique ligula. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Vestibulum in dui arcu. Nunc at orci volutpat, elementum magna eget, pellentesque sem. Etiam id placerat nibh. Vestibulum varius at elit ut mattis.  Suspendisse ipsum sem, placerat id blandit ac, cursus eget purus. Vestibulum pretium ante eu augue aliquam, ultrices fermentum nibh condimentum. Pellentesque pulvinar feugiat augue vel accumsan. Nulla imperdiet viverra nibh quis rhoncus. Nunc tincidunt sollicitudin urna, eu efficitur elit gravida ut. Quisque eget urna convallis, commodo diam eu, pretium erat. Nullam quis magna a dolor ullamcorper malesuada. Donec bibendum sem lectus, sit amet faucibus nisi sodales eget. Integer lobortis lacus et volutpat dignissim. Suspendisse cras amet.";

        public static string RandomName(string prefix = "itprodev-demo") => $"{prefix}-{Guid.NewGuid()}";

        public static string CreateTempPath(string extension = ".txt") => Path.ChangeExtension(Path.GetTempFileName(), extension);

        public static string CreateTempFile(string content = SampleFileContent)
        {
            string path = CreateTempPath();
            File.WriteAllText(path, content);
            return path;
        }

        public static void DeleteFile(string path) => File.Delete(path);
    }
}
