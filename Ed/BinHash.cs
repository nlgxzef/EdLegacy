// Thanks to Kyle Repinski (MWisbest)!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ed
{
    public static class BinHash
    {
        /// <summary>
        /// Calculates BIN hash, which is used at many places in NFS games.
        /// String is converted to a byte array before processing.
        /// </summary>
        public static int Hash(String StringToHash)
        {
           byte[] ByteArrayToHash = Encoding.GetEncoding(1252).GetBytes(StringToHash);

           return Hash(ByteArrayToHash);
        }

        /// <summary>
        /// Calculates BIN hash, which is used at many places in NFS games.
        /// </summary>
        public static int Hash(byte[] ByteArrayToHash)
        {
            int v1 = 0;
            int i = -1;

            while (v1 < ByteArrayToHash.Length)
            {
                i = ByteArrayToHash[v1] + 33 * i;
                v1++;
            }

            return i;
        }
    }
}
