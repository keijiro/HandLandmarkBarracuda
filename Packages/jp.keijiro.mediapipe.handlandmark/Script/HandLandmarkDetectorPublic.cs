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

    public ComputeBuffer InputBuffer
      => _preprocess.Buffer;

    public bool InputIsNCHW
      => _preprocess.IsNCHW;

    public GraphicsBuffer OutputBuffer
      => _output;

    public System.ReadOnlySpan<Vector4> VertexArray
      => _readCache.Cached.Slice(1);

    #endregion

    #region Public methods

    public HandLandmarkDetector(ResourceSet resources)
      => AllocateObjects(resources);

    public void Dispose()
      => DeallocateObjects();

    public void ProcessInput()
      => RunModel();

    public void ProcessImage(Texture image)
      => RunModel(image);

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
      => _readCache.Cached[(int)point + 1];

    #endregion

    #region Optional properties

    public float Score
      => _readCache.Cached[0].x;

    public float Handedness
      => _readCache.Cached[0].y;

    #endregion
}

} // namespace MediaPipe.HandLandmark
