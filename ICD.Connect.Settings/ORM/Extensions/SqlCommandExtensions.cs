using System.Collections.Generic;
using System.Data;

namespace ICD.Connect.Settings.ORM.Extensions
{
	public static class SqlCommandExtensions
	{
		/// <summary>
		/// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
		/// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN ({paramNameRoot}))
		/// </summary>
		/// <param name="cmd">The SqlCommand object to add parameters to.</param>
		/// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This value surrounded by {} in the CommandText will be replaced.</param>
		/// <param name="values">The array of strings that need to be added as parameters.</param>
		public static IDbDataParameter[] AddArrayParameters<T>(this IDbCommand cmd, string paramNameRoot,
		                                                       IEnumerable<T> values)
		{
			List<IDbDataParameter> parameters = new List<IDbDataParameter>();
			List<string> parameterNames = new List<string>();
			int paramNbr = 1;

			foreach (T value in values)
			{
				string paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
				parameterNames.Add(paramName);

				IDbDataParameter p = cmd.CreateParameter();
				{
					p.ParameterName = paramName;
					p.Value = value;
				}

#if SIMPLSHARP
				// Hack - Crestron's Add(object) method doesn't do the necessary cast
				cmd.Parameters.Add(p.InnerObject);
#else
				cmd.Parameters.Add(p);
#endif
				parameters.Add(p);
			}

			cmd.CommandText = cmd.CommandText.Replace("@" + paramNameRoot, string.Join(",", parameterNames));

			return parameters.ToArray();
		}
	}
}
