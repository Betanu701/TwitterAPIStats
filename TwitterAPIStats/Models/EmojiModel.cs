using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TwitterAPIStats_DataAccessLayer;

namespace TwitterAPIStats.Models
{
	[Serializable]
	[DataContract(IsReference = true)]
	public class EmojiModel
	{
		[DataMember]
		public string Emoji { get; set; }
		[DataMember]
		public int Total { get; set; }

		public static List<EmojiModel> GetEmojis()
		{
			EmojiModelDAO dao = new EmojiModelDAO();
			return dao.GetEmojis();
		}


		private class EmojiModelDAO : TwitterAPIStats_DataAccessLayer.DataAccess<EmojiModel>
		{
			protected override EmojiModel MapRow(SQLiteDataReader reader)
			{
				EmojiModel model = new EmojiModel();

				model.Total = NullUtils.GetInt(reader, reader.GetOrdinal("total"),0);
				model.Emoji = NullUtils.GetString(reader, reader.GetOrdinal("emoji"));


				return model;
			}


			public List<EmojiModel> GetEmojis()
			{
				EmojiModelDAO dao = new EmojiModelDAO();

				return dao.GetList("SELECT `VALUE` AS 'emoji', COUNT(`VALUE`) AS 'total' FROM emojis GROUP BY `VALUE` ORDER BY COUNT(`VALUE`) DESC limit 5;");
			}
		}
	}
}