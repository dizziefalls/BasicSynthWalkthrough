using System.Media;

namespace BasicSynthesizer
{
    public partial class BasicSynthesizer : Form
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;
        public BasicSynthesizer()
        {
            InitializeComponent();
        }

        private void BasicSynthesizer_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BasicSynthesizer_KeyDown(object sender, KeyEventArgs e)
        {
            // Sine wave sample creation
            short[] wave  = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)];
            float frequency = 240f;
            for (int i = 0; i < SAMPLE_RATE; i++)
            {
                wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i));  
            }
            //byte conversion
            Buffer.BlockCopy(wave, 0, binaryWave, 0, binaryWave.Length);

            // wave file formatting
            using (MemoryStream ms = new())
            using (BinaryWriter bw = new(ms))
            {
                // num of bytes for one sample all channels inclusive
                short blockAlign = BITS_PER_SAMPLE / 8;
                // num samples * num channels * block align
                int subChunkTwoSize = SAMPLE_RATE * 1 * blockAlign;

                //*****WRITE FORMAT*****//

                //-----RIFF HEADER-----//
                // ChunkID
                bw.Write(new[] { 'R', 'I', 'F', 'F' });
                // ChunkSize
                bw.Write(36 + subChunkTwoSize);
                // Format
                bw.Write(new[] { 'W', 'A', 'V', 'E' });

                //-----fmt subchunk-----//
                // Subchunk1ID
                bw.Write(new[] { 'f', 'm', 't', ' ' });
                // Subchunk1Size
                bw.Write(16);
                // AudioFormat
                bw.Write((short)1);
                // NumChannels
                bw.Write((short)1);
                // SampleRate
                bw.Write(SAMPLE_RATE);
                // ByteRate
                bw.Write(SAMPLE_RATE * 1 * blockAlign);
                // BlockAlign
                bw.Write(blockAlign);
                // BitsPerSample
                bw.Write(BITS_PER_SAMPLE);

                //-----data subchunk-----//

                // Subchunk2Id
                bw.Write(new[] { 'd', 'a', 't', 'a' });
                // Subchunk2Size
                bw.Write(subChunkTwoSize);
                // Data --binary write requires a byte array
                bw.Write(binaryWave);

                // reset mem stream
                ms.Position = 0;
                new SoundPlayer(ms).Play();
            }

        }
    }
}