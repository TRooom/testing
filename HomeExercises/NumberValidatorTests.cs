using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidator_Should
	{
        [TestCase("0",1,TestName = "ReturnTrue_ForZero",ExpectedResult = true)]
        [TestCase("1", 1, TestName = "ReturnTrue_ForPositiveNumberWithoutSign", ExpectedResult = true)]
        [TestCase("-1", 2, TestName = "ReturnTrue_ForNegative", ExpectedResult = true)]
        [TestCase("1.1", 2, 1, TestName = "ReturnTrue_ForFract", ExpectedResult = true)]
        [TestCase("00", 2, TestName = "ReturnTrue_ForDoubleZero", ExpectedResult = true)]
        [TestCase("+1", 2, TestName = "ReturnTrue_ForCorrectNumerWhithPositiveSign", ExpectedResult = true)]
        [TestCase("0,1", 2, 1, TestName = "ReturnTrue_ForDotDelimiter", ExpectedResult = true)]
        [TestCase("0.1", 2, 1, TestName = "ReturnTrue_ForCommaDelimiter", ExpectedResult = true)]
        [TestCase("11", 2, 0, true, TestName = "ReturnTrue_ForPositive_WhenOnlyPositiveTrue", ExpectedResult = true)]
        [TestCase("11", 1, TestName = "ReturnFalse_WhenNumbersCountMoreThenPrecision", ExpectedResult = false)]
        [TestCase("1.11", 2,1, TestName = "ReturnFalse_WhenFractNumbersCountMoreThenScale", ExpectedResult = false)]
        [TestCase("a", 1, TestName = "ReturnFalse_WhenValueContainsNonNumber", ExpectedResult = false)]
        [TestCase("11", 1, 0, TestName = "ReturnFalse_WhenNumbersCountMoreThenPrecision", ExpectedResult = false)]
        [TestCase("-1", 1, 0, true, TestName = "ReturnFalse_ForNegative_WhenOnlyPositiveTrue", ExpectedResult = false)]
        public bool Check(string value, int precision, int scale = 0, bool onlyPositive = false)
	    {
	        return  new NumberValidator(precision,scale,onlyPositive).IsValidNumber(value);
	    }

        [TestCase(-1,0, TestName = "Throw_WhenPrecisionIsNegative")]
        [TestCase(1, -1, TestName = "Throw_WhenScaleIsNegative")]
        [TestCase(1, 2, TestName = "Throw_WhenScaleMoreThenPrecision")]
        public void Check(int precision, int scale)
	    {
	        Action act = () => new NumberValidator(precision,scale);
	        act.ShouldThrow<ArgumentException>();
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