using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoodTimeStudio.ServerPinger
{
    public class VarintHelper
    {
        public static byte[] IntToVarint(int value)
        {
            List<byte> list = new List<byte>();
            while ((value & 128) != 0)
            {
                list.Add(((byte)(value & 127 | 128)));
                value = (int)((uint)value) >> 7;
            }
            list.Add(((byte)value));
            return list.ToArray<byte>();
        }

        public static int ReadVarInt(BinaryReader reader)
        {
            var s = reader;
            int i = 0;
            int j = 0;

            while (true)
            {
                int k = s.ReadByte();
                i |= (k & 0x7F) << j++ * 7;
                if (j > 5) return 0;
                if ((k & 0x80) != 128) break;
            }
            return i;
        }
    }


}
