using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace BadPiggiesMP {
    /// <summary>
    /// TcpClient with extra methods for reading protocol values.
    /// </summary>
    public class BPStream : IDisposable {
        //constants for varint/varlong decoding
        private const int SegmentBits = 0x7F;
        private const int ContinueBit = 0x80;

        private readonly Stream backingStream;

        public BPStream(Stream backingStream) {
            this.backingStream = backingStream;
        }

        public byte ReadByte() {
            //the "byte" is an int, so it can be -1 when it's the end of the stream
            int byteVal = backingStream.ReadByte();

            if (byteVal == -1) {
                throw new IOException("Tried to ReadByte at end of stream!");
            }

            return (byte)byteVal;
        }

        public void WriteByte(PacketTypes.ClientPackets val) {
            backingStream.WriteByte((byte)val);
        }

        public void WriteByte(int val) {
            backingStream.WriteByte((byte)val);
        }

        public short ReadShort() {
            //a short is 2 bytes, so create a 2byte arr
            byte[] bytes = new byte[2];
            //read 2 bytes into the array
            backingStream.ReadExactly(bytes, 0, bytes.Length);
            //and return the bytes as a short
            return BitConverter.ToInt16(bytes, 0);
        }

        public void WriteShort(short val) {
            //get the short's bytes
            byte[] bytes = BitConverter.GetBytes(val);
            //and write the bytes
            backingStream.Write(bytes, 0, bytes.Length);
        }

        public int ReadInt() {
            //an int is 4 bytes, so create a 4byte arr
            byte[] bytes = new byte[4];
            //read 4 bytes into the array
            backingStream.ReadExactly(bytes, 0, bytes.Length);
            //and return the bytes as a short
            return BitConverter.ToInt32(bytes, 0);
        }

        public void WriteInt(int val) {
            //get the ints bytes
            byte[] bytes = BitConverter.GetBytes(val);
            //and write the bytes
            backingStream.Write(bytes, 0, bytes.Length);
        }

        public float ReadFloat() {
            //an int is 4 bytes, so create a 4byte arr
            byte[] bytes = new byte[4];
            //read 4 bytes into the array
            backingStream.ReadExactly(bytes, 0, bytes.Length);
            //and return the bytes as a short
            return BitConverter.ToSingle(bytes, 0);
        }

        public void WriteFloat(float val) {
            //get the float's bytes
            byte[] bytes = BitConverter.GetBytes(val);
            //and write the bytes
            backingStream.Write(bytes, 0, bytes.Length);
        }

        public double ReadDouble() {
            //an int is 4 bytes, so create a 4byte arr
            byte[] bytes = new byte[8];
            //read 8 bytes into the array
            backingStream.ReadExactly(bytes, 0, bytes.Length);
            //and return the bytes as a short
            return BitConverter.ToDouble(bytes, 0);
        }

        public void WriteDouble(double val) {
            //get the double's bytes
            byte[] bytes = BitConverter.GetBytes(val);
            //and write the bytes
            backingStream.Write(bytes, 0, bytes.Length);
        }

        //variable length integer reading code
        public int ReadVarInt() {
            int value = 0;
            int position = 0;
            byte currentByte;

            while (true) {
                currentByte = ReadByte();
                value |= (currentByte & SegmentBits) << position;

                if ((currentByte & ContinueBit) == 0) break;

                position += 7;

                if (position >= 32) throw new IOException("VarInt is too big");
            }

            return value;
        }

        //variable length integer writing code
        public void WriteVarInt(int val) {
            while (true) {
                if ((val & ~SegmentBits) == 0) {
                    WriteByte(val);
                    return;
                }

                WriteByte((val & SegmentBits) | ContinueBit);

                //Note: >>> means that the sign bit is shifted with the rest of the number rather than being left alone
                val >>>= 7;
            }
        }

        public Vector2 ReadVec2() {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        public void WriteVec2(Vector2 vec) {
            WriteFloat(vec.X);
            WriteFloat(vec.Y);
        }

        public string ReadString() {
            //read length
            int length = ReadVarInt();

            //create string byte arr
            byte[] strBytes = new byte[length];

            //ACTUALLY READ THE STRING...
            backingStream.ReadExactly(strBytes, 0, length);

            //return bytes as str
            return Encoding.UTF8.GetString(strBytes);
        }

        public void WriteString(string str) {
            //get str bytes
            byte[] strBytes = Encoding.UTF8.GetBytes(str);

            //write length
            WriteVarInt(strBytes.Length);

            //write bytes
            backingStream.Write(strBytes, 0, strBytes.Length);
        }

        public void WriteBytes(byte[] buffer, int offset, int count)
        {
            backingStream.Write(buffer, offset, count);
        }

        public void Dispose() {
            backingStream.Dispose();
        }
    }
}