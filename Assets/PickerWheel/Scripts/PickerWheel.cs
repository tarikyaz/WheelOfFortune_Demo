using UnityEngine ;
using DG.Tweening ;
using UnityEngine.Events ;
using TMPro;

public class PickerWheel : MonoBehaviour
{

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
    [SerializeField][Range(0f, 1f)] private float volume = .5f;
    [SerializeField][Range(-3f, 3f)] private float pitch = 1f;

    [Space]
    [Header("Picker wheel settings :")]
    [Range(1, 20)] public int spinDuration = 8;
    [SerializeField][Range(.2f, 2f)] private float wheelSize = 1f;

    [Space]
    private WheelPiece[] wheelPieces;
    [SerializeField] int MaxNumber = 8;
    // Events
    private UnityAction onSpinStartEvent;
    private UnityAction<WheelPiece> onSpinEndEvent;


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
        wheelPieces = new WheelPiece[MaxNumber];
        pieceAngle = 360 / wheelPieces.Length;
        halfPieceAngle = pieceAngle / 2f;
        halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle / 4f);

        Generate();
        SetupAudio();

    }

    private void SetupAudio()
    {
        audioSource.clip = tickAudioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }

    private void Generate()
    {
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
        Transform pieceTrns = InstantiatePiece().transform.GetChild(0);

        pieceTrns.GetChild(0).GetComponent<TMP_Text>().text = (index+1).ToString();

        //Line
        Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent).transform;
        lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);

        pieceTrns.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
    }

    private GameObject InstantiatePiece()
    {
        return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
    }


    public void Spin(int index = -1)
    {
        if (!_isSpinning)
        {
            _isSpinning = true;

            onSpinStartEvent?.Invoke();
            if (index < 0)
            {
                index = GetRandomPieceIndex();
            }
            Debug.Log("Spint to number" + (index + 1));

            WheelPiece piece = wheelPieces[index];


            float angle = -(pieceAngle * index);

            float rightOffset = (angle - halfPieceAngleWithPaddings) % 360;
            float leftOffset = (angle + halfPieceAngleWithPaddings) % 360;

            float randomAngle = Random.Range(leftOffset, rightOffset);

            Vector3 targetRotation = Vector3.back * (randomAngle + 2 * 360 * spinDuration);

            float prevAngle, currentAngle;
            prevAngle = currentAngle = wheelCircle.eulerAngles.z;

            bool isIndicatorOnTheLine = false;

            wheelCircle
            .DORotate(targetRotation, spinDuration, RotateMode.Fast)
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
                onSpinEndEvent?.Invoke(piece);

                onSpinStartEvent = null;
                onSpinEndEvent = null;
            });

        }
    }


    public void OnSpinStart(UnityAction action)
    {
        onSpinStartEvent = action;
    }

    public void OnSpinEnd(UnityAction<WheelPiece> action)
    {
        onSpinEndEvent = action;
    }


    private int GetRandomPieceIndex()
    {
        return Random.Range(0, wheelPieces.Length);
    }





    private void OnValidate()
    {
        if (PickerWheelTransform != null)
            PickerWheelTransform.localScale = new Vector3(wheelSize, wheelSize, 1f);

        if (MaxNumber > piecesMax || MaxNumber < piecesMin)
            Debug.LogError("[ PickerWheelwheel ]  pieces length must be between " + piecesMin + " and " + piecesMax);
    }
}
