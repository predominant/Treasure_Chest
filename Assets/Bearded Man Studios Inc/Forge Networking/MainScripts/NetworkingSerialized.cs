/*-----------------------------+------------------------------\
|                                                             |
|                        !!!NOTICE!!!                         |
|                                                             |
|  These libraries are under heavy development so they are    |
|  subject to make many changes as development continues.     |
|  For this reason, the libraries may not be well commented.  |
|  THANK YOU for supporting forge with all your feedback      |
|  suggestions, bug reports and comments!                     |
|                                                             |
|                               - The Forge Team              |
|                                 Bearded Man Studios, Inc.   |
|                                                             |
|  This source code, project files, and associated files are  |
|  copyrighted by Bearded Man Studios, Inc. (2012-2015) and   |
|  may not be redistributed without written permission.       |
|                                                             |
\------------------------------+-----------------------------*/



using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BeardedManStudios.Network
{
	public abstract class NetworkingSerialized : INetworkingSerialized
	{
		private List<PropertyInfo> Properties { get; set; }

		public NetworkingSerialized(bool lightVersion = true)
		{
			if (lightVersion)
			{
				Properties = new List<PropertyInfo>();
#if NETFX_CORE
				IEnumerable<PropertyInfo> properties = this.GetType().GetRuntimeProperties().OrderBy(x => x.Name);
#else
				PropertyInfo[] properties = this.GetType().GetProperties().OrderBy(x => x.Name).ToArray();
#endif
				foreach (PropertyInfo property in properties)
				{
#if NETFX_CORE
					if (property.GetCustomAttribute<NetworkSerialized>() != null)
#else
					if (property.GetCustomAttributes(typeof(NetworkSerialized), true).Length > 0)
#endif
					{
						Properties.Add(property);
					}
				}
			}
		}

		private BMSByte serializeBuffer = new BMSByte();
		public BMSByte Serialized()
		{
			serializeBuffer.Clear();

			foreach (var property in Properties)
			{
				object val = property.GetValue(this, null);
				if (property.PropertyType == typeof(string))
					serializeBuffer.Append(BitConverter.GetBytes(((string)val).Length));

				ObjectMapper.MapBytes(serializeBuffer, val);
			}

			return serializeBuffer;
		}

		public void Deserialize(NetworkingStream stream)
		{
			foreach (PropertyInfo property in Properties)
				property.SetValue(this, ObjectMapper.Map(property.PropertyType, stream), null);
		}
	}
}