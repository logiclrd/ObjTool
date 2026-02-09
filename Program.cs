using System;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
	static int Main(string[] args)
	{
		if ((args.Length != 4)
		 || (args[1] != "scale")
		 || !double.TryParse(args[2], out var desiredLongestAxis))
		{
			Console.Error.WriteLine("usage: ObjTool <infile> scale <maxdimension> <outfile>");
			return 1;
		}

		Stream inStream, outStream;

		try
		{
			inStream = File.OpenRead(args[0]);
		}
		catch
		{
			Console.Error.WriteLine("error opening file: {0}", args[0]);
			return 1;
		}

		using (inStream)
		{
			try
			{
				outStream = File.OpenWrite(args[3]);
			}
			catch
			{
				Console.Error.WriteLine("error opening file: {0}", args[3]);
				return 1;
			}

			using (outStream)
			{
				outStream.SetLength(0);
				
				var min = new double[3];
				var max = new double[3];

				min.AsSpan().Fill(double.MaxValue);
				max.AsSpan().Fill(double.MinValue);

				using (var reader = new StreamReader(inStream, leaveOpen: true))
				{
					while (true)
					{
						string? line = reader.ReadLine();

						if (line == null)
							break;

						if (line.StartsWith("v "))
						{
							string[] parameters = line.Substring(2).Split(' ');

							for (int i=0; i < parameters.Length; i++)
							{
								if (double.TryParse(parameters[i], out var coord))
								{
									if (coord < min[i])
										min[i] = coord;
									if (coord > max[i])
										max[i] = coord;
								}
							}
						}
					}
				}

				var extent = new double[3];

				for (int i=0; i < 3; i++)
					extent[i] = max[i] - min[i];

				double longestAxis = extent.Max();

				double scaleFactor = desiredLongestAxis / longestAxis;

				inStream.Position = 0;

				using (var reader = new StreamReader(inStream))
				using (var writer = new StreamWriter(outStream))
				{
					while (true)
					{
						string? line = reader.ReadLine();

						if (line == null)
							break;

						if (line.StartsWith("v "))
						{
							string[] parameters = line.Substring(2).Split(' ');

							var newLineBuilder = new StringBuilder();

							newLineBuilder.Append('v');

							for (int i=0; i < parameters.Length; i++)
								if (double.TryParse(parameters[i], out var coord))
									newLineBuilder.Append(' ').Append((coord * scaleFactor).ToString("#.#####"));

							line = newLineBuilder.ToString();
						}

						writer.WriteLine(line);
					}
				}
			}
		}

		return 0;
	}
}