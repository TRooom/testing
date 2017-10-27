using System;
using System.Collections;
using System.Dynamic;
using System.Xml.Serialization;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class ObjectComparison
    {
        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода?
            /* Если тест упадет, будет непонятна причина, и дебагер не сильно поможет в обнаружении.
               Логику AreEqual разумнее поместить в Person, и использовать Assert.Equals(),
               чтобы тест не падал, если логика изменится*/
            Assert.True(AreEqual(actualTsar, expectedTsar));

        }

        private bool AreEqual(Person actual, Person expected)
        {
            if (actual == expected) return true;
            if (actual == null || expected == null) return false;
            return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
        }
    }
    public class Tsar_Should
    {
        public static Person ActualTsar;
        public static Person ExpectedTsar;

        [Test, TestCaseSource(nameof(HasSameCharacteristics))]
        public void IsSame(object expected, object actual)
        {
            expected.Should().Be(actual);
        }

        public static IEnumerable HasSameCharacteristics
        {
            get
            {
                var actualTsar = TsarRegistry.GetCurrentTsar();
                var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                    new Person("Vasili III of Russia", 28, 170, 60, null));
                yield return
                    Generate_IsSame_CaseData("HasSameName", actualTsar.Name, expectedTsar.Name);
                yield return
                    Generate_IsSame_CaseData("HasSameAge", actualTsar.Age, expectedTsar.Age);
                yield return
                    Generate_IsSame_CaseData("HasSameHeight", actualTsar.Height, expectedTsar.Height);
                yield return
                    Generate_IsSame_CaseData("HasSameWeight", actualTsar.Weight, expectedTsar.Weight);
                yield return
                    Generate_IsSame_CaseData("HasSameParent'sName", expectedTsar.Parent.Name, actualTsar.Parent.Name);
                yield return
                    Generate_IsSame_CaseData("HasSameParent'sAge", expectedTsar.Parent.Age, actualTsar.Parent.Age);
                yield return
                    Generate_IsSame_CaseData("HasSameParent'sHeight", expectedTsar.Parent.Height, actualTsar.Parent.Height);
                //Add missing test
                yield return
                    Generate_IsSame_CaseData("HasSameParent'sHeight", expectedTsar.Parent.Weight, actualTsar.Parent.Weight);
                //This test is bad, it falls when two objects have the same values of Person.Parent.Parent but different references on it
                yield return
                    Generate_IsSame_CaseData("HasSameGrandfather", expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
            }
        }

        public static TestCaseData Generate_IsSame_CaseData(string name, params object[] args)
        {
            return new TestCaseData(args) { TestName = name };
        }
    }

    public class TsarRegistry
    {
        public static Person GetCurrentTsar()
        {
            return new Person(
                "Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));
        }
    }

    public class Person
    {
        public static int IdCounter = 0;
        public int Age, Height, Weight;
        public string Name;
        public Person Parent;
        public int Id;

        public Person(string name, int age, int height, int weight, Person parent)
        {
            Id = IdCounter++;
            Name = name;
            Age = age;
            Height = height;
            Weight = weight;
            Parent = parent;
        }
    }
}
