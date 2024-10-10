public abstract class IEffect
{
	public Trigger trigger = Trigger.None;
	public abstract bool TargetCondition(Card card, Card target);
	public abstract bool EffectCondition(Card card);
	public abstract void Effect(Card card);
	public abstract Card AutoTargetFunction(Card card);
	public abstract void RemovePassiveEffect(Card card);
}
