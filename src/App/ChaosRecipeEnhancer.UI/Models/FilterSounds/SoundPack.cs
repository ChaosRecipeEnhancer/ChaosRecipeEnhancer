using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Models.FilterSounds;

public class SoundPack
{
    public string AuthorName { get; set; }
    public string DisplayName { get; set; }
    public string Category { get; set; }
    public List<SoundEntry> Sounds { get; set; } = new();
}

public class SoundEntry
{
    public string FileName { get; set; }
    public string DisplayName { get; set; }
    public string RelativePath { get; set; }
}
