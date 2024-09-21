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
        dance.InitialPositions();
        Console.WriteLine("\nINITIAL \n" + dance);

        // "kindly shuffle" run the dance UPTO the 'masked guests' arrive
        dance.KindlyShuffle();
        Console.WriteLine("\nSHUFFLE \n" + dance);

        // "reverie" calculate the 2 person dance values UPTO the character SWAP with the 'masked guests'
        dance.Reverie();
        Console.WriteLine("\nPresolve REVERIE \n" + dance);


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
    int[] reverieOutput;
    char[] danceOutput;
    char[] alphabet;
    int[] seatNumbers;
    char[] seatLetters;

    public Masquerade(string phrase, string cipher) 
    {
        dancePhrase = phrase; // test phrase - FOURKETTLESWARMKEEPWINTERATBAY
        danceCipher = cipher; // test cipher - MYSTERIOUS

        alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        dedupedPhrase = DedupePhrase(dancePhrase + new string(alphabet)).ToCharArray();

        seatNumbers = new int[26];
        seatLetters = new char[26];
        reverieOutput = new int[danceCipher.Length];
        danceOutput = new char[danceCipher.Length];
    }

    public void InitialPositions()
    {
        for(int i = 0; i < dedupedPhrase.Length; i++)
        {
            seatNumbers[i] = i;
            seatLetters[i] = dedupedPhrase[i];
        }
    }

    public void KindlyShuffle()
    {
        int masterDial = 0;

        Console.WriteLine("\nShuffle Instructions\n");

        for(int masterIndex = 0; masterIndex < seatNumbers.Length; masterIndex++)
        {
            // Advance by number on floor
            masterDial += seatNumbers[masterIndex];

            // Advance by alphaindex of the letter
            masterDial += Array.IndexOf(alphabet, seatLetters[masterIndex]);

            // Swap number values  between masterIndex and masterDial(wrapped) - cool tuple syntax
            (seatNumbers[WrappedIndex(masterDial, seatNumbers.Length)], seatNumbers[masterIndex]) = (seatNumbers[masterIndex], seatNumbers[WrappedIndex(masterDial, seatNumbers.Length)]);
            Console.WriteLine($"Position {masterIndex} <--> {WrappedIndex(masterDial, 26)}");
        }

        /*// Wrapping tests
        Console.WriteLine($"Using Len {seatNumbers.Length} Index {0} Corrects to {WrappedIndex(0, seatNumbers.Length)}");
        Console.WriteLine($"Using Len {seatNumbers.Length} Index {12} Corrects to {WrappedIndex(12, seatNumbers.Length)}");
        Console.WriteLine($"Using Len {seatNumbers.Length} Index {25} Corrects to {WrappedIndex(25, seatNumbers.Length)}");
        Console.WriteLine($"Using Len {seatNumbers.Length} Index {26} Corrects to {WrappedIndex(26, seatNumbers.Length)}");
        Console.WriteLine($"Using Len {seatNumbers.Length} Index {30} Corrects to {WrappedIndex(30, seatNumbers.Length)}");
        Console.WriteLine($"Using Len {seatNumbers.Length} Index {52} Corrects to {WrappedIndex(52, seatNumbers.Length)}");
        Console.WriteLine($"Using Len {seatNumbers.Length} Index {-2} Corrects to {WrappedIndex(-2, seatNumbers.Length)}");*/
    }

    public void Reverie()
    {
        Console.WriteLine("\nReverie Instructions\n");

        // Remove characters
        for(int i = 0; i < seatLetters.Length; i++)
        {
            seatLetters[i] = '_';
        }

        int masterDial = 0;

        // Iterate from 1 <-- (the master ignores 0 for some reason)
        // One iteration per char of cipher
        for(int i = 1; i <= danceCipher.Length; i++) //note the 1, so the end conditional needed +1
        {
            // calc new master dial
            masterDial += seatNumbers[i]; //inc by number placard

            // calc dance 'target' value (start at zero increment by dancer 1 number + dancer 2 number)
            int danceEndpoint = WrappedIndex(seatNumbers[i]+seatNumbers[WrappedIndex(masterDial,seatNumbers.Length)], seatNumbers.Length);

            // swap dancers
            (seatNumbers[WrappedIndex(masterDial, seatNumbers.Length)], seatNumbers[i]) = (seatNumbers[i], seatNumbers[WrappedIndex(masterDial, seatNumbers.Length)]);

            // DONT RESOLVE THE ASSISTANT DIAL HERE - cache answer so we can do fwd and reverse lookup later
            reverieOutput[i-1] = seatNumbers[danceEndpoint];
            Console.WriteLine($"Dancers {i} & {WrappedIndex(masterDial,seatNumbers.Length)} --> Seat {danceEndpoint} Val {seatNumbers[danceEndpoint]}");
        }

    }

    public override string ToString()
    {
        StringBuilder outputString = new StringBuilder();

        outputString.AppendLine($"DEDUPE-{new string(dedupedPhrase)} HIDDEN-{danceCipher} \n");

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

    private static int WrappedIndex(int index, int arrayLength)
    {
        // The formula (index % array.Length + array.Length) % array.Length ensures that both positive and negative indices are wrapped correctly.
        // For positive indices, if the index exceeds the array length, it wraps around.
        // For negative indices, it converts the negative index into a positive one by wrapping around in the other direction.
        // The inner index % array.Length handles both positive overflow and negative values, but might leave negatives, 
        // so adding array.Length ensures a positive index, followed by another % array.Length to account for potential overshooting.
        return (index % arrayLength + arrayLength) % arrayLength;
    }
}