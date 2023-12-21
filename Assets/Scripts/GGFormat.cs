using System;
using System.Collections.Generic;
using System.Text;

public class GGFormat
{
	public static string JavaScriptStringEncode(string value, bool addDoubleQuotes = false)
	{
		if (string.IsNullOrEmpty(value))
		{
			if (!addDoubleQuotes)
			{
				return string.Empty;
			}
			return "\"\"";
		}
		int length = value.Length;
		bool flag = false;
		for (int i = 0; i < length; i++)
		{
			char c = value[i];
			if ((c >= '\0' && c <= '\u001f') || c == '"' || c == '\'' || c == '<' || c == '>' || c == '\\')
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			if (!addDoubleQuotes)
			{
				return value;
			}
			return "\"" + value + "\"";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (addDoubleQuotes)
		{
			stringBuilder.Append('"');
		}
		for (int j = 0; j < length; j++)
		{
			char c = value[j];
			if (c < '\0' || c > '\a')
			{
				switch (c)
				{
				default:
					if (c != '\'' && c != '<' && c != '>')
					{
						switch (c)
						{
						case '\b':
							stringBuilder.Append("\\b");
							break;
						case '\t':
							stringBuilder.Append("\\t");
							break;
						case '\n':
							stringBuilder.Append("\\n");
							break;
						case '\f':
							stringBuilder.Append("\\f");
							break;
						case '\r':
							stringBuilder.Append("\\r");
							break;
						case '"':
							stringBuilder.Append("\\\"");
							break;
						case '\\':
							stringBuilder.Append("\\\\");
							break;
						default:
							stringBuilder.Append(c);
							break;
						}
						continue;
					}
					break;
				case '\v':
				case '\u000e':
				case '\u000f':
				case '\u0010':
				case '\u0011':
				case '\u0012':
				case '\u0013':
				case '\u0014':
				case '\u0015':
				case '\u0016':
				case '\u0017':
				case '\u0018':
				case '\u0019':
				case '\u001a':
				case '\u001b':
				case '\u001c':
				case '\u001d':
				case '\u001e':
				case '\u001f':
					break;
				}
			}
			stringBuilder.AppendFormat("\\u{0:x4}", (int)c);
		}
		if (addDoubleQuotes)
		{
			stringBuilder.Append('"');
		}
		return stringBuilder.ToString();
	}

	public static string FormatPrice(int price, bool rem = false)
	{
		if (price >= 1000000)
		{
			string str = rem ? (price / 1000000).ToString("D3") : (price / 1000000).ToString();
			str += " ";
			int num = price % 1000000;
			if (num >= 1000)
			{
				return str + FormatPrice(num, rem: true);
			}
			if (num % 1000 > 0)
			{
				return str + "000 " + FormatPrice(num % 1000, rem: true);
			}
			return str + "000 000";
		}
		if (price >= 1000)
		{
			string str2 = rem ? (price / 1000).ToString("D3") : (price / 1000).ToString();
			str2 = str2 + " " + (price % 1000).ToString("D3");
			while (price.ToString().Length >= str2.Length)
			{
				str2 += "0";
			}
			return str2;
		}
		if (!rem)
		{
			return price.ToString();
		}
		return price.ToString("D3");
	}

	public static string FormatPrice(long price, bool rem = false)
	{
		if (price >= 1000000)
		{
			string str = rem ? (price / 1000000).ToString("D3") : (price / 1000000).ToString();
			str += " ";
			long num = price % 1000000;
			if (num >= 1000)
			{
				return str + FormatPrice(num, rem: true);
			}
			if (num % 1000 > 0)
			{
				return str + "000 " + FormatPrice(num % 1000, rem: true);
			}
			return str + "000 000";
		}
		if (price >= 1000)
		{
			string str2 = rem ? (price / 1000).ToString("D3") : (price / 1000).ToString();
			str2 = str2 + " " + (price % 1000).ToString("D3");
			while (price.ToString().Length >= str2.Length)
			{
				str2 += "0";
			}
			return str2;
		}
		if (!rem)
		{
			return price.ToString();
		}
		return price.ToString("D3");
	}

	public static string FormatPercent(float p)
	{
		return ((int)(p * 100f)).ToString();
	}

	public static string Implode(IEnumerable<string> list, string glue)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(glue);
			}
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}

	public static string FormatTime(int time)
	{
		if (time < 10)
		{
			return "0" + time;
		}
		return time.ToString();
	}

	public static string FormatTimeSpan(TimeSpan span)
	{
		string str = "";
		if (span.TotalDays >= 1.0)
		{
			str = str + FormatTime(span.Days) + ":";
		}
		if (span.TotalHours >= 1.0)
		{
			str = str + FormatTime(span.Hours) + ":";
		}
		return str + FormatTime(span.Minutes) + ":" + FormatTime(span.Seconds);
	}
}
