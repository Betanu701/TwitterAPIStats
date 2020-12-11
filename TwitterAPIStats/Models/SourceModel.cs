using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;

namespace TwitterAPIStats.Models
{
	[Serializable]
	[DataContract(IsReference = true)]
	public class SourceModel
	{
		private class SourceModelDAO : TwitterAPIStats_DataAccessLayer.DataAccess<SourceModel>
		{
			protected override SourceModel MapRow(SQLiteDataReader reader)
			{
				// SourceModel model = new SourceModel();

				// model = reader.GetInt32(reader.GetOrdinal("total"));


				// return model;
				throw new NotImplementedException();
			}
		}
	}
}