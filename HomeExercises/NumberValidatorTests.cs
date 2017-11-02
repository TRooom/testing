using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidator_Should
	{
	    [TestCase("0", 1, TestName = "ForZero_ReturnTrue")]
	    [TestCase("1", 1, TestName = "ForPositiveNumberWithoutSign_ReturnTrue")]
	    [TestCase("-1", 2, TestName = "ForNegative_ReturnTrue")]
	    [TestCase("1.1", 2, 1, TestName = "ForFract_ReturnTrue")]
	    [TestCase("00", 2, TestName = "ForDoubleZero_ReturnTrue")]
	    [TestCase("+1", 2, TestName = "ForCorrectNumerWithPositiveSign_ReturnTrue")]
	    [TestCase("0,1", 2, 1, TestName = "ForDotDelimiter_ReturnTrue")]
	    [TestCase("0.1", 2, 1, TestName = "ForCommaDelimiter_ReturnTrue")]
	    [TestCase("11", 2, 0, true, TestName = "ForPositive_WhenOnlyPositiveIsTrue_ReturnTrue")]
	    public void CheckValidNubmers(string value, int precision, int scale = 0, bool onlyPositive = false)
        {
	        new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
        }
        [TestCase("11", 1, TestName = "ForNumber_WhenNumbersCountMoreThenPrecision_ReturnFalse")]
        [TestCase("1.11", 2,1, TestName = "ForNumber_WhenFractNumbersCountMoreThenScale_ReturnFalse")]
        [TestCase("a", 1, TestName = "ForNonNumber_ReturnFalse")]
        [TestCase("-1", 1, 0, true, TestName = "ForNegative_WhenOnlyPositiveIsTrue_ReturnFalse")]
        public void CheckInvalidNumber(string value, int precision, int scale = 0, bool onlyPositive = false)
	    {
	        new NumberValidator(precision,scale,onlyPositive).IsValidNumber(value).Should().BeFalse();
	    }

        [TestCase(-1,0, TestName = "ForNegativePrecision_Throw")]
        [TestCase(1, -1, TestName = "ForNegativeScale_Throw")]
        [TestCase(1, 2, TestName = "ForPositiveScaleAndPresition_WhenScaleMoreThanPrecision_Throw")]
        public void CheckThrow(int precision, int scale)
	    {
	        Action act = () => new NumberValidator(precision,scale);
	        act.ShouldThrow<ArgumentException>();
	    }

	    [TestCase(-1, 0, "precision must be a positive number", TestName = "ForNegativePrecision_ThrowCorrectMessage")]
	    [TestCase(1, -1, "scale must be a non-negative number less or equal than precision", TestName = "ForNegativeScale_ThrowCorrectMessage")]
	    [TestCase(1, 2, "scale must be a non-negative number less or equal than precision", TestName = "ForPositiveScaleAndPresition_WhenScaleMoreThanPrecision_ThrowCorrectMessage")]
	    public void CheckThrowWithMessage(int precision, int scale, string message)
	    {
	        Action act = () => new NumberValidator(precision, scale);
	        act.ShouldThrow<ArgumentException>().Which.Message.Should().Be(message);
	    }
    }

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}