using UnityEngine;
using Unity.Barracuda;

namespace MediaPipe.HandLandmark {

//
// ScriptableObject class used to hold references to internal assets
//
[CreateAssetMenu(fileName = "HandLandmark",
                 menuName = "ScriptableObjects/MediaPipe/Hand Landmark Resource Set")]
public sealed class ResourceSet : ScriptableObject
{
    public NNModel model;
    public ComputeShader preprocess;
    public ComputeShader postprocess;
}

} // namespace MediaPipe.HandLandmark
