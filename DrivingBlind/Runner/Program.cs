using Infrastructure;
using System.Diagnostics;
using System.Text;

namespace Runner;

internal class Program
{
    static void Main(string[] args)
    {
        var timer = Stopwatch.StartNew();
        var scenarios = new CoolerScenarioParser().Parse(File.ReadAllLines("1.in"));
        var expectedAnswers = new ScenarioAnswerParser().Parse(File.ReadAllLines("1.ans"));

        if (scenarios.Length != expectedAnswers.Length)
        {
            Console.WriteLine("We have a different number of answers compared to questions, are you using the right combination of scenarios and answers files?");
            return;
        }


        var failedScenarios = 0;
        var builder = new StringBuilder();

        for (int i = 12; i < 13; i++)
        {
            Console.WriteLine("Case: " + (i + 1) + "\n---------");

            var scenario = scenarios[i];
            var expectedAnswer = expectedAnswers[i];
            var computedAnswer = DetermineAnswer(scenario);

            if (!expectedAnswer.IsMatch(computedAnswer))
            {
                failedScenarios++;
                builder.AppendLine($"Case #{i + 1}: No Match, expected {expectedAnswer.Value} but computed {computedAnswer.Value}");
            }
            else
            {
                builder.AppendLine($"Case #{i + 1}: {computedAnswer.Value}");
            }
        }

        Console.Write(builder.ToString());

        Console.WriteLine($"Total Failed Scenarios: {failedScenarios}");
        Console.WriteLine($"Total Passed Scenarios: {scenarios.Length - failedScenarios}");
        timer.Stop();
        Console.WriteLine($"Execution Time: {timer.Elapsed}");
        Console.ReadLine();
    }

    private static ScenarioAnswer DetermineAnswer(CoolerScenario scenario)
    {
        var p = PairFinder2.Calculate(scenario);
        return p;
    }
}