using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;

namespace Schema
{
    public static class MessageChunker
    {
        public const int ChunkSize = 512;
        private static byte[] buffer = new byte[ChunkSize];

        public static List<Schema.Packet> Chunk(List<Oneof_GameServerToPlayer> updates, uint currentVersion)
        {
            List<Schema.Packet> packets = new List<Schema.Packet>();
            Schema.Packet currentPacket = new Schema.Packet();
            int currentPacketSize = 0;

            foreach (Oneof_GameServerToPlayer update in updates)
            {
                byte[] bytes = update.ToByteArray();
                int totalChunks = (int)Math.Ceiling((double)bytes.Length / ChunkSize);
                for (int i = 0; i < totalChunks; i++)
                {
                    int chunkSize = Math.Min(ChunkSize, bytes.Length - i * ChunkSize);

                    Buffer.BlockCopy(bytes, i * ChunkSize, buffer, 0, chunkSize);

                    var chunk = new Schema.Chunk
                    {
                        Index = i,
                        MaxIndex = totalChunks - 1,
                        Data = Google.Protobuf.ByteString.CopyFrom(buffer, 0, chunkSize)
                    };

                    if (currentPacketSize + chunkSize > ChunkSize)
                    {
                        packets.Add(currentPacket);
                        currentPacket = new Schema.Packet();
                        currentPacketSize = 0;
                    }

                    currentPacket.Chunks.Add(chunk);
                    currentPacketSize += chunkSize;
                }
            }

            if (currentPacket.Chunks.Count > 0)
            {
                packets.Add(currentPacket);
            }

            foreach (var packet in packets)
            {
                packet.Id = currentVersion;
                currentVersion++;
            }

            return packets;
        }

        public static Oneof_GameServerToPlayer? ExtractFullUpdate(ref List<Schema.Packet> packets)
        {
            int? updateEndPacketIndex = null;
            int? updateEndChunkIndex = null;

            bool shouldBail = false;
            for (int i = 0; i < packets.Count; i++)
            {
                var packet = packets[i];
                if (packet == null)
                {
                    break;
                }

                for (int c = 0; c < packet.Chunks.Count; c++)
                {
                    if (packet.Chunks[c].Index == packet.Chunks[c].MaxIndex)
                    {
                        updateEndPacketIndex = i;
                        updateEndChunkIndex = c;
                        shouldBail = true;
                        break;
                    }
                }

                if (shouldBail)
                {
                    break;
                }
            }

            if (updateEndPacketIndex == null || updateEndChunkIndex == null)
            {
                return null;
            }

            List<Schema.Chunk> chunks = new List<Schema.Chunk>();
            for (int i = 0; i <= updateEndPacketIndex; i++)
            {
                if (i < updateEndPacketIndex)
                {
                    chunks.AddRange(packets[i]!.Chunks);
                }
                else if (i == updateEndPacketIndex)
                {
                    chunks.AddRange(packets[i]!.Chunks.Take((int)updateEndChunkIndex + 1));

                    for (int j = 0; j <= updateEndChunkIndex; j++)
                    {
                        packets[i]!.Chunks.RemoveAt(0);
                    }
                }
            }

            int numPacketsToRemove =
                (int)updateEndPacketIndex +
                (packets[updateEndPacketIndex.Value]!.Chunks.Count == 0 ? 1 : 0);
            packets.RemoveRange(0, numPacketsToRemove);

            byte[] data = ReassembleUpdateData(chunks);
            return Oneof_GameServerToPlayer.Parser.ParseFrom(data);
        }

        private static byte[] ReassembleUpdateData(List<Schema.Chunk> chunks)
        {
            int totalSize = chunks.Sum(c => c.Data.Length);
            byte[] data = new byte[totalSize];

            int currentPosition = 0;
            foreach (var chunk in chunks.OrderBy(c => c.Index))
            {
                var chunkBytes = chunk.Data.ToByteArray();
                Buffer.BlockCopy(chunkBytes, 0, data, currentPosition, chunkBytes.Length);
                currentPosition += chunkBytes.Length;
            }

            return data;
        }
    }
}