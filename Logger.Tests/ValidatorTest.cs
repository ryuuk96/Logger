using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests
{
    /// <summary>
    /// The test has been checked for Type name and int/float/string/decimal values being different.
    /// If any additional checks needs to be done kindly add the test here, as and when needed.
    /// The test is also for checking if the object passed to validate are null or not.
    /// If both expected and actual values are null, validation test returns ValuesNull, thinking it is expected, if they only one value is null validation test is failed
    /// </summary>
    [TestClass]
    public class ValidatorTest
    {
        [TestMethod]
        public void InvalidResultForUnrelatedObjectTypes ()
        {
            Validator.ValidationResult result = Validator.Validate(123, "123");
            Assert.AreEqual(Validator.ValidationResult.InvalidCheck, result);
        }

        [TestMethod]
        public void  FailIfIntValuesAreDifferent()
        {
            Validator.ValidationResult result = Validator.Validate(123, 465);
            Assert.AreEqual(Validator.ValidationResult.Failed, result);
        }

        [TestMethod]
        public void FailIfStringValuesAreDifferent ()
        {
            Validator.ValidationResult result = Validator.Validate("123", "124");
            Assert.AreEqual(Validator.ValidationResult.Failed, result);
        }

        [TestMethod]
        public void FailIfFloatValuesAreDifferent ()
        {
            Validator.ValidationResult result = Validator.Validate(123.01, 123.04);
            Assert.AreEqual(Validator.ValidationResult.Failed, result);
        }

        [TestMethod]
        public void FailIfDecimalValuesAreDifferent ()
        {
            Validator.ValidationResult result = Validator.Validate(123.01M, 123.04M);
            Assert.AreEqual(Validator.ValidationResult.Failed, result);
        }

        [TestMethod]
        public void  PassIfIntValuesAreSame()
        {
            Validator.ValidationResult result = Validator.Validate(123, 123);
            Assert.AreEqual(Validator.ValidationResult.Passed, result);
        }

        [TestMethod]
        public void  PassIfStringValuesAreSame()
        {
            Validator.ValidationResult result = Validator.Validate("123", "123");
            Assert.AreEqual(Validator.ValidationResult.Passed, result);
        }

        [TestMethod]
        public void  PassIfFloatValuesAreSame()
        {
            Validator.ValidationResult result = Validator.Validate(123.01, 123.01);
            Assert.AreEqual(Validator.ValidationResult.Passed, result);
        }

        [TestMethod]
        public void  PassIfDecimalValuesAreSame()
        {
            Validator.ValidationResult result = Validator.Validate(123.01M, 123.01M);
            Assert.AreEqual(Validator.ValidationResult.Passed, result);
        }

        [TestMethod]
        public void  PassIfValuesToValidateAreNull()
        {
            Validator.ValidationResult result = Validator.Validate(null, null);
            Assert.AreEqual(Validator.ValidationResult.ValuesNull, result);
        }

        [TestMethod]
        public void  FailIfAValueToValidateIsNull()
        {
            Validator.ValidationResult result = Validator.Validate(null, 123);
            Assert.AreEqual(Validator.ValidationResult.Failed, result);
        }
    }
}
