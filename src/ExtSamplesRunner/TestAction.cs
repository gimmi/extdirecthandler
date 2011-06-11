using System.Collections.Generic;
using System.Linq;

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
			IEnumerable<GetGridResult> data = new[]{
				new GetGridResult{ Name = "ABC Accounting", Turnover = 50000 },
				new GetGridResult{ Name = "Ezy Video Rental", Turnover = 106300 },
				new GetGridResult{ Name = "Greens Fruit Grocery", Turnover = 120000 },
				new GetGridResult{ Name = "Icecream Express", Turnover = 73000 },
				new GetGridResult{ Name = "Ripped Gym", Turnover = 88400 },
				new GetGridResult{ Name = "Smith Auto Mechanic", Turnover = 222980 }
			};
			foreach(GetGridParams.SortInfo sortInfo in par.Sort)
			{
				data = sortInfo.Sort(data);
			}
			return data;
		}

		#region Nested type: GetGridParams

		public class GetGridParams
		{
			public int Page;
			public int Start;
			public int Limit;
			public SortInfo[] Sort;

			#region Nested type: SortInfo

			public class SortInfo
			{
				public string Property;
				public string Direction;

				public IEnumerable<GetGridResult> Sort(IEnumerable<GetGridResult> data)
				{
					if(Property == "name" && Direction == "ASC")
					{
						data = data.OrderBy(x => x.Name);
					}
					else if(Property == "name" && Direction == "DESC")
					{
						data = data.OrderByDescending(x => x.Name);
					}
					else if(Property == "turnover" && Direction == "ASC")
					{
						data = data.OrderBy(x => x.Turnover);
					}
					else if(Property == "turnover" && Direction == "DESC")
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
	}
}