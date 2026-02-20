using UnityEngine;

public class ParticleDuplicator : MonoBehaviour
{
    private ParticleSystem ps;

    private ParticleSystem.Particle[] particles;

    public void Activate(bool includeBlack, Vector2 posOffset, int dusterType = 0)
    {
        string path = (new string[2] { "vfx/MonsterDuster", "vfx/KnightDuster" })[dusterType];
        ps = Object.Instantiate(Resources.Load<GameObject>(path)).GetComponent<ParticleSystem>();
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        particles = new ParticleSystem.Particle[sprite.texture.width * sprite.texture.height];
        ps.Emit(particles.Length);
        ps.GetParticles(particles);
        RectTransform rectTransform = ((!(GetComponent<RectTransform>() == null)) ? GetComponent<RectTransform>() : base.gameObject.AddComponent<RectTransform>());
        float num = 1f / 24f;
        int num2 = 0;
        for (int i = 0; i < sprite.texture.height; i++)
        {
            float num3 = (float)(sprite.texture.height - i) / (float)sprite.texture.height;
            for (int j = 0; j < sprite.texture.width; j++)
            {
                Color pixel = sprite.texture.GetPixel(j, i);
                if (pixel.a != 0f && (pixel.r + pixel.b + pixel.g != 0f || includeBlack))
                {
                    particles[num2].position = new Vector3((float)j * num - rectTransform.rect.width / 2f + base.transform.position.x + posOffset.x, (float)i * num + base.transform.position.y + posOffset.y, 0f);
                    particles[num2].startColor = pixel;
                    particles[num2].startSize = num;
                    particles[num2].startLifetime = num3 * 1.5f + Random.value * 0.3f;
                    particles[num2].startLifetime = particles[num2].startLifetime;
                    num2++;
                }
            }
        }
        ps.SetParticles(particles, num2);
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Activate(int dusterType = 0)
    {
        Activate(includeBlack: false, Vector2.zero, dusterType);
    }
}
