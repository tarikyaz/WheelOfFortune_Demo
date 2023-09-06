using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class PickerWheel : MonoBehaviour
{
    // References to various game objects and components
    [Header("References :")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform linesParent;

    [Space]
    [SerializeField] private Transform PickerWheelTransform;
    [SerializeField] private Transform wheelCircle;
    [SerializeField] private GameObject wheelPiecePrefab;
    [SerializeField] private Transform wheelPiecesParent;

    [Space]
    [Header("Sounds :")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tickAudioClip;
    [SerializeField, Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField, Range(-3f, 3f)] private float pitch = 1f;

    [Space]
    [Header("Picker wheel settings :")]
    [Range(1, 20)] public int spinDuration = 8;
    [SerializeField, Range(0.2f, 2f)] private float wheelSize = 1f;

    [Space]
    private WheelPiece[] wheelPieces;
    [SerializeField] int MaxNumber = 8;

    private bool _isSpinning = false;

    public bool IsSpinning { get { return _isSpinning; } }

    private Vector2 pieceMinSize = new Vector2(81f, 146f);
    private Vector2 pieceMaxSize = new Vector2(144f, 213f);
    private int piecesMin = 2;
    private int piecesMax = 12;
    private float pieceAngle;
    private float halfPieceAngle;
    private float halfPieceAngleWithPaddings;

    private void Start()
    {
        // Initialize the picker wheel and set up audio
        wheelPieces = new WheelPiece[MaxNumber];
        pieceAngle = 360f / wheelPieces.Length;
        halfPieceAngle = pieceAngle / 2f;
        halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);

        Generate();
        SetupAudio();
    }

    private void SetupAudio()
    {
        // Configure audio settings for the wheel spinning sound
        audioSource.clip = tickAudioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }

    private void Generate()
    {
        // Generate the wheel pieces and lines
        wheelPiecePrefab = InstantiatePiece();

        RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
        float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
        float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

        for (int i = 0; i < wheelPieces.Length; i++)
            DrawPiece(i);

        Destroy(wheelPiecePrefab);
    }

    private void DrawPiece(int index)
    {
        // Draw a piece on the wheel
        Transform pieceTrns = InstantiatePiece().transform.GetChild(0);

        pieceTrns.GetChild(0).GetComponent<TMP_Text>().text = (index + 1).ToString();

        // Line
        Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent).transform;
        lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

        pieceTrns.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
    }

    private GameObject InstantiatePiece()
    {
        // Instantiate a new wheel piece
        return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
    }

    public void Spin(int index, Action onFinish)
    {
        if (!_isSpinning)
        {
            _isSpinning = true;

            if (index < 0)
            {
                index = GetRandomPieceIndex();
            }
            Debug.Log("Spinning to number " + (index + 1));

            WheelPiece piece = wheelPieces[index];

            // Calculate the rotation angle and spin the wheel
            float angle = -(pieceAngle * index);
            float rightOffset = (angle - halfPieceAngleWithPaddings) % 360;
            float leftOffset = (angle + halfPieceAngleWithPaddings) % 360;

            float randomAngle = UnityEngine.Random.Range(leftOffset, rightOffset);

            Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration);

            float prevAngle, currentAngle;
            prevAngle = currentAngle = wheelCircle.eulerAngles.z;

            bool isIndicatorOnTheLine = false;

            // Rotate the wheel to the target angle with audio feedback
            wheelCircle
                .DORotate(targetRotation, spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuart)
                .OnUpdate(() =>
                {
                    float diff = Mathf.Abs(prevAngle - currentAngle);
                    if (diff >= halfPieceAngle)
                    {
                        if (isIndicatorOnTheLine)
                        {
                            audioSource.PlayOneShot(audioSource.clip);
                        }
                        prevAngle = currentAngle;
                        isIndicatorOnTheLine = !isIndicatorOnTheLine;
                    }
                    currentAngle = wheelCircle.eulerAngles.z;
                })
                .OnComplete(() =>
                {
                    _isSpinning = false;
                    onFinish?.Invoke();
                });
        }
    }

    private int GetRandomPieceIndex()
    {
        // Get a random index for the wheel piece
        return UnityEngine.Random.Range(0, wheelPieces.Length);
    }

    private void OnValidate()
    {
        if (PickerWheelTransform != null)
            PickerWheelTransform.localScale = new Vector3(wheelSize, wheelSize, 1f);

        if (MaxNumber > piecesMax || MaxNumber < piecesMin)
            Debug.LogError("[ PickerWheel ] Pieces length must be between " + piecesMin + " and " + piecesMax);
    }
}
