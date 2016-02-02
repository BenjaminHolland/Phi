using Phi.Core.Development;
namespace Phi.Core
{
    /// <summary>
    /// Endianness of a converter
    /// </summary>
    /// 
    [CodeSource("MiscUtil", "Custom", @"http://www.yoda.arachsys.com/csharp/miscutil/licence.txt")]
    public enum Endianness
	{
		/// <summary>
		/// Little endian - least significant byte first
		/// </summary>
		LittleEndian,
		/// <summary>
		/// Big endian - most significant byte first
		/// </summary>
		BigEndian
	}
}
