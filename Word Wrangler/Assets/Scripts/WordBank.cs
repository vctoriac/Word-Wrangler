using System.Collections;
using System.Collections.Generic;

public class WordBank
{
    public static Dictionary<string, List<string>> Words = new Dictionary<string, List<string>>()
    {
        {"fast", new List<string>{ "quick", "speedy", "rapid", "swift", "hasty", "brisk", "prompt" }},
        {"small", new List<string>{ "tiny", "petite", "compact", "miniature", "mini", "little", "minute", "teeny" }},
        {"big", new List<string>{ "large", "huge", "gigantic", "massive", "enormous", "ginormous", "giant", "colossal", "vast", "great" }},
        {"clean", new List<string>{ "tidy", "neat", "spotless", "pristine", "sanitary", "pure", "organize", "organized", "fresh" }},
        {"cold", new List<string>{ "chill", "cool", "icy", "frost", "frosty", "chilly", "freezing", "frigid" }},
        { "sad", new List<string> { "down", "upset", "gloomy", "blue", "depressed", "sorrowful", "melancholy", "downcast", "miserable" }},
        { "smart", new List<string> { "clever", "bright", "sharp", "wise", "intelligent", "brilliant", "savvy", "witty" }}

    };
}
