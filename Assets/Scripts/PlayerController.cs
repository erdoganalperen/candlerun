using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerDirection
{
    Left,
    Center,
    Right
}

public class PlayerController : MonoBehaviour
{
    Vector2 _firstPressPos;
    Vector2 _secondPressPos;

    Vector2 _currentSwipe;

    //
    private PlayerDirection _currentDirection;
    private Vector3 _targetPosition;
    private Vector3 _targetScale;
    public Vector3 _maxScale;
    public Vector3 _minScale;
    public float speed;
    private bool _isGameStarted;
    private bool _isGameFinished;
    [SerializeField] private Slider _scaleSlider;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _playText;
    [SerializeField] private Text _restartText;

    void Start()
    {
        _targetScale = transform.localScale;
        _currentDirection = PlayerDirection.Center;
        _isGameStarted = false;
        _isGameFinished = false;
        _scaleSlider.maxValue = _maxScale.x;
        _scaleSlider.minValue = _minScale.x;
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isGameStarted = true;
                _playText.gameObject.SetActive(false);
                //close tap to play ui
            }

            return;
        }

        if (_isGameFinished)
        {
            FinishGame();
            return;
        }

        Swipe();
    }

    void FixedUpdate()
    {
        if (!_isGameStarted)
        {
            return;
        }

        if (_isGameFinished)
        {
            return;
        }

        #region Transform

        MoveForward();
        SetPositionToTargetPosition();
        SetScaleToTargetScale();

        #endregion
    }

    private void MoveForward()
    {
        _targetPosition += Vector3.forward * (Time.fixedDeltaTime * speed);
    }

    void SetScaleToTargetScale()
    {
        transform.localScale = _targetScale;
        _scaleSlider.value = transform.localScale.x;
    }

    private void SetPositionToTargetPosition()
    {
        var pos = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10);
        pos.y = transform.position.y;
        transform.position = pos;
    }

    private void Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);
            _currentSwipe.Normalize();
            //swipe left
            if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
            {
                if (_currentDirection == PlayerDirection.Left)
                {
                    return;
                }

                _currentDirection = _currentDirection.Previous();
                _targetPosition += Vector3.left;
            }

            //swipe right
            if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
            {
                if (_currentDirection == PlayerDirection.Right)
                {
                    return;
                }

                _currentDirection = _currentDirection.Next();
                _targetPosition += Vector3.right;
            }
        }
    }

    private float _scaleSpeed = 2; // higher of this value, slower scale change speed 

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Candle>() != null)
        {
            var ratio = (Time.deltaTime / _scaleSpeed) * (_maxScale - transform.localScale);
            _targetScale += ratio;
        }
        else if (other.GetComponent<Lava>() != null)
        {
            var ratio = (transform.localScale - _minScale) * Time.deltaTime / _scaleSpeed;
            _targetScale -= ratio;
        }
        else if (other.GetComponent<FinishPlane>() != null)
        {
            _isGameFinished = true;
        }
    }

    int score = 0;

    void FinishGame()
    {
        if (Mathf.Abs(transform.localScale.x - _minScale.x) > .05f)
        {
            var scale = Vector3.Lerp(transform.localScale, _minScale, Time.deltaTime);
            transform.localScale = scale;
            _scaleSlider.value = scale.x;
            score += 1;
            _scoreText.text = score.ToString();
            MoveForward();
            SetPositionToTargetPosition();
        }
        else
        {
            if (_scaleSlider.value != _scaleSlider.minValue)
            {
                _scaleSlider.value = _scaleSlider.minValue;
            }

            if (_restartText.gameObject.activeSelf!=true)
            {
                _restartText.gameObject.SetActive(true);
            }
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

     
    }
}

public static class Extensions
{
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[]) Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
        T[] Arr = (T[]) Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) - 1;
        return (j <= 0) ? Arr[0] : Arr[j];
    }
}