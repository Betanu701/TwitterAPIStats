using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data.SQLite;
using TwitterAPIStats_DataAccessLayer;

namespace TwitterAPIStats.Models
{
	[Serializable]
	[DataContract(IsReference = true)]
	public class TweetStatModel
	{
		[DataMember]
		public int Total { get; set; }
		[DataMember]
		public int TotalLikes { get; set; }
		[DataMember]
		public int TotalRetweet { get; set; }
		[DataMember]
		public float PercentContainsEmoji { get; set; }
		[DataMember]
		public float PercentContainsUrl { get; set; }
		[DataMember]
		public float PercentContainsImage { get; set; }
		[DataMember]
		public List<AverageModel> Averages { get; set; }
		[DataMember]
		public List<HashtagModel> Hashtags { get; set; }
		[DataMember]
		public List<MentionModel> Mentions { get; set; }
		[DataMember]
		public List<LanguageModel> Languages { get; set; }
		[DataMember]
		public List<UrlModel> Urls { get; set; }
		[DataMember]
		public List<EmojiModel> Emojis { get; set; }
		[DataMember]
		public List<SourceModel> Sources { get; set; }


		private class TweetStatModelDAO : TwitterAPIStats_DataAccessLayer.DataAccess<TweetStatModel>
		{

			protected override TweetStatModel MapRow(SQLiteDataReader reader)
			{
				TweetStatModel model = new TweetStatModel();

				model.Total = NullUtils.GetInt(reader, reader.GetOrdinal("total"), 0);


				return model;
			}

		}

	}
}
