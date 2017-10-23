using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge
{
	[TestFixture]
	public class WordsStatistics_Tests
	{
		public static string Authors = "Туркин Волков"; // "Egorov Shagalina"

		public virtual IWordsStatistics CreateStatistics()
		{
			// меняется на разные реализации при запуске exe
			return new WordsStatistics();
		}

        [TestCase("", ExpectedResult = false, TestName = "AddWord_DontAdd_EmptyStr")]
        [TestCase(" ", ExpectedResult = false, TestName = "AddWord_DontAdd_WhiteCpace")]
        [TestCase("word", ExpectedResult = true, TestName = "AddWord_Add_StringLengthLessThanTen")]
        [TestCase("wordwordwordwordword", ExpectedResult = false, TestName = "AddWord_Add_StringLengthMoreThanTen")]
        [TestCase("wwwwwwwwww", ExpectedResult = true, TestName = "AddWord_Add_StringLengthTen")]
        [TestCase("wordwordwordwordword", ExpectedResult = false, TestName = "AddWord_Add_StringLengthMoreThanTen")]
        [TestCase("W", ExpectedResult = false, TestName = "AddWord_Add_Capital Char")]
        [TestCase("wwwww2wwwww", ExpectedResult = false, TestName = "AddWord_Add_StringLengthEleven")]
        [TestCase("                    WW2", ExpectedResult = false, TestName = "AddWord_DontAdd_ALot")]
        public bool ContainsWord(string word)
	    {
            statistics.AddWord(word);
	        var values = statistics.GetStatistics();
	        //return values.Any(e => e.Item2 == word);
	        foreach (var e in values)
	            if (e.Item2 == word)
	                return true;
	        return false;
	    }
        [TestCase("word", 1, ExpectedResult = 1, TestName = "AddWordOneTimes")]
        [TestCase("word", 2, ExpectedResult = 2, TestName = "AddWordTwoTimes")]
        public int GetCountOfAdding(string word, int repitadly)
        {
            for (int i = 0; i < repitadly; i++)
                statistics.AddWord(word);
            var values = statistics.GetStatistics();
            foreach (var e in values)
                if (e.Item2 == word)
                    return e.Item1;
            return -1;
        }

        private IWordsStatistics statistics;

		[SetUp]
		public void SetUp()
		{
			statistics = CreateStatistics();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterCreation()
		{
			statistics.GetStatistics().Should().BeEmpty();
		}

		[Test]
		public void GetStatistics_ContainsItem_AfterAddition()
		{
			statistics.AddWord("abc");
			statistics.GetStatistics().Should().Equal(Tuple.Create(1, "abc"));
		}

		[Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		{
			statistics.AddWord("abc");
			statistics.AddWord("def");
			statistics.GetStatistics().Should().HaveCount(2);
		}

	    [Test]
	    public void AddWord_Fail_OnNullWord()
	    {
	        Assert.Throws<ArgumentNullException>(() => statistics.AddWord(null));
	    }

	    [Test]
	    public void AddWord_DontAdd_EmptyWord()
	    {
	        statistics.AddWord("");
            //Assert.False(statistics.);
	    }

	    [Test]
	    public void AddWord()
	    {
	        
	    }

		// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
	}
}