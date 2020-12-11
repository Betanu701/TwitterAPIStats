using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterAPIStats.Models
{
	[Serializable]
	[DataContract(IsReference = true)]
	public class AverageModel
	{
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public int Amount { get; set; }
		

	}

	
}