using System;

namespace TwitterAPIStats_DataAccessLayer
{
	public class NullUtils
	{
		public static object DbNullValue { get { return DBNull.Value; } }

		#region string
		

		public static string GetString(System.Data.IDataReader reader, int rowNum)
		{
			return reader.IsDBNull(rowNum) ? null : reader.GetString(rowNum);
		}

		
		#endregion
		#region int
		public static int? GetInt(System.Data.IDataReader reader, string rowName)
		{
			if (ColumnExists(reader, rowName))
				return GetInt(reader, reader.GetOrdinal(rowName));
			return null;
		}

		public static int? GetInt(System.Data.IDataReader reader, int rowNum)
		{
			return reader.IsDBNull(rowNum) ? (int?)null : reader.GetInt32(rowNum);
		}

		public static int GetInt(System.Data.IDataReader reader, string rowName, int defaultValue)
		{
			if (ColumnExists(reader, rowName))
				return GetInt(reader, reader.GetOrdinal(rowName), defaultValue);
			return defaultValue;
		}

		public static int GetInt(System.Data.IDataReader reader, int rowNum, int defaultValue)
		{
			return reader.IsDBNull(rowNum) ? defaultValue : reader.GetInt32(rowNum);
		}

		public static short? GetInt16(System.Data.IDataReader reader, string rowName)
		{
			if (ColumnExists(reader, rowName))
				return GetInt16(reader, reader.GetOrdinal(rowName));
			return null;
		}

		public static short? GetInt16(System.Data.IDataReader reader, int rowNum)
		{
			return reader.IsDBNull(rowNum) ? (short?)null : reader.GetInt16(rowNum);
		}

		public static short GetInt16(System.Data.IDataReader reader, string rowName, short defaultValue)
		{
			if (ColumnExists(reader, rowName))
				return GetInt16(reader, reader.GetOrdinal(rowName), defaultValue);
			return defaultValue;
		}

		public static short GetInt16(System.Data.IDataReader reader, int rowNum, short defaultValue)
		{
			return reader.IsDBNull(rowNum) ? defaultValue : reader.GetInt16(rowNum);
		}

		public static long? GetInt64(System.Data.IDataReader reader, string rowName)
		{
			if (ColumnExists(reader, rowName))
				return GetInt64(reader, reader.GetOrdinal(rowName));
			return null;
		}

		public static long? GetInt64(System.Data.IDataReader reader, int rowNum)
		{
			return reader.IsDBNull(rowNum) ? (long?)null : reader.GetInt64(rowNum);
		}

		public static long GetInt64(System.Data.IDataReader reader, string rowName, short defaultValue)
		{
			if (ColumnExists(reader, rowName))
				return GetInt64(reader, reader.GetOrdinal(rowName), defaultValue);
			return defaultValue;
		}

		public static long GetInt64(System.Data.IDataReader reader, int rowNum, short defaultValue)
		{
			return reader.IsDBNull(rowNum) ? defaultValue : reader.GetInt64(rowNum);
		}

		#endregion



		private static bool ColumnExists(System.Data.IDataReader reader, string rowName)
		{
			try
			{
				if (string.IsNullOrEmpty(rowName)) return false;
				foreach (System.Data.DataRow row in reader.GetSchemaTable().Rows)
					if (((row["ColumnName"] as string) ?? "").Equals(rowName, StringComparison.CurrentCultureIgnoreCase)) return true;
				return false;
			}
			catch
			{
				//uhhhhh...... doesn't exist.
				return false;
			}
		}
	}
}
