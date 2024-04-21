using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.ImgprocModule;
using System.Collections;

public class FaceTracker : MonoBehaviour
{
    private CascadeClassifier cascadeClassifier;
    private WebCamTexture webCamTexture;

    public Transform cameraTarget;
    private Vector3 originalCameraOffset;
    public Vector2 facePosition { get; private set; }
    public float movementSpeed = 0.05f;
    public float detectionFrequency = 0.1f;

    private void Start()
    {
        if (cameraTarget == null)
        {
            GameObject spaceshipObject = GameObject.Find("Ship");
            if (spaceshipObject != null)
            {
                Transform cameraTargetTransform = spaceshipObject.transform.Find("CameraTarget");
                if (cameraTargetTransform != null)
                {
                    cameraTarget = cameraTargetTransform;
                }
                else
                {
                    Debug.LogError("Camera target GameObject not found under the spaceship GameObject. Make sure it's named correctly.");
                    return;
                }
            }
            else
            {
                Debug.LogError("Spaceship GameObject not found in the scene. Make sure it's named correctly.");
                return;
            }
        }

        // Store the original offset of the camera from the spaceship
        originalCameraOffset = transform.position - cameraTarget.position;

        cascadeClassifier = new CascadeClassifier();
        string cascadeFilePath = Application.streamingAssetsPath + "/haarcascade_frontalface_alt.xml";
        Debug.Log("Cascade classifier file path: " + cascadeFilePath);

        try
        {
            if (cascadeClassifier.load(cascadeFilePath))
            {
                Debug.Log("Cascade classifier loaded successfully.");
            }
            else
            {
                throw new System.Exception("Failed to load cascade classifier.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading cascade classifier: " + e.Message);
            return;
        }

        webCamTexture = new WebCamTexture();
        if (!webCamTexture.isPlaying)
        {
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("Failed to start webcam texture.");
        }

        StartCoroutine(DetectFaces());
    }

    private IEnumerator DetectFaces()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionFrequency);

            if (webCamTexture == null || !webCamTexture.isPlaying)
            {
                Debug.LogWarning("Webcam texture is not ready or not playing.");
                continue;
            }

            Mat frame = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
            Utils.webCamTextureToMat(webCamTexture, frame);

            MatOfRect faces = new MatOfRect();
            cascadeClassifier.detectMultiScale(frame, faces);

            if (faces.total() > 0)
            {
                OpenCVForUnity.CoreModule.Rect faceRect = faces.toArray()[0];
                float faceCenterX = faceRect.x + faceRect.width / 2f;
                float faceCenterY = webCamTexture.height - (faceRect.y + faceRect.height / 2f);
                facePosition = new Vector2(faceCenterX, faceCenterY);

                float faceMovementX = faceCenterX - webCamTexture.width / 2f;
                float faceMovementY = faceCenterY - webCamTexture.height / 2f;

                // Calculate the movement direction based on face movement
                Vector3 movementDirection = new Vector3(faceMovementX, faceMovementY, 0f).normalized;

                // Calculate the target position based on the spaceship's position and movement direction
                Vector3 targetPosition = transform.position + originalCameraOffset + movementDirection * movementSpeed;

                // Move the spaceship towards the target position
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
            }
        }
    }
}
