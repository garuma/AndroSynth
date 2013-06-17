using System;
using System.Threading;
using System.Collections.Concurrent;
using Android.Media;

using Log = Android.Util.Log;

namespace AndroSynth
{
	public class AsyncPlayer
	{
		short[] soundBuffer;
		AudioTrack soundTrack;
		Thread processThread;
		SineWaveGenerator generator;

		public AsyncPlayer (SineWaveGenerator generator)
		{
			this.generator = generator;

			var streamType = Stream.Music;
			var sampleRate = AudioTrack.GetNativeOutputSampleRate (streamType);
			var encoding = Android.Media.Encoding.Pcm16bit;
			var bufferSize = AudioTrack.GetMinBufferSize (sampleRate, ChannelOut.Mono, encoding) + 2;

			Log.Info ("AudioInit", "{0} / {1}", sampleRate, bufferSize);

			soundTrack = new AudioTrack (streamType,
			                             sampleRate,
			                             ChannelConfiguration.Mono,
			                             encoding,
			                             bufferSize,
			                             AudioTrackMode.Stream);
			soundBuffer = new short [bufferSize];

			processThread = new Thread (ThreadStart);
		}

		public void Play ()
		{
			processThread.Start ();
			soundTrack.Play ();
		}

		void ThreadStart ()
		{
			while (true) {
				soundTrack.Write (soundBuffer, 0, soundBuffer.Length);
				generator.FillBuffer (soundBuffer);
			}
		}
	}
}

