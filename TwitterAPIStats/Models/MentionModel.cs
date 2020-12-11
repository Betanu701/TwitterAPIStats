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
	public class MentionModel
	{
		[DataMember]
		public string User { get; set; }
		[DataMember]
		public int Total { get; set; }

		public static List<MentionModel> GetMentions()
		{
			MentionModelDAO dao = new MentionModelDAO();
			return dao.GetMentions();
		}

		private class MentionModelDAO : TwitterAPIStats_DataAccessLayer.DataAccess<MentionModel>
		{
			protected override MentionModel MapRow(SQLiteDataReader reader)
			{
				MentionModel model = new MentionModel();

				model.Total = NullUtils.GetInt(reader, reader.GetOrdinal("total"), 0);
				model.User = NullUtils.GetString(reader, reader.GetOrdinal("user"));


				return model;
				throw new NotImplementedException();
			}

			public List<MentionModel> GetMentions()
			{

				MentionModelDAO dao = new MentionModelDAO();

				return dao.GetList("SELECT `name` AS 'user', COUNT(`name`) AS 'total' FROM mentions GROUP BY `name` ORDER BY COUNT(`name`) DESC LIMIT 5;");
			}
		}
	}
}