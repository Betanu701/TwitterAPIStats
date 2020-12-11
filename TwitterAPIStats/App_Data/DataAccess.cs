using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Data;

/**
 * Note: Normally I would have methods that could be used for either Stored Procedures or Text SQLite does not have Sprocs. 
 * What is here is allowing for bad practice. 
 * The reason I am using "text" commands is for simplicity since I am not using sprocs with this applicaiton.
 * 
 * The methods belwo are written in a way I could easily come back and add the sproc code.
 * This would require changing the dataaccess to another SQL solution though.
 * 
 * */
namespace TwitterAPIStats_DataAccessLayer
{
	public abstract class DataAccess<T>
	{
		private string _DBConnection { get; set; }
		public DataAccess(string connection = null)
		{
			if(connection != null)
			{
				_DBConnection = connection;
			}
			else
			{
				_DBConnection = System.Configuration.ConfigurationManager.ConnectionStrings["localSql"].ConnectionString;
			}
			
		}

		protected abstract T MapRow(SQLiteDataReader reader);

		protected T GetSingleObject(string cmd)
		{
			return GetSingleObject(CommandType.Text, cmd, null);
		}

		protected T GetSingleObject(CommandType type, string cmd, List<SQLiteParameter> parameters = null)
		{
			T retval = default(T);

			using (SQLiteConnection con = new SQLiteConnection(_DBConnection))
			{
				using (SQLiteCommand command = new SQLiteCommand(cmd, con))
				{
					con.Open();
					command.CommandType = type;

					using (SQLiteDataReader rdr = command.ExecuteReader())
					{
						if (rdr.Read())
							retval = MapRow(rdr);
					}
				}
			}

			return retval;
		}

		protected List<T> GetList(string cmd)
		{
			return GetList(CommandType.Text, cmd, null);
		}

		protected List<T> GetList(CommandType type, string cmd, List<SQLiteParameter> parameters = null)
		{
			List<T> retval = new List<T>();

			using (SQLiteConnection con = new SQLiteConnection(_DBConnection))
			{
				using (SQLiteCommand command = new SQLiteCommand(cmd, con))
				{
					con.Open();
					command.CommandType = type;

					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							retval.Add(MapRow(reader));
						}
					}
				}
			}

			return retval;
		}
		
		protected object ExecuteScalar(string cmd)
		{
			return ExecuteScalar(CommandType.Text, cmd, null);
		}

		protected object ExecuteScalar(CommandType type, string cmd, List<SQLiteParameter> parameters = null)
		{
			object retval = null;
			using (SQLiteConnection con = new SQLiteConnection(_DBConnection))
			{
				using (SQLiteCommand command = new SQLiteCommand(cmd, con))
				{
					con.Open();
					command.CommandType = type;


					retval = command.ExecuteScalar();
				}
			}
			return retval;
		}


		protected object ExecuteNonQuery(string cmd, List<SQLiteParameter> parameters = null)
		{
			return ExecuteNonQuery(CommandType.Text, cmd, parameters);
		}

		protected object ExecuteNonQuery(CommandType type, string cmd, List<SQLiteParameter> parameters = null)
		{
			object retval = null;
			using (SQLiteConnection con = new SQLiteConnection(_DBConnection))
			{
				using (SQLiteCommand command = new SQLiteCommand(cmd, con))
				{
					con.Open();
					command.CommandType = type;


					if (parameters != null)
					{
						foreach (SQLiteParameter param in parameters)
							command.Parameters.Add(param);
					}

					retval = command.ExecuteNonQuery();
					
				}
			}
			return retval;
		}

		/// <summary>
		/// Creates a new parameter. Do not include "@" in the parameter name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public SQLiteParameter NewParameter(string name, object value)
		{
			string paramName = string.Format("@{0}", name);
			SQLiteParameter param = new SQLiteParameter(paramName, value);

			return param;
		}


	}
}
