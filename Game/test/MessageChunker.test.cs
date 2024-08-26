using System.Text;
using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schema;
using TestHelpers;
using Tests;

namespace KeepLordWarriors
{
    [TestClass]
    public class MessageChunkerTests
    {
        private Oneof_GameServerToPlayer getBigMessage()
        {
            Game game = new(TH.GetGameSettings(map: TestMaps.ThirtyByTwenty));
            return new Oneof_GameServerToPlayer
            {
                InitialState = game.GetInitialState(),
            };
        }

        private Oneof_GameServerToPlayer getSmolMessage()
        {
            Game game = new(TH.GetGameSettings(map: TestMaps.ThreeByThree));
            return new Oneof_GameServerToPlayer
            {
                InitialState = game.GetInitialState(),
            };
        }

        [TestMethod]
        public void Chunk_SplitsLargeMessageIntoMultipleChunks()
        {
            var bigMessage = getBigMessage();
            var updates = new List<Oneof_GameServerToPlayer> { bigMessage, bigMessage };

            var packets = MessageChunker.Chunk(updates, 0u);
            Assert.IsTrue(packets.Count >= 3);

            Oneof_GameServerToPlayer m1 = MessageChunker.ExtractFullUpdate(ref packets!)!;
            Oneof_GameServerToPlayer m2 = MessageChunker.ExtractFullUpdate(ref packets!)!;

            CollectionAssert.AreEqual(updates[0].ToByteArray(), m1.ToByteArray());
            CollectionAssert.AreEqual(updates[1].ToByteArray(), m2.ToByteArray());
            Assert.AreEqual(0, packets.Count);
        }

        [TestMethod]
        public void Chunk_CombinesMultipleSmallMessagesIntoSinglePacket()
        {
            var smolMessage = getSmolMessage();
            var updates = new List<Oneof_GameServerToPlayer>();
            for (int i = 0; i < 5; i++)
                updates.Add(smolMessage);

            var packets = MessageChunker.Chunk(updates, 0u);
            Assert.AreEqual(1, packets.Count);

            var unchunked = new List<Oneof_GameServerToPlayer>();
            for (int i = 0; i < updates.Count; i++)
                unchunked.Add(MessageChunker.ExtractFullUpdate(ref packets!)!);

            Assert.AreEqual(5, unchunked.Count);
            for (int i = 0; i < updates.Count; i++)
            {
                CollectionAssert.AreEqual(updates[i].ToByteArray(), unchunked[i].ToByteArray());
            }
            Assert.AreEqual(0, packets.Count);
        }

        [TestMethod]
        public void Chunk_OneBigMessageFollowedByMultipleSmall()
        {
            var bigMessage = getBigMessage();
            var smallMessage = getSmolMessage();
            var updates = new List<Oneof_GameServerToPlayer>();
            updates.Add(bigMessage);

            for (int i = 0; i < 100; i++)
            {
                updates.Add(smallMessage);
            }

            var packets = MessageChunker.Chunk(updates, 0u);

            var unchunked = new List<Oneof_GameServerToPlayer>();
            for (int i = 0; i < updates.Count; i++)
            {
                unchunked.Add(MessageChunker.ExtractFullUpdate(ref packets)!);
            }
            Assert.AreEqual(updates.Count, unchunked.Count);
            for (int i = 0; i < updates.Count; i++)
            {
                CollectionAssert.AreEqual(updates[i].ToByteArray(), unchunked[i].ToByteArray());
            }
            Assert.AreEqual(0, packets.Count);
        }

        [TestMethod]
        public void Chunk_OnlyExtractsFullUpdates()
        {
            Oneof_GameServerToPlayer bigMessage = getBigMessage();
            Oneof_GameServerToPlayer smallMessage = getSmolMessage();

            var updates = new List<Oneof_GameServerToPlayer>();
            updates.Add(smallMessage);
            updates.Add(bigMessage);
            updates.Add(smallMessage);

            var all = MessageChunker.Chunk(updates, 0);
            var packets = all.Take(all.Count / 2).ToList();
            var secondHalf = all.Skip(all.Count / 2).ToList();
            Oneof_GameServerToPlayer? smallUnchunked = MessageChunker.ExtractFullUpdate(ref packets);
            CollectionAssert.AreEqual(smallMessage.ToByteArray(), smallUnchunked.ToByteArray());
            int prevCount = packets.Count;
            Assert.IsNull(MessageChunker.ExtractFullUpdate(ref packets));
            Assert.AreEqual(prevCount, packets.Count);

            packets.AddRange(secondHalf);
            var bigUnchunked = MessageChunker.ExtractFullUpdate(ref packets);
            CollectionAssert.AreEqual(bigMessage.ToByteArray(), bigUnchunked.ToByteArray());

            var smallUnchunked2 = MessageChunker.ExtractFullUpdate(ref packets);
            CollectionAssert.AreEqual(smallMessage.ToByteArray(), smallUnchunked2.ToByteArray());
            Assert.AreEqual(0, packets.Count);
        }

        [TestMethod]
        public void Chunk_GivesPacketsCorrectId()
        {
            Oneof_GameServerToPlayer bigMessage = getBigMessage();
            Oneof_GameServerToPlayer smallMessage = getSmolMessage();

            var updates = new List<Oneof_GameServerToPlayer> { smallMessage, bigMessage, smallMessage };

            var packets = MessageChunker.Chunk(updates, 9u);
            Assert.IsTrue(packets.Count > 0);
            for (int i = 0; i < packets.Count; i++)
            {
                Assert.AreEqual(9u + (uint)i, packets[i].Id);
            }
        }
    }
}