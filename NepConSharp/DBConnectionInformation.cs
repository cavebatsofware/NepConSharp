using System;
using System.Runtime.Serialization;

namespace NepConSharp
{
	[DataContract]
	public class DBConnectionInformation
	{
		#region DATA_MEMBERS

		[DataMember]
		public string host;

		[DataMember]
		public string port;

		[DataMember]
		public string user;

		[DataMember]
		public string password;

		[DataMember]
		public string timeout;

		[DataMember]
		public string pooling;

		[DataMember]
		public string database;

		#endregion

		public DBConnectionInformation ()
		{

		}
	}
}

