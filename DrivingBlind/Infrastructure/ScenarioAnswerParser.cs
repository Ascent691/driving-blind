namespace Infrastructure;

public class ScenarioAnswerParser
{
    public ScenarioAnswerParser() { }

    public ScenarioAnswer[] Parse(string[] lines)
    {
        var result = new List<ScenarioAnswer>();

        foreach (string line in lines)
        {
            int lineIndex = line.IndexOf(":") + 1;
            result.Add(new ScenarioAnswer(line.Substring(lineIndex).Trim()));
        }

        return result.ToArray();
    }
}
