// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2013-12-03</date>
// <summary>Basic ini file with group support</summary>

using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace KGF
{
	public class IniFile
	{
		#region internal data
		Dictionary<string,Dictionary<string,string>> itsValues = new Dictionary<string, Dictionary<string, string>>();
		#endregion
		
		#region internal methods
		string SerializeGroup(string theGroup)
		{
			if (itsValues.ContainsKey(theGroup))
			{
				string aData = string.Empty;
				
				if (itsValues[theGroup].Count > 0)
				{
					if (theGroup != string.Empty)
						aData += string.Format("[{0}]",theGroup) + System.Environment.NewLine;
					
					foreach (string aKey in itsValues[theGroup].Keys)
					{
						aData += string.Format("{0}={1}",aKey,itsValues[theGroup][aKey]) + System.Environment.NewLine;
					}
				}
				
				return aData;
			}
			return string.Empty;
		}
		#endregion
		
		#region public interface
		/// <summary>
		/// Create new inifile instance
		/// </summary>
		public IniFile(){}
		
		/// <summary>
		/// create new inifile instance with data from string
		/// </summary>
		/// <param name="theData">data produced by ToString() method</param>
		public IniFile(string theData)
		{
			using (StringReader aReader = new StringReader(theData))
			{
				string aGroupCurrent = string.Empty;
				while (true)
				{
					string aLine = aReader.ReadLine();
					if (aLine == null)
						break;
					
					if (aLine.Trim().StartsWith("#"))
					{
						// ignore
					}
					else if (aLine.Contains("="))
					{
						string[] aLineArr = aLine.Split('=');
						if (aLineArr.Length == 2)
						{
							SetString(aGroupCurrent,aLineArr[0].Trim(),aLineArr[1].Trim());
						}
						else
						{
							// error
						}
					}
					else if (aLine.Trim().StartsWith("["))
					{
						if (aLine.Trim().EndsWith("]"))
						{
							aGroupCurrent = aLine.Trim().Substring(1,aLine.Trim().Length-2);
						}
						else
						{
							// error
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Set value of inifile structure
		/// </summary>
		public void SetString(string theKey, string theValue)
		{
			SetString(string.Empty,theKey,theValue);
		}
		
		/// <summary>
		/// Set value of inifile structure within a group
		/// </summary>
		public void SetString(string theGroup, string theKey, string theValue)
		{
			if (!itsValues.ContainsKey(theGroup))
				itsValues[theGroup] = new Dictionary<string, string>();
			
			itsValues[theGroup][theKey] = theValue;
		}
		
		/// <summary>
		/// Get a list of all group names
		/// </summary>
		public string[] GetGroups()
		{
			return new List<string>(itsValues.Keys).ToArray();
		}
		
		/// <summary>
		/// Get a list of all keys in main group
		/// </summary>
		public string[] GetKeys()
		{
			return GetKeys(string.Empty);
		}
		
		/// <summary>
		/// Get a list of all keys in a group
		/// </summary>
		public string[] GetKeys(string theGroup)
		{
			List<string> aList = new List<string>();
			
			if (itsValues.ContainsKey(theGroup))
			{
				aList.AddRange(itsValues[theGroup].Keys);
			}
			
			return aList.ToArray();
		}
		
		/// <summary>
		/// Get a value from the main group if it exists
		/// </summary>
		public string GetString(string theKey,string theDefaultValue)
		{
			return GetString(string.Empty,theKey,theDefaultValue);
		}
		
		/// <summary>
		/// Get a value from a group if it exists
		/// </summary>
		public string GetString(string theGroup, string theKey,string theDefaultValue)
		{
			if (itsValues.ContainsKey(theGroup))
			{
				if (itsValues[theGroup].ContainsKey(theKey))
				{
					return itsValues[theGroup][theKey];
				}
			}
			return theDefaultValue;
		}
		
		/// <summary>
		/// Save structure to file
		/// </summary>
		public void SaveToFile(string theFilePath)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(theFilePath));
			using (StreamWriter aWriter = new StreamWriter(theFilePath,false,System.Text.Encoding.UTF8))
			{
				aWriter.Write(ToString());
			}
		}
		
		/// <summary>
		/// Serialize this inifile structure to string. Can be imported by the constructor again.
		/// </summary>
		public override string ToString()
		{
			string aData = SerializeGroup(string.Empty);
			
			foreach (string aGroupKey in itsValues.Keys)
			{
				if (aGroupKey != string.Empty)
					aData += SerializeGroup(aGroupKey);
			}
			
			return aData;
		}
		
		/// <summary>
		/// Remove a value from the main group if it exists
		/// </summary>
		public void RemoveValue(string theKey)
		{
			RemoveValue(string.Empty,theKey);
		}
		
		/// <summary>
		/// Remove a value from a group if it exists.
		/// </summary>
		public void RemoveValue(string theGroup,string theKey)
		{
			if (itsValues.ContainsKey(theGroup))
			{
				if (itsValues[theGroup].ContainsKey(theKey))
				{
					itsValues[theGroup].Remove(theKey);
				}
			}
		}
		
		/// <summary>
		/// Remove a whole group with all values in it.
		/// </summary>
		public void RemoveGroup(string theGroup)
		{
			if (itsValues.ContainsKey(theGroup))
			{
				itsValues.Remove(theGroup);
			}
		}
		
		/// <summary>
		/// Clear all values and groups from this instance.
		/// </summary>
		public void Clear()
		{
			itsValues.Clear();
		}
		
		/// <summary>
		/// Create a new inifile instance and load data from a file.
		/// </summary>
		public static IniFile LoadFromFile(string theFilePath)
		{
			using (StreamReader aReader = new StreamReader(theFilePath,System.Text.Encoding.UTF8))
			{
				return new IniFile(aReader.ReadToEnd());
			}
		}
		#endregion
	}
}