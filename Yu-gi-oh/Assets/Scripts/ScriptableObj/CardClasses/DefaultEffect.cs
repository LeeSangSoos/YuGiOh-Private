public class DefaultEffect : IEffect
{
	public override bool TargetCondition(Card card, Card target)
	{
		return true;
	}
	public override bool EffectCondition(Card card)
	{
		return false;
	}
	public override void Effect(Card card) { }
	public override Card AutoTargetFunction(Card card)
	{
		return null;
	}
	public override void RemovePassiveEffect(Card card)
	{
		return;
	}
}
