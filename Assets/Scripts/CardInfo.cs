using System.Collections.Generic;

public enum CardRelation {
	PairL, PairR, Summon
}

public class CardInfo {
	
	public enum CardType {
		Monster, Auxiliary, Ruse, Unique
	}	
	
	public enum CardAffinity {
		Darkness, Death, Dragon, Fire, Forest, Ice, Insect, Light, Myth, Water, Wind
	}
	
	private int id;
    private string cname, desc;
    private CardType type;
	public Dictionary<CardRelation, CardInfo> AssoCardInfo { get; set; }
	public int ID { get {return id;} }
	public string Name { get {return cname;} }
	public CardType Type { get {return type;} }
	public string Desc { get {return desc;} }
	
	public CardInfo(int idPar, string namePar, CardType typePar, string descPar) {
		
		id = idPar;
		cname = namePar;
		type = typePar;
		desc = descPar;
		
		AssoCardInfo = new Dictionary<CardRelation, CardInfo>();
	}
	
	public void associateCardInfo(CardInfo ci, CardRelation rel) {
		
		AssoCardInfo.Add (rel, ci);
	}
}

public class MonsterInfo : CardInfo {
	
	private int level, attack, defense;
	private CardAffinity affinity;
	public int Level { get {return level;} }
	public int Attack { get {return attack;} }
	public int Defense { get {return defense;} }
	public CardAffinity Affinity { get {return affinity;} }
	
	public MonsterInfo(int idPar, string namePar, CardAffinity affinityPar, CardType typePar,
	    int levelPar, int attackPar, int defensePar, string descPar ) : base(idPar, namePar, typePar, descPar) {
		
		affinity = affinityPar;
		level = levelPar;
		attack = attackPar;
		defense = defensePar;
	}
	
	public static MonsterInfo operator +(MonsterInfo ci1, MonsterInfo ci2) {
		MonsterInfo toReturn = new MonsterInfo (ci1.ID, ci1.Name, ci1.Affinity, ci1.Type, ci1.Level, ci1.Attack + ci2.Attack, ci1.Defense + ci2.Defense, ci1.Desc);
		foreach (KeyValuePair<CardRelation, CardInfo> kvp in ci1.AssoCardInfo) {
			toReturn.associateCardInfo(kvp.Value, kvp.Key);
		}
		return toReturn;
	}
	
	public static MonsterInfo operator +(MonsterInfo ci1, StatEffect se) {
		MonsterInfo toReturn = new MonsterInfo (ci1.ID, ci1.Name, ci1.Affinity, ci1.Type, ci1.Level, ci1.Attack + se.AttackEff, ci1.Defense + se.DefenseEff, ci1.Desc);
		foreach (KeyValuePair<CardRelation, CardInfo> kvp in ci1.AssoCardInfo) {
			toReturn.associateCardInfo(kvp.Value, kvp.Key);
		}
		return toReturn;
	}
}

public class SpecialInfo : CardInfo {
	
	public Dictionary<EffectTrigger, Effect>  Effects { get; set; }
	
	public SpecialInfo(int idPar, string namePar, CardType typePar, string descPar)
	: base(idPar, namePar, typePar, descPar) {
		
		Effects = new Dictionary<EffectTrigger, Effect>();
	}
	
	public SpecialInfo registerEffect (Effect a) {
		
		Effects.Add (a.Trigger, a);
		return this;
	}
}

public static class CardPool {

	public static CardInfo[] Cards = {
		
		new MonsterInfo(1, "Achilles", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 22, 11,
		    "The hero whose name gained immortality for his unimaginable feats.  His enemies quake upon hearing it.  " +
		    "His renowned infamy is not unearned.  It is best not to face him in combat if possible.  Those who dare " +
		    "challenge him will soon beg for mercy."),
		new MonsterInfo(2, "Aditya", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 3, 6, 14,
		    "Pray that you not be the one to wake her slumber.  Those dark of heart will falter under her gaze as " +
		    "vengeance is swiftly delivered upon any impure of heart who enter her domain."),
		new MonsterInfo(3, "Adnama Lavode", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 11, 4,
		    "A beast whose origins are unknown.  Every millennia it awakens, recreating the earth and building it " +
		    "anew.  Made from the heart of the earth itself, there are few who can survive meeting this creature."),
		new MonsterInfo(4, "Aerian Eater", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 8, 6,
		    "An envoy of darkness, the Aerian Eater forever seeks to quench the emptiness echoing in its soul.  It is" +
		    " said to be cursed for broken oaths of the past, but none dare to verify such as truth."),
		new MonsterInfo(5, "Agan", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 4, 10,
		    "Agan, a true guardian of the forest. Countless trespassers have fallen victim to its clever mind and " +
		    "fierce strength.  A wise traveller would do well not to underestimate it."),
		new MonsterInfo(6, "Age of Storms", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 1, 3, 4,
		    "The Age of Storms rages across the land, scorching the earth wherever it goes.  None know the source of" +
		    " its wrath, but all know to fear it."),
		new MonsterInfo(7, "Aginism", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 9, 3,
		    "More than a boy's best friend."),
		new MonsterInfo(8, "Aijiren", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 18, 15,
		    "Aijiren, a Pharaoh of old, refused to cede his rule.  Having found the key to true immortality, " +
		    "he now rules as a god over kings and queens.  He safeguards the true Book of Dead, using it to exact " +
		    "his might. "),
		new MonsterInfo(9, "Air Elemental", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 6, 27, 16,
		    "The source of all western, eastern, northern, and southern winds, the Air Elemental holds absolute " +
		    "control over the weather, bending it to its will.  Born of the air itself, there is little that can " +
		    "threaten this creature."),
		new MonsterInfo(10, "Air Kiho", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 20, 18,
		    "Asako Shugenja was the originator and only true master of Air Kiho.  Many believe her to be legend.  " +
		    "Others pray to her as a goddess.  This priestess' martial arts and air spells are unparalleled."),
		new MonsterInfo(11, "Ajanivengeant", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 15, 17,
		    "Ajanivengeant is a savage born of the forest's rage.  He desires only to test his blade against the " +
		    "strongest opponents.  When he wanders the forest, the hunter becomes the hunted."),
		new MonsterInfo(12, "Akantor", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 17, 3,
		    "This swordsman traded not only his soul, but that of his entire battalion in order to grasp " +
		    "powers forbidden to man.  Akantor now sits awaiting a challenger so that he might expand his power " +
		    "further still."),
		new MonsterInfo(13, "Akoum Dra", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 4, 15, 11,
		    "Akoum Dra is believed to have been born from a dragon himself and as such is the only true dragon rider " +
		    "to have existed.  Entire armies feared the black shadow of death he cast upon the field of battle."),
		new MonsterInfo(14, "Al Djinn", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 19, 13,
		    "Many believe Al Djinn to be a demon born from the hatred of mankind.  He wanders battle fields cleaving " +
		    "a path, feeding upon the bloodshed.  So long as war exists, he will continue to walk the earth."),
		new MonsterInfo(15, "Alteil", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 5, 20, 15,
		    "The colossus bringer of death.  It is said that Alteil carries a city upon his back where the souls of" +
		    " those he's culled are imprisoned for all eternity."),
		new MonsterInfo(16, "Amansazz", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 5, 21, 14,
		    "Amansazz lays waste to all that it touches.  Its approach is heralded by a blackened sky on the horizon" +
		    " signalling the doom that nears.  It cannot be stopped.  The only option is to run."),
		new MonsterInfo(17, "Amazon Scout", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 6, 17,
		    "The deadly Amazon Scout is rarely seen.  The only sign of her presence is the arrows she leaves behind " +
		    "in the heads of her victims.  The forest is her playground and you are trespassing."),
		new MonsterInfo(18, "Andromeda", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 3, 12, 11,
		    "Andromeda is a spirit of the wind.  While appearing helpless, she is far from it.  Melkith and " +
		    "Daenairon are her constant companions that whisper in her ear all her opponents' secrets and weaknesses."),
		new MonsterInfo(19, "Angelus", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 4, 8, 22,
		    "Angelus is a human so valiant, he was gifted heavenly prestige.  Those who look upon him swear they can " +
		    "see the otherworldly glow of wings.  All who meet his blade swiftly find their earthly lives at an end."),
		new MonsterInfo(20, "Angra Mainryu", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 5, 22, 10,
		    "Angra Mainryu is renowned for his twin blades of flame.  So hot are his weapons that they cut through " +
		    "his opponents' blades and armour as if they were butter.  Wherever he fights, there is sure to be a " +
		    "trail of scorched bodies."),
		new MonsterInfo(21, "AnubArak", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 7, 24, 22,
		    "Many believe this beast crawled up from the belly of the earth millennia ago.  The ancient " +
		    "scarab safeguards the slumbering Egyptian gods and goddesses.  None have ever made it passed his " +
		    "vigilant watch."),
		new MonsterInfo(22, "Anubis", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 23, 19,
		    "Anubis is the final judge of souls.  It is his measurement of the heart that determines whether one " +
		    "may pass on to the afterlife or be condemned to being consumed by Ammit and eternal restlessness."),
		new MonsterInfo(23, "Apocalyptic Librarian", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 6, 28, 14,
		    "The chronology of Death itself, the Apocalyptic Librarian is forever bound to the knowledge it holds.  " +
		    "Should the chains holding this creature be broken, even Death would have reason to fear."),
		new MonsterInfo(24, "Arbiter Asra", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 7, 29, 20,
		    "Arbiter Asra is the soul's final judge on the battlefield.  Her blade is the gavel of fate to which " +
		    "there is no appeal.  She is not an opponent who can be fought.  She is the executioner prowling for " +
		    "prey. The antithesis of Arkaid the Arbiter."),
		new MonsterInfo(25, "Arcane", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 3, 11, 9,
		    "Arcane is the reason sailors fear the sea.  This dragon forwent its wings for the endless depths of " +
		    "the ocean.  Those who are unfortunate to encounter it can only hope for a quick death."),
		new MonsterInfo(26, "Arcangel", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 8, 0, 30,
		    "The pure embodiment of light, Arcangel carries the entire might of heaven.  Her gaze alone is enough " +
		    "to set the soul ablaze.  Many cannot even look upon her without feeling the weight of their sins."),
		new MonsterInfo(27, "Arcangel", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 8, 30, 0,
		    "The pure embodiment of light, Arcangel carries the entire might of heaven.  Her gaze alone is enough " +
		    "to set the soul ablaze.  Many cannot even look upon her without feeling the weight of their sins."),
		new MonsterInfo(28, "Arch Chimera", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 3, 7,
		    "From the belly of the forest, this beast was born.  It stalks the woods in search of prey to sate its" +
		    " unquenchable hunger.  Its mindless ruthlessness certainly makes it a fearsome opponent."),
		new MonsterInfo(29, "Ardicolico", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 5, 18, 19,
		    "Ardicolico rules the glaciers and is as ancient as them.  The very ice bends to his will serving as " +
		    "both weapon and armour alike."),
		new MonsterInfo(30, "Ares", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 20, 16,
		    "The god of war himself, Ares feeds off the battles of mortal-kind.  Described as overwhelming, " +
		    "insatiable in battle, destructive, and man-slaughtering. Those associated with him are endowed with a" +
		    " savage, dangerous, or militarized quality."),
		new MonsterInfo(31, "Arkaid the Arbiter", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 3, 5, 17,
		    "Arkaid the Arbiter of Angels.  The antithesis of Arbiter Asra, she embodies the very will of heaven. " +
		    "Those who oppose her face a judgement of their soul.  When paired with an angel's blessing,  her spear " +
		    "is just and none can oppose her."),
		new MonsterInfo(32, "Armaud Gaul", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 18, 8,
		    "A messenger of the shadows, Armaud Gaul delivers omens of death.  Whether it be by his blade or another," +
		    " his very presence signals that the end is near."),
		new MonsterInfo(33, "Aroalxys", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 11, 21,
		    "A dryad disillusioned and embittered by the destruction of her home, Aroalxys abandoned her gentle " +
		    "nature and turned to that of a warrior.  None are welcome in her forest."),
		new MonsterInfo(34, "Arctic Chimera", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 3, 8, 17,
		    "The Cerberus of the north, the Arctic Chimera is a vicious beast that takes pleasure in the hunting and " +
		    "killing of its prey.  One would be wise not to disturb such a creature."),
		new MonsterInfo(35, "Austringer", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 2, 10, 5,
		    "A warrior renown for his command of all nature of winged beast.  His profound bond with these creatures" +
		    " of flight was so strong it allowed him to see through their eyes giving him unparalleled vision of " +
		    "his battles."),
		new MonsterInfo(36, "Ayslozius", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 6, 15, 26,
		    "The heart of the forest itself, Ayslozius can take many forms but prefers that of the free roaming wolf." +
		    " It serves as keeper and protector encouraging life and bringing swift death to those seeking to harm."),
		new MonsterInfo(37, "Azarath", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 5, 20, 17,
		    "The four-armed agent of Death charged with the collection of evasive souls.  One pair of arms wields" +
		    " his crippling scythe while the other rips forth his victim's very soul ensuring that none escape " +
		    "Death's reach."),
		new MonsterInfo(38, "Azazel", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 4, 20, 9,
		    "Once an angel of light, Azazel fell to earth.  Cast aside, he found fellowship with winged creatures of" +
		    " flight.  None but these avians are safe from his mercurial wrath."),
		new MonsterInfo(39, "Azopar", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 10, 5,
		    "Born from a stone deep within an ocean trench, Azopar guards the secrets of these depths.  None know" +
		    " exactly what it is this creature is protecting, only that it's best to avoid it at all costs."),
		new MonsterInfo(40, "Azraen", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 13, 13,
		    "Said to pull the very warmth from the air, this dark warrior exists solely to torment the souls of the" +
		    " living.  It has no allegiance, no master only an inexhaustible and unstoppable need to destroy.  The " +
		    "might of it's blade alone is said to be able to cleave a building in two"),
		new MonsterInfo(41, "Azriel", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 7, 26, 21,
		    "Azriel--the result of an unstoppable force meeting an immovable one.  Decimation is all that remains, " +
		    "all of existence rocked by its echoes.  "),
		new MonsterInfo(42, "Bael Kometani", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 1, 7, 5,
		    "The Bael Kometani are insectoid mutations from deep within the belly of Isgraemal.  It is believed that " +
		    "they are the result of magic gone awry in defence of a city long forgotten--it's warriors fused and " +
		    "melded with the very creatures they had sought to destroy."),
		new MonsterInfo(43, "Bahamut", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 18, 11,
		    "Possibly once a draconian warrior, this dark juggernaut is legendary for his armour which is said to be " +
		    "wrapped in a scintillating aura of light so brilliant that it was impossible to tell its color or " +
		    "material.  No man-made weapon can mar its surface let alone pierce it."),
		new MonsterInfo(44, "Banshee", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 3, 13, 11, ""),
		new MonsterInfo(45, "Baphomet", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 6, 22, 20, ""),
		new MonsterInfo(46, "Bat Mite", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 2, 10, 8, ""),
		new MonsterInfo(47, "Beezlebub", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 4, 16, 15, ""),
		new MonsterInfo(48, "Behemoth", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 6, 24, 15, ""),
		new MonsterInfo(49, "Black Phoenix", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 5, 17, 15, ""),
		new MonsterInfo(50, "Blackwind Rider", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 13, 7, ""),
		new MonsterInfo(51, "Blood Elemental", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 1, 5, 3, ""),
		new MonsterInfo(52, "Brine", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 4, 16, 10, ""),
		new MonsterInfo(53, "Brood Arsenal", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 5, 19, 18, ""),
		new MonsterInfo(54, "Byakko", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 7, 19, 27, ""),
		new MonsterInfo(55, "Byyperzo", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 10, 4, ""),
		new MonsterInfo(56, "Calypso", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 5, 17, 15, ""),
		new MonsterInfo(57, "Cayah", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 12, 6, ""),
		new MonsterInfo(58, "Cerberix", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 4, 3, ""),
		new MonsterInfo(59, "Chaos Librarian", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 9, 5, ""),
		new MonsterInfo(60, "Chaos", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 23, 13, ""),
		new MonsterInfo(61, "Cheirotonus", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 3, 12, 10, ""),
		new MonsterInfo(62, "Chichus", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 1, 5, 3, ""),
		new MonsterInfo(63, "Conienies Kilara", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 2, 6, 12, ""),
		new MonsterInfo(64, "Coralle", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 9, 3, ""),
		new MonsterInfo(65, "Core Hound", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 10, 8, ""),
		new MonsterInfo(66, "Corvus Promaethon", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 21, 11, ""),
		new MonsterInfo(67, "Crombhala", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 1, 4, 3, ""),
		new MonsterInfo(68, "Cronus", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 26, 17, ""),
		new MonsterInfo(69, "Crypt Crawler", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 6, 21, 18, ""),
		new MonsterInfo(70, "Cthulhu", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 6, 22, 19, ""),
		new MonsterInfo(71, "Cu Chulainn", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 16, 16, ""),
		new MonsterInfo(72, "Daarken", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 13, 12, ""),
		new MonsterInfo(73, "Daeh Lluks", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 4, 18, 13, ""),
		new MonsterInfo(74, "Dajobas", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 5, 20, 16, ""),
		new MonsterInfo(75, "Dao", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 6, 7, ""),
		new MonsterInfo(76, "Dark Chimera", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 4, 4, ""),
		new MonsterInfo(77, "Dark Unicorn", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 9, 8, ""),
		new MonsterInfo(78, "Death Bringer", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 4, 16, 13, ""),
		new MonsterInfo(79, "Death Talker", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 4, 18, 9, ""),
		new MonsterInfo(80, "Death's Dog", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 2, 10, 7, ""),
		new MonsterInfo(81, "Deathdealer", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 8, 8, ""),
		new MonsterInfo(82, "Deathwing", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 3, 13, 10, ""),
		new MonsterInfo(83, "Deligarisa", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 4, 10, 16, ""),
		new MonsterInfo(84, "Detniat", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 6, 22, 21, ""),
		new MonsterInfo(85, "Dionesis", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 12, 20, ""),
		new MonsterInfo(86, "Dragon Rider", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 1, 5, 7, ""),
		new MonsterInfo(87, "Drak Undon", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 2, 4, 12, ""),
		new MonsterInfo(88, "Drapoel", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 4, 14, ""),
		new MonsterInfo(89, "Dross Ripper", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 6, 5, ""),
		new MonsterInfo(90, "Druaga", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 18, 11, ""),
		new MonsterInfo(91, "Earth Elemental", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 7, 19, 0, ""),
		new MonsterInfo(92, "Earth Elemental", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 7, 0, 30, ""),
		new MonsterInfo(93, "Earth Summoner", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 5, 12, ""),
		new MonsterInfo(94, "Eastern Flare", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 5, 15, 17, ""),
		new MonsterInfo(95, "Ecafee", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 4, 10, 17, ""),
		new MonsterInfo(96, "El'zorn", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 7, 30, 0, ""),
		new MonsterInfo(97, "El'zorn", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 7, 0, 21, ""),
		new MonsterInfo(98, "Elian", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 7, 27, 17, ""),
		new MonsterInfo(99, "Elienai", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 5, 15, 19, ""),
		new MonsterInfo(100, "Elnoire", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 3, 9, 11, ""),
		new MonsterInfo(101, "Elyograg", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 12, 10, ""),
		new MonsterInfo(102, "Emrakul Hatchling", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 2, 7, 7, ""),
		new MonsterInfo(103, "Enaus", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 7, 0, 20, ""),
		new MonsterInfo(104, "Enaus", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 7, 30, 0, ""),
		new MonsterInfo(105, "Enenra", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 1, 4, 3, ""),
		new MonsterInfo(106, "Enzoma", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 5, 5, ""),
		new MonsterInfo(107, "Eris", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 24, 14, ""),
		new MonsterInfo(108, "Ertacalti", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 6, 18, 20, ""),
		new MonsterInfo(109, "Esaeler", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 3, 13, 12, ""),
		new MonsterInfo(110, "Etik", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 1, 7, 3, ""),
		new MonsterInfo(111, "Exemplar", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 5, 6, ""),
		new MonsterInfo(112, "Eyes of Envy", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(113, "Eylados", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 4, 13, 14, ""),
		new MonsterInfo(114, "Eyliskes", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 4, 5, ""),
		new MonsterInfo(115, "Fangren Marauder", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 7, 12, ""),
		new MonsterInfo(116, "Fate", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 7, 29, 17, ""),
		new MonsterInfo(117, "Feng Yi", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 8, 12, ""),
		new MonsterInfo(118, "Fenrir", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 4, 10, 16, ""),
		new MonsterInfo(119, "Fire Elemental", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 7, 0, 18, ""),
		new MonsterInfo(120, "Fire Elemental", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 7, 30, 0, ""),
		new MonsterInfo(121, "Fire Fairy", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(122, "Fire Golem", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 17, 6, ""),
		new MonsterInfo(123, "Fire Lord Burninates", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 18, 5, ""),
		new MonsterInfo(124, "Flaming Minotaur", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 7, 4, ""),
		new MonsterInfo(125, "Flytrap", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 1, 5, 3, ""),
		new MonsterInfo(126, "Forest Berserker", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 4, 6, ""),
		new MonsterInfo(127, "Forest Dryads", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 14, 20, ""),
		new MonsterInfo(128, "Forest Spirit", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 7, 11, ""),
		new MonsterInfo(129, "Frost Army", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 5, 14, 18, ""),
		new MonsterInfo(130, "Frost Spirit", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 2, 6, 12, ""),
		new MonsterInfo(131, "Frost", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 1, 5, 7, ""),
		new MonsterInfo(132, "Fuyunomi", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 4, 5, ""),
		new MonsterInfo(133, "Gaison Naka", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 4, 13, 15, ""),
		new MonsterInfo(134, "Garih", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 7, 3, ""),
		new MonsterInfo(135, "Ghost of the Arena", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 1, 7, 4, ""),
		new MonsterInfo(136, "Ghostare", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 1, 7, 3, ""),
		new MonsterInfo(137, "God of the Forest", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 7, 17, 29, ""),
		new MonsterInfo(138, "Gorilla King", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 4, 8, ""),
		new MonsterInfo(139, "Gorislav", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 2, 11, 6, ""),
		new MonsterInfo(140, "Grand Amaterasu", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 7, 19, 25, ""),
		new MonsterInfo(141, "Great Basilisk", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 5, 16, ""),
		new MonsterInfo(142, "Grimlock", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 6, 5, ""),
		new MonsterInfo(143, "Grof", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 8, 12, ""),
		new MonsterInfo(144, "Gruneath", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 8, 0, 30, ""),
		new MonsterInfo(145, "Gruneath", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 8, 30, 0, ""),
		new MonsterInfo(146, "Guide of Depth", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 6, 3, ""),
		new MonsterInfo(147, "Gwathnor", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 21, 12, ""),
		new MonsterInfo(148, "Gwiber", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(149, "Hades", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 7, 28, 19, ""),
		new MonsterInfo(150, "Harpy Lord", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 6, 26, 14, ""),
		new MonsterInfo(151, "Harpy Warrior", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 2, 12, 4, ""),
		new MonsterInfo(152, "Heartwood", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 6, 7, ""),
		new MonsterInfo(153, "Hecaton Sigma", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 4, 14, 12, ""),
		new MonsterInfo(154, "Hera", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 24, 18, ""),
		new MonsterInfo(155, "Horus", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 20, 13, ""),
		new MonsterInfo(156, "Ibmab", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 4, 15, 16, ""),
		new MonsterInfo(157, "Ice Gorgul", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 2, 9, 10, ""),
		new MonsterInfo(158, "Ice Queen", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 20, 30, 30, ""),
		new MonsterInfo(159, "Ice Wizard", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 4, 11, 15, ""),
		new MonsterInfo(160, "Ickthiasar", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 5, 18, 15, ""),
		new MonsterInfo(161, "Ider", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 3, 15, 10, ""),
		new MonsterInfo(162, "Il Tairu", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 14, 11, ""),
		new MonsterInfo(163, "Illyrias", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 5, 20, 14, ""),
		new MonsterInfo(164, "Inferno Juggernaut", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 15, 4, ""),
		new MonsterInfo(165, "Infinity Orb", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 1, 4, 3, ""),
		new MonsterInfo(166, "Iona", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 5, 16, 21, ""),
		new MonsterInfo(167, "Iraneous", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 14, 10, ""),
		new MonsterInfo(168, "Isader", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 4, 20, 8, ""),
		new MonsterInfo(169, "Isdain", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 19, 11, ""),
		new MonsterInfo(170, "Ishtar", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 23, 19, ""),
		new MonsterInfo(171, "Iteru the Space Weaver", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 3, 5, 17, ""),
		new MonsterInfo(172, "Jack O Lantern", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(173, "Jallu", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 4, 5, ""),
		new MonsterInfo(174, "Janiel", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 4, 12, ""),
		new MonsterInfo(175, "Jungle Protector", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 7, 14, ""),
		new MonsterInfo(176, "Kage", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 5, 20, 15, ""),
		new MonsterInfo(177, "Kattait", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 1, 3, 7, ""),
		new MonsterInfo(178, "Keeper of the Forest", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 4, 7, 19, ""),
		new MonsterInfo(179, "Kerem'beyit", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 3, 15, 9, ""),
		new MonsterInfo(180, "Kerembeyit", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 4, 10, 16, ""),
		new MonsterInfo(181, "Khelek'sul", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 3, 15, 9, ""),
		new MonsterInfo(182, "Kilara", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 6, 18, 20, ""),
		new MonsterInfo(183, "Kirtanis", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 13, 8, ""),
		new MonsterInfo(184, "Kometani", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 3, 6, 14, ""),
		new MonsterInfo(185, "Kometani", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 3, 13, 12, ""),
		new MonsterInfo(186, "Konosuk", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 3, 16, 7, ""),
		new MonsterInfo(187, "Kwoan Wakfu", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(188, "Kyler", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 1, 8, 3, ""),
		new MonsterInfo(189, "Kysduslr", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 4, 21, 9, ""),
		new MonsterInfo(190, "L'wokrad", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 3, 6, ""),
		new MonsterInfo(191, "La Tirana", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 21, 11, ""),
		new MonsterInfo(192, "Lady Death", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 4, 17, 14, ""),
		new MonsterInfo(193, "Lady Water", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 11, 7, ""),
		new MonsterInfo(194, "Latirus", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 11, 7, ""),
		new MonsterInfo(195, "latsyr", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 2, 7, 9, ""),
		new MonsterInfo(196, "Lava Swimmer", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 6, 3, ""),
		new MonsterInfo(197, "Legantsa", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 2, 8, 11, ""),
		new MonsterInfo(198, "Leucetius", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 6, 20, 21, ""),
		new MonsterInfo(199, "Liatgnol", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 2, 8, 11, ""),
		new MonsterInfo(200, "Liege of Tangle", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 4, 14, 16, ""),
		new MonsterInfo(201, "Light Shepherd", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 4, 15, 16, ""),
		new MonsterInfo(202, "Light Umbre", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 2, 7, 7, ""),
		new MonsterInfo(203, "Lizard Army", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 6, 13, 25, ""),
		new MonsterInfo(204, "Lovrec", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 9, 3, ""),
		new MonsterInfo(205, "Lumichi", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 1, 5, 7, ""),
		new MonsterInfo(206, "Lumonius", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 5, 10, 22, ""),
		new MonsterInfo(207, "LungKnot", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 2, 6, 8, ""),
		new MonsterInfo(208, "M'raval", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 5, 7, ""),
		new MonsterInfo(209, "Magma Mage", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 4, 17, 12, ""),
		new MonsterInfo(210, "Majsem", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 6, 21, 20, ""),
		new MonsterInfo(211, "Malfegor", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 11, 7, ""),
		new MonsterInfo(212, "Mankind's Fate", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 19, 13, ""),
		new MonsterInfo(213, "Marionette", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 3, 15, 9, ""),
		new MonsterInfo(214, "Markust", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 3, 13, 11, ""),
		new MonsterInfo(215, "Mera Griffin", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 1, 4, 4, ""),
		new MonsterInfo(216, "Mermaid", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 9, 5, ""),
		new MonsterInfo(217, "Mjolner", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(218, "Monkey King", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 6, 14, 24, ""),
		new MonsterInfo(219, "Morrigan", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 24, 16, ""),
		new MonsterInfo(220, "Mystic Shaman", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 14, 18, ""),
		new MonsterInfo(221, "Naiad Woman", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 8, 5, ""),
		new MonsterInfo(222, "Naka", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 5, 21, 15, ""),
		new MonsterInfo(223, "Nam d'Lo", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 13, 11, ""),
		new MonsterInfo(224, "Nam Demra", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 16, 10, ""),
		new MonsterInfo(225, "Namrem", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 4, 14, 12, ""),
		new MonsterInfo(226, "Necropolis Knight", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 15, 9, ""),
		new MonsterInfo(227, "Nekark", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 3, 13, 11, ""),
		new MonsterInfo(228, "Nertnal", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 1, 5, 7, ""),
		new MonsterInfo(229, "Niffirg", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 1, 5, 3, ""),
		new MonsterInfo(230, "Nightmare Nemod", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 20, 12, ""),
		new MonsterInfo(231, "Nightmare Witch", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 11, 7, ""),
		new MonsterInfo(232, "Nijuyr", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 8, 4, ""),
		new MonsterInfo(233, "Nike", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 20, 12, ""),
		new MonsterInfo(234, "Nikolas Nevar", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 11, 8, ""),
		new MonsterInfo(235, "Njoo", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 7, 6, ""),
		new MonsterInfo(236, "Noahkn", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 4, 20, 6, ""),
		new MonsterInfo(237, "Nosiop", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 15, 9, ""),
		new MonsterInfo(238, "Noth", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 2, 14, 4, ""),
		new MonsterInfo(239, "Novawuff", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 2, 9, 10, ""),
		new MonsterInfo(240, "Nyacin", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 6, 22, 21, ""),
		new MonsterInfo(241, "Oareiles", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 6, 8, ""),
		new MonsterInfo(242, "Obuun Channelers", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 14, 23, ""),
		new MonsterInfo(243, "Odanrot", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 7, 29, 19, ""),
		new MonsterInfo(244, "Odanrot", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 4, 16, 15, ""),
		new MonsterInfo(245, "Ohaguro Bettari", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 6, 22, 16, ""),
		new MonsterInfo(246, "Okenuth", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 5, 20, 15, ""),
		new MonsterInfo(247, "Onmora", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(248, "Ophiel the Fallen", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 11, 9, ""),
		new MonsterInfo(249, "Orianas", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 3, 10, 11, ""),
		new MonsterInfo(250, "Ottpurdis", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 3, 12, 12, ""),
		new MonsterInfo(251, "Out of the Ether", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 3, 10, 13, ""),
		new MonsterInfo(252, "Owlorne", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 2, 10, 8, ""),
		new MonsterInfo(253, "Ozanius", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 5, 18, 17, ""),
		new MonsterInfo(254, "Peacock Spider", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 1, 7, 6, ""),
		new MonsterInfo(255, "Pele", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 15, 10, ""),
		new MonsterInfo(256, "Pharoah Teefah", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 20, 14, ""),
		new MonsterInfo(257, "Phoenix", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 6, 26, 12, ""),
		new MonsterInfo(258, "Pious Petrifous", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 6, 5, ""),
		new MonsterInfo(259, "Poseidon", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 7, 24, 22, ""),
		new MonsterInfo(260, "Puppet Master", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 0, 0, 0, ""),
		new MonsterInfo(261, "Queen Solyas", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 7, 17, 29, ""),
		new MonsterInfo(262, "Quellious", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 2, 5, 13, ""),
		new MonsterInfo(263, "Quezcatli", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 4, 7, ""),
		new MonsterInfo(264, "Radon Aurora", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 5, 17, 15, ""),
		new MonsterInfo(265, "Raebyd Det", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 14, 12, ""),
		new MonsterInfo(266, "Ragnaros", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 4, 15, 11, ""),
		new MonsterInfo(267, "Ragnerok", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 6, 13, 25, ""),
		new MonsterInfo(268, "Ragorak", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 7, 27, 21, ""),
		new MonsterInfo(269, "Rakasht", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 6, 5, ""),
		new MonsterInfo(270, "RakkaMar", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 19, 6, ""),
		new MonsterInfo(271, "Ramses II", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 20, 19, ""),
		new MonsterInfo(272, "Ra", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 7, 25, 25, ""),
		new MonsterInfo(273, "Raven Spirit", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 11, 24, ""),
		new MonsterInfo(274, "Raygon", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 2, 6, 8, ""),
		new MonsterInfo(275, "Rayncid", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 6, 22, 16, ""),
		new MonsterInfo(276, "Razor Fly", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 1, 7, 5, ""),
		new MonsterInfo(277, "Reclamer", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 2, 7, 7, ""),
		new MonsterInfo(278, "Regis Ryanos", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 6, 27, 14, ""),
		new MonsterInfo(279, "Reinhold", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 9, 5, ""),
		new MonsterInfo(280, "Riahgnimalf", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 2, 9, 10, ""),
		new MonsterInfo(281, "Roc", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 5, 21, 16, ""),
		new MonsterInfo(282, "Romero", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 3, 9, 11, ""),
		new MonsterInfo(283, "Rotan Imretex", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 19, 7, ""),
		new MonsterInfo(284, "Rudraskha", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 8, 0, 30, ""),
		new MonsterInfo(285, "Rudraskha", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 8, 30, 0, ""),
		new MonsterInfo(286, "Ryujin", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 7, 24, 24, ""),
		new MonsterInfo(287, "S'gniwon", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 4, 16, 14, ""),
		new MonsterInfo(288, "Sabatav", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 2, 6, 8, ""),
		new MonsterInfo(289, "Sage of Age", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 7, 8, ""),
		new MonsterInfo(290, "Salafite", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 11, 9, ""),
		new MonsterInfo(291, "Salaman", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 11, 9, ""),
		new MonsterInfo(292, "Sandara", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 3, 7, 13, ""),
		new MonsterInfo(293, "Sankra", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 4, 18, 13, ""),
		new MonsterInfo(294, "Sarail", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 5, 8, ""),
		new MonsterInfo(295, "Screecher", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 2, 9, 8, ""),
		new MonsterInfo(296, "Sedna", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 4, 16, 12, ""),
		new MonsterInfo(297, "Seiryuu", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 5, 19, 17, ""),
		new MonsterInfo(298, "Sekaciz", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 7, 6, ""),
		new MonsterInfo(299, "Selcatnet", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 16, 10, ""),
		new MonsterInfo(300, "Selena the Dark Witch", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 21, 15, ""),
		new MonsterInfo(301, "Selscum", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 3, 13, 9, ""),
		new MonsterInfo(302, "Semalf", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 12, 7, ""),
		new MonsterInfo(303, "Senobn niks", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 5, 18, 15, ""),
		new MonsterInfo(304, "Sephor", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 10, 14, ""),
		new MonsterInfo(305, "Sey Eynam", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 5, 3, ""),
		new MonsterInfo(306, "Seyegib", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 1, 3, 6, ""),
		new MonsterInfo(307, "Shadowlord", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 6, 23, 20, ""),
		new MonsterInfo(308, "Sheoldred", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 5, 19, 18, ""),
		new MonsterInfo(309, "Sidhe", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 4, 12, 17, ""),
		new MonsterInfo(310, "Siren", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 3, 14, 10, ""),
		new MonsterInfo(311, "Slave of Darkness", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 6, 22, 16, ""),
		new MonsterInfo(312, "Snowcap", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 1, 4, 7, ""),
		new MonsterInfo(313, "Solaris", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 6, 17, 24, ""),
		new MonsterInfo(314, "Solyas", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 5, 12, 20, ""),
		new MonsterInfo(315, "Soul Eater", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 4, 17, 11, ""),
		new MonsterInfo(316, "Sphinx of Magos", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 11, 14, ""),
		new MonsterInfo(317, "Spirit of the Har", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 6, 5, ""),
		new MonsterInfo(318, "Spirit of Vengence", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 6, 16, 22, ""),
		new MonsterInfo(319, "Sraegnol", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 11, 6, ""),
		new MonsterInfo(320, "Stis Tahtnam", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 4, 17, 9, ""),
		new MonsterInfo(321, "Sunliastis", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 4, 8, 19, ""),
		new MonsterInfo(322, "Suzaku", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 10, 7, ""),
		new MonsterInfo(323, "Swamp Devil", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 6, 10, ""),
		new MonsterInfo(324, "Sylvanas", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 3, 11, 14, ""),
		new MonsterInfo(325, "Taluo", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 3, 18, 7, ""),
		new MonsterInfo(326, "Team Chow", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 14, 21, ""),
		new MonsterInfo(327, "Tegehel", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 4, 17, 11, ""),
		new MonsterInfo(328, "Tengu", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 2, 11, 6, ""),
		new MonsterInfo(329, "Thanatos", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 7, 3, ""),
		new MonsterInfo(330, "The Carrier", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 8, 30, 0, ""),
		new MonsterInfo(331, "The Carrier", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 8, 0, 30, ""),
		new MonsterInfo(332, "The Colonel", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 2, 11, 5, ""),
		new MonsterInfo(333, "The Crying Tree", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 3, 5, ""),
		new MonsterInfo(334, "The Dead Countess", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 4, 15, 12, ""),
		new MonsterInfo(335, "The Desecrator", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 8, 30, 0, ""),
		new MonsterInfo(336, "The Desecrator", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 8, 0, 30, ""),
		new MonsterInfo(337, "The Heart of Fire", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 4, 19, 9, ""),
		new MonsterInfo(338, "The Heshe", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 1, 8, 5, ""),
		new MonsterInfo(339, "The Invincible", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 4, 11, 15, ""),
		new MonsterInfo(340, "The Lamp Maker", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 1, 3, 6, ""),
		new MonsterInfo(341, "The Moth Eater", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 2, 9, 6, ""),
		new MonsterInfo(342, "The Mother", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 7, 23, 26, ""),
		new MonsterInfo(343, "The Pretorian", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 7, 23, 23, ""),
		new MonsterInfo(344, "The Rose Eater", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 6, 9, ""),
		new MonsterInfo(345, "The Unseen", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 2, 14, 5, ""),
		new MonsterInfo(346, "The Ursurper", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 5, 12, 23, ""),
		new MonsterInfo(347, "Themis", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 12, 21, ""),
		new MonsterInfo(348, "Thenight", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 6, 13, 25, ""),
		new MonsterInfo(349, "Thor", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 6, 24, 17, ""),
		new MonsterInfo(350, "Thorn Lion", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 2, 6, 8, ""),
		new MonsterInfo(351, "Tiafeonas", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 4, 12, 18, ""),
		new MonsterInfo(352, "Tiamat", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 19, 15, ""),
		new MonsterInfo(353, "Tir'ri", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 2, 5, 9, ""),
		new MonsterInfo(354, "Tiranac", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 3, 16, 5, ""),
		new MonsterInfo(355, "Tohder", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 1, 5, 4, ""),
		new MonsterInfo(356, "Tortoise Barrage", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 13, 23, ""),
		new MonsterInfo(357, "Traversia", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 3, 14, 11, ""),
		new MonsterInfo(358, "Tree Walker", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 6, 14, ""),
		new MonsterInfo(359, "Tsugsid", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 1, 5, 3, ""),
		new MonsterInfo(360, "Tyranus", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 4, 15, 16, ""),
		new MonsterInfo(361, "Ucamulbas", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 6, 16, 22, ""),
		new MonsterInfo(362, "Uhndeck", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 3, 5, 15, ""),
		new MonsterInfo(363, "Undead Bire", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 1, 7, 4, ""),
		new MonsterInfo(364, "Unidan", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 3, 11, 10, ""),
		new MonsterInfo(365, "Urabrask", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 2, 10, 9, ""),
		new MonsterInfo(366, "Valamadarance", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 5, 15, 20, ""),
		new MonsterInfo(367, "Valkyrie", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 21, 14, ""),
		new MonsterInfo(368, "Vantid", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 4, 18, 13, ""),
		new MonsterInfo(369, "Varden", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 5, 14, 18, ""),
		new MonsterInfo(370, "Vayl", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 4, 4, ""),
		new MonsterInfo(371, "Viccolatte", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 4, 16, 15, ""),
		new MonsterInfo(372, "Volac Etani", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 1, 3, 4, ""),
		new MonsterInfo(373, "Voodoo Witch", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 2, 10, 8, ""),
		new MonsterInfo(374, "Vyrilien", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 7, 20, 28, ""),
		new MonsterInfo(375, "Vyrilien", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 10, 7, ""),
		new MonsterInfo(376, "Wadjet", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 5, 17, 15, ""),
		new MonsterInfo(377, "Wandering Woman", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 2, 7, 7, ""),
		new MonsterInfo(378, "Water Elemental", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 8, 30, 0, ""),
		new MonsterInfo(379, "Water Elemental", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 8, 0, 30, ""),
		new MonsterInfo(380, "Water Sprite", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 5, 20, 14, ""),
		new MonsterInfo(381, "Wind Spirit", CardInfo.CardAffinity.Wind, CardInfo.CardType.Monster, 2, 9, 5, ""),
		new MonsterInfo(382, "Wushi", CardInfo.CardAffinity.Forest, CardInfo.CardType.Monster, 6, 18, 20, ""),
		new MonsterInfo(383, "Xanochoy", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 6, 20, 18, ""),
		new MonsterInfo(384, "XII", CardInfo.CardAffinity.Darkness, CardInfo.CardType.Monster, 5, 23, 13, ""),
		new MonsterInfo(385, "Xuan Yuan", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 6, 19, 19, ""),
		new MonsterInfo(386, "Yakami Hime", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 3, 5, 15, ""),
		new MonsterInfo(387, "Yastai", CardInfo.CardAffinity.Light, CardInfo.CardType.Monster, 6, 16, 22, ""),
		new MonsterInfo(388, "Ydald'lo", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 3, 12, 8, ""),
		new MonsterInfo(389, "Yekno'm", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 8, 6, ""),
		new MonsterInfo(390, "Yetius", CardInfo.CardAffinity.Ice, CardInfo.CardType.Monster, 1, 5, 7, ""),
		new MonsterInfo(391, "Ylgueht", CardInfo.CardAffinity.Dragon, CardInfo.CardType.Monster, 2, 8, 6, ""),
		new MonsterInfo(392, "Yogg Saron", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 2, 8, 6, ""),
		new MonsterInfo(393, "Ypeerc", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 6, 21, 17, ""),
		new MonsterInfo(394, "Yurei", CardInfo.CardAffinity.Death, CardInfo.CardType.Monster, 3, 17, 8, ""),
		new MonsterInfo(395, "Zealot", CardInfo.CardAffinity.Insect, CardInfo.CardType.Monster, 2, 9, 8, ""),
		new MonsterInfo(396, "Zeus", CardInfo.CardAffinity.Myth, CardInfo.CardType.Monster, 7, 26, 23, ""),
		new MonsterInfo(397, "Zniro", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 9, 4, ""),
		new MonsterInfo(398, "Znuese", CardInfo.CardAffinity.Water, CardInfo.CardType.Monster, 1, 7, 3, ""),
		new MonsterInfo(399, "Zniro", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 9, 4, ""),
		new MonsterInfo(400, "Zniro", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 9, 4, ""),
		new MonsterInfo(401, "Zniro", CardInfo.CardAffinity.Fire, CardInfo.CardType.Monster, 1, 9, 4, ""),
		new SpecialInfo(402, "Rallying Heart", CardInfo.CardType.Auxiliary, "Increases a monster's ATTACK by 1.  Light monsters gain 2 ATTACK.").registerEffect(new Effect402())
		
	};
	
	public static void associateCards () {
		//Multi Part Cards
		//
		//Arcangel
		Cards [25].associateCardInfo (Cards [26], CardRelation.PairL);
		Cards [26].associateCardInfo (Cards [25], CardRelation.PairR);
		//Earth Elemental
		Cards [90].associateCardInfo (Cards [91], CardRelation.PairL);
		Cards [91].associateCardInfo (Cards [90], CardRelation.PairR);
		//El'zorn
		Cards [95].associateCardInfo (Cards [96], CardRelation.PairR);
		Cards [96].associateCardInfo (Cards [95], CardRelation.PairL);
		//Enaus
		Cards [102].associateCardInfo (Cards [103], CardRelation.PairL);
		Cards [103].associateCardInfo (Cards [102], CardRelation.PairR);
		//Fire Elemental
		Cards [118].associateCardInfo (Cards [119], CardRelation.PairR);
		Cards [119].associateCardInfo (Cards [118], CardRelation.PairL);
		//Gruneath
		Cards [143].associateCardInfo (Cards [144], CardRelation.PairL);
		Cards [144].associateCardInfo (Cards [143], CardRelation.PairR);
		//Rudraskha
		Cards [283].associateCardInfo (Cards [284], CardRelation.PairL);
		Cards [284].associateCardInfo (Cards [283], CardRelation.PairR);
		//The Carrier
		Cards [329].associateCardInfo (Cards [330], CardRelation.PairL);
		Cards [330].associateCardInfo (Cards [329], CardRelation.PairR);
		//The Desecrator
		Cards [334].associateCardInfo (Cards [334], CardRelation.PairR);
		Cards [335].associateCardInfo (Cards [335], CardRelation.PairL);
		//Water Elemental
		Cards [377].associateCardInfo (Cards [378], CardRelation.PairR);
		Cards [378].associateCardInfo (Cards [377], CardRelation.PairL);
	}
}
