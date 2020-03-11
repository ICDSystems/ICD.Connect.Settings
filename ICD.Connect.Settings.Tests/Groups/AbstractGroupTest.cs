using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Settings.Groups;
using ICD.Connect.Settings.Originators;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Groups
{
	[TestFixture, UsedImplicitly]
	public abstract class AbstractGroupTest<TGroup, TGroupSettings, TOriginator>
		where TGroup : AbstractGroup<TOriginator,TGroupSettings>
		where TOriginator : class, IOriginator
		where TGroupSettings : IGroupSettings, new()
	{

		protected abstract TGroup InstantiateGroup();

		/// <summary>
		/// Method to get an originator of TOriginator with the given id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		protected abstract TOriginator GetOriginator(int id);

		[TestCase(1000,2000,3000)]
		[TestCase(20000000)]
		[TestCase(20000000,20000001,20000002)]
		public void GetItemsTest(params int[] ids )
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();

			group.AddItems(originators);

			TOriginator[] items = group.GetItems().ToArray();

			Assert.AreEqual(items.Length, originators.Length,"GetItems doesn't return expected number of items");
			
			foreach(var originator in originators)
				Assert.Contains(originator, items, "Items doesn't contain expected originator, id {0}", originator.Id);
		}

		[TestCase(2000000,2000001,2000002,2000003)]
		public void ContainsTestTrue(int testOriginatorId, params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();
			TOriginator testOriginator = GetOriginator(testOriginatorId);

			group.AddItems(originators);
			group.AddItem(testOriginator);

			Assert.True(group.Contains(testOriginator));
		}

		[TestCase(2000000,2000001,2000002,2000003,2000004)]
		public void ContainsTestFalse(int testOriginatorId, params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();
			TOriginator testOriginator = GetOriginator(testOriginatorId);

			group.AddItems(originators);

			Assert.False(group.Contains(testOriginator));
		}

		[TestCase(1,2,3,4,5)]
		[TestCase(2000000, 2000001, 2000002, 2000003, 2000004)]
		public void AddItemsTest(params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();

			@group.AddItems(originators);

			TOriginator[] items = @group.GetItems().ToArray();

			Assert.AreEqual(items.Length, originators.Length, "GetItems doesn't return expected number of items");

			foreach (var originator in originators)
				Assert.Contains(originator, items, "Items doesn't contain expected originator, id {0}", originator.Id);

		}

		[TestCase(1,2,3,4,5,6)]
		public void AddItemTestTrue(int itemId, params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();
			TOriginator testOriginator = GetOriginator(itemId);

			group.AddItems(originators);

			Assert.True(group.AddItem(testOriginator));
		}

		[TestCase(1,2,3,4,5,6)]
		public void AddItemTestFalse(int itemId, params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();
			TOriginator testOriginator = GetOriginator(itemId);

			group.AddItems(originators);
			group.AddItem(testOriginator);

			Assert.False(group.AddItem(testOriginator));
		}

		[TestCase(1, 2, 3, 4, 5, 6)]
		public void OnItemsChangedTestInitialRaised(params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();

			bool raised = false;
			group.OnItemsChanged += (sender, args) => raised = true;

			group.AddItems(originators);

			Assert.True(raised);
		}

		[TestCase(1, 2, 3, 4, 5, 6)]
		public void OnItemsChangedTestRaised(int itemId, params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();
			TOriginator testOriginator = GetOriginator(itemId);

			group.AddItems(originators);
			bool raised = false;

			group.OnItemsChanged += (sender, args) => raised = true;

			group.AddItem(testOriginator);

			Assert.True(raised);
		}

		[TestCase(1, 2, 3, 4, 5, 6)]
		public void OnItemsChangedTestNotRaised(int itemId, params int[] ids)
		{
			TGroup group = InstantiateGroup();

			TOriginator[] originators = ids.Select(GetOriginator).ToArray();
			TOriginator testOriginator = GetOriginator(itemId);

			group.AddItems(originators);
			group.AddItem(testOriginator);
			bool raised = false;

			group.OnItemsChanged += (sender, args) => raised = true;

			group.AddItem(testOriginator);

			Assert.False(raised);
		}
	}
}
