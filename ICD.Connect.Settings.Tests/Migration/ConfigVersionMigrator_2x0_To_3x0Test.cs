using ICD.Connect.Settings.Migration;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Migration
{
	[TestFixture]
	public sealed class ConfigVersionMigrator_2x0_To_3x0Test : AbstractConfigVersionMigratorTest
	{
		protected override string BeforeConfig
		{
			get
			{
				return @"<!--
This configuration is generated automatically.
Only change this file if you know what you are doing.
Any invalid data, whitespace, and comments will be deleted the next time this is generated.
-->

<IcdConfig id=""1"" type=""Krang"">
	<Name>IcdCore</Name>
	<CombineName />
	<Header>
		<ConfigVersion>2.0</ConfigVersion>
	</Header>
	<Themes>
		<Theme id=""100"" type=""MetlifeTheme"">
			<Name />
			<CombineName />
			<TvPresets>TvPresets.xml</TvPresets>
			<SupportPhoneNumber />
			<PartitionMenu>
				<VisibilityJoin>0</VisibilityJoin>
				<InvertCombinedState>false</InvertCombinedState>
				<Endpoints />
				<Partitions />
			</PartitionMenu>
		</Theme>
	</Themes>
	<Panels>
		<Panel id=""301000"" type=""Tsw750""> <!-- Change Panel Type Here -->
			<Name>Table Panel</Name>
			<CombineName />
			<IPID>0x10</IPID> <!-- IPID = 10 + (slot number - 1) -->
			<EnableVoIP>false</EnableVoIP>
		</Panel>
	</Panels>
	<Ports>
		<Port id=""101001"" type=""ComPort"">
			<Name>Scaler to Display serial</Name>
			<CombineName />
			<Device>201005</Device>
			<Address>1</Address>
		</Port>
		<Port id=""101002"" type=""IrPort"">
			<Name>Scaler to TV Tuner IR</Name>
			<CombineName />
			<Device>201005</Device>
			<Address>1</Address>
			<Driver>Amino H140 RSDICD Standard.ir</Driver> <!-- Change IR File for non-Amino Tuners -->
			<PulseTime>100</PulseTime>
			<BetweenTime>750</BetweenTime>
		</Port>
		<Port id=""101007"" type=""TCP"">
			<Name>Lighting - Shades Processor</Name>
			<CombineName />
			<Address><!-- Lighting and Shades Processor IP Here --></Address>
			<Port>8027</Port>
			<BufferSize>16384</BufferSize>
		</Port>     
	</Ports>
	<Devices>
		<Device id=""201000"" type=""FusionRoom"">
			<Name>Fusion</Name>
			<CombineName />
			<IPID>0xF0</IPID> <!-- IPID = F0 + (slot number - 1) -->
			<RoomName> <!-- Room Name Here: ""US NC Cary Met 1 - 02.461"" --></RoomName>
			<RoomId /> <!-- RoomId is a guid that gets generated at start, unique per room -->
			<FusionSigsPath>FusionSigs.xml</FusionSigsPath>
		</Device>
		<Device id=""201003"" type=""IrTvTuner"">
			<Name>TV Tuner</Name>
			<CombineName />
			<Port>101002</Port>
		</Device>
		<Device id=""201004"" type=""DmTx201C"">
			<Name>Laptop</Name>
			<CombineName />
			<IPID>0x30</IPID> <!-- IPID = 30 + (slot number - 1) -->
			<DmSwitch />
			<DmInput />
		</Device>
		<Device id=""201005"" type=""DmRmcScalerC"">
			<Name>DisplayController</Name>
			<CombineName />
			<IPID>0x60</IPID> <!-- IPID = 60 + (slot number - 1) -->
			<DmSwitch />
			<DmOutput />
		</Device>
		<Device id=""201007"" type=""SharpDisplay""> <!-- Change Display Type Here -->
			<Name>Sharp TV</Name>
			<CombineName />
			<Port>101001</Port>
			<MinVolume>5</MinVolume>
			<MaxVolume>50</MaxVolume>
		</Device>
		<Device id=""201020"" type=""BmsLightingProcessorClient"">
			<Name>Lighting - Shades</Name>
			<CombineName />
			<Port>101007</Port>
			<RoomId>1000</RoomId> <!-- This should match the Room ID on the lighting and shades processor config -->
		</Device>  
	</Devices>
	<Rooms>
		<Room id=""1000"" type=""MetlifeRoom"">
			<Name>Huddle R3</Name>
			<CombineName />
			<Prefix><!-- Room Prefix Here: US NC Cary Met 1 --></Prefix>
			<Number><!-- Room Number/Name Here: 02.461 Collaborate --></Number>
			<PhoneNumber />
			<Owner>
				<Name><!-- Room Owner Name --></Name>
				<Phone><!-- Room Owner Phone --></Phone>
				<Email><!-- Room Owner Email --></Email>
			</Owner>
			<DialingPlan>
				<Config>DialingPlan.xml</Config>
			</DialingPlan>
			<VolumePoints>
				<VolumePoint>
					<Device>201007</Device>
					<Control>1</Control>
					<VolumeType>Program</VolumeType>
				</VolumePoint>
			</VolumePoints>
			<DisplayRelays />
			<CombinePriority>0</CombinePriority>
			<Devices>
				<Device>201000</Device>
				<Device>201003</Device>
				<Device>201004</Device>
				<Device>201005</Device>
				<Device>201007</Device>
				<Device>201020</Device>        
			</Devices>
			<Panels>
				<Panel>301000</Panel>
			</Panels>
			<Ports>
				<Port>101001</Port>
				<Port>101002</Port>
				<Port>101007</Port>
			</Ports>
			<Sources>
				<Source>601001</Source>
				<Source>601002</Source>
			</Sources>
			<Destinations>
				<Destination>701000</Destination>
				<Destination>701001</Destination>
			</Destinations>
			<DestinationGroups />
			<Partitions />
		</Room>
	</Rooms>
	<Routing id=""200"" type=""RoutingGraph"">
		<Name />
		<CombineName />
		<Connections>
			<Connection id=""401000"" type=""Connection"">
				<Name>Tx to Scaler</Name>
				<CombineName />
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceDevice>201004</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>201005</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""401001"" type=""Connection"">
				<Name>Scaler to TV</Name>
				<CombineName />
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceDevice>201005</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>201007</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""401003"" type=""Connection"">
				<Name>TV Tuner to TV</Name>
				<CombineName />
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceDevice>201003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>201007</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>2</DestinationAddress>
			</Connection>
		</Connections>
		<StaticRoutes />
		<Sources>
			<Source id=""601001"" type=""MetlifeSource"">
				<Name>TV</Name>
				<CombineName />
				<Device>201003</Device>
				<Control>0</Control>
				<Address>1</Address>
				<ConnectionType>Audio, Video</ConnectionType>
				<Order>0</Order>
				<SourceType>CableBox</SourceType>
				<SourceFlags>MainNav</SourceFlags>
				<EnableWhenNotTransmitting>false</EnableWhenNotTransmitting>
				<InhibitAutoRoute>false</InhibitAutoRoute>
				<InhibitAutoUnroute>false</InhibitAutoUnroute>
			</Source>
			<Source id=""601002"" type=""MetlifeSource"">
				<Name>Laptop</Name>
				<CombineName />
				<Device>201004</Device>
				<Control>0</Control>
				<Address>1</Address>
				<ConnectionType>Audio, Video</ConnectionType>
				<Order>0</Order>
				<SourceType>Laptop</SourceType>
				<SourceFlags>Share</SourceFlags>
				<EnableWhenNotTransmitting>false</EnableWhenNotTransmitting>
				<InhibitAutoRoute>false</InhibitAutoRoute>
				<InhibitAutoUnroute>false</InhibitAutoUnroute>
			</Source>
		</Sources>
		<Destinations>
			<Destination id=""701000"" type=""MetlifeDestination"">
				<Name>Display</Name>
				<CombineName />
				<Device>201007</Device>
				<Control>0</Control>
				<Address>1</Address>
				<ConnectionType>Audio, Video</ConnectionType>
				<Order>0</Order>
				<VtcOption>Main</VtcOption>
				<AudioOption>Program</AudioOption>
				<ShareByDefault>true</ShareByDefault>
			</Destination>
			<Destination id=""701001"" type=""MetlifeDestination"">
				<Name>Display</Name>
				<CombineName />
				<Device>201007</Device>
				<Control>0</Control>
				<Address>2</Address>
				<ConnectionType>Audio, Video</ConnectionType>
				<Order>0</Order>
				<VtcOption>Main</VtcOption>
				<AudioOption>Program</AudioOption>
				<ShareByDefault>true</ShareByDefault>
			</Destination>
		</Destinations>
		<DestinationGroups />
	</Routing>
	<Partitioning id=""300"" type=""PartitionManager"">
		<Name />
		<CombineName />
		<Partitions />
	</Partitioning>
</IcdConfig>";
			}
		}

		protected override string AfterConfig
		{
			get
			{
				return @"<!--
This configuration is generated automatically.
Only change this file if you know what you are doing.
Any invalid data, whitespace, and comments will be deleted the next time this is generated.
-->
<IcdConfig id=""1"" type=""Krang"">
  <Name>IcdCore</Name>
  <CombineName />
  <Header>
    <ConfigVersion>2.0</ConfigVersion>
  </Header>
  <Themes>
    <Theme id=""100"" type=""MetlifeTheme"">
      <Name />
      <CombineName />
      <TvPresets>TvPresets.xml</TvPresets>
      <SupportPhoneNumber />
      <PartitionMenu>
        <VisibilityJoin>0</VisibilityJoin>
        <InvertCombinedState>false</InvertCombinedState>
        <Endpoints />
        <Partitions />
      </PartitionMenu>
    </Theme>
  </Themes>
  <Panels>
    <Panel id=""301000"" type=""Tsw750"">
      <!-- Change Panel Type Here -->
      <Name>Table Panel</Name>
      <CombineName />
      <IPID>0x10</IPID>
      <!-- IPID = 10 + (slot number - 1) -->
      <EnableVoIP>false</EnableVoIP>
    </Panel>
  </Panels>
  <Ports>
    <Port id=""101001"" type=""ComPort"">
      <Name>Scaler to Display serial</Name>
      <CombineName />
      <Device>201005</Device>
      <Address>1</Address>
    </Port>
    <Port id=""101002"" type=""IrPort"">
      <Name>Scaler to TV Tuner IR</Name>
      <CombineName />
      <Device>201005</Device>
      <Address>1</Address>
      <Driver>Amino H140 RSDICD Standard.ir</Driver>
      <!-- Change IR File for non-Amino Tuners -->
      <PulseTime>100</PulseTime>
      <BetweenTime>750</BetweenTime>
    </Port>
    <Port id=""101007"" type=""TCP"">
      <Name>Lighting - Shades Processor</Name>
      <CombineName />
      <Address>
        <!-- Lighting and Shades Processor IP Here -->
      </Address>
      <Port>8027</Port>
      <BufferSize>16384</BufferSize>
    </Port>
  </Ports>
  <Devices>
    <Device id=""201000"" type=""FusionRoom"">
      <Name>Fusion</Name>
      <CombineName />
      <IPID>0xF0</IPID>
      <!-- IPID = F0 + (slot number - 1) -->
      <RoomName>
        <!-- Room Name Here: ""US NC Cary Met 1 - 02.461"" -->
      </RoomName>
      <RoomId />
      <!-- RoomId is a guid that gets generated at start, unique per room -->
      <FusionSigsPath>FusionSigs.xml</FusionSigsPath>
    </Device>
    <Device id=""201003"" type=""IrTvTuner"">
      <Name>TV Tuner</Name>
      <CombineName />
      <Port>101002</Port>
    </Device>
    <Device id=""201004"" type=""DmTx201C"">
      <Name>Laptop</Name>
      <CombineName />
      <IPID>0x30</IPID>
      <!-- IPID = 30 + (slot number - 1) -->
      <DmSwitch />
      <DmInput />
    </Device>
    <Device id=""201005"" type=""DmRmcScalerC"">
      <Name>DisplayController</Name>
      <CombineName />
      <IPID>0x60</IPID>
      <!-- IPID = 60 + (slot number - 1) -->
      <DmSwitch />
      <DmOutput />
    </Device>
    <Device id=""201007"" type=""SharpDisplay"">
      <!-- Change Display Type Here -->
      <Name>Sharp TV</Name>
      <CombineName />
      <Port>101001</Port>
      <MinVolume>5</MinVolume>
      <MaxVolume>50</MaxVolume>
    </Device>
    <Device id=""201020"" type=""BmsLightingProcessorClient"">
      <Name>Lighting - Shades</Name>
      <CombineName />
      <Port>101007</Port>
      <RoomId>1000</RoomId>
      <!-- This should match the Room ID on the lighting and shades processor config -->
    </Device>
  </Devices>
  <Rooms>
    <Room id=""1000"" type=""MetlifeRoom"">
      <Name>Huddle R3</Name>
      <CombineName />
      <Prefix>
        <!-- Room Prefix Here: US NC Cary Met 1 -->
      </Prefix>
      <Number>
        <!-- Room Number/Name Here: 02.461 Collaborate -->
      </Number>
      <PhoneNumber />
      <Owner>
        <Name>
          <!-- Room Owner Name -->
        </Name>
        <Phone>
          <!-- Room Owner Phone -->
        </Phone>
        <Email>
          <!-- Room Owner Email -->
        </Email>
      </Owner>
      <DialingPlan>
        <Config>DialingPlan.xml</Config>
      </DialingPlan>
      <VolumePoints>
        <VolumePoint>9</VolumePoint>
      </VolumePoints>
      <DisplayRelays />
      <CombinePriority>0</CombinePriority>
      <Devices>
        <Device>201000</Device>
        <Device>201003</Device>
        <Device>201004</Device>
        <Device>201005</Device>
        <Device>201007</Device>
        <Device>201020</Device>
      </Devices>
      <Panels>
        <Panel>301000</Panel>
      </Panels>
      <Ports>
        <Port>101001</Port>
        <Port>101002</Port>
        <Port>101007</Port>
      </Ports>
      <Sources>
        <Source>601001</Source>
        <Source>601002</Source>
      </Sources>
      <Destinations>
        <Destination>701000</Destination>
      </Destinations>
      <DestinationGroups />
      <Partitions />
    </Room>
  </Rooms>
  <Routing id=""200"" type=""RoutingGraph"">
    <Name />
    <CombineName />
    <Connections>
      <Connection id=""401000"" type=""Connection"">
        <Name>Tx to Scaler</Name>
        <CombineName />
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceDevice>201004</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>201005</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""401001"" type=""Connection"">
        <Name>Scaler to TV</Name>
        <CombineName />
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceDevice>201005</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>201007</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""401003"" type=""Connection"">
        <Name>TV Tuner to TV</Name>
        <CombineName />
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceDevice>201003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>201007</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>2</DestinationAddress>
      </Connection>
    </Connections>
    <StaticRoutes />
    <Sources>
      <Source id=""601001"" type=""MetlifeSource"">
        <Name>TV</Name>
        <CombineName />
        <Device>201003</Device>
        <Control>0</Control>
        <Address>1</Address>
        <ConnectionType>Audio, Video</ConnectionType>
        <Order>0</Order>
        <SourceType>CableBox</SourceType>
        <SourceFlags>MainNav</SourceFlags>
        <EnableWhenNotTransmitting>false</EnableWhenNotTransmitting>
        <InhibitAutoRoute>false</InhibitAutoRoute>
        <InhibitAutoUnroute>false</InhibitAutoUnroute>
      </Source>
      <Source id=""601002"" type=""MetlifeSource"">
        <Name>Laptop</Name>
        <CombineName />
        <Device>201004</Device>
        <Control>0</Control>
        <Address>1</Address>
        <ConnectionType>Audio, Video</ConnectionType>
        <Order>0</Order>
        <SourceType>Laptop</SourceType>
        <SourceFlags>Share</SourceFlags>
        <EnableWhenNotTransmitting>false</EnableWhenNotTransmitting>
        <InhibitAutoRoute>false</InhibitAutoRoute>
        <InhibitAutoUnroute>false</InhibitAutoUnroute>
      </Source>
    </Sources>
    <Destinations>
      <Destination id=""701000"" type=""MetlifeDestination"">
        <Name>Display</Name>
        <CombineName />
        <Device>201007</Device>
        <Control>0</Control>
        <ConnectionType>Audio, Video</ConnectionType>
        <Order>0</Order>
        <VtcOption>Main</VtcOption>
        <AudioOption>Program</AudioOption>
        <ShareByDefault>true</ShareByDefault>
        <Addresses>
          <Address>1</Address>
          <Address>2</Address>
        </Addresses>
      </Destination>
    </Destinations>
    <DestinationGroups />
  </Routing>
  <Partitioning id=""300"" type=""PartitionManager"">
    <Name />
    <CombineName />
    <Partitions />
  </Partitioning>
  <VolumePoints>
    <VolumePoint id=""9"" type=""MetlifeVolumePoint"">
      <Device>201007</Device>
      <Control>2</Control>
      <VolumeType>Program</VolumeType>
    </VolumePoint>
  </VolumePoints>
</IcdConfig>";
			}
		}

		protected override IConfigVersionMigrator InstantiateMigrator()
		{
			return new ConfigVersionMigrator_2x0_To_3x0();
		}
	}
}
