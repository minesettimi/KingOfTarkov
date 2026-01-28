using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace KingOfTarkov;

public static class KoTMetadata
{
    public record ModMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.minesettimi.kingoftarkov";
        public override string Name { get; init; } = "King Of Tarkov";
        public override string Author { get; init; } = "minesettimi";
        public override List<string>? Contributors { get; init; }
        public override Version Version { get; init; } = new(0, 9, 0);
        public override Range SptVersion { get; init; } = new("~4.0.4");

        public override List<string>? Incompatibilities { get; init; }
        public override Dictionary<string, Range>? ModDependencies { get; init; }

        public override string? Url { get; init; } = "https://github.com/minesettimi/FleaSimulator";
        public override bool? IsBundleMod { get; init; }
        public override string License { get; init; } = "MIT";
    }
}