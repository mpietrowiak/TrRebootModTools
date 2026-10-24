using System;
using System.IO;
using System.Linq;
using System.Threading;
using TrRebootTools.Shared.Cdc;

if (args.Length is < 2 or > 3)
{
    Console.Error.WriteLine("usage: MacAudit <RiseOfTheTombRaiderData> <mod-folder> [--include-mods]");
    return 2;
}

string gameData = Path.GetFullPath(args[0]);
string modFolder = Path.GetFullPath(args[1]);

bool includeMods = args.Length == 3 && args[2] == "--include-mods";
using ArchiveSet archives = ArchiveSet.Open(gameData, includeGame: true, includeMods, CdcGame.Rise);
Console.WriteLine($"Opened {archives.Archives.Count} archive parts and indexed {archives.Files.Count} files.");

ResourceUsageCache cache = new();
cache.AddArchiveSet(archives, null, CancellationToken.None);

int resources = 0;
int matched = 0;
int ignored = 0;
foreach (string file in Directory.EnumerateFiles(modFolder, "*", SearchOption.AllDirectories))
{
    if (!ResourceNaming.TryGetResourceKey(file, out ResourceKey key, CdcGame.Rise))
    {
        if (Path.GetFileName(file) != ".DS_Store")
            Console.WriteLine($"IGNORED  {Path.GetRelativePath(modFolder, file)}");
        ignored++;
        continue;
    }

    resources++;
    int usages = cache.GetResourceUsages(archives, key).Count();
    if (usages > 0)
    {
        matched++;
        ResourceDescriptor? descriptor = cache.GetResourceDescriptor(archives, key);
        string source = descriptor != null ? $", active archive {descriptor.ArchiveId}:{descriptor.ArchiveSubId}" : "";
        Console.WriteLine($"MATCH    {Path.GetRelativePath(modFolder, file)} ({usages} use{(usages == 1 ? "" : "s")}{source})");
    }
    else
    {
        Console.WriteLine($"MISSING  {Path.GetRelativePath(modFolder, file)}");
    }
}

Console.WriteLine($"Result: {matched}/{resources} resources matched; {ignored} non-resource file(s) ignored.");
return matched == resources ? 0 : 1;
