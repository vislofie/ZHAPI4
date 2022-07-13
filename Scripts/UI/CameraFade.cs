using UnityEngine;

public class CameraFade : MonoBehaviour
{
    [SerializeField]
    private Camera _foregroundCamera;

    [SerializeField]
    private float _speedScale = 1f;
    private Color _fadeColor = Color.black;

    // Rather than Lerp or Slerp, we allow adaptability with a configurable curve
    [SerializeField]
    private AnimationCurve _curve = new AnimationCurve(new Keyframe(0, 1),
        new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));

    [SerializeField]
    private bool _startFadedOut = false;

    private float _alpha = 0f;
    private Texture2D _texture;
    private int _direction = 0;
    private float _time = 0f;

    private void Start()
    {
        if (_startFadedOut) _alpha = 1f; else _alpha = 0f;
        _texture = new Texture2D(1, 1);
        _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
        _texture.Apply();
    }

    public void OnGUI()
    {
        if (_alpha > 0f) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
        if (_direction != 0)
        {
            _time += _direction * Time.deltaTime * _speedScale;
            _alpha = _curve.Evaluate(_time);
            _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
            _texture.Apply();
            if (_alpha <= 0f || _alpha >= 1f) _direction = 0;
        }

        if (Event.current.type == EventType.Repaint)
            _foregroundCamera.Render();

    }

    public void FadeIn()
    {
        if (_alpha < 1f)
        {
            _alpha = 0f;
            _time = 1f;
            _direction = -1;
        }
    }

    public void FadeOut()
    {
        if (_alpha >= 1f)
        {
            _alpha = 1f;
            _time = 0f;
            _direction = 1;
        }
    }
}