namespace Infrastructure
{
    public class ScenarioAnswer
    {
        private readonly string _value;

        public string Value { get { return _value; } }

        public ScenarioAnswer(string value)
        {
            _value = value;
        }

        public ScenarioAnswer(IEnumerable<string> pairs)
        {
            _value = string.Join(' ', pairs.Order(StringComparer.OrdinalIgnoreCase).ToArray());
        }

        public bool IsMatch(ScenarioAnswer other)
        {
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }
    }
}
