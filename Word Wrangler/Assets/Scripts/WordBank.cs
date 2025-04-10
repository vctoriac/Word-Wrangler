using System.Collections;
using System.Collections.Generic;

public class WordBank
{
    public static Dictionary<string, List<string>> Words = new Dictionary<string, List<string>>()
    {
        {"fast", new List<string>{ "quick", "speedy" }},
        {"small", new List<string>{ "tiny", "petite", "compact", "miniature" }},
        {"big", new List<string>{ "large", "huge", "gigantic", "massive" }},
        {"clean", new List<string>{ "tidy", "neat", "spotless", "pristine", "sanitary" }},
    };
}
