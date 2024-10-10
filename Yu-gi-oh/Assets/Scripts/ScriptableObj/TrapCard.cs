using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "TrapCard")]
public class TrapCard : Card
{
	public TrapCardtype traptype;

	public override void CopyProperties(Card other)
	{
		if (other is TrapCard trapCard)
		{
			base.CopyProperties(trapCard);
			this.traptype = trapCard.traptype;
		}
	}
}
