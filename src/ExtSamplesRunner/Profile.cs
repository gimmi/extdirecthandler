using System.Collections.Generic;
using ExtDirectHandler.Configuration;

namespace ExtSamplesRunner
{
	public class Profile
	{
		public object GetBasicInfo(int userId, string foo)
		{
			return new {
				Success = true,
				Data = new {
					Foo = foo,
					Name = "Aaron Conran",
					Company = "Sencha Inc.",
					Email = "aaron@sencha.com"
				}
			};
		}

		public object GetPhoneInfo(int userId)
		{
			return new {
				Success = true,
				Data = new {
					Cell = "443-555-1234",
					Office = "1-800-CALLEXT",
					Home = ""
				}
			};
		}

		public object GetLocationInfo(int userId)
		{
			return new {
				Success = true,
				Data = new {
					Street = "1234 Red Dog Rd.",
					City = "Seminole",
					State = "FL",
					Zip = 33776
				}
			};
		}

		[DirectMethod(FormHandler = true)]
		public object UpdateBasicInfo(string foo, string uid, string name, string email, string company)
		{
			if("aaron@sencha.com".Equals(email))
			{
				return new {
					Success = false,
					Errors = new Dictionary<string, string> {
						{ "email", "already taken" }
					}
				};
			}
			return new { Success = true };
		}
	}
}