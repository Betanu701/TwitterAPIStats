using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using TwitterAPIStats_DataAccessLayer;

namespace TwitterAPIStats.Models
{
	[Serializable]
	[DataContract(IsReference = true)]
	public class HashtagModel
	{
		[DataMember]
		public string Tag { get; set; }
		[DataMember]
		public int Total { get; set; }

		public static List<HashtagModel> GetHashtags()
		{
			HashtagModelDAO dao = new HashtagModelDAO();
			return dao.GetHashtags();
		}

		private class HashtagModelDAO : TwitterAPIStats_DataAccessLayer.DataAccess<HashtagModel>
		{
			protected override HashtagModel MapRow(SQLiteDataReader reader)
			{
				HashtagModel model = new HashtagModel();

				model.Total = NullUtils.GetInt(reader, reader.GetOrdinal("total"), 0);
				model.Tag = NullUtils.GetString(reader, reader.GetOrdinal("tag"));


				return model;
			}

			public List<HashtagModel> GetHashtags()
			{
				HashtagModelDAO dao = new HashtagModelDAO();

				return dao.GetList("SELECT `tag`, COUNT(`tag`) as 'total' FROM hashtags GROUP BY `tag` ORDER BY COUNT(`tag`) DESC limit 5;");
			}
		}
	}
}