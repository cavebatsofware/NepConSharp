using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace NepConSharp
{
	[DataContract]
	public class SystemInformation
	{

		private static List<SystemInformation> Items;

		#region DATA_MEMBERS

		[DataMember]
		public string host;

		[DataMember]
		public string port;

		[DataMember]
		public string user;

		[DataMember]
		public string authtype;

		[DataMember]
		public string authkey;

		[DataMember]
		public string isgw;

		[DataMember]
		public string hasgw;

		[DataMember]
		public string protocol;

		[DataMember]
		public string name;

		#endregion

		public SystemInformation ()
		{

		}

		public bool Save ()
		{
			return true;
		}

		public static List<SystemInformation> GetAll ()
		{
			if (Items == null) {
				LoadAll ();
			}
			return Items;
		}

		public static List<SystemInformation> GetAllGW ()
		{
			var allGW = new List<SystemInformation> ();
			foreach (SystemInformation info in Items) {
				if (info.isgw.ToLower() == "true") {
					allGW.Add(info);
				}
			}
			return allGW;
		}

		public static bool LoadAll (bool reload = false)
		{
			if (reload || Items == null) {
				var jsonParser = new DataContractJsonSerializer (typeof(List<SystemInformation>));
				Items = (List<SystemInformation>)jsonParser.ReadObject (File.OpenText (GetSystemsFile (true)).BaseStream);
			} else {
				return true;
			}
		}

		public static string GetSystemsFile (bool fullPath = false)
		{
			return Config.GetSystemsFile (fullPath);
		}
	}
}

