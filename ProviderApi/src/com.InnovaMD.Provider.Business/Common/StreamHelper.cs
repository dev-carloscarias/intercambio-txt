using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Business.Common
{
    public static class StreamHelper
    {

        public static byte[] ReadAllBytes(this Stream inStream)
        {
            if(inStream is MemoryStream inMemoryStream)
            {
                return inMemoryStream.ToArray();
            }

            using (var outStream = new MemoryStream())
            {
                inStream.CopyTo(outStream);
                return outStream.ToArray();
            }
        }
    }
}
