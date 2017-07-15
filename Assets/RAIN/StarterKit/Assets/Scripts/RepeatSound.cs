using UnityEngine;
using System.Collections;

public class RepeatSound : MonoBehaviour
{
    [SerializeField]
    private float _repeatFrequency = 0f;
    [SerializeField]
    private Color _blinkColor = Color.red;

    private float _timer = 0f;

    private Renderer _parentRenderer = null;
    private Color _origColor = Color.white;

    private int _blinkCount = 0;
    private float _blinkTimer = 0f;

    void Start()
    {
        if (_repeatFrequency < 0f)
            _repeatFrequency = 0f;

        _timer = _repeatFrequency - (UnityEngine.Random.Range(0f, _repeatFrequency) + (_repeatFrequency / 2f));

        _parentRenderer = transform.parent.GetComponent<Renderer>();
        _origColor = _parentRenderer.material.color;
    }

    void Update()
    {
        if (_repeatFrequency <= 0f)
            return;

        if (_timer >= _repeatFrequency)
        {
            AudioSource tAlarm = GetComponent<AudioSource>();
            if (tAlarm != null)
            {
                tAlarm.Stop();
                tAlarm.Play();

                _blinkCount = 3;
                _blinkTimer = 0.5f;
            }

            _timer = _repeatFrequency - (UnityEngine.Random.Range(0f, _repeatFrequency) + (_repeatFrequency / 2f));
        }

        if (_blinkCount > 0)
        {
            if (_parentRenderer != null)
            {
                if (_blinkTimer > 0)
                    _parentRenderer.material.color = _blinkColor;
                else
                    _parentRenderer.material.color = _origColor;

                _blinkTimer -= Time.deltaTime;
                if (_blinkTimer < -0.5f)
                {
                    _blinkTimer += 1f;
                    _blinkCount -= 1;

                    if (_blinkCount == 0)
                        _parentRenderer.material.color = _origColor;
                }
            }
            else
                _blinkCount = 0;
        }

        _timer += Time.deltaTime;
    }
}
