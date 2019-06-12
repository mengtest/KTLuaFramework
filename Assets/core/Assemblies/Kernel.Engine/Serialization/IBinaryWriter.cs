using System.IO;
using System.Text;
using Kernel.Engine;

namespace Kernel.Config
{
	public interface IBinaryWriter
	{
		void Write(byte value);
		void Write(bool value);
		void Write(short value);
		void Write(int value);
		void Write(long value);
		void Write(ushort value);
		void Write(uint value);
		void Write(ulong value);
		void Write(float value);
		void Write(double value);
		void Write(string value);
		void WriteFixed(Fixed value); //由于有隐式转换，需要和其它方法区别开来
	}

	public class BinWriter : BinaryWriter, IBinaryWriter
	{
		public BinWriter(Stream output) : base(output)
		{
			
		}

		public BinWriter(Stream output, Encoding encoding) : base(output, encoding)
		{
			
		}

		public void WriteFixed(Fixed value)
		{
			Write(value.RawValue);
		}
	}
}
