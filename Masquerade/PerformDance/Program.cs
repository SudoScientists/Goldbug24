using System.Text;

class Program 
{

    static void Main(string[] args) 
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: <DancePhraseNoSpaces> <HiddenWord> <FWD/REV>");
            return;
        }

        // Initialise the dance
        Masquerade dance = new Masquerade(args[0], args[1]);

        // Set the initial positions
        dance.CalculateInitialPositions();
        Console.WriteLine("\nINITIAL \n" + dance);

        // "kindly shuffle" run the dance UPTO the 'masked guests' arrive
        dance.KindlyShuffle();
        Console.WriteLine("\nSHUFFLE \n" + dance);

        // "reverie" calculate the 2 person dance values UPTO the character SWAP with the 'masked guests'

        if(args[2] == "FWD")
        {
            // Follow the tutorial where we KNOW the cipher, to generate the ciphertext
            // complete the reverie as described - generate the CIPHERTEXT

        }
        else
        {
            // Do a reverse resolve where we calculate the cipher FROM the ciphertext
            // reverse engineer the cipher - generate the CIPHER
        }
    }
}

class Masquerade 
{
    string dancePhrase;
    string danceCipher;
    char[] dedupedPhrase;
    char[] danceOutput;
    char[] alphabet;
    int[] seatNumbers;
    char[] seatLetters;

    public Masquerade(string phrase, string cipher) 
    {
        this.dancePhrase = phrase; // test phrase - FOURKETTLESWARMKEEPWINTERATBAY
        this.danceCipher = cipher; // test cipher - MYSTERIOUS

        alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        dedupedPhrase = DedupePhrase(this.dancePhrase + new string(this.alphabet)).ToCharArray();

        seatNumbers = new int[26];
        seatLetters = new char[26];
        danceOutput = new char[this.danceCipher.Length];
    }

    public void CalculateInitialPositions()
    {
        for(int i = 0; i < dedupedPhrase.Length; i++)
        {
            seatNumbers[i] = i;
            seatLetters[i] = dedupedPhrase[i];
        }
    }

    public void KindlyShuffle()
    {

    }

    public void Reverie()
    {

    }

    public override string ToString()
    {
        StringBuilder outputString = new StringBuilder();

        outputString.AppendLine($"DEDUPE-{new string(this.dedupedPhrase)} HIDDEN-{this.danceCipher} \n");

        for(int i = 0; i < seatNumbers.Length; i++)
        {
            outputString.Append($"Pos{i}-[Num-{seatNumbers[i]}-Letter-{seatLetters[i]}] \t");
            if((i+1) % 4 == 0) outputString.Append("\n");
        }

        return outputString.ToString();
    }

    private static string DedupePhrase(string phrase)
    {
        var outputChars = new StringBuilder();
        
        // build a frequency map of the characters used in the combined dance phrase
        var charFrequency = new Dictionary<char, int>();

        foreach(var ch in phrase)
        {
            if(char.IsLetter(ch))
            {
                if(charFrequency.ContainsKey(ch))
                {
                    charFrequency[ch]++;
                }
                else
                {
                    charFrequency[ch] = 1;
                    outputChars.Append(ch);
                }
            }
        }

        Console.WriteLine($"Input {phrase} dedupes to {outputChars} length {outputChars.Length}");

        return outputChars.ToString();
    }
}