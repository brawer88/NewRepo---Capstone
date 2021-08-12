using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class LastTenContent {
		public List<Recipe> lstLastTen { get; set; }
		public User user { get; set; }
	}
}