
public enum Page
{
	Draw,
	Standby,
	Main1,
	Battle,
	Main2,
	End,
	Any
}
public enum Turn
{
	Twelve,
	Six
}
public enum CardPosition
{
	Hand,
	MainDeck,
	ExtraDeck,
	Grave,
	Exiled,
	MonsterField,
	MagicField,
	FieldMagic,
	Unknown
}
public enum SummonMethod
{
	Normal,
	Special,
	Ritual,
	Fusion,
	Synchro,
	XYZ,
	Pendulum,
	Link,
	Set,
	Null
}
public enum PageTime
{
	Start,
	OnGoing,
	End
}
public enum PlayerType
{
	User,
	Ai
}
public enum MonsterCardType
{
	Spellcaster, Warrior, Beast_Warrior, Dinosaur, Beast, Aqua,
	Zombie
}
public enum Attribute
{
	DARK, EARTH, WATER
}
public enum Archetype
{
	Normal, Effect
}
public enum MagicCardtype
{
	Normal, Equip
}
public enum TrapCardtype
{
	Normal
}
public enum WhatWork
{
	Summon,
	Effect,
	Reverse,
	SetMagicTrap,
	ChainEffect,
	Null,
	Attack,
	EffectFromHand,
	ChooseTarget
}
public enum Formation
{
	Attack,
	Defence,
	Set
}
public enum Timing
{
	Summond,
	MainPage,
	AnyTime,
	Normal
}
public enum Trigger
{
	None,
	Any,
	NormalSummon,
	SpecialSummon,
	EffectActivated,
	Attack
}
public enum DeathReason
{
	BattleDestory,
	EffectDestroy,
	SentToGraveByEffect,
	SentToGraveBySummon
}

public enum TargetType
{
	Monster,
	Magic,
	Trap,
	FieldMagic,
	AnyThing
}

public enum TargetOwner
{
	Mine,
	Enemy,
	Both
}
