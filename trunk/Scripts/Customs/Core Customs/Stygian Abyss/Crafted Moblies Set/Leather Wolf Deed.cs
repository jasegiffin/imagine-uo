using System; 
using System.Collections;
using Server.Items; 
using Server.Mobiles; 
using Server.Misc;
using Server.Network;

namespace Server.Items 
{ 
   	public class LeatherWolfDeed: Item 
   	{ 
		public bool m_AllowConstruction;
		public Timer m_ConstructionTimer;
		private DateTime m_End;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowConstruction
		{
			get{ return m_AllowConstruction; }
			set{ m_AllowConstruction = value; }
		}

		[Constructable]
		public LeatherWolfDeed() : base( 0x14F0 )
		{
			Weight = 0.01;
			Name = "a leather wolf deed";
			AllowConstruction = false;

			m_ConstructionTimer = new ConstructionTimer( this, TimeSpan.FromDays( 0.0 ) );
			m_ConstructionTimer.Start();
			m_End = DateTime.Now + TimeSpan.FromDays( 0.0 );
		}

            	public LeatherWolfDeed( Serial serial ) : base ( serial ) 
            	{             
           	}

		public override void OnDoubleClick( Mobile from )
		{
                        if ( !IsChildOf( from.Backpack ) ) 
                        { 
                                from.SendMessage( "You must have the deed in your backpack to use it." ); 
                        } 
			else if ( this.AllowConstruction == true )
			{
				this.Delete();
				from.SendMessage( "You have constructed a leather wolf!!" );

				CraftedLeatherWolf lwolf = new CraftedLeatherWolf();

         			lwolf.Map = from.Map; 
         			lwolf.Location = from.Location; 

				lwolf.Controlled = true;

				lwolf.ControlMaster = from;

				lwolf.IsBonded = true;
			}
			else
			{
				from.SendMessage( "This can not be constructed yet, please wait...." );
			}
		}

           	public override void Serialize( GenericWriter writer ) 
           	{ 
              		base.Serialize( writer ); 
              		writer.Write( (int) 1 ); 
			writer.WriteDeltaTime( m_End );
           	} 
            
           	public override void Deserialize( GenericReader reader ) 
           	{ 
              		base.Deserialize( reader ); 
              		int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_End = reader.ReadDeltaTime();
					m_ConstructionTimer = new ConstructionTimer( this, m_End - DateTime.Now );
					m_ConstructionTimer.Start();

					break;
				}
				case 0:
				{
					TimeSpan duration = TimeSpan.FromDays( 0.0 );

					m_ConstructionTimer = new ConstructionTimer( this, duration );
					m_ConstructionTimer.Start();
					m_End = DateTime.Now + duration;

					break;
				} 
			}
           	} 

		private class ConstructionTimer : Timer
		{ 
			private LeatherWolfDeed de;

			private int cnt = 0;

			public ConstructionTimer( LeatherWolfDeed owner, TimeSpan duration ) : base( duration )
			{ 
				de = owner;
			}

			protected override void OnTick() 
			{
				cnt += 1;

				if ( cnt == 1 )
				{
					de.AllowConstruction = true;
				}
			}
		}
        } 
} 