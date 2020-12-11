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
	public class LanguageModel
	{
		[DataMember]
		public string Language { get; set; }
		[DataMember]
		public int Total { get; set; }

		public static List<LanguageModel> GetLanguages()
		{
			LanguageModelDAO dao = new LanguageModelDAO();
			return dao.GetLanguages();
		}
		private class LanguageModelDAO : TwitterAPIStats_DataAccessLayer.DataAccess<LanguageModel>
		{
			protected override LanguageModel MapRow(SQLiteDataReader reader)
			{
				LanguageModel model = new LanguageModel();

				model.Total = NullUtils.GetInt(reader, reader.GetOrdinal("total"), 0);
				model.Language = NullUtils.GetString(reader, reader.GetOrdinal("language"));


				return model;
				throw new NotImplementedException();
			}

			public List<LanguageModel> GetLanguages() { 

				LanguageModelDAO dao = new LanguageModelDAO();

				return dao.GetList("SELECT `lang` AS 'language', COUNT(`lang`) as 'total' FROM tweets GROUP BY `lang` ORDER BY COUNT(`lang`) DESC LIMIT 5;");
			}
		}
	}
}