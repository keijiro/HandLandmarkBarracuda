using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MediaPipe.HandLandmark {

//
// Public interface of the hand landmark detector class
//
public sealed partial class HandLandmarkDetector
{
    #region Accessor properties

    public const int ImageSize = 224;
    public const int VertexCount = 21;

    public ComputeBuffer OutputBuffer
      => _postBuffer;

    public IEnumerable<Vector4> VertexArray
      => PostReadCache.Skip(1);

    #endregion

    #region Public methods

    public HandLandmarkDetector(ResourceSet resources)
    {
        _resources = resources;
        AllocateObjects();
    }

    public void Dispose()
      => DeallocateObjects();

    public void ProcessImage(Texture image)
      => RunModel(Preprocess(image));

    public void ProcessImage(ComputeBuffer buffer)
      => RunModel(buffer);

    #endregion

    #region Key point retrieval

    public enum KeyPoint
    {
        Wrist,
        Thumb1,  Thumb2,  Thumb3,  Thumb4,
        Index1,  Index2,  Index3,  Index4,
        Middle1, Middle2, Middle3, Middle4,
        Ring1,   Ring2,   Ring3,   Ring4,
        Pinky1,  Pinky2,  Pinky3,  Pinky4
    }

    public Vector2 GetKeyPoint(KeyPoint point)
      => PostReadCache[(int)point + 1];

    #endregion

    #region Optional properties

    public float Score
      => PostReadCache[0].x;

    public float Handedness
      => PostReadCache[0].y;

    #endregion
}

} // namespace MediaPipe.HandLandmark
