using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PDF
{
	class Program
	{
		const string path = @"E:\SGK";
		const string output = @"E:\PDF";

		static void Main(string[] args)
		{
			var directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).
				Where(folder => folder.Contains("Data", StringComparison.OrdinalIgnoreCase));

			var watch = new Stopwatch();
			watch.Start();
			double time = 0f;

			foreach (var data in directories)
			{

				string subjectName = Directory.GetParent(data).Name;
				string className = Directory.GetParent(data).Parent.Parent.Name;
				string level = Directory.GetParent(data).Parent.Parent.Parent.Name;

				string outputPath = Path.Combine(output, level, className);
				Directory.CreateDirectory(outputPath);
				string pdfFile = Path.Combine(outputPath, subjectName + ".pdf");

				Document.Compress = true;
				Document document = null;
				try
				{
					string[] imgFiles = Directory.GetFiles(data, "*.jpg");
					Array.Sort(imgFiles, StringComparer.InvariantCulture);

					Image tempImg = Image.GetInstance(imgFiles[0]);
					document = new(new Rectangle(tempImg.Width, tempImg.Height));
					PdfWriter.GetInstance(document, new FileStream(pdfFile, FileMode.Create));
					document.Open();

					foreach (var img in imgFiles)
					{
						Image image = Image.GetInstance(img);
						image.SetAbsolutePosition(0, 0);
						document.Add(image);
						document.NewPage();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
				finally
				{
					if (document != null)
						document.Close();
					Console.WriteLine($"Created file {pdfFile} in {watch.ElapsedMilliseconds - time} ms");
					time = watch.ElapsedMilliseconds;
				}
			}
			watch.Stop();
			Console.WriteLine($"PDF files were created completely in {time} ms");
		}
	}
}
