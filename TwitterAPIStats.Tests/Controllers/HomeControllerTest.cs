using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterAPIStats;
using TwitterAPIStats.Controllers;
using TwitterAPIStats.Utility;
using System.Text.RegularExpressions;

namespace TwitterAPIStats.Tests.Controllers
{
	[TestClass]
	public class HomeControllerTest
	{
		[TestMethod]
		public void Index()
		{
			// Arrange
			HomeController controller = new HomeController();

			// Act
			ViewResult result = controller.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void RegexTest()
		{
			string test = "This is a test 🤣😂";
			RegexPattern pattern = new RegexPattern();
			var emoji = pattern.emoji.Matches(test);
			if(emoji == null || emoji.Count != 2)
			{
				Assert.Fail("Regex is finding correct number of Emojis");
			}
		}

	}
}
