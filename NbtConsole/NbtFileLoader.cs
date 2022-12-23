using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryashtarUtils.Nbt;

namespace NbtConsole
{
    internal static class NbtFileLoader
    {
        internal static NbtFile LoadFile(string path)
        {
            if (TryCreateFromSnbt(path, out NbtFile? file))
                return file;

            if (TryCreateFromNbt(path, NbtCompression.AutoDetect, out file, big_endian: true)) // java files
                return file;

            if (TryCreateFromNbt(path, NbtCompression.AutoDetect, out file, big_endian: false)) // bedrock files
                return file;

            if (TryCreateFromNbt(path, NbtCompression.AutoDetect, out file, big_endian: true, bedrock_header: true)) // bedrock level.dat files
                return file;

            throw new InvalidDataException($"Unable to load file: {path}");
        }

        internal static bool TryCreateFromSnbt(string path, out NbtFile? file)
        {
            try
            {
                file = CreateFromSnbt(path);
                return true;
            }
            catch
            {
                file = null;
                return false;
            }
        }

        private static NbtFile CreateFromSnbt(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                char[] firstchar = new char[1];
                reader.ReadBlock(firstchar, 0, 1);
                if (firstchar[0] != '{') // optimization to not load in huge files
                    throw new FormatException("File did not begin with a '{'");
                var text = firstchar[0] + reader.ReadToEnd();
                var tag = SnbtParser.Parse(text, named: false);
                if (tag is not NbtCompound compound)
                    throw new FormatException("File did not contain an NBT compound");
                compound.Name = "";
                var file = new fNbt.NbtFile(compound);
                return file;
            }
        }

        internal static bool TryCreateFromNbt(string path, NbtCompression compression, out NbtFile? file, bool big_endian = true, bool bedrock_header = false)
        {
            try
            {
                file = CreateFromNbt(path, compression, big_endian, bedrock_header);
                return true;
            }
            catch
            {
                file = null;
                return false;
            }
        }

        private static NbtFile CreateFromNbt(string path, NbtCompression compression, bool big_endian = true, bool bedrock_header = false)
        {
            var file = new fNbt.NbtFile();
            file.BigEndian = big_endian;
            using (var reader = File.OpenRead(path))
            {
                if (bedrock_header)
                {
                    var header = new byte[8];
                    reader.Read(header, 0, header.Length);
                }
                file.LoadFromStream(reader, compression);
            }
            if (file.RootTag is null)
                throw new FormatException("File had no root tag");
            if (file.RootTag is not NbtCompound)
                throw new FormatException("File did not contain an NBT compound");

            return file;
        }
    }
}
