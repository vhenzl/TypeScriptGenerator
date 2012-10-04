﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Microsoft.Samples.VisualStudio.GeneratorSample;

namespace VaH.CodeGenerators.TypeScript
{
	[Guid("f1995ea3-9766-4e74-b841-06b6444ece5b")]
	public class TypeScriptGenerator : BaseCodeGeneratorWithSite
	{

		protected override string GetDefaultExtension()
		{
			if (Path.GetExtension(InputFilePath).ToLowerInvariant() == ".ts")
				return ".js";
			return ".generated.js";
		}

		protected override byte[] GenerateCode(string inputFileContent)
		{
			try
			{
				var outputJs = Path.GetTempFileName() + ".js";

				using (var process = new Process())
				{
					var args = string.Format("\"{0}\" --out \"{1}\"", InputFilePath, outputJs);
					var startInfo = new ProcessStartInfo("tsc", args)
					{
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true
					};

					process.StartInfo = startInfo;
					process.Start();
					StreamReader stdErr = process.StandardError;
					process.WaitForExit();

					if (process.ExitCode != 0)
					{
						string error = stdErr.ReadToEnd();
						GeneratorError(0, error, 0, 0);
					}

					var js = File.ReadAllText(outputJs);
					return Encoding.UTF8.GetBytes(GenerateHeader() + js);
				}
			}
			catch (Exception exception)
			{
				GeneratorError(0, exception.Message, 0, 0);
			}
			return null;
		}

		private string GenerateHeader()
		{
			return new StringBuilder()
				.AppendLine("/**")
				.AppendLine(" * This code was generated by a tool.")
				.AppendFormat(" * Date: {0}", DateTime.Now).AppendLine()
				.AppendFormat(" * Source: {0}", InputFilePath).AppendLine()
				.AppendLine(" *")
				.AppendLine(" * Changes to this file will be lost if the code is regenerated.")
				.AppendLine(" */")
				.AppendLine()
				.ToString();
		}
	}
}


