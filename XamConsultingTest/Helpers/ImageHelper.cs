using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XamConsultingTest.Helpers
{
    public static class ImageHelper
    {
        public static string ToBase64(Stream stream)
        {
            var dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, dataBytes.Length);

            return $"data:image/jpeg;base64,{Convert.ToBase64String(dataBytes)}";
        }
    }
}
