using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NbtConsole.Json
{
    public class NbtTagJsonConverter : JsonConverter<NbtTag>
    {
        public override NbtTag? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, NbtTag value, JsonSerializerOptions options)
        {
            if (value is NbtByte @byte)
                writer.WriteNumberValue(@byte.Value);

            else if (value is NbtShort @short)
                writer.WriteNumberValue(@short.Value);

            else if (value is NbtInt @int)
                writer.WriteNumberValue(@int.Value);

            else if (value is NbtLong @long)
                writer.WriteNumberValue(@long.Value);

            else if (value is NbtFloat @float)
                writer.WriteNumberValue(@float.Value);

            else if (value is NbtDouble @double)
                writer.WriteNumberValue(@double.Value);

            else if (value is NbtString @string)
                writer.WriteStringValue(@string.Value);

            else if (value is NbtList @list)
            {
                writer.WriteStartArray();

                var tags = @list.ToArray();
                foreach (var tag in tags)
                    Write(writer, tag, options);

                writer.WriteEndArray();
            }

            else if (value is NbtByteArray @bytes)
            {
                writer.WriteStartArray();

                foreach (var b in @bytes.Value)
                    writer.WriteNumberValue(b);

                writer.WriteEndArray();
            }

            else if (value is NbtIntArray @ints)
            {
                writer.WriteStartArray();

                foreach (var i in @ints.Value)
                    writer.WriteNumberValue(i);

                writer.WriteEndArray();
            }

            else if (value is NbtLongArray @longs)
            {
                writer.WriteStartArray();

                foreach (var l in @longs.Value)
                    writer.WriteNumberValue(l);

                writer.WriteEndArray();
            }

            else if (value is NbtCompound @compound)
            {
                writer.WriteStartObject();

                foreach (var tag in compound)
                {
                    writer.WritePropertyName(tag.Name);
                    Write(writer, tag, options);
                }

                writer.WriteEndObject();
            }

            else
                throw new InvalidOperationException($"Invalid NBT Tag type: {value.GetType().Name}");
        }
    }
}
