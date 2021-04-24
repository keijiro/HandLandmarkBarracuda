using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MediaPipe.HandLandmark {

//
// Extension helper class for HandLandmarkDetector
//
public static class HandLandmarkDetectorExtensions
{
    #region Keypoint accessors

    public static Vector2 GetWrist(this HandLandmarkDetector detector)
      => detector.VertexArray.ElementAt(0);

    #endregion
}

} // namespace MediaPipe.HandLandmark
