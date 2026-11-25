using System.Collections.Generic;
using System.IO;

namespace ACE.Server.Network.GameMessages.Messages
{
    /// <summary>
    /// Each character's heritage, gender and level, appended after the character list's own fields: the client reads those
    /// fields and stops, and Ammolite's login plugin reads this block to filter the character screen.
    /// </summary>
    public static class CharacterListDetails
    {
        public const uint Marker = 0x6C6D6D41;
        public const uint Version = 1;

        public readonly record struct Details(uint CharacterId, int Heritage, int Gender, int Level);

        public static void Write(BinaryWriter writer, IReadOnlyList<Details> characters)
        {
            writer.Write(Marker);
            writer.Write(Version);
            writer.Write((uint)characters.Count);
            foreach (var character in characters)
            {
                writer.Write(character.CharacterId);
                writer.Write((uint)character.Heritage);
                writer.Write((uint)character.Gender);
                writer.Write((uint)character.Level);
            }
        }
    }
}
