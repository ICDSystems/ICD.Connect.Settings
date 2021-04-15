using ICD.Connect.Settings.Migration.Migrators;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Migration.Migrators
{
	[TestFixture]
	public sealed class ConfigVersionMigrator_4x0_To_5x0Test : AbstractConfigVersionMigratorTest
	{
		protected override string BeforeConfig
		{
			get { return @"<IcdConfig id=""1"" type=""Krang"">
	<Name>IcdCore</Name>
	<Themes>
		<Theme id=""100"" type=""MetlifeTheme"">
			<TvPresets>TvPresets.xml</TvPresets>
			<InvertCombinedState>True</InvertCombinedState>
			<PartitionMenu>
				<InvertedCombinedState>False</InvertedCombinedState>
				<VisibilityJoin>3832</VisibilityJoin>
				<Endpoints>
					<Endpoint>
						<Index>0</Index>
						<EndpointId>701001</EndpointId>
					</Endpoint>
					<Endpoint>
						<Index>1</Index>
						<EndpointId>702001</EndpointId>
					</Endpoint>
				</Endpoints>
				<Partitions>
					<Partition>
						<Index>0</Index>
						<PartitionId>800001</PartitionId>
					</Partition>
				</Partitions>
				<RoomLables>
					<RoomLabel>
						<Index>0</Index>
						<LabelText>Room 1D-102 </LabelText>
					</RoomLabel>
					<RoomLabel>
						<Index>1</Index>
						<LabelText>Room 1D-103</LabelText>
					</RoomLabel>
				</RoomLables>
			</PartitionMenu>
		</Theme>
	</Themes>
	<Panels>
		<Panel id=""300101"" type=""Tsw1060"">
			<Name>Room A Panel</Name>
			<IPID>0x10</IPID>
		</Panel>
		<Panel id=""300201"" type=""Tsw1060"">
			<Name>Room B Panel</Name>
			<IPID>0x11</IPID>
		</Panel>
	</Panels>
	<Ports>
		<Port id=""100000"" type=""TCP"">
			<Name>CP3 to Biamp</Name>
			<Address>10.22.20.47</Address>
			<Port>23</Port>
		</Port>
		<Port id=""101001"" type=""ComPort"">
			<Name>Scaler A to Display serial</Name>
			<Device>201011</Device>
			<Address>1</Address>
		</Port>
		<Port id=""101002"" type=""SSH"">
			<Name>SSH port for Room A Codec</Name>
			<CombineName />
			<Address>10.22.199.104</Address>
			<Port>22</Port>
			<Username>crestron</Username>
			<Password>NFUPcPxs5p2XeKn6W6Qn</Password>
		</Port>
		<Port id=""101003"" type=""IrPort"">
			<Name>CP3 to 1D-102 TV Tuner IR</Name>
			<CombineName />
			<Device>200001</Device>
			<Address>1</Address>
			<Driver>Amino H140 RSDICD Standard.ir</Driver>
			<PulseTime>100</PulseTime>
			<BetweenTime>750</BetweenTime>
		</Port>
		<Port id=""101005"" type=""RelayPort"">
			<Name>CP3 Relay 1 - 1D-102 Screen Up</Name>
			<CombineName />
			<Device>200001</Device>
			<Address>1</Address>
		</Port>
		<Port id=""101006"" type=""RelayPort"">
			<Name>CP3 Relay 2 - 1D-102 Screen Down</Name>
			<CombineName />
			<Device>200001</Device>
			<Address>2</Address>
		</Port>
		<Port id=""102001"" type=""ComPort"">
			<Name>Scaler B to Display serial</Name>
			<Device>202011</Device>
			<Address>1</Address>
		</Port>
		<Port id=""102002"" type=""SSH"">
			<Name>SSH port for Room B Codec</Name>
			<CombineName />
			<Address>10.22.199.105</Address>
			<Port>22</Port>
			<Username>crestron</Username>
			<Password>NFUPcPxs5p2XeKn6W6Qn</Password>
		</Port>
		<Port id=""102003"" type=""IrPort"">
			<Name>CP3 to 1D-103 TV Tuner IR</Name>
			<CombineName />
			<Device>200001</Device>
			<Address>2</Address>
			<Driver>Amino H140 RSDICD Standard.ir</Driver>
			<PulseTime>100</PulseTime>
			<BetweenTime>750</BetweenTime>
		</Port>
		<Port id=""102005"" type=""RelayPort"">
			<Name>CP3 Relay 3 - 1D-103 Screen Up</Name>
			<CombineName />
			<Device>200001</Device>
			<Address>3</Address>
		</Port>
		<Port id=""102006"" type=""RelayPort"">
			<Name>CP3 Relay 4 - 1D-103 Screen Down</Name>
			<CombineName />
			<Device>200001</Device>
			<Address>4</Address>
		</Port>
	</Ports>
	<Devices>
		<Device id=""200001"" type=""ControlSystem"">
			<Name>CP3</Name>
		</Device>
		<Device id=""200002"" type=""BiampTesira"">
			<Name>Biamp</Name>
			<Port>100000</Port>
			<Username/>
			<Config>ControlConfig.xml</Config>
		</Device>
		<Device id=""200003"" type=""DmMd16x16"">
			<Name>Video Switcher</Name>
			<IPID>0xE0</IPID>
		</Device>
		<Device id=""200004"" type=""Dmc4kC"">
			<Name>DM Input Card 1</Name>
			<CardNumber>1</CardNumber>
			<SwitcherId>200003</SwitcherId>
			<CresnetId />
		</Device>
		<Device id=""200005"" type=""Dmc4kC"">
			<Name>DM Input Card 6</Name>
			<CardNumber>6</CardNumber>
			<SwitcherId>200003</SwitcherId>
			<CresnetId />
		</Device>
		<Device id=""200006"" type=""Dmc4kCoHd"">
			<Name>DM Output Card 1-2</Name>
			<CardNumber>1</CardNumber>
			<SwitcherId>200003</SwitcherId>
		</Device>
		<Device id=""200007"" type=""GlsPartCn"">
			<Name>Partition Sensor</Name>
			<CresnetID>97</CresnetID>
			<Sensitivity>2</Sensitivity>
		</Device>
		<Device id=""201001"" type=""DmTx201C"">
			<Name>1D-102 Laptop</Name>
			<DmInput>1</DmInput>
			<DmSwitch>200003</DmSwitch>
			<IPID>0xE1</IPID>
		</Device>
		<Device id=""201002"" type=""CiscoCodec"">
			<Name>1D-102 Cisco SX80</Name>
			<CombineName />
			<Port>101002</Port>
			<PeripheralsID/>
			<!-- PeripheralsID is a guid that gets generated at start, unique per room -->
			<Input1Type>Camera</Input1Type>
			<Input2Type>None</Input2Type>
			<Input3Type>Content</Input3Type>
			<Input4Type>None</Input4Type>
			<Hide>false</Hide>
		</Device>
		<Device id=""201003"" type=""CiscoCamera"">
			<Name>1D-102 Rear Camera</Name>
			<CombineName />
			<Codec>201002</Codec>
			<CameraId>1</CameraId>
			<PanTiltSpeed>2</PanTiltSpeed>
			<ZoomSpeed>2</ZoomSpeed>
		</Device>
		<Device id=""201004"" type=""CiscoCamera"">
			<Name>1D-102 Front Camera</Name>
			<CombineName />
			<Codec>201002</Codec>
			<CameraId>2</CameraId>
			<PanTiltSpeed>2</PanTiltSpeed>
			<ZoomSpeed>2</ZoomSpeed>
		</Device>
		<Device id=""201005"" type=""IrTvTuner"">
			<Name>1D-102 TV Tuner</Name>
			<CombineName />
			<Port>101003</Port>
			<IrCommands>
				<Channels>
					<Clear>clear</Clear>
					<Enter>enter</Enter>
					<ChannelUp>+</ChannelUp>
					<ChannelDown>-</ChannelDown>
				</Channels>
				<Playback>
					<Repeat>repeat</Repeat>
					<Rewind>rewind</Rewind>
					<FastForward>fastforward</FastForward>
					<Stop>stop</Stop>
					<Play>play</Play>
					<Pause>pause</Pause>
					<Record>record</Record>
				</Playback>
				<Menus>
					<PageUp>pageup</PageUp>
					<PageDown>pagedown</PageDown>
					<TopMenu>topmenu</TopMenu>
					<PopupMenu>popupmenu</PopupMenu>
					<Return>return</Return>
					<Info>info</Info>
					<Eject>eject</Eject>
					<Power>power</Power>
					<Red>red</Red>
					<Green>green</Green>
					<Yellow>yellow</Yellow>
					<Blue>blue</Blue>
					<Up>up</Up>
					<Down>down</Down>
					<Left>left</Left>
					<Right>right</Right>
					<Select>select</Select>
				</Menus>
			</IrCommands>
		</Device>
		<Device id=""201010"" type=""DisplayScreenRelayControl"">
			<Name>1D-102 Projector Screen</Name>
			<CombineName>1D-102 Projector Screen</CombineName>
			<Display>201012</Display>
			<DisplayOffRelay>101005</DisplayOffRelay>
			<DisplayOnRelay>101006</DisplayOnRelay>
			<LatchRelay>false</LatchRelay>
			<RelayHoldTime>5000</RelayHoldTime>
		</Device>
		<Device id=""201011"" type=""DmRmcScalerC"">
			<Name>1D-102 DmRmcScalerC</Name>
			<DmOutput>1</DmOutput>
			<DmSwitch>200003</DmSwitch>
			<IPID>0xE3</IPID>
		</Device>
		<Device id=""201012"" type=""PanasonicDisplay"">
			<Name>1D-102 Display</Name>
			<Port>101001</Port>
		</Device>
		<Device id=""201013"" type=""MockDisplayWithAudio"">
			<Name>1D-102 Audio</Name>
		</Device>
		<Device id=""202001"" type=""DmTx201C"">
			<Name>1D-102 Laptop</Name>
			<DmInput>6</DmInput>
			<DmSwitch>200003</DmSwitch>
			<IPID>0xE2</IPID>
		</Device>
		<Device id=""202002"" type=""CiscoCodec"">
			<Name>1D-103 Cisco SX80</Name>
			<CombineName />
			<Port>102002</Port>
			<PeripheralsID/>
			<!-- PeripheralsID is a guid that gets generated at start, unique per room -->
			<Input1Type>Camera</Input1Type>
			<Input2Type>None</Input2Type>
			<Input3Type>Content</Input3Type>
			<Input4Type>None</Input4Type>
			<Hide>false</Hide>
		</Device>
		<Device id=""202003"" type=""CiscoCamera"">
			<Name>1D-103 Front Camera</Name>
			<CombineName />
			<Codec>202002</Codec>
			<CameraId>1</CameraId>
			<PanTiltSpeed>5</PanTiltSpeed>
			<ZoomSpeed>5</ZoomSpeed>
		</Device>
		<Device id=""202004"" type=""CiscoCamera"">
			<Name>1D-103 Rear Camera</Name>
			<CombineName />
			<Codec>202002</Codec>
			<CameraId>2</CameraId>
			<PanTiltSpeed>5</PanTiltSpeed>
			<ZoomSpeed>5</ZoomSpeed>
		</Device>
		<Device id=""202005"" type=""IrTvTuner"">
			<Name>1D-103 TV Tuner</Name>
			<CombineName />
			<Port>102003</Port>
			<IrCommands>
				<Channels>
					<Clear>clear</Clear>
					<Enter>enter</Enter>
					<ChannelUp>+</ChannelUp>
					<ChannelDown>-</ChannelDown>
				</Channels>
				<Playback>
					<Repeat>repeat</Repeat>
					<Rewind>rewind</Rewind>
					<FastForward>fastforward</FastForward>
					<Stop>stop</Stop>
					<Play>play</Play>
					<Pause>pause</Pause>
					<Record>record</Record>
				</Playback>
				<Menus>
					<PageUp>pageup</PageUp>
					<PageDown>pagedown</PageDown>
					<TopMenu>topmenu</TopMenu>
					<PopupMenu>popupmenu</PopupMenu>
					<Return>return</Return>
					<Info>info</Info>
					<Eject>eject</Eject>
					<Power>power</Power>
					<Red>red</Red>
					<Green>green</Green>
					<Yellow>yellow</Yellow>
					<Blue>blue</Blue>
					<Up>up</Up>
					<Down>down</Down>
					<Left>left</Left>
					<Right>right</Right>
					<Select>select</Select>
				</Menus>
			</IrCommands>
		</Device>
		<Device id=""202010"" type=""DisplayScreenRelayControl"">
			<Name>Projector Screen</Name>
			<CombineName>Display Relay</CombineName>
			<Display>202012</Display>
			<DisplayOffRelay>102005</DisplayOffRelay>
			<DisplayOnRelay>102006</DisplayOnRelay>
			<LatchRelay>false</LatchRelay>
			<RelayHoldTime>5000</RelayHoldTime>
		</Device>
		<Device id=""202011"" type=""DmRmcScalerC"">
			<Name>1D-103 DmRmcScalerC</Name>
			<DmOutput>2</DmOutput>
			<DmSwitch>200003</DmSwitch>
			<IPID>0xE4</IPID>
		</Device>
		<Device id=""202012"" type=""PanasonicDisplay"">
			<Name>1D-103 Display</Name>
			<Port>102001</Port>
		</Device>
		<Device id=""202013"" type=""MockDisplayWithAudio"">
			<Name>1D-103 Audio</Name>
		</Device>
	</Devices>
	<Rooms>
		<Room id=""1000"" type=""MetlifeRoom"">
			<Name>Educate Room A</Name>
			<Prefix>US FL Tampa Bldg4 - </Prefix>
			<Number>1D-102</Number>
			<CombinePriority>10</CombinePriority>
			<PhoneNumber>(813) 631 1222</PhoneNumber>
			<Owner>
				<Name>Travis Bennett</Name>
				<Phone />
				<Email>multimediasupport@metlife.com</Email>
			</Owner>
			<DialingPlan>
				<Config>DialingPlan.xml</Config>
				<AudioEndpoint>
					<Device>200002</Device>
					<Control>15</Control>
				</AudioEndpoint>
			</DialingPlan>
			<Devices>
				<Device>200001</Device>
				<Device>200002</Device>
				<Device>200003</Device>
				<Device>200004</Device>
				<Device>200006</Device>
				<Device>200007</Device>
				<Device>201001</Device>
				<Device>201002</Device>
				<Device>201003</Device>
				<Device>201004</Device>
				<Device>201005</Device>
				<Device>201010</Device>
				<Device>201011</Device>
				<Device>201012</Device>
				<Device>201013</Device>
			</Devices>
			<Panels>
				<Panel>300101</Panel>
			</Panels>
			<Ports>
				<Port>100000</Port>
				<Port>101001</Port>
				<Port>101002</Port>
				<Port>101003</Port>
				<Port>101005</Port>
				<Port>101006</Port>
			</Ports>
			<Sources>
				<Source combine=""Single,Master"">601001</Source>
				<Source combine=""Single,Master"">602001</Source>
			</Sources>
			<Destinations>
				<Destination combine=""Single,Master"">701001</Destination>
				<Destination combine=""Single,Master"">701002</Destination>
			</Destinations>
			<DestinationGroups />
			<VolumePoints>
				<VolumePoint>
					<Device>200002</Device>
					<Control>10</Control>
					<VolumeType>Program</VolumeType>
				</VolumePoint>
				<VolumePoint>
					<Device>200002</Device>
					<Control>11</Control>
					<VolumeType>Vtc</VolumeType>
				</VolumePoint>
				<VolumePoint>
					<Device>200002</Device>
					<Control>12</Control>
					<VolumeType>Atc</VolumeType>
				</VolumePoint>
				<VolumePoint>
					<Device>200002</Device>
					<Control>13</Control>
					<VolumeType>Sr</VolumeType>
				</VolumePoint>
			</VolumePoints>
		</Room>
		<Room id=""2000"" type=""MetlifeRoom"">
			<Name>Educate Room B</Name>
			<Prefix>US FL Tampa Bldg4 - </Prefix>
			<Number>1D-103</Number>
			<CombinePriority>20</CombinePriority>
			<PhoneNumber>(813) 613 1223</PhoneNumber>
			<Owner>
				<Name>Travis Bennett</Name>
				<Phone />
				<Email>multimediasupport@metlife.com</Email>
			</Owner>
			<DialingPlan>
				<Config>DialingPlan.xml</Config>
				<AudioEndpoint>
					<Device>200002</Device>
					<Control>25</Control>
				</AudioEndpoint>
			</DialingPlan>
			<Devices>
				<Device>200001</Device>
				<Device>200002</Device>
				<Device>200003</Device>
				<Device>200005</Device>
				<Device>200006</Device>
				<Device>200007</Device>
				<Device>202001</Device>
				<Device>202002</Device>
				<Device>202003</Device>
				<Device>202005</Device>
				<Device>202010</Device>
				<Device>202011</Device>
				<Device>202012</Device>
				<Device>202013</Device>
			</Devices>
			<Panels>
				<Panel>300201</Panel>
			</Panels>
			<Ports>
				<Port>100000</Port>
				<Port>102001</Port>
				<Port>102002</Port>
				<Port>102003</Port>
				<Port>102005</Port>
				<Port>102006</Port>
			</Ports>
			<Sources>
				<Source combine=""Single,Slave"">602001</Source>
				<Source combine=""Single"">602002</Source>
			</Sources>
			<Destinations>
				<Destination combine=""Single,Slave"">702001</Destination>
				<Destination combine=""Single"">702002</Destination>
			</Destinations>
			<DestinationGroups />
			<VolumePoints>
				<VolumePoint>
					<Device>200002</Device>
					<Control>20</Control>
					<VolumeType>Program</VolumeType>
				</VolumePoint>
				<VolumePoint>
					<Device>200002</Device>
					<Control>21</Control>
					<VolumeType>Vtc</VolumeType>
				</VolumePoint>
				<VolumePoint>
					<Device>200002</Device>
					<Control>22</Control>
					<VolumeType>Atc</VolumeType>
				</VolumePoint>
				<VolumePoint>
					<Device>200002</Device>
					<Control>23</Control>
					<VolumeType>Sr</VolumeType>
				</VolumePoint>
			</VolumePoints>
		</Room>
	</Rooms>
	<Partitioning id=""300"" type=""PartitionManager"">
		<Partitions>
			<Partition id=""800001"" type=""MetlifePartition"">
				<Name>Wall</Name>
				<PartitionControls>
					<PartitionControl>
						<Device>200002</Device>
						<Control>3</Control>
					</PartitionControl>
					<PartitionControl>
						<Device>200007</Device>
						<Control>0</Control>
					</PartitionControl>
				</PartitionControls>
				<Rooms>
					<Room>1000</Room>
					<Room>2000</Room>
				</Rooms>
			</Partition>
		</Partitions>
	</Partitioning>
	<Routing id=""101010"" type=""RoutingGraph"">
		<Name />
		<Connections>
			<Connection id=""401001"" type=""Connection"">
				<Name>1D-102 Laptop</Name>
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceDevice>201001</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""401002"" type=""Connection"">
				<Name>1D-103 Laptop</Name>
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceDevice>202001</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>6</DestinationAddress>
			</Connection>
			<Connection id=""401003"" type=""Connection"">
				<Name>1D-102 Rear Camera to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>201003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>2</DestinationAddress>
			</Connection>
			<Connection id=""401004"" type=""Connection"">
				<Name>1D-102 Front Camera to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>201004</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>3</DestinationAddress>
			</Connection>
			<Connection id=""401005"" type=""Connection"">
				<Name>1D-102 SX80 Out1 to Switcher</Name>
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceDevice>201002</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>4</DestinationAddress>
			</Connection>
			<Connection id=""401006"" type=""Connection"">
				<Name>1D-102 SX80 Out2 to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>201002</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>2</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>5</DestinationAddress>
			</Connection>
			<Connection id=""401007"" type=""Connection"">
				<Name>1D-103 Rear Camera to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>202003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>7</DestinationAddress>
			</Connection>
			<Connection id=""401008"" type=""Connection"">
				<Name>1D-103 Front Camera to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>202004</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>8</DestinationAddress>
			</Connection>
			<Connection id=""401009"" type=""Connection"">
				<Name>1D-103 SX80 Out1 to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>202002</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>9</DestinationAddress>
			</Connection>
			<Connection id=""401010"" type=""Connection"">
				<Name>1D-103 SX80 Out2 to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>202002</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>2</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>10</DestinationAddress>
			</Connection>
			<Connection id=""401011"" type=""Connection"">
				<Name>1D-102 TV Tuner to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>201005</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>11</DestinationAddress>
			</Connection>
			<Connection id=""401012"" type=""Connection"">
				<Name>1D-103 TV Tuner to Switcher</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>202005</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>200003</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>12</DestinationAddress>
			</Connection>
			<Connection id=""402001"" type=""Connection"">
				<Name>Room A RMC</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>201011</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""402002"" type=""Connection"">
				<Name>Room B RMC</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>2</SourceAddress>
				<DestinationDevice>202011</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""403001"" type=""Connection"">
				<Name>Room A RMC to Display</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>201011</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>201012</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""403002"" type=""Connection"">
				<Name>Room B RMC to Display</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>202011</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>1</SourceAddress>
				<DestinationDevice>202012</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""402003"" type=""Connection"">
				<Name>Switcher to 1D-102 SX80 Camera Input</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>3</SourceAddress>
				<DestinationDevice>201002</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""402004"" type=""Connection"">
				<Name>Switcher to 1D-102 SX80 Content Input</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>4</SourceAddress>
				<DestinationDevice>201002</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>3</DestinationAddress>
			</Connection>
			<Connection id=""402005"" type=""Connection"">
				<Name>Switcher to 1D-103 SX80 Camera Input</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>5</SourceAddress>
				<DestinationDevice>202002</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""402006"" type=""Connection"">
				<Name>Switcher to 1D-103 SX80 Content Input</Name>
				<ConnectionType>Video</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>6</SourceAddress>
				<DestinationDevice>202002</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>3</DestinationAddress>
			</Connection>
			<Connection id=""404001"" type=""Connection"">
				<Name>1D-102 Audio</Name>
				<ConnectionType>Audio</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>5</SourceAddress>
				<DestinationDevice>201013</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
			<Connection id=""404002"" type=""Connection"">
				<Name>1D-103 Audio</Name>
				<ConnectionType>Audio</ConnectionType>
				<SourceDevice>200003</SourceDevice>
				<SourceControl>0</SourceControl>
				<SourceAddress>6</SourceAddress>
				<DestinationDevice>202013</DestinationDevice>
				<DestinationControl>0</DestinationControl>
				<DestinationAddress>1</DestinationAddress>
			</Connection>
		</Connections>
		<StaticRoutes />
		<Sources>
			<Source id=""601001"" type=""MetlifeSource"">
				<Name>Laptop</Name>
				<CombineName>1D-102 Laptop</CombineName>
				<Device>201001</Device>
				<Control>0</Control>
				<Address>1</Address>
				<SourceType>Laptop</SourceType>
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceFlags>Share</SourceFlags>
			</Source>
			<Source id=""601002"" type=""MetlifeSource"">
				<Name>TV</Name>
				<CombineName />
				<Device>201005</Device>
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
			<Source id=""602001"" type=""MetlifeSource"">
				<Name>Laptop</Name>
				<CombineName>1D-103 Laptop</CombineName>
				<Device>202001</Device>
				<Control>0</Control>
				<Address>1</Address>
				<SourceType>Laptop</SourceType>
				<ConnectionType>Audio, Video</ConnectionType>
				<SourceFlags>Share</SourceFlags>
			</Source>
			<Source id=""602002"" type=""MetlifeSource"">
				<Name>TV</Name>
				<CombineName />
				<Device>202005</Device>
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
		</Sources>
		<Destinations>
			<Destination id=""701001"" type=""MetlifeDestination"">
				<Name>1D-102 Projector</Name>
				<Device>201012</Device>
				<Control>0</Control>
				<Address>1</Address>
				<ConnectionType>Video</ConnectionType>
				<VtcOption>Main</VtcOption>
				<AudioOption>None</AudioOption>
				<ShareByDefault>true</ShareByDefault>
			</Destination>
			<Destination id=""701002"" type=""MetlifeDestination"">
				<Name>1D-102 Program Audio</Name>
				<Device>202013</Device>
				<Control>0</Control>
				<Address>1</Address>
				<ConnectionType>Audio</ConnectionType>
				<VtcOption>Main</VtcOption>
				<AudioOption>Program, Call</AudioOption>
				<ShareByDefault>true</ShareByDefault>
			</Destination>
			<Destination id=""702001"" type=""MetlifeDestination"">
				<Name>1D-103 Projector</Name>
				<Device>202012</Device>
				<Control>0</Control>
				<Address>1</Address>
				<ConnectionType>Video</ConnectionType>
				<VtcOption>Main</VtcOption>
				<AudioOption>None</AudioOption>
				<ShareByDefault>true</ShareByDefault>
			</Destination>
			<Destination id=""702002"" type=""MetlifeDestination"">
				<Name>1D-103 Program Audio</Name>
				<Device>202013</Device>
				<Control>0</Control>
				<Address>1</Address>
				<ConnectionType>Audio</ConnectionType>
				<VtcOption>Main</VtcOption>
				<AudioOption>Program, Call</AudioOption>
				<ShareByDefault>true</ShareByDefault>
			</Destination>
		</Destinations>
		<DestinationGroups />
	</Routing>
</IcdConfig>"; }
		}

		protected override string AfterConfig { get { return @"<IcdConfig id=""1"" type=""Krang"">
  <Name>IcdCore</Name>
  <Themes>
    <Theme id=""100"" type=""MetlifeTheme"">
      <TvPresets>TvPresets.xml</TvPresets>
      <InvertCombinedState>True</InvertCombinedState>
      <PartitionMenu>
        <InvertedCombinedState>False</InvertedCombinedState>
        <VisibilityJoin>3832</VisibilityJoin>
        <Endpoints>
          <Endpoint>
            <Index>0</Index>
            <EndpointId>701001</EndpointId>
          </Endpoint>
          <Endpoint>
            <Index>1</Index>
            <EndpointId>702001</EndpointId>
          </Endpoint>
        </Endpoints>
        <Partitions>
          <Partition>
            <Index>0</Index>
            <PartitionId>800001</PartitionId>
          </Partition>
        </Partitions>
        <RoomLables>
          <RoomLabel>
            <Index>0</Index>
            <LabelText>Room 1D-102 </LabelText>
          </RoomLabel>
          <RoomLabel>
            <Index>1</Index>
            <LabelText>Room 1D-103</LabelText>
          </RoomLabel>
        </RoomLables>
      </PartitionMenu>
    </Theme>
  </Themes>
  <Panels>
    <Panel id=""300101"" type=""Tsw1060"">
      <Name>Room A Panel</Name>
      <IPID>0x10</IPID>
    </Panel>
    <Panel id=""300201"" type=""Tsw1060"">
      <Name>Room B Panel</Name>
      <IPID>0x11</IPID>
    </Panel>
  </Panels>
  <Ports>
    <Port id=""100000"" type=""TCP"">
      <Name>CP3 to Biamp</Name>
      <Address>10.22.20.47</Address>
      <Port>23</Port>
    </Port>
    <Port id=""101001"" type=""ComPort"">
      <Name>Scaler A to Display serial</Name>
      <Device>201011</Device>
      <Address>1</Address>
    </Port>
    <Port id=""101002"" type=""SSH"">
      <Name>SSH port for Room A Codec</Name>
      <CombineName />
      <Address>10.22.199.104</Address>
      <Port>22</Port>
      <Username>crestron</Username>
      <Password>NFUPcPxs5p2XeKn6W6Qn</Password>
    </Port>
    <Port id=""101003"" type=""IrPort"">
      <Name>CP3 to 1D-102 TV Tuner IR</Name>
      <CombineName />
      <Device>200001</Device>
      <Address>1</Address>
      <Driver>Amino H140 RSDICD Standard.ir</Driver>
      <PulseTime>100</PulseTime>
      <BetweenTime>750</BetweenTime>
    </Port>
    <Port id=""101005"" type=""RelayPort"">
      <Name>CP3 Relay 1 - 1D-102 Screen Up</Name>
      <CombineName />
      <Device>200001</Device>
      <Address>1</Address>
    </Port>
    <Port id=""101006"" type=""RelayPort"">
      <Name>CP3 Relay 2 - 1D-102 Screen Down</Name>
      <CombineName />
      <Device>200001</Device>
      <Address>2</Address>
    </Port>
    <Port id=""102001"" type=""ComPort"">
      <Name>Scaler B to Display serial</Name>
      <Device>202011</Device>
      <Address>1</Address>
    </Port>
    <Port id=""102002"" type=""SSH"">
      <Name>SSH port for Room B Codec</Name>
      <CombineName />
      <Address>10.22.199.105</Address>
      <Port>22</Port>
      <Username>crestron</Username>
      <Password>NFUPcPxs5p2XeKn6W6Qn</Password>
    </Port>
    <Port id=""102003"" type=""IrPort"">
      <Name>CP3 to 1D-103 TV Tuner IR</Name>
      <CombineName />
      <Device>200001</Device>
      <Address>2</Address>
      <Driver>Amino H140 RSDICD Standard.ir</Driver>
      <PulseTime>100</PulseTime>
      <BetweenTime>750</BetweenTime>
    </Port>
    <Port id=""102005"" type=""RelayPort"">
      <Name>CP3 Relay 3 - 1D-103 Screen Up</Name>
      <CombineName />
      <Device>200001</Device>
      <Address>3</Address>
    </Port>
    <Port id=""102006"" type=""RelayPort"">
      <Name>CP3 Relay 4 - 1D-103 Screen Down</Name>
      <CombineName />
      <Device>200001</Device>
      <Address>4</Address>
    </Port>
  </Ports>
  <Devices>
    <Device id=""200001"" type=""ControlSystem"">
      <Name>CP3</Name>
    </Device>
    <Device id=""200002"" type=""BiampTesira"">
      <Name>Biamp</Name>
      <Port>100000</Port>
      <Username />
      <Config>ControlConfig.xml</Config>
    </Device>
    <Device id=""200003"" type=""DmMd16x16"">
      <Name>Video Switcher</Name>
      <IPID>0xE0</IPID>
    </Device>
    <Device id=""200004"" type=""Dmc4kC"">
      <Name>DM Input Card 1</Name>
      <CardNumber>1</CardNumber>
      <SwitcherId>200003</SwitcherId>
      <CresnetId />
    </Device>
    <Device id=""200005"" type=""Dmc4kC"">
      <Name>DM Input Card 6</Name>
      <CardNumber>6</CardNumber>
      <SwitcherId>200003</SwitcherId>
      <CresnetId />
    </Device>
    <Device id=""200006"" type=""Dmc4kCoHd"">
      <Name>DM Output Card 1-2</Name>
      <CardNumber>1</CardNumber>
      <SwitcherId>200003</SwitcherId>
    </Device>
    <Device id=""200007"" type=""GlsPartCn"">
      <Name>Partition Sensor</Name>
      <CresnetID>97</CresnetID>
      <Sensitivity>2</Sensitivity>
    </Device>
    <Device id=""201001"" type=""DmTx201C"">
      <Name>1D-102 Laptop</Name>
      <DmInput>1</DmInput>
      <DmSwitch>200003</DmSwitch>
      <IPID>0xE1</IPID>
    </Device>
    <Device id=""201002"" type=""CiscoCodec"">
      <Name>1D-102 Cisco SX80</Name>
      <CombineName />
      <Port>101002</Port>
      <PeripheralsID />
      <!-- PeripheralsID is a guid that gets generated at start, unique per room -->
      <Input1Type>Camera</Input1Type>
      <Input2Type>None</Input2Type>
      <Input3Type>Content</Input3Type>
      <Input4Type>None</Input4Type>
      <Hide>false</Hide>
    </Device>
    <Device id=""201003"" type=""CiscoCamera"">
      <Name>1D-102 Rear Camera</Name>
      <CombineName />
      <Codec>201002</Codec>
      <CameraId>1</CameraId>
      <PanTiltSpeed>2</PanTiltSpeed>
      <ZoomSpeed>2</ZoomSpeed>
    </Device>
    <Device id=""201004"" type=""CiscoCamera"">
      <Name>1D-102 Front Camera</Name>
      <CombineName />
      <Codec>201002</Codec>
      <CameraId>2</CameraId>
      <PanTiltSpeed>2</PanTiltSpeed>
      <ZoomSpeed>2</ZoomSpeed>
    </Device>
    <Device id=""201005"" type=""IrTvTuner"">
      <Name>1D-102 TV Tuner</Name>
      <CombineName />
      <Port>101003</Port>
      <IrCommands>
        <Channels>
          <Clear>clear</Clear>
          <Enter>enter</Enter>
          <ChannelUp>+</ChannelUp>
          <ChannelDown>-</ChannelDown>
        </Channels>
        <Playback>
          <Repeat>repeat</Repeat>
          <Rewind>rewind</Rewind>
          <FastForward>fastforward</FastForward>
          <Stop>stop</Stop>
          <Play>play</Play>
          <Pause>pause</Pause>
          <Record>record</Record>
        </Playback>
        <Menus>
          <PageUp>pageup</PageUp>
          <PageDown>pagedown</PageDown>
          <TopMenu>topmenu</TopMenu>
          <PopupMenu>popupmenu</PopupMenu>
          <Return>return</Return>
          <Info>info</Info>
          <Eject>eject</Eject>
          <Power>power</Power>
          <Red>red</Red>
          <Green>green</Green>
          <Yellow>yellow</Yellow>
          <Blue>blue</Blue>
          <Up>up</Up>
          <Down>down</Down>
          <Left>left</Left>
          <Right>right</Right>
          <Select>select</Select>
        </Menus>
      </IrCommands>
    </Device>
    <Device id=""201010"" type=""DisplayScreenRelayControl"">
      <Name>1D-102 Projector Screen</Name>
      <CombineName>1D-102 Projector Screen</CombineName>
      <Display>201012</Display>
      <DisplayOffRelay>101005</DisplayOffRelay>
      <DisplayOnRelay>101006</DisplayOnRelay>
      <LatchRelay>false</LatchRelay>
      <RelayHoldTime>5000</RelayHoldTime>
    </Device>
    <Device id=""201011"" type=""DmRmcScalerC"">
      <Name>1D-102 DmRmcScalerC</Name>
      <DmOutput>1</DmOutput>
      <DmSwitch>200003</DmSwitch>
      <IPID>0xE3</IPID>
    </Device>
    <Device id=""201012"" type=""PanasonicDisplay"">
      <Name>1D-102 Display</Name>
      <Port>101001</Port>
    </Device>
    <Device id=""201013"" type=""MockDisplayWithAudio"">
      <Name>1D-102 Audio</Name>
    </Device>
    <Device id=""202001"" type=""DmTx201C"">
      <Name>1D-102 Laptop</Name>
      <DmInput>6</DmInput>
      <DmSwitch>200003</DmSwitch>
      <IPID>0xE2</IPID>
    </Device>
    <Device id=""202002"" type=""CiscoCodec"">
      <Name>1D-103 Cisco SX80</Name>
      <CombineName />
      <Port>102002</Port>
      <PeripheralsID />
      <!-- PeripheralsID is a guid that gets generated at start, unique per room -->
      <Input1Type>Camera</Input1Type>
      <Input2Type>None</Input2Type>
      <Input3Type>Content</Input3Type>
      <Input4Type>None</Input4Type>
      <Hide>false</Hide>
    </Device>
    <Device id=""202003"" type=""CiscoCamera"">
      <Name>1D-103 Front Camera</Name>
      <CombineName />
      <Codec>202002</Codec>
      <CameraId>1</CameraId>
      <PanTiltSpeed>5</PanTiltSpeed>
      <ZoomSpeed>5</ZoomSpeed>
    </Device>
    <Device id=""202004"" type=""CiscoCamera"">
      <Name>1D-103 Rear Camera</Name>
      <CombineName />
      <Codec>202002</Codec>
      <CameraId>2</CameraId>
      <PanTiltSpeed>5</PanTiltSpeed>
      <ZoomSpeed>5</ZoomSpeed>
    </Device>
    <Device id=""202005"" type=""IrTvTuner"">
      <Name>1D-103 TV Tuner</Name>
      <CombineName />
      <Port>102003</Port>
      <IrCommands>
        <Channels>
          <Clear>clear</Clear>
          <Enter>enter</Enter>
          <ChannelUp>+</ChannelUp>
          <ChannelDown>-</ChannelDown>
        </Channels>
        <Playback>
          <Repeat>repeat</Repeat>
          <Rewind>rewind</Rewind>
          <FastForward>fastforward</FastForward>
          <Stop>stop</Stop>
          <Play>play</Play>
          <Pause>pause</Pause>
          <Record>record</Record>
        </Playback>
        <Menus>
          <PageUp>pageup</PageUp>
          <PageDown>pagedown</PageDown>
          <TopMenu>topmenu</TopMenu>
          <PopupMenu>popupmenu</PopupMenu>
          <Return>return</Return>
          <Info>info</Info>
          <Eject>eject</Eject>
          <Power>power</Power>
          <Red>red</Red>
          <Green>green</Green>
          <Yellow>yellow</Yellow>
          <Blue>blue</Blue>
          <Up>up</Up>
          <Down>down</Down>
          <Left>left</Left>
          <Right>right</Right>
          <Select>select</Select>
        </Menus>
      </IrCommands>
    </Device>
    <Device id=""202010"" type=""DisplayScreenRelayControl"">
      <Name>Projector Screen</Name>
      <CombineName>Display Relay</CombineName>
      <Display>202012</Display>
      <DisplayOffRelay>102005</DisplayOffRelay>
      <DisplayOnRelay>102006</DisplayOnRelay>
      <LatchRelay>false</LatchRelay>
      <RelayHoldTime>5000</RelayHoldTime>
    </Device>
    <Device id=""202011"" type=""DmRmcScalerC"">
      <Name>1D-103 DmRmcScalerC</Name>
      <DmOutput>2</DmOutput>
      <DmSwitch>200003</DmSwitch>
      <IPID>0xE4</IPID>
    </Device>
    <Device id=""202012"" type=""PanasonicDisplay"">
      <Name>1D-103 Display</Name>
      <Port>102001</Port>
    </Device>
    <Device id=""202013"" type=""MockDisplayWithAudio"">
      <Name>1D-103 Audio</Name>
    </Device>
  </Devices>
  <Rooms>
    <Room id=""1000"" type=""MetlifeRoom"">
      <Name>Educate Room A</Name>
      <Prefix>US FL Tampa Bldg4 - </Prefix>
      <Number>1D-102</Number>
      <CombinePriority>10</CombinePriority>
      <PhoneNumber>(813) 631 1222</PhoneNumber>
      <Owner>
        <Name>Travis Bennett</Name>
        <Phone />
        <Email>multimediasupport@metlife.com</Email>
      </Owner>
      <DialingPlan>
        <Config>DialingPlan.xml</Config>
        <AudioEndpoint>
          <Device>200002</Device>
          <Control>15</Control>
        </AudioEndpoint>
      </DialingPlan>
      <Devices>
        <Device>200001</Device>
        <Device>200002</Device>
        <Device>200003</Device>
        <Device>200004</Device>
        <Device>200006</Device>
        <Device>200007</Device>
        <Device>201001</Device>
        <Device>201002</Device>
        <Device>201003</Device>
        <Device>201004</Device>
        <Device>201005</Device>
        <Device>201010</Device>
        <Device>201011</Device>
        <Device>201012</Device>
        <Device>201013</Device>
      </Devices>
      <Panels>
        <Panel>300101</Panel>
      </Panels>
      <Ports>
        <Port>100000</Port>
        <Port>101001</Port>
        <Port>101002</Port>
        <Port>101003</Port>
        <Port>101005</Port>
        <Port>101006</Port>
      </Ports>
      <Sources>
        <Source combine=""Single,Master"">601001</Source>
        <Source combine=""Single,Master"">602001</Source>
      </Sources>
      <Destinations>
        <Destination combine=""Single,Master"">701001</Destination>
        <Destination combine=""Single,Master"">701002</Destination>
      </Destinations>
      <DestinationGroups />
      <VolumePoints>
        <VolumePoint>
          <Device>200002</Device>
          <Control>10</Control>
          <VolumeType>Program</VolumeType>
        </VolumePoint>
        <VolumePoint>
          <Device>200002</Device>
          <Control>11</Control>
          <VolumeType>Vtc</VolumeType>
        </VolumePoint>
        <VolumePoint>
          <Device>200002</Device>
          <Control>12</Control>
          <VolumeType>Atc</VolumeType>
        </VolumePoint>
        <VolumePoint>
          <Device>200002</Device>
          <Control>13</Control>
          <VolumeType>Sr</VolumeType>
        </VolumePoint>
      </VolumePoints>
    </Room>
    <Room id=""2000"" type=""MetlifeRoom"">
      <Name>Educate Room B</Name>
      <Prefix>US FL Tampa Bldg4 - </Prefix>
      <Number>1D-103</Number>
      <CombinePriority>20</CombinePriority>
      <PhoneNumber>(813) 613 1223</PhoneNumber>
      <Owner>
        <Name>Travis Bennett</Name>
        <Phone />
        <Email>multimediasupport@metlife.com</Email>
      </Owner>
      <DialingPlan>
        <Config>DialingPlan.xml</Config>
        <AudioEndpoint>
          <Device>200002</Device>
          <Control>25</Control>
        </AudioEndpoint>
      </DialingPlan>
      <Devices>
        <Device>200001</Device>
        <Device>200002</Device>
        <Device>200003</Device>
        <Device>200005</Device>
        <Device>200006</Device>
        <Device>200007</Device>
        <Device>202001</Device>
        <Device>202002</Device>
        <Device>202003</Device>
        <Device>202005</Device>
        <Device>202010</Device>
        <Device>202011</Device>
        <Device>202012</Device>
        <Device>202013</Device>
      </Devices>
      <Panels>
        <Panel>300201</Panel>
      </Panels>
      <Ports>
        <Port>100000</Port>
        <Port>102001</Port>
        <Port>102002</Port>
        <Port>102003</Port>
        <Port>102005</Port>
        <Port>102006</Port>
      </Ports>
      <Sources>
        <Source combine=""Single,Slave"">602001</Source>
        <Source combine=""Single"">602002</Source>
      </Sources>
      <Destinations>
        <Destination combine=""Single,Slave"">702001</Destination>
        <Destination combine=""Single"">702002</Destination>
      </Destinations>
      <DestinationGroups />
      <VolumePoints>
        <VolumePoint>
          <Device>200002</Device>
          <Control>20</Control>
          <VolumeType>Program</VolumeType>
        </VolumePoint>
        <VolumePoint>
          <Device>200002</Device>
          <Control>21</Control>
          <VolumeType>Vtc</VolumeType>
        </VolumePoint>
        <VolumePoint>
          <Device>200002</Device>
          <Control>22</Control>
          <VolumeType>Atc</VolumeType>
        </VolumePoint>
        <VolumePoint>
          <Device>200002</Device>
          <Control>23</Control>
          <VolumeType>Sr</VolumeType>
        </VolumePoint>
      </VolumePoints>
    </Room>
  </Rooms>
  <Partitioning id=""300"" type=""PartitionManager"">
    <Cells>
      <Cell id=""110000000"" type=""Cell"">
        <Name>Cell 1</Name>
        <Room>1000</Room>
      </Cell>
      <Cell id=""110000001"" type=""Cell"">
        <Name>Cell 2</Name>
        <Room>2000</Room>
      </Cell>
    </Cells>
    <Partitions>
      <Partition id=""800001"" type=""MetlifePartition"">
        <Name>Wall</Name>
        <PartitionControls>
          <PartitionControl>
            <Device>200002</Device>
            <Control>3</Control>
          </PartitionControl>
          <PartitionControl>
            <Device>200007</Device>
            <Control>0</Control>
          </PartitionControl>
        </PartitionControls>
        <CellA>110000000</CellA>
        <CellB>110000001</CellB>
      </Partition>
    </Partitions>
  </Partitioning>
  <Routing id=""101010"" type=""RoutingGraph"">
    <Name />
    <Connections>
      <Connection id=""401001"" type=""Connection"">
        <Name>1D-102 Laptop</Name>
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceDevice>201001</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""401002"" type=""Connection"">
        <Name>1D-103 Laptop</Name>
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceDevice>202001</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>6</DestinationAddress>
      </Connection>
      <Connection id=""401003"" type=""Connection"">
        <Name>1D-102 Rear Camera to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>201003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>2</DestinationAddress>
      </Connection>
      <Connection id=""401004"" type=""Connection"">
        <Name>1D-102 Front Camera to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>201004</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>3</DestinationAddress>
      </Connection>
      <Connection id=""401005"" type=""Connection"">
        <Name>1D-102 SX80 Out1 to Switcher</Name>
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceDevice>201002</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>4</DestinationAddress>
      </Connection>
      <Connection id=""401006"" type=""Connection"">
        <Name>1D-102 SX80 Out2 to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>201002</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>2</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>5</DestinationAddress>
      </Connection>
      <Connection id=""401007"" type=""Connection"">
        <Name>1D-103 Rear Camera to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>202003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>7</DestinationAddress>
      </Connection>
      <Connection id=""401008"" type=""Connection"">
        <Name>1D-103 Front Camera to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>202004</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>8</DestinationAddress>
      </Connection>
      <Connection id=""401009"" type=""Connection"">
        <Name>1D-103 SX80 Out1 to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>202002</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>9</DestinationAddress>
      </Connection>
      <Connection id=""401010"" type=""Connection"">
        <Name>1D-103 SX80 Out2 to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>202002</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>2</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>10</DestinationAddress>
      </Connection>
      <Connection id=""401011"" type=""Connection"">
        <Name>1D-102 TV Tuner to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>201005</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>11</DestinationAddress>
      </Connection>
      <Connection id=""401012"" type=""Connection"">
        <Name>1D-103 TV Tuner to Switcher</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>202005</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>200003</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>12</DestinationAddress>
      </Connection>
      <Connection id=""402001"" type=""Connection"">
        <Name>Room A RMC</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>201011</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""402002"" type=""Connection"">
        <Name>Room B RMC</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>2</SourceAddress>
        <DestinationDevice>202011</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""403001"" type=""Connection"">
        <Name>Room A RMC to Display</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>201011</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>201012</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""403002"" type=""Connection"">
        <Name>Room B RMC to Display</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>202011</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>1</SourceAddress>
        <DestinationDevice>202012</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""402003"" type=""Connection"">
        <Name>Switcher to 1D-102 SX80 Camera Input</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>3</SourceAddress>
        <DestinationDevice>201002</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""402004"" type=""Connection"">
        <Name>Switcher to 1D-102 SX80 Content Input</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>4</SourceAddress>
        <DestinationDevice>201002</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>3</DestinationAddress>
      </Connection>
      <Connection id=""402005"" type=""Connection"">
        <Name>Switcher to 1D-103 SX80 Camera Input</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>5</SourceAddress>
        <DestinationDevice>202002</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""402006"" type=""Connection"">
        <Name>Switcher to 1D-103 SX80 Content Input</Name>
        <ConnectionType>Video</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>6</SourceAddress>
        <DestinationDevice>202002</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>3</DestinationAddress>
      </Connection>
      <Connection id=""404001"" type=""Connection"">
        <Name>1D-102 Audio</Name>
        <ConnectionType>Audio</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>5</SourceAddress>
        <DestinationDevice>201013</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
      <Connection id=""404002"" type=""Connection"">
        <Name>1D-103 Audio</Name>
        <ConnectionType>Audio</ConnectionType>
        <SourceDevice>200003</SourceDevice>
        <SourceControl>0</SourceControl>
        <SourceAddress>6</SourceAddress>
        <DestinationDevice>202013</DestinationDevice>
        <DestinationControl>0</DestinationControl>
        <DestinationAddress>1</DestinationAddress>
      </Connection>
    </Connections>
    <StaticRoutes />
    <Sources>
      <Source id=""601001"" type=""MetlifeSource"">
        <Name>Laptop</Name>
        <CombineName>1D-102 Laptop</CombineName>
        <Device>201001</Device>
        <Control>0</Control>
        <Address>1</Address>
        <SourceType>Laptop</SourceType>
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceFlags>Share</SourceFlags>
      </Source>
      <Source id=""601002"" type=""MetlifeSource"">
        <Name>TV</Name>
        <CombineName />
        <Device>201005</Device>
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
      <Source id=""602001"" type=""MetlifeSource"">
        <Name>Laptop</Name>
        <CombineName>1D-103 Laptop</CombineName>
        <Device>202001</Device>
        <Control>0</Control>
        <Address>1</Address>
        <SourceType>Laptop</SourceType>
        <ConnectionType>Audio, Video</ConnectionType>
        <SourceFlags>Share</SourceFlags>
      </Source>
      <Source id=""602002"" type=""MetlifeSource"">
        <Name>TV</Name>
        <CombineName />
        <Device>202005</Device>
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
    </Sources>
    <Destinations>
      <Destination id=""701001"" type=""MetlifeDestination"">
        <Name>1D-102 Projector</Name>
        <Device>201012</Device>
        <Control>0</Control>
        <Address>1</Address>
        <ConnectionType>Video</ConnectionType>
        <VtcOption>Main</VtcOption>
        <AudioOption>None</AudioOption>
        <ShareByDefault>true</ShareByDefault>
      </Destination>
      <Destination id=""701002"" type=""MetlifeDestination"">
        <Name>1D-102 Program Audio</Name>
        <Device>202013</Device>
        <Control>0</Control>
        <Address>1</Address>
        <ConnectionType>Audio</ConnectionType>
        <VtcOption>Main</VtcOption>
        <AudioOption>Program, Call</AudioOption>
        <ShareByDefault>true</ShareByDefault>
      </Destination>
      <Destination id=""702001"" type=""MetlifeDestination"">
        <Name>1D-103 Projector</Name>
        <Device>202012</Device>
        <Control>0</Control>
        <Address>1</Address>
        <ConnectionType>Video</ConnectionType>
        <VtcOption>Main</VtcOption>
        <AudioOption>None</AudioOption>
        <ShareByDefault>true</ShareByDefault>
      </Destination>
      <Destination id=""702002"" type=""MetlifeDestination"">
        <Name>1D-103 Program Audio</Name>
        <Device>202013</Device>
        <Control>0</Control>
        <Address>1</Address>
        <ConnectionType>Audio</ConnectionType>
        <VtcOption>Main</VtcOption>
        <AudioOption>Program, Call</AudioOption>
        <ShareByDefault>true</ShareByDefault>
      </Destination>
    </Destinations>
    <DestinationGroups />
  </Routing>
</IcdConfig>"; } }

		protected override IConfigVersionMigrator InstantiateMigrator()
		{
			return new ConfigVersionMigrator_4x0_To_5x0();
		}
	}
}
