using System;
using System.Reflection;
using System.Collections;
using Server;
using System.Collections.Generic;

namespace Server.Factions
{
	public class Reflector
	{
		private static List<Town> m_Towns;

		public static List<Town> Towns
		{
			get
			{
				if ( m_Towns == null )
					ProcessTypes();

				return m_Towns;
			}
		}

		private static List<Faction> m_Factions;

		public static List<Faction> Factions
		{
			get
			{
				if ( m_Factions == null )
					Reflector.ProcessTypes();

				return m_Factions;
			}
		}

		private static object Construct( Type type )
		{
			try{ return Activator.CreateInstance( type ); }
			catch{ return null; }
		}

		private static void ProcessTypes()
		{
			m_Factions = new List<Faction>();
			m_Towns = new List<Town>();

			Assembly[] asms = ScriptCompiler.Assemblies;

			for ( int i = 0; i < asms.Length; ++i )
			{
				Assembly asm = asms[i];
				TypeCache tc = ScriptCompiler.GetTypeCache( asm );
				Type[] types = tc.Types;

				for ( int j = 0; j < types.Length; ++j )
				{
					Type type = types[j];

					if ( type.IsSubclassOf( typeof( Faction ) ) )
					{
						if ( Construct( type ) is Faction faction )
							Faction.Factions.Add( faction );
					}
					else if ( type.IsSubclassOf( typeof( Town ) ) )
					{
						if ( Construct( type ) is Town town )
							Town.Towns.Add( town );
					}
				}
			}
		}
	}
}
