public abstract class StatusEffect
{
    public abstract void Apply();

}

public class StatChangeEffect : StatusEffect
{
    public int Attack { get; set; }
    public int Defense { get; set; }

    public override void Apply()
    {
        
    }
}