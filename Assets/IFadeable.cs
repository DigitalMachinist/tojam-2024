public interface IFadeable
{
    public void FadeIn(float delay, float targetAlpha = 1f, bool useUnscaledTime = true);
    public void FadeOut(float delay, float targetAlpha = 0f, bool useUnscaledTime = true);
}
