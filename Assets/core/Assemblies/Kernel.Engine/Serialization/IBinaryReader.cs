using System.IO;
using System.Text;
using Kernel.Engine;

namespace Kernel.Config
{
	public interface IBinaryReader
	{
		byte ReadByte();
		bool ReadBoolean();
		short ReadInt16();
		int ReadInt32();
		long ReadInt64();
		ushort ReadUInt16();
		uint ReadUInt32();
		ulong ReadUInt64();
		float ReadSingle();
		double ReadDouble();
		string ReadString();
		Fixed ReadFixed();
	}

	public class BinReader : BinaryReader, IBinaryReader
	{
		public BinReader(Stream input) : base(input)
		{

		}

		public BinReader(Stream input, Encoding encoding) : base(input, encoding)
		{

		}

		public Fixed ReadFixed()
		{
			return Fixed.FromRaw(ReadInt64());
		}
	}
}
