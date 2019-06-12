using System;
using System.IO;

namespace Kernel.core
{
	public static class ExtendStream
	{
		public static void WriteToEx(this Stream source, Stream destination, byte[] transferBuffer)
		{
			source.WriteToEx(destination, transferBuffer, int.MaxValue);
		}

		public static void WriteToEx(this Stream source, Stream destination, byte[] transferBuffer, int count)
		{
			if(source == null || count <= 0)
			{
				return;
			}

			if(destination == null)
			{
				throw new ArgumentNullException("destination is null");
			}

			if(transferBuffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}

			if(transferBuffer.Length < 128)
			{
				throw new ArgumentException("Buffer is too small", "buffer");
			}

			var bufferLength = transferBuffer.Length;
			var leftCount = count;

			while(true)
			{
				var readCount = leftCount > bufferLength ? bufferLength : leftCount;
				int num = source.Read(transferBuffer, 0, readCount);

				if(num > 0)
				{
					destination.Write(transferBuffer, 0, num);

					leftCount -= num;
					if(leftCount == 0)
					{
						break;
					}
				}
				else
				{
					break;
				}
			}

			destination.Flush();
		}
	}
}