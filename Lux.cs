using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lux : Controller_Agent
{
    Light2D fireflyLight;

    float _min = 0.6f;
    float _max = 0.9f;
    float _duration = 2f;

    private float timer = 0f;

    protected override void Start()
    {
        base.Start();

        fireflyLight = GetComponent<Light2D>();
    }

    protected override void SubscribeToEvents()
    {
        Manager_Dialogue.Instance.luxIntroEvent?.AddListener(LuxIntro);
    }

    protected override void Update()
    {
        base.Update();

        _flicker();
    }

    void _flicker()
    {
        timer += Time.deltaTime;

        float lerpRatio = (Mathf.Sin(timer / _duration * Mathf.PI * 2f) + 1f) / 2f;

        fireflyLight.falloffIntensity = Mathf.Lerp(_min, _max, lerpRatio);
        fireflyLight.pointLightOuterRadius = Mathf.Lerp(_max, _min, lerpRatio);

        if (timer > _duration) timer = 0f;
    }

    void OnDestroy()
    {
        Manager_Dialogue.Instance.luxIntroEvent?.RemoveListener(LuxIntro);
    }

    void LuxIntro()
    {
        WanderAroundUrsus();
    }

    void WanderAroundUrsus()
    {
        SetAgentDetails(targetGO: Manager_Game.Instance.Player.gameObject);
    }
}
