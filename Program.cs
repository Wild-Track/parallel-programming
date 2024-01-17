using System.Diagnostics;

internal class Program
{
    static async Task Main(string[] args)
    {
        Stopwatch timer = new Stopwatch();
        Stopwatch globalTimer = new Stopwatch();
        globalTimer.Start();

        // Step 1
        Console.WriteLine("--------------------");
        Console.WriteLine("STEP 1 :");
        int start = 1;
        int end = 3000;
        long totalSum = 0;

        timer.Start();

        totalSum = ParallelEnumerable.Range(start, end - start + 1).Aggregate(
            totalSum,
            (partialSum, number) => partialSum + number);
        Console.WriteLine("La somme de tous les nombres entre 1 et 3000 est : " + totalSum);

        timer.Stop();
        Console.WriteLine("{0} Elapsed time to process sum of integers", timer.Elapsed.ToString());


        // Step 2 & 3
        Console.WriteLine("--------------------");
        Console.WriteLine("STEP 2 & 3 :");

        List<string> filePaths = new List<string>
        {
            "../../../Eval_file1.txt",
            "../../../Eval_file2.txt"
        };

        Task<int> wordCountTask1 = CountWordsAsync(filePaths[0]);
        Task<int> wordCountTask2 = CountWordsAsync(filePaths[1]);

        Task<int> loremCountTask1 = CountOccurrencesAsync(filePaths[0], "Lorem ipsum");
        Task<int> loremCountTask2 = CountOccurrencesAsync(filePaths[1], "Lorem ipsum");

        await Task.WhenAll(wordCountTask1, wordCountTask2, loremCountTask1, loremCountTask2);

        Console.WriteLine($"Number of words for file 1: {wordCountTask1.Result}");
        Console.WriteLine($"Total occurrences of 'Lorem ipsum' for file 1: {loremCountTask1.Result}");
        Console.WriteLine($"Number of words for file 2: {wordCountTask2.Result}");
        Console.WriteLine($"Total occurrences of 'Lorem ipsum' for file 2: {loremCountTask2.Result}");

        // Final Step
        var finalResult = totalSum + wordCountTask1.Result + loremCountTask1.Result + wordCountTask2.Result + loremCountTask2.Result;
        Console.WriteLine("Final result (sum of all results): {0}", finalResult);

        globalTimer.Stop();
        Console.WriteLine("{0} Elapsed time from begining", globalTimer.Elapsed.ToString());


    }

    static async Task<int> CountWordsAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            int totalWords = 0;
            string[] contents = File.ReadAllLines(filePath);

            Parallel.ForEach(contents, (content) =>
            {
                string[] words = content.Split(' ');
                Interlocked.Add(ref totalWords, words.Length);
            });

            return totalWords;
        });
    }

    static async Task<int> CountOccurrencesAsync(string filePath, string searchTerm)
    {
        return await Task.Run(() =>
        {
            int totalOccurrences = 0;
            string[] contents = File.ReadAllLines(filePath);

            Parallel.ForEach(contents, (content) =>
            {
                Interlocked.Add(ref totalOccurrences, content.Split(new[] { searchTerm }, StringSplitOptions.None).Length - 1);
            });

            return totalOccurrences;
        });
    }

    // Cette version en utilisant les fonction existantes fait la mme chose (par contre il faut change le délimiteur pour pour ' ' pour prendre aussi les sauts de lignes
    //static async Task<int> CountWordsAsync(string filePath)
    //{
    //    var content = await File.ReadAllTextAsync(filePath);
    //    var words = content.Split(" ");
    //    return words.Length;
    //}

    //static async Task<int> CountOccurrencesAsync(string filePath, string searchTerm)
    //{
    //    var content = await File.ReadAllTextAsync(filePath);
    //    return content.Split(new[] { searchTerm }, StringSplitOptions.None).Length - 1;
    //}
}