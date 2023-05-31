using UnityEngine;

public class FireLight : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private float _minIntensity = 0.5f;
    [SerializeField] private float _maxIntensity = 1.5f;
    [SerializeField] private float _flickerSpeed = 0.1f;

    private float _targetIntensity;

    private void Start()
    {
        // Set the initial target intensity
        _targetIntensity = Random.Range(_minIntensity, _maxIntensity);
    }

    private void Update()
    {
        // Lerp the light intensity towards the target intensity
        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, _flickerSpeed * Time.deltaTime);

        // If the light intensity is close to the target intensity, set a new target intensity
        if (Mathf.Abs(_light.intensity - _targetIntensity) < 0.1f)
        {
            _targetIntensity = Random.Range(_minIntensity, _maxIntensity);
        }
    }
}