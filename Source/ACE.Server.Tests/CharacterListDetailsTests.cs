using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.Tests
{
    [TestClass]
    public class CharacterListDetailsTests
    {
        [TestMethod]
        public void WriteLaysOutTheMarkerVersionCountAndEachCharactersFourValues()
        {
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, true))
            {
                CharacterListDetails.Write(writer, new[]
                {
                    new CharacterListDetails.Details(0x50000010, 2, 1, 126),
                    new CharacterListDetails.Details(0x50000011, 13, 2, 275),
                });
            }
            using var reader = new BinaryReader(new MemoryStream(stream.ToArray()));
            var values = new uint[11];
            for (var index = 0; index < values.Length; index++)
                values[index] = reader.ReadUInt32();
            CollectionAssert.AreEqual(new uint[] { 0x6C6D6D41, 1, 2, 0x50000010, 2, 1, 126, 0x50000011, 13, 2, 275 }, values);
            Assert.AreEqual(reader.BaseStream.Length, reader.BaseStream.Position);
        }
    }
}
