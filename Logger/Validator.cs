using System;

namespace Logger
{
    public class Validator
    {
        public static ValidationResult Validate ( object expected, object actual )
        {
            if (expected == null)
            {
                if (actual == null)
                    return ValidationResult.ValuesNull;
                else return ValidationResult.Failed;
            }
            if (!expected.GetType().Name.Equals(actual.GetType().Name))
                return ValidationResult.InvalidCheck;
            if (!actual.Equals(expected))
                return ValidationResult.Failed;
            return ValidationResult.Passed;
        }

        public enum ValidationResult
        {
            Passed,
            Failed,
            InvalidCheck,
            ValuesNull
        }
    }
}
