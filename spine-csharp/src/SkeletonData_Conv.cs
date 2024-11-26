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
            public readonly Dictionary<string, object> FrameExtraObjects = new();
            public readonly List<CurveFrameBezierData> CurveFrameBezierDatas= new List<CurveFrameBezierData>();
        }
        public readonly Dictionary<Timeline, Dictionary<int, CurveFrameData>> CurveFrameDatas = new Dictionary<Timeline, Dictionary<int, CurveFrameData>>();

        private CurveFrameData GetCurveFrameData(Timeline tl, int frame) {
            if (!CurveFrameDatas.TryGetValue(tl, out var m))
                CurveFrameDatas[tl] = m = new Dictionary<int, CurveFrameData>();
            
            if (!m.TryGetValue(frame, out var fd))
                m[frame] = fd = new CurveFrameData();
            return fd;
        }
        
        internal void RecordFrameExtraObject<T>(Timeline tl, int frame, Dictionary<string, T> map, string key, T defaultValue=default(T)) {
            if (map.TryGetValue(key, out var v) && !v.Equals(defaultValue))
                GetCurveFrameData(tl, frame).FrameExtraObjects[key] = v;
        }

        internal void RecordCurveStepped(Timeline tl, int frame) {
            GetCurveFrameData(tl, frame).CurveType = SkeletonBinary.CURVE_STEPPED;
        }
        internal void RecordCurveBezier(Timeline tl, int frame, float cx1, float cy1, float cx2, float cy2) {
            var fd = GetCurveFrameData(tl, frame);
            fd.CurveType = SkeletonBinary.CURVE_BEZIER;
            fd.CurveFrameBezierDatas.Add(new CurveFrameBezierData{
                cx1 = cx1,
                cy1 = cy1,
                cx2 = cx2,
                cy2 = cy2,
            });
        }

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