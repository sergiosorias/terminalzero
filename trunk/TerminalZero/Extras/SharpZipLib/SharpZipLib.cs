using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace XUtil
{
	public class SharpZipLib
	{
		#region UNZIP
		public static void UnZip(byte[] inputZipBinary, string destinationPath)
		{
			DirectoryInfo outDirInfo = new DirectoryInfo(destinationPath);
			if (!outDirInfo.Exists)
				outDirInfo.Create();

			using (MemoryStream msZipBinary = new MemoryStream(inputZipBinary))
			{
				using (ZipInputStream zipFile = new ZipInputStream(msZipBinary))
				{
					ZipEntry zipEntry;
					while ((zipEntry = zipFile.GetNextEntry()) != null)
					{
						FileStream fsOut = File.Create(outDirInfo.FullName + "\\" + zipEntry.Name);
						byte[] buffer = new byte[4096]; int count = 0;

#if DEBUG
						Console.WriteLine("Descomprimiendo: " + zipEntry.Name +
							" |Tamaño comprimido: " + zipEntry.CompressedSize +
							" |Tamano descomprimido: " + zipEntry.Size +
							" |CRC: " + zipEntry.Crc);
#endif

						while ((count = zipFile.Read(buffer, 0, buffer.Length)) > 0)
							fsOut.Write(buffer, 0, count);
						fsOut.Flush();
						fsOut.Close();
					}
				}
			}
		}

		public static void UnZip(string inputZipPath, string destinationPath)
		{
			destinationPath = destinationPath.Replace("\\", "\\\\");
			FileInfo inputZipInfo = new FileInfo(inputZipPath);

			if (inputZipInfo.DirectoryName == null && (destinationPath == null || destinationPath == string.Empty))
				throw new Exception("No directory specified");
            
			if (inputZipInfo.Name == null)
				throw new Exception("No zip file name specified");

			DirectoryInfo outDirInfo;
			if (destinationPath == null || destinationPath == string.Empty)
			{
				destinationPath = inputZipInfo.DirectoryName; // Si el directorio destino no existe, se pone el mismo de origen
				outDirInfo = new DirectoryInfo(destinationPath);
			}
			else if ((inputZipPath.Split('\\')).Length == 1)
			{
				outDirInfo = new DirectoryInfo(destinationPath);
				inputZipInfo = new FileInfo(outDirInfo.FullName + "\\" + inputZipInfo.Name);
			}
			else
			{
				outDirInfo = new DirectoryInfo(destinationPath);
			}

			if (!outDirInfo.Exists)
				outDirInfo.Create(); // Si el directorio destino no existe, se crea.

			if (!inputZipInfo.Exists)
				throw new FileNotFoundException("Zip file does not exist");

			// Finalmente llamo la funcion
			UnZip(inputZipInfo.Name, inputZipInfo.DirectoryName, outDirInfo.FullName);
		}

		public static void UnZip(string inputZipName, string sourcePath, string destinationPath)
		{
			if ((sourcePath == null || sourcePath.Length == 0) &&
				(destinationPath == null || destinationPath.Length == 0))
					throw new Exception("Directory not defined");

			// Si alguno de los directorios no vino, se toma el otro como default
			DirectoryInfo inDirInfo, outDirInfo;
            
			if (sourcePath.Length > 0)
				inDirInfo = new DirectoryInfo(sourcePath);
			else
				inDirInfo = new DirectoryInfo(destinationPath);

			if (!inDirInfo.Exists)
				throw new Exception("Source Directory does not exist");
            
			if (destinationPath.Length > 0)
				outDirInfo = new DirectoryInfo(destinationPath);
			else
				outDirInfo = new DirectoryInfo(sourcePath);

			if (!outDirInfo.Exists)
				outDirInfo.Create();

			FileInfo inputZipFileInfo = new FileInfo(
				((sourcePath.TrimEnd()).EndsWith("\\")) ? sourcePath.Trim() : sourcePath.Trim() + "\\" + inputZipName.Trim());

			if (!inputZipFileInfo.Exists)
				throw new Exception("Zip file does not exists");

			using (ZipInputStream zipFile = new ZipInputStream(File.OpenRead(inputZipFileInfo.FullName)))
			{
				ZipEntry zipEntry;
				while ((zipEntry = zipFile.GetNextEntry()) != null)
				{
					FileStream fsOut = File.Create(outDirInfo.FullName + "\\" + zipEntry.Name);
					byte[] buffer = new byte[4096]; int count = 0;

#if DEBUG
					Console.WriteLine("Descomprimiendo: " + zipEntry.Name +
						" |Tamaño comprimido: " + zipEntry.CompressedSize +
						" |Tamano descomprimido: " + zipEntry.Size +
						" |CRC: " + zipEntry.Crc);
#endif

					while ((count = zipFile.Read(buffer, 0, buffer.Length)) > 0)
						fsOut.Write(buffer, 0, count);
					fsOut.Flush();
					fsOut.Close();
				}
			}
		}
		#endregion

		#region ZIP
		public static void Zip(string destinationPath, string[] filesToZip)
		{
			// Creacion del nombre del archivo zip. Rutina especifica para el generate dataout.
			string outFilename = string.Format("{0:00000000}.ZIP", filesToZip.Length);
			Zip(outFilename, destinationPath, filesToZip);
		}

		public static void Zip(string zipName, string destinationPath, string[] filesToZip)
		{
			string zipFilePath = destinationPath + "\\" + zipName;

			if (File.Exists(zipFilePath))
				File.Delete(zipFilePath);

			using (ZipOutputStream zipOut = new ZipOutputStream(File.Create(zipFilePath)))
			{
				zipOut.SetLevel(5);
				byte[] buffer = new byte[4096];

				foreach (string fileToZip in filesToZip)
				{
					ZipEntry zipEntry = new ZipEntry(Path.GetFileName(fileToZip));
					zipEntry.DateTime = DateTime.Now;
					zipEntry.Size = new FileInfo(fileToZip).Length;
					zipOut.PutNextEntry(zipEntry);

					using (FileStream fs = File.OpenRead(fileToZip))
					{
						int sourceBytes;
						do
						{
							sourceBytes = fs.Read(buffer, 0, buffer.Length);
							zipOut.Write(buffer, 0, sourceBytes);
						} while (sourceBytes > 0);
					}
				}

				zipOut.Finish();
				zipOut.Close();
			}
		}

		public static void Zip(out byte[] zipInMemory, byte[] fileInMemory, string fileName)
		{
			using (MemoryStream msFileData = new MemoryStream())
			{
				using (ZipOutputStream zipOut = new ZipOutputStream(msFileData))
				{
					zipOut.SetLevel(5);
					byte[] buffer = new byte[4096];

					ZipEntry zipEntry = new ZipEntry(fileName);
					zipEntry.DateTime = DateTime.Now;
					zipEntry.Size = fileInMemory.Length;
					zipOut.PutNextEntry(zipEntry);
					zipOut.Write(fileInMemory, 0, fileInMemory.Length);
					zipOut.Finish();
                    
					msFileData.Seek(0, SeekOrigin.Begin);
					zipInMemory = new byte[msFileData.Length];
					msFileData.Read(zipInMemory, 0, zipInMemory.Length);

					zipOut.Close();
				}
			}
		}
		#endregion
	}
}
