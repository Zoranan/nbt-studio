// See https://aka.ms/new-console-template for more information

using fNbt;
using NbtConsole;
using NbtConsole.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

if (args.Length != 1)
{
    Console.Error.WriteLine("Invalid number of args. Specify the file name you want to read");
    return;
}

if (!File.Exists(args[0]))
{
    Console.Error.WriteLine("Specified file does not exist");
    return;
}

NbtFile file = NbtFileLoader.LoadFile(args[0]); // No tag selector currently...

var opts = new JsonSerializerOptions();
opts.Converters.Add(new NbtTagJsonConverter());

var json = JsonSerializer.Serialize(file.RootTag, opts);
Console.WriteLine(json);


