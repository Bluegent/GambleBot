namespace SoulCollector.Entities
{

    public interface StatModifier
    {
        int GetValue(int baseValue);
    }
    public class FlatModifier : StatModifier
    {
        private int _value;

        public FlatModifier(int value)
        {
            _value = value;
        }
        public int GetValue(int baseValue)
        {
            return _value;
        }
    }

    public class PercentageModifier : StatModifier
    {
        private int _value;
        public PercentageModifier(float value)
        {
            _value = (int)(value * 1000.0f);
        }

        public int GetValue(int baseValue)
        {
            return (int)(_value / 1000.0f * baseValue);
        }

    }
}