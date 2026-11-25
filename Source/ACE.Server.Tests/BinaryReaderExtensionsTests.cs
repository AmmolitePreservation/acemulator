using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ACE.Common.Extensions;

namespace ACE.Server.Tests
{
    [TestClass]
    public class BinaryReaderExtensionsTests
    {
        private static byte[] PackString32L(string text)
        {
            var packedLength = text.Length < 0x80 ? new[] { (byte)text.Length } : new[] { (byte)((text.Length >> 8) | 0x80), (byte)(text.Length & 0xFF) };
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write((uint)(packedLength.Length + text.Length));
            writer.Write(packedLength);
            writer.Write(Encoding.ASCII.GetBytes(text));
            return stream.ToArray();
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(127)]
        [DataRow(128)]
        [DataRow(200)]
        [DataRow(255)]
        [DataRow(256)]
        [DataRow(300)]
        public void ReadString32LReturnsTheStringPackedAtEachLength(int length)
        {
            var text = new string('x', length - 1) + "z";
            using var reader = new BinaryReader(new MemoryStream(PackString32L(text)));
            Assert.AreEqual(text, reader.ReadString32L());
        }
    }
}
