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
	public class UrlModel
	{
		[DataMember]
		public string Domain { get; set; }
		[DataMember]
		public int Total { get; set; }
		public static List<UrlModel> GetUrls()
		{
			UrlModelDAO dao = new UrlModelDAO();
			return dao.GetUrls();
		}
		private class UrlModelDAO : TwitterAPIStats_DataAccessLayer.DataAccess<UrlModel>
		{
			protected override UrlModel MapRow(SQLiteDataReader reader)
			{
				UrlModel model = new UrlModel();

				model.Domain = NullUtils.GetString(reader, reader.GetOrdinal("hostname"));
				model.Total = NullUtils.GetInt(reader, reader.GetOrdinal("total"), 0);


				return model;
			}

			public List<UrlModel> GetUrls()
			{

				UrlModelDAO dao = new UrlModelDAO();

				return dao.GetList("SELECT `hostname`, COUNT(`hostname`) AS 'total' FROM urls GROUP BY `hostname` ORDER BY COUNT(`hostname`) DESC LIMIT 5;");
			}
		}
	}
}