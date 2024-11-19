using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Spine {
	public partial class SkeletonData {
        public readonly ExposedList<Timeline> SlotTimelines = new ExposedList<Timeline>();
        public readonly ExposedList<Timeline> BoneTimelines = new ExposedList<Timeline>();
        public readonly ExposedList<Timeline> IKConstraintTimelines = new ExposedList<Timeline>();
        public readonly ExposedList<Timeline> TransformConstraintTimelines = new ExposedList<Timeline>();
        public readonly ExposedList<Timeline> PathConstraintTimelines = new ExposedList<Timeline>();
        public readonly ExposedList<Timeline> PhysicsConstraintTimelines = new ExposedList<Timeline>();
        public readonly ExposedList<(int, Attachment, Timeline)> AttachmentTimelines = new ExposedList<(int, Attachment, Timeline)>();
        public readonly ExposedList<Timeline> DrawOrderTimelines = new ExposedList<Timeline>();
        public readonly ExposedList<Timeline> EventTimelines = new ExposedList<Timeline>();
   
        public class CurveFrameBezierData {
            public float cx1;
            public float cy1;
            public float cx2;
            public float cy2;
        }
        public class CurveFrameData {
            public int CurveType;
            public int BezierCount;
            public float FrameTime;
            public readonly List<float> ExtraValues = new List<float>();
            public readonly byte[] FrameExtraData;
            public readonly List<CurveFrameBezierData> CurveFrameBezierDatas= new List<CurveFrameBezierData>();
        }
        public readonly Dictionary<Timeline, ExposedList<CurveFrameData>> CurveFrameDatas = new Dictionary<Timeline, ExposedList<CurveFrameData>>();

        internal class StreamBytesScope : IDisposable
        {
            private Stream stream;
            private long initialPosition;
            private Action<byte[]> cb;
            public StreamBytesScope(Stream s, Action<byte[]> cb) {
                this.stream = s;
                this.cb = cb;
                initialPosition = s.Position;
            }

            public void Dispose()
            {
                var endPosition = stream.Position;
                stream.Position = initialPosition;
                var bs = new byte[endPosition-initialPosition];
                stream.Read(bs);
                stream.Position = initialPosition;
                cb(bs);
            }
        }
    }
}