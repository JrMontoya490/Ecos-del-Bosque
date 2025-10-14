using UnityEngine;
using UnityEngine.Video;

public class VideoDebug : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }
}
