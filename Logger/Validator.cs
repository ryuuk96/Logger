namespace Logger
{
    public class Validator
    {
        public ValidationResult Validate(object expected, object actual)
        {
            if (!expected.GetType().Name.Equals(actual.GetType().Name))
                return ValidationResult.InvalidCheck;
            return ValidationResult.Passed;
        }

        public enum ValidationResult
        {
            Passed,
            Failed,
            InvalidCheck
        }
    }
}
