public class TextSet
{
    public string[] stuffToSay;

    public string[] sound;

    public int[] speed;

    public int location;

    public bool giveBackControl;

    public string[] portraitNames;

    public string font = "DTM-Mono";

    private bool autoLocation = true;

    public TextSet(string[] stuffToSay, string[] sound, int[] speed, int location, bool giveBackControl, string[] portraitNames)
    {
        this.stuffToSay = stuffToSay;
        this.sound = sound;
        this.speed = speed;
        this.location = location;
        this.giveBackControl = giveBackControl;
        this.portraitNames = portraitNames;
        autoLocation = false;
    }

    public TextSet(string[] stuffToSay, string[] sound, int[] speed, int location, bool giveBackControl, string[] portraitNames, string font)
    {
        this.font = font;
        this.stuffToSay = stuffToSay;
        this.sound = sound;
        this.speed = speed;
        this.location = location;
        this.giveBackControl = giveBackControl;
        this.portraitNames = portraitNames;
        autoLocation = false;
    }

    public TextSet(string[] stuffToSay, string[] sound, int[] speed, int location, bool giveBackControl)
    {
        string[] array = new string[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = null;
        }
        this.stuffToSay = stuffToSay;
        this.sound = sound;
        this.speed = speed;
        this.location = location;
        this.giveBackControl = giveBackControl;
        portraitNames = array;
        autoLocation = false;
    }

    public TextSet(string[] stuffToSay, string sound, int speed, int location, bool giveBackControl)
    {
        string[] array = new string[stuffToSay.Length];
        int[] array2 = new int[stuffToSay.Length];
        string[] array3 = new string[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = sound;
            array2[i] = speed;
        }
        for (int j = 0; j < stuffToSay.Length; j++)
        {
            array3[j] = null;
        }
        this.stuffToSay = stuffToSay;
        this.sound = array;
        this.speed = array2;
        this.location = location;
        this.giveBackControl = giveBackControl;
        portraitNames = array3;
        autoLocation = false;
    }

    public TextSet(string[] stuffToSay, string[] sound, int[] speed, bool giveBackControl)
    {
        string[] array = new string[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = null;
        }
        this.stuffToSay = stuffToSay;
        this.sound = sound;
        this.speed = speed;
        this.giveBackControl = giveBackControl;
        portraitNames = array;
    }

    public TextSet(string[] stuffToSay, string[] sound, int[] speed, string[] portraitNames)
    {
        this.stuffToSay = stuffToSay;
        this.sound = sound;
        this.speed = speed;
        this.portraitNames = portraitNames;
    }

    public TextSet(string[] stuffToSay, string sound, int speed, bool giveBackControl)
    {
        string[] array = new string[stuffToSay.Length];
        int[] array2 = new int[stuffToSay.Length];
        string[] array3 = new string[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = sound;
            array2[i] = speed;
        }
        for (int j = 0; j < stuffToSay.Length; j++)
        {
            array3[j] = null;
        }
        this.stuffToSay = stuffToSay;
        this.sound = array;
        this.speed = array2;
        this.giveBackControl = giveBackControl;
        portraitNames = array3;
    }

    public TextSet(string[] stuffToSay, bool giveBackControl)
    {
        string[] array = new string[stuffToSay.Length];
        int[] array2 = new int[stuffToSay.Length];
        string[] array3 = new string[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = "snd_text";
            array2[i] = 0;
        }
        for (int j = 0; j < stuffToSay.Length; j++)
        {
            array3[j] = null;
        }
        this.stuffToSay = stuffToSay;
        sound = array;
        speed = array2;
        this.giveBackControl = giveBackControl;
        portraitNames = array3;
    }

    public TextSet(string[] stuffToSay)
    {
        string[] array = new string[stuffToSay.Length];
        int[] array2 = new int[stuffToSay.Length];
        string[] array3 = new string[stuffToSay.Length];
        for (int i = 0; i < stuffToSay.Length; i++)
        {
            array[i] = "snd_text";
            array2[i] = 0;
        }
        for (int j = 0; j < stuffToSay.Length; j++)
        {
            array3[j] = null;
        }
        this.stuffToSay = stuffToSay;
        sound = array;
        speed = array2;
        portraitNames = array3;
    }

    public TextSet(string[] text, string[] sounds, int[] speeds, string[] portraits, int pos)
    {
        stuffToSay = text;
        sound = sounds;
        speed = speeds;
        giveBackControl = false;
        portraitNames = portraits;
        location = pos;
        autoLocation = false;
    }
}
