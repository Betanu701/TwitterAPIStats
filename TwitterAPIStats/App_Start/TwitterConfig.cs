using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tweetinvi;
using Tweetinvi.Models;

using System.Data.SQLite;
using TwitterAPIStats.Models;
using TwitterAPIStats.Utility;
using TwitterAPIStats_DataAccessLayer;

namespace TwitterAPIStats
{
	public class TwitterConfig
	{
		public static async Task StartTwitterAPI(bool skip)
		{
			TwitterAPIDAO dao = new TwitterAPIDAO();
			/*
			 * Since this is an app that does not need to store tweets/stats, we recreate the database each time. 
			 * Normally, you would not build/maintain the database in code (at least with .Net) 
			 * 
			 */

			if (!skip)
			{
				dao.BuildSQliteDatabase();
			}

			var appCredentials = new ConsumerOnlyCredentials(System.Configuration.ConfigurationManager.AppSettings["TwitterKey"], System.Configuration.ConfigurationManager.AppSettings["TwitterSecret"])
			{
				BearerToken = System.Configuration.ConfigurationManager.AppSettings["TwitterBearerToken"] 
			};

			var appClient = new TwitterClient(appCredentials);

			/**
			 * This is looping through the tweets automatically. As long as the application is running. tweets will be saved to a sqllite table. 
			 * The only time I have seen this fail is if I put a breakpoint within the receiver. This causes the connection to twitter to be severed.
			 * Also, since this is running in a seperate process, there is no blocking the other parts of the app!
			 * 
			 * 
			 * 
			 */
			var sampleStreamV2 = appClient.StreamsV2.CreateSampleStream();
			sampleStreamV2.TweetReceived += (sender, args) =>
			{
				bool hasUrl = false;
				bool hasImage = false;
				bool hasEmoji = false;
				RegexPattern pattern = new RegexPattern();
				var emojis = pattern.emoji.Matches(args.Tweet.Text);
				foreach(System.Text.RegularExpressions.Match emj in emojis)
				{
					hasEmoji = true;
					string text = "INSERT INTO emojis(value) VALUES(@value)";
					List<SQLiteParameter> InnerParamList = new List<SQLiteParameter>
					{
						dao.NewParameter("value", emj.Value)
					};
					dao.RunSQLiteCommand(text, InnerParamList);
				}
				

				if(args.Tweet.Entities != null)
				{
					if(args.Tweet.Entities.Hashtags != null && args.Tweet.Entities.Hashtags.Count() > 0)
					{
						foreach(Tweetinvi.Models.V2.HashtagV2 hashtag in args.Tweet.Entities.Hashtags)
						{
							List<SQLiteParameter> InnerParamList = new List<SQLiteParameter>
							{
								dao.NewParameter("tag", hashtag.Tag)
							};
							string text = "INSERT INTO hashtags(tag) VALUES(@tag)";
							dao.RunSQLiteCommand(text, InnerParamList);
						}
					}

					if (args.Tweet.Entities.Urls != null && args.Tweet.Entities.Urls.Count() > 0)
					{
						hasUrl = true;
						foreach (Tweetinvi.Models.V2.UrlV2 urlEntity in args.Tweet.Entities.Urls)
						{
							// No need to run string checkers we have already set hasImage.
							if(!hasImage && (urlEntity.DisplayUrl.Contains("pic.twitter") || urlEntity.DisplayUrl.Contains("instagram")))
							{
								hasImage = true;
							}

							Uri tweetURL = new Uri(urlEntity.ExpandedUrl);
							List<SQLiteParameter> InnerParamList = new List<SQLiteParameter>
							{
								dao.NewParameter("url", urlEntity.DisplayUrl),
								dao.NewParameter("hostname", tweetURL.Host)
							};
							string text = "INSERT INTO urls(url, hostname) VALUES(@url, @hostname)";
							dao.RunSQLiteCommand(text, InnerParamList);
							
							
						}
					}

					if (args.Tweet.Entities.Mentions != null && args.Tweet.Entities.Mentions.Count() > 0)
					{
						foreach (Tweetinvi.Models.V2.UserMentionV2 mention in args.Tweet.Entities.Mentions)
						{
							List<SQLiteParameter> InnerParamList = new List<SQLiteParameter>
							{
								dao.NewParameter("name", mention.Username)
							};
							string text = "INSERT INTO mentions(name) VALUES(@name)";
							dao.RunSQLiteCommand(text, InnerParamList);
						}
					}
				}


				List<SQLiteParameter> ParamList = new List<SQLiteParameter>
				{
						dao.NewParameter("text", args.Tweet.Text),
						dao.NewParameter("date", args.Tweet.CreatedAt.ToUnixTimeSeconds()),
						dao.NewParameter("lang", args.Tweet.Lang),
						dao.NewParameter("source", args.Tweet.Source),
						dao.NewParameter("like", args.Tweet.PublicMetrics.LikeCount),
						dao.NewParameter("retweet", args.Tweet.PublicMetrics.RetweetCount),
						dao.NewParameter("hasUrl", hasUrl),
						dao.NewParameter("hasImage", hasImage),
						dao.NewParameter("hasEmoji", hasEmoji)
				};
				string CommandText = "INSERT INTO tweets(text, date, lang, source, like, retweet, hasUrl, hasImage, hasEmoji) VALUES(@text, @date, @lang, @source, @like, @reTweet, @hasUrl, @hasImage, @hasEmoji)";
				dao.RunSQLiteCommand(CommandText, ParamList);


			};

			await sampleStreamV2.StartAsync();
		}

		public static TweetStatModel GetTweetStats()
		{

			TwitterAPIDAO dao = new TwitterAPIDAO();

			return dao.GetStats();
		}
		protected class TwitterAPIDAO : TwitterAPIStats_DataAccessLayer.DataAccess<TweetStatModel>
		{

			protected override TweetStatModel MapRow(SQLiteDataReader reader)
			{
				TweetStatModel tweetStats = new TweetStatModel();
				

				decimal total = NullUtils.GetInt(reader,reader.GetOrdinal("total"),0);
				tweetStats.TotalLikes = NullUtils.GetInt(reader, reader.GetOrdinal("totalLikes"),0);
				tweetStats.TotalRetweet = NullUtils.GetInt(reader, reader.GetOrdinal("totalRetweets"),0);
				decimal totalEmoji = NullUtils.GetInt(reader, reader.GetOrdinal("totalEmojis"),0);
				decimal totalImages = NullUtils.GetInt(reader, reader.GetOrdinal("totalImages"),0);
				decimal totalUrls = NullUtils.GetInt(reader, reader.GetOrdinal("totalUrls"),0);
				int LastSecond = NullUtils.GetInt(reader, reader.GetOrdinal("lastSecond"),0);
				int LastMinute = NullUtils.GetInt(reader, reader.GetOrdinal("lastMinute"),0);
				int LastHour = NullUtils.GetInt(reader, reader.GetOrdinal("lastHour"),0);

				if (total > 0)
				{
					var emojiPercent = (totalEmoji / total) * 100;
					tweetStats.PercentContainsEmoji = (int)emojiPercent;
					var imagePercent = (totalImages / total) * 100;
					tweetStats.PercentContainsImage = (int)imagePercent;
					var urlPercent = (totalUrls / total) * 100;
					tweetStats.PercentContainsUrl = (int)urlPercent;
				}
				tweetStats.Total = (int)total;
				List<AverageModel> averages = new List<AverageModel>();
				AverageModel second = new AverageModel()
				{
					Title = "Second",
					Amount = LastSecond
				};
				averages.Add(second);
				AverageModel minute = new AverageModel()
				{
					Title = "Minute",
					Amount = LastMinute
				};
				averages.Add(minute);
				AverageModel hour = new AverageModel()
				{
					Title = "Hour",
					Amount = LastHour
				};
				averages.Add(hour);
				tweetStats.Averages = averages;

				return tweetStats;
			}

			
			
			public void BuildSQliteDatabase()
			{
				/**
				 * NOTE:
				 * This is VERY bad practice. 
				 * I would normally NEVER have straight sql commands inline like this. 
				 * It is much better practice to use stored procedures through a DataAccess Layer. 
				 * 
				 */
				TwitterAPIDAO dao = new TwitterAPIDAO();
				dao.ExecuteNonQuery("DROP TABLE IF EXISTS tweets");
				dao.ExecuteNonQuery(@"CREATE TABLE tweets(id INTEGER PRIMARY KEY, text TEXT, date INTEGER, source TEXT, lang TEXT, like INTEGER, reTweet INTEGER, hasUrl INTEGER, hasImage INTEGER, hasEmoji INTEGER)");

				dao.ExecuteNonQuery("DROP TABLE IF EXISTS hashtags");

				dao.ExecuteNonQuery(@"CREATE TABLE hashtags(id INTEGER PRIMARY KEY, tag TEXT)");

				dao.ExecuteNonQuery("DROP TABLE IF EXISTS urls");

				dao.ExecuteNonQuery(@"CREATE TABLE urls(id INTEGER PRIMARY KEY, url TEXT, hostname TEXT)");

				dao.ExecuteNonQuery("DROP TABLE IF EXISTS emojis");

				dao.ExecuteNonQuery(@"CREATE TABLE emojis(id INTEGER PRIMARY KEY, value TEXT)");

				dao.ExecuteNonQuery("DROP TABLE IF EXISTS mentions");

				dao.ExecuteNonQuery(@"CREATE TABLE mentions(id INTEGER PRIMARY KEY, name TEXT)");

			}

			public void RunSQLiteCommand(string CommandText, List<SQLiteParameter> Parameters)
			{
				TwitterAPIDAO dao = new TwitterAPIDAO();
				dao.ExecuteNonQuery(CommandText, Parameters);
			}

			/**
			 * This probably should be in the TweetStatModel. 
			 * 
			 */
			public TweetStatModel GetStats()
			{
				var Now = DateTimeOffset.Now.ToUnixTimeSeconds();
				var PreviousMinute = 60;
				var PreviousHour = (60 * 60);
				TwitterAPIDAO dao = new TwitterAPIDAO();
				string command = "SELECT Count(*) AS total";
				command += ", (SELECT SUM(`LIKE`) FROM tweets) AS 'totalLikes'";
				command += ", (SELECT SUM(`reTweet`) FROM tweets) AS 'totalRetweets'";
				command += ", (SELECT COUNT(*) FROM tweets WHERE hasEmoji = 1) AS 'totalEmojis'";
				command += ", (SELECT COUNT(*) FROM tweets WHERE hasImage = 1) AS 'totalImages'";
				command += ", (SELECT COUNT(*) FROM tweets WHERE hasUrl = 1) AS 'totalUrls'";
				command += ", (SELECT COUNT(*) FROM tweets WHERE `date` >= (SELECT MAX(`date`) FROM tweets) - 1) AS 'lastSecond'";
				command += ", (SELECT COUNT(*) FROM tweets WHERE `date` >= (SELECT MAX(`date`) FROM tweets) - " + PreviousMinute +") AS 'lastMinute'";
				command += ", (SELECT COUNT(*) FROM tweets WHERE `date` >= (SELECT MAX(`date`) FROM tweets) - " + PreviousHour + ") AS 'lastHour'";
				command += "FROM tweets";

				TweetStatModel tweetStats = dao.GetSingleObject(command);
				tweetStats.Emojis = EmojiModel.GetEmojis();
				tweetStats.Hashtags = HashtagModel.GetHashtags();
				tweetStats.Languages = LanguageModel.GetLanguages();
				tweetStats.Mentions = MentionModel.GetMentions();
				tweetStats.Urls = UrlModel.GetUrls();

				return tweetStats;
			}
		}

		

	}
}
