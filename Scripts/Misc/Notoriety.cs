using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.ConPVP;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Factions;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells;

namespace Server.Misc
{
	public class NotorietyHandlers
	{
		public static void Initialize()
		{
			Notoriety.Hues[Notoriety.Innocent]		= 0x59;
			Notoriety.Hues[Notoriety.Ally]			= 0x3F;
			Notoriety.Hues[Notoriety.CanBeAttacked]	= 0x3B2;
			Notoriety.Hues[Notoriety.Criminal]		= 0x3B2;
			Notoriety.Hues[Notoriety.Enemy]			= 0x90;
			Notoriety.Hues[Notoriety.Murderer]		= 0x22;
			Notoriety.Hues[Notoriety.Invulnerable]	= 0x35;

			Notoriety.Handler = new NotorietyHandler( MobileNotoriety );

			Mobile.AllowBeneficialHandler = new AllowBeneficialHandler( Mobile_AllowBeneficial );
			Mobile.AllowHarmfulHandler = new AllowHarmfulHandler( Mobile_AllowHarmful );
		}

		private enum GuildStatus { None, Peaceful, Waring }

		private static GuildStatus GetGuildStatus( Mobile m )
		{
			if ( m.Guild == null )
				return GuildStatus.None;
			else if ( ((Guild)m.Guild).Enemies.Count == 0 && m.Guild.Type == GuildType.Regular )
				return GuildStatus.Peaceful;

			return GuildStatus.Waring;
		}

		private static bool CheckBeneficialStatus( GuildStatus from, GuildStatus target )
		{
			if ( from == GuildStatus.Waring || target == GuildStatus.Waring )
				return false;

			return true;
		}

		/*private static bool CheckHarmfulStatus( GuildStatus from, GuildStatus target )
		{
			if ( from == GuildStatus.Waring && target == GuildStatus.Waring )
				return true;

			return false;
		}*/

		public static bool Mobile_AllowBeneficial( Mobile from, Mobile target )
		{
			if ( from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player )
				return true;

			#region Dueling
			PlayerMobile pmFrom = from as PlayerMobile;
			PlayerMobile pmTarg = target as PlayerMobile;

			if ( pmFrom == null && @from is BaseCreature bcFrom )
			{
				if ( bcFrom.Summoned )
					pmFrom = bcFrom.SummonMaster as PlayerMobile;
			}

			if ( pmTarg == null && target is BaseCreature bcTarg )
			{
				if ( bcTarg.Summoned )
					pmTarg = bcTarg.SummonMaster as PlayerMobile;
			}

			if ( pmFrom != null && pmTarg != null )
			{
				if ( pmFrom.DuelContext != pmTarg.DuelContext && ((pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg.DuelContext != null && pmTarg.DuelContext.Started)) )
					return false;

				if ( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && ((pmFrom.DuelContext.StartedReadyCountdown && !pmFrom.DuelContext.Started) || pmFrom.DuelContext.Tied || pmFrom.DuelPlayer.Eliminated || pmTarg.DuelPlayer.Eliminated) )
					return false;

				if ( pmFrom.DuelPlayer != null && !pmFrom.DuelPlayer.Eliminated && pmFrom.DuelContext != null && pmFrom.DuelContext.IsSuddenDeath )
					return false;

				if ( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.m_Tournament != null && pmFrom.DuelContext.m_Tournament.IsNotoRestricted && pmFrom.DuelPlayer != null && pmTarg.DuelPlayer != null && pmFrom.DuelPlayer.Participant != pmTarg.DuelPlayer.Participant )
					return false;

				if ( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.Started )
					return true;
			}

			if ( (pmFrom?.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg?.DuelContext != null && pmTarg.DuelContext.Started) )
				return false;

			if ( @from.Region.GetRegion( typeof( Engines.ConPVP.SafeZone ) ) is SafeZone sz /*&& sz.IsDisabled()*/ )
				return false;

			sz = target.Region.GetRegion( typeof( Engines.ConPVP.SafeZone ) ) as Engines.ConPVP.SafeZone;

			if ( sz != null /*&& sz.IsDisabled()*/ )
				return false;
			#endregion

			Map map = from.Map;

			#region Factions
			Faction targetFaction = Faction.Find( target, true );

			if ( (!Core.ML || map == Faction.Facet) && targetFaction != null )
			{
				if ( Faction.Find( from, true ) != targetFaction )
					return false;
			}
			#endregion


			if ( map != null && (map.Rules & MapRules.BeneficialRestrictions) == 0 )
				return true; // In felucca, anything goes

			if ( !from.Player )
				return true; // NPCs have no restrictions

			if ( target is BaseCreature creature && !creature.Controlled )
				return false; // Players cannot heal uncontrolled mobiles

			if ( pmFrom?.Young == true || pmTarg?.Young == true )
				return false; // Young players cannot perform beneficial actions towards older players

			if ( @from.Guild is Guild fromGuild && target.Guild is Guild targetGuild && (targetGuild == fromGuild || fromGuild.IsAlly( targetGuild )) )
				return true; // Guild members can be beneficial

			return CheckBeneficialStatus( GetGuildStatus( from ), GetGuildStatus( target ) );
		}

		public static bool Mobile_AllowHarmful( Mobile from, Mobile target )
		{
			if ( from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player )
				return true;

			#region Dueling
			PlayerMobile pmFrom = from as PlayerMobile;
			PlayerMobile pmTarg = target as PlayerMobile;
			BaseCreature bcTarg = target as BaseCreature;

			if ( pmFrom == null && @from is BaseCreature bcFrom && bcFrom.Summoned )
				pmFrom = bcFrom.SummonMaster as PlayerMobile;

			if ( pmTarg == null && bcTarg?.Summoned == true )
				pmTarg = bcTarg.SummonMaster as PlayerMobile;

			if ( pmFrom != null && pmTarg != null )
			{
				if ( pmFrom.DuelContext != pmTarg.DuelContext && ((pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg.DuelContext != null && pmTarg.DuelContext.Started)) )
					return false;

				if ( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && ((pmFrom.DuelContext.StartedReadyCountdown && !pmFrom.DuelContext.Started) || pmFrom.DuelContext.Tied || pmFrom.DuelPlayer.Eliminated || pmTarg.DuelPlayer.Eliminated) )
					return false;

				if ( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.m_Tournament != null && pmFrom.DuelContext.m_Tournament.IsNotoRestricted && pmFrom.DuelPlayer != null && pmTarg.DuelPlayer != null && pmFrom.DuelPlayer.Participant == pmTarg.DuelPlayer.Participant )
					return false;

				if ( pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.Started )
					return true;
			}

			if ( (pmFrom?.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg?.DuelContext != null && pmTarg.DuelContext.Started) )
				return false;

			if ( @from.Region.GetRegion( typeof( Engines.ConPVP.SafeZone ) ) is SafeZone sz /*&& sz.IsDisabled()*/ )
				return false;

			sz = target.Region.GetRegion( typeof( Engines.ConPVP.SafeZone ) ) as Engines.ConPVP.SafeZone;

			if ( sz != null /*&& sz.IsDisabled()*/ )
				return false;
			#endregion

			Map map = from.Map;

			if ( map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0 )
				return true; // In felucca, anything goes

			if ( !from.Player && !(@from is BaseCreature bc && bc.GetMaster() != null && bc.GetMaster().AccessLevel == AccessLevel.Player ) )
			{
				if ( !CheckAggressor( from.Aggressors, target ) && !CheckAggressed( from.Aggressed, target ) && pmTarg?.CheckYoungProtection( from ) == true )
					return false;

				return true; // Uncontrolled NPCs are only restricted by the young system
			}

			Guild fromGuild = GetGuildFor( from.Guild as Guild, from );
			Guild targetGuild = GetGuildFor( target.Guild as Guild, target );

			if ( fromGuild != null && targetGuild != null && (fromGuild == targetGuild || fromGuild.IsAlly( targetGuild ) || fromGuild.IsEnemy( targetGuild )) )
				return true; // Guild allies or enemies can be harmful

			if ( bcTarg?.Controlled == true || bcTarg?.Summoned == true && bcTarg?.SummonMaster != from )
				return false; // Cannot harm other controlled mobiles

			if ( target.Player )
				return false; // Cannot harm other players

			if ( bcTarg?.InitialInnocent != true )
			{
				if ( Notoriety.Compute( from, target ) == Notoriety.Innocent )
					return false; // Cannot harm innocent mobiles
			}

			return true;
		}

		public static Guild GetGuildFor( Guild def, Mobile m )
		{
			Guild g = def;

			if ( m is BaseCreature c && c.Controlled && c.ControlMaster != null )
			{
				c.DisplayGuildTitle = false;

				if ( c.Map != Map.Internal && (Core.AOS || Guild.NewGuildSystem || c.ControlOrder == OrderType.Attack || c.ControlOrder == OrderType.Guard) )
					g = (Guild)(c.Guild = c.ControlMaster.Guild);
				else if ( c.Map == Map.Internal || c.ControlMaster.Guild == null )
					g = (Guild)(c.Guild = null);
			}

			return g;
		}

		public static int CorpseNotoriety( Mobile source, Corpse target )
		{
			if ( target.AccessLevel > AccessLevel.Player )
				return Notoriety.CanBeAttacked;

			Body body = target.Amount;

			Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
			Guild targetGuild = GetGuildFor( target.Guild, target.Owner );

			Faction srcFaction = Faction.Find( source, true, true );
			Faction trgFaction = Faction.Find( target.Owner, true, true );
			List<Mobile> list = target.Aggressors;

			if ( sourceGuild != null && targetGuild != null )
			{
				if ( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
					return Notoriety.Ally;
				if ( sourceGuild.IsEnemy( targetGuild ) )
					return Notoriety.Enemy;
			}

			if ( target.Owner is BaseCreature creature )
			{
				if ( srcFaction != null && trgFaction != null && srcFaction != trgFaction && source.Map == Faction.Facet )
					return Notoriety.Enemy;

				if ( CheckHouseFlag( source, creature, target.Location, target.Map ) )
					return Notoriety.CanBeAttacked;

				int actual = Notoriety.CanBeAttacked;

				if ( target.Kills >= 5 || body.IsMonster && IsSummoned( creature ) || creature.AlwaysMurderer || creature.IsAnimatedDead )
					actual = Notoriety.Murderer;

				if ( DateTime.UtcNow >= target.TimeOfDeath + Corpse.MonsterLootRightSacrifice )
					return actual;

				Party sourceParty = Party.Get( source );

				for( int i = 0; i < list.Count; ++i )
				{
					if ( list[i] == source || sourceParty != null && Party.Get( list[i] ) == sourceParty )
						return actual;
				}

				return Notoriety.Innocent;
			}

			if ( target.Kills >= 5 || body.IsMonster )
				return Notoriety.Murderer;

			if (target.Criminal && target.Map != null && (target.Map.Rules & MapRules.HarmfulRestrictions) == 0)
				return Notoriety.Criminal;

			if ( srcFaction != null && trgFaction != null && srcFaction != trgFaction && source.Map == Faction.Facet )
			{
				for ( int i = 0; i < list.Count; ++i )
				{
					if ( list[i] == source || list[i] is BaseFactionGuard )
						return Notoriety.Enemy;
				}
			}

			if ( CheckHouseFlag( source, target.Owner, target.Location, target.Map ) )
				return Notoriety.CanBeAttacked;

			if ( !(target.Owner is PlayerMobile) )
				return Notoriety.CanBeAttacked;

			for ( int i = 0; i < list.Count; ++i )
			{
				if ( list[i] == source )
					return Notoriety.CanBeAttacked;
			}

			return Notoriety.Innocent;
		}

		/* Must be thread-safe */
		public static int MobileNotoriety( Mobile source, Mobile target )
		{
			BaseCreature bcTarg = target as BaseCreature;

			if ( Core.AOS && ( target.Blessed || bcTarg?.IsInvulnerable == true || target is PlayerVendor || target is TownCrier ) )
				return Notoriety.Invulnerable;

			PlayerMobile pmFrom = source as PlayerMobile;
			PlayerMobile pmTarg = target as PlayerMobile;

			#region Dueling
			if ( pmFrom != null && pmTarg != null )
			{
				if ( pmFrom.DuelContext != null && pmFrom.DuelContext.StartedBeginCountdown && !pmFrom.DuelContext.Finished && pmFrom.DuelContext == pmTarg.DuelContext )
					return pmFrom.DuelContext.IsAlly( pmFrom, pmTarg ) ? Notoriety.Ally : Notoriety.Enemy;
			}
			#endregion

			if ( target.AccessLevel > AccessLevel.Player )
				return Notoriety.CanBeAttacked;

			if ( source.Player && !target.Player && pmFrom != null && bcTarg != null )
			{
				Mobile master = bcTarg.GetMaster();

				if ( master != null && master.AccessLevel > AccessLevel.Player )
					return Notoriety.CanBeAttacked;

				master = bcTarg.ControlMaster;

				if ( Core.ML && master != null )
				{
					if ( ( source == master && CheckAggressor( bcTarg.Aggressors, source ) ) || ( CheckAggressor( source.Aggressors, bcTarg ) ) )
						return Notoriety.CanBeAttacked;

					return MobileNotoriety( source, master );
				}

				if ( !bcTarg.Summoned && !bcTarg.Controlled && pmFrom.EnemyOfOneType == bcTarg.GetType() )
					return Notoriety.Enemy;
			}

			if ( target.Kills >= 5 || target.Body.IsMonster && IsSummoned( bcTarg ) && !( target is BaseFamiliar ) && !( target is ArcaneFey ) && !( target is Golem ) || bcTarg?.AlwaysMurderer == true || bcTarg?.IsAnimatedDead == true )
				return Notoriety.Murderer;

			if ( target.Criminal )
				return Notoriety.Criminal;

			Guild sourceGuild = GetGuildFor( source.Guild as Guild, source );
			Guild targetGuild = GetGuildFor( target.Guild as Guild, target );

			if ( sourceGuild != null && targetGuild != null )
			{
				if ( sourceGuild == targetGuild || sourceGuild.IsAlly( targetGuild ) )
					return Notoriety.Ally;
				if ( sourceGuild.IsEnemy( targetGuild ) )
					return Notoriety.Enemy;
			}

			Faction srcFaction = Faction.Find( source, true, true );
			Faction trgFaction = Faction.Find( target, true, true );

			if ( srcFaction != null && trgFaction != null && srcFaction != trgFaction && source.Map == Faction.Facet )
				return Notoriety.Enemy;

			if ( SkillHandlers.Stealing.ClassicMode && pmTarg?.PermaFlags.Contains( source ) == true )
				return Notoriety.CanBeAttacked;

			if ( bcTarg?.AlwaysAttackable == true )
				return Notoriety.CanBeAttacked;

			if ( CheckHouseFlag( source, target, target.Location, target.Map ) )
				return Notoriety.CanBeAttacked;

			if ( bcTarg?.InitialInnocent != true )
			{
				if ( !target.Body.IsHuman && !target.Body.IsGhost && !IsPet( bcTarg ) && pmTarg == null || !Core.ML && !target.CanBeginAction( typeof( Server.Spells.Seventh.PolymorphSpell ) ) )
					return Notoriety.CanBeAttacked;
			}

			if ( CheckAggressor( source.Aggressors, target ) )
				return Notoriety.CanBeAttacked;

			if ( CheckAggressed( source.Aggressed, target ) )
				return Notoriety.CanBeAttacked;

			if ( bcTarg != null && bcTarg.Controlled && bcTarg.ControlOrder == OrderType.Guard && bcTarg.ControlTarget == source )
				return Notoriety.CanBeAttacked;

			if ( source is BaseCreature bc )
			{
				Mobile master = bc.GetMaster();

				if ( master != null && CheckAggressor( master.Aggressors, target ) || MobileNotoriety( master, target ) == Notoriety.CanBeAttacked || bcTarg != null )
					return Notoriety.CanBeAttacked;
			}

			return Notoriety.Innocent;
		}

		public static bool CheckHouseFlag( Mobile from, Mobile m, Point3D p, Map map )
		{
			BaseHouse house = BaseHouse.FindHouseAt( p, map, 16 );

			if ( house == null || house.Public || !house.IsFriend( from ) )
				return false;

			if ( m != null && house.IsFriend( m ) )
				return false;

			if ( m is BaseCreature c && !c.Deleted && c.Controlled && c.ControlMaster != null )
				return !house.IsFriend( c.ControlMaster );

			return true;
		}

		public static bool IsPet( BaseCreature c )
		{
			return (c != null && c.Controlled);
		}

		public static bool IsSummoned( BaseCreature c )
		{
			return (c != null && /*c.Controlled &&*/ c.Summoned);
		}

		public static bool CheckAggressor( List<AggressorInfo> list, Mobile target )
		{
			for( int i = 0; i < list.Count; ++i )
				if ( list[i].Attacker == target )
					return true;

			return false;
		}

		public static bool CheckAggressed( List<AggressorInfo> list, Mobile target )
		{
			for( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if ( !info.CriminalAggression && info.Defender == target )
					return true;
			}

			return false;
		}
	}
}
