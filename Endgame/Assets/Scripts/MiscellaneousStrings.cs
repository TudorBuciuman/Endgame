using System.Collections.Generic;

public class MiscellaneousStrings : TranslatableBehaviour
{
    public override Dictionary<string, string[]> GetDefaultStrings()
    {
        Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
        dictionary.Add("napsta_approach_hardmode", new string[1] { "* Here comes Napstablook.^05\n* Same as usual." });
        dictionary.Add("default_enemy_approach", new string[1] { "* Enemy approaches!" });
        return dictionary;
    }

    private void Start()
    {
        SetStrings(GetDefaultStrings(), GetType());
    }
}
