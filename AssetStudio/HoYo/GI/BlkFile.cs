using System;
using System.Collections.Generic;
using System.IO;

namespace AssetStudio
{
    public class BlkFile
    {
        public Dictionary<long, StreamFile[]> Bundles = new Dictionary<long, StreamFile[]>();
        public BlkFile(FileReader reader)
        {
            reader.Endian = EndianType.LittleEndian;

            var magic = reader.ReadStringToNull();
            if (magic != "blk")
                throw new Exception("not a blk");

            var count = reader.ReadInt32();
            var key = reader.ReadBytes(count);
            reader.ReadBytes(count);

            var blockSize = reader.ReadUInt16();
            var data = reader.ReadBytes((int)(reader.Length - reader.Position));

            data = Crypto.Decrypt(key, data, blockSize);

            using (var ms = new MemoryStream(data))
            using (var subReader = new EndianBinaryReader(ms, reader.Endian))
            {
                long pos = -1;
                try
                {
                    if (reader.BundlePos.Length != 0)
                    {
                        for (int i = 0; i < reader.BundlePos.Length; i++)
                        {
                            pos = reader.BundlePos[i];
                            subReader.Position = pos;
                            var mhy0 = new Mhy0File(subReader, reader.FullPath);
                            Bundles.Add(pos, mhy0.FileList);
                        }
                    }
                    else
                    {
                        while (subReader.Position != subReader.BaseStream.Length)
                        {
                            pos = subReader.Position;
                            var mhy0 = new Mhy0File(subReader, reader.FullPath);
                            Bundles.Add(pos, mhy0.FileList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load a mhy0 at {string.Format("0x{0:x8}", pos)} in {Path.GetFileName(reader.FullPath)}");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
