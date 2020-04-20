using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Originators;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests
{
	public abstract class AbstractSettingsTest<TSettings>
		where TSettings : AbstractSettings
	{
		/// <summary>
		/// Creates a new instance of the settings class
		/// </summary>
		/// <returns></returns>
		protected virtual TSettings Instantiate()
		{
			return ReflectionUtils.CreateInstance<TSettings>();
		}

		#region Events

		[Test]
		public void NameChangedFeedbackTest()
		{
			TSettings instance = Instantiate();
			List<StringEventArgs> eventArgs = new List<StringEventArgs>();

			instance.OnNameChanged += (sender, args) => eventArgs.Add(args);

			instance.Name = null;
			instance.Name = "test";
			instance.Name = "test";
			instance.Name = "test2";

			Assert.AreEqual(2, eventArgs.Count);
			Assert.AreEqual("test", eventArgs[0].Data);
			Assert.AreEqual("test2", eventArgs[1].Data);
		}

		#endregion

		#region Properties

		[TestCase(1)]
		public void IdTest(int id)
		{
			TSettings instance = Instantiate();
			instance.Id = id;

			Assert.AreEqual(id, instance.Id);
		}

		[TestCase("test")]
		public void NameTest(string name)
		{
			TSettings instance = Instantiate();
			instance.Name = name;

			Assert.AreEqual(name, instance.Name);
		}

		[TestCase("test")]
		public void CombineNameTest(string combineName)
		{
			TSettings instance = Instantiate();
			instance.CombineName = combineName;

			Assert.AreEqual(combineName, instance.CombineName);
		}

		[TestCase("test")]
		public void DescriptionTest(string description)
		{
			TSettings instance = Instantiate();
			instance.Description = description;

			Assert.AreEqual(description, instance.Description);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void HideTest(bool hide)
		{
			TSettings instance = Instantiate();
			instance.Hide = hide;

			Assert.AreEqual(hide, instance.Hide);
		}

		[Test]
		public void FactoryNameTest()
		{
			TSettings instance = Instantiate();

			Assert.NotNull(instance.FactoryName);
			Assert.IsNotEmpty(instance.FactoryName);
		}

		[Test]
		public void OriginatorTypeTest()
		{
			TSettings instance = Instantiate();

			Assert.NotNull(instance.OriginatorType);
			Assert.IsTrue(instance.OriginatorType.IsAssignableTo(typeof(IOriginator)));
		}

		[Test]
		public void PermissionsTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void DependencyCountTest()
		{
			Assert.Inconclusive();
		}

		#endregion

		#region Methods

		[Test]
		public void ToStringTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ParseXmlTest()
		{
		}

		[Test]
		public void ToXmlTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void ToOriginatorTest()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void HasDeviceDependencyTest()
		{
			Assert.Inconclusive();
		}

		#endregion
	}
}
