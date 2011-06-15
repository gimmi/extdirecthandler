using System.Collections.Generic;
using System.Linq;
using ExtDirectHandler.Configuration;

namespace ExtSamplesRunner
{
	public class TestAction
	{
		public string DoEcho(string par)
		{
			return par;
		}

		public double Multiply(double par)
		{
			return par*8;
		}

		public IEnumerable<GetGridResult> GetGrid(GetGridParams par)
		{
			IEnumerable<GetGridResult> data = new[] {
				new GetGridResult {Name = "ABC Accounting", Turnover = 50000},
				new GetGridResult {Name = "Ezy Video Rental", Turnover = 106300},
				new GetGridResult {Name = "Greens Fruit Grocery", Turnover = 120000},
				new GetGridResult {Name = "Icecream Express", Turnover = 73000},
				new GetGridResult {Name = "Ripped Gym", Turnover = 88400},
				new GetGridResult {Name = "Smith Auto Mechanic", Turnover = 222980}
			};
			foreach (GetGridParams.SortInfo sortInfo in par.Sort)
			{
				data = sortInfo.Sort(data);
			}
			return data;
		}

		public IEnumerable<GetTreeResult> GetTree(string id)
		{
			if (id == "root")
			{
				return new[] {1, 2, 3, 4, 5}.Select(n => new GetTreeResult {Id = n.ToString(), Text = "Node " + n, Leaf = false});
			}
			return new[] {1, 2, 3, 4, 5}.Select(n => new GetTreeResult {Id = id + n, Text = "Node " + id + "." + n, Leaf = true});
		}

		[DirectMethod(NamedArguments = true)]
		public string ShowDetails(string firstName, string lastName, int age)
		{
			return string.Format("Hi {0} {1}, you are {2} years old.", firstName, lastName, age);
		}

		#region Nested type: GetGridParams

		public class GetGridParams
		{
			public int Limit;
			public int Page;
			public SortInfo[] Sort;
			public int Start;

			#region Nested type: SortInfo

			public class SortInfo
			{
				public string Direction;
				public string Property;

				public IEnumerable<GetGridResult> Sort(IEnumerable<GetGridResult> data)
				{
					if (Property == "name" && Direction == "ASC")
					{
						data = data.OrderBy(x => x.Name);
					}
					else if (Property == "name" && Direction == "DESC")
					{
						data = data.OrderByDescending(x => x.Name);
					}
					else if (Property == "turnover" && Direction == "ASC")
					{
						data = data.OrderBy(x => x.Turnover);
					}
					else if (Property == "turnover" && Direction == "DESC")
					{
						data = data.OrderByDescending(x => x.Turnover);
					}
					return data;
				}
			}

			#endregion
		}

		#endregion

		#region Nested type: GetGridResult

		public class GetGridResult
		{
			public string Name;
			public int Turnover;
		}

		#endregion

		#region Nested type: GetTreeResult

		public class GetTreeResult
		{
			public string Id;
			public bool Leaf;
			public string Text;
		}

		#endregion
	}
}