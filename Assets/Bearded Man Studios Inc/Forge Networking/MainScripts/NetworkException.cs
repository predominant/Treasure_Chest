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



using System;

namespace BeardedManStudios.Network
{
	public class NetworkException : Exception
	{
		/// <summary>
		/// Error code to return
		/// </summary>
		public ushort Code { get; private set; }

		/// <summary>
		/// Constructor for a networked exception
		/// </summary>
		/// <param name="message">Message of the exception</param>
		public NetworkException(string message) : base(message) { Code = 0; }

		/// <summary>
		/// Constructor for a networked exception
		/// </summary>
		/// <param name="code">Error code of the exception</param>
		/// <param name="message">Message of the exception</param>
		public NetworkException(ushort code, string message) : base(message) { Code = code; }
	}
}