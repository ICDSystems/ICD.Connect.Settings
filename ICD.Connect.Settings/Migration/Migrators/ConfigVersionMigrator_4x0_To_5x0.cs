using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.CrestronXmlLinq;

namespace ICD.Connect.Settings.Migration.Migrators
{
	public sealed class ConfigVersionMigrator_4x0_To_5x0 : AbstractConfigVersionMigrator
	{
		/*
		<Partitioning id="101011" type="PartitionManager">
			<Partitions>
				<Partition id="12000" type="MetlifePartition">
					<Name>Wall 1</Name>
					<PartitionControls>
						<PartitionControl>
							<Device>200001</Device>
						</PartitionControl>
					</PartitionControls>
					<Rooms>
						<Room>10001</Room>
						<Room>10002</Room>
					</Rooms>
				</Partition>
				<Partition id="12001" type="MetlifePartition">
					<Name>Wall 2</Name>
					<PartitionControls>
						<PartitionControl>
							<Device>200002</Device>
						</PartitionControl>
					</PartitionControls>
					<Rooms>
						<Room>10002</Room>
						<Room>10003</Room>
					</Rooms>
				</Partition>
			</Partitions>
		</Partitioning>
		*/

		// Becomes

		/*
		<Partitioning id="20000" type="PartitionManager">
			<Cells>
				<Cell id="20001" type="Cell">
					<Name>Cell 1</Name>
					<Room>10001</Room>
					<Column>1</Column>
					<Row>0</Row>
				</Cell>
				<Cell id="20002" type="Cell">
					<Name>Cell 2</Name>
					<Room>10001</Room>
					<Column>1</Column>
					<Row>1</Row>
				</Cell>
				<Cell id="20003" type="Cell">
					<Name>Cell 3</Name>
					<Room>10002</Room>
					<Column>1</Column>
					<Row>2</Row>
				</Cell>
				<Cell id="20004" type="Cell">
					<Name>Cell 4</Name>
					<Room>10002</Room>
					<Column>1</Column>
					<Row>3</Row>
				</Cell>
				<Cell id="20005" type="Cell">
					<Name>Cell 5</Name>
					<Room>10002</Room>
					<Column>2</Column>
					<Row>3</Row>
				</Cell>
				<Cell id="20006" type="Cell">
					<Name>Cell 6</Name>
					<Room>10003</Room>
					<Column>3</Column>
					<Row>3</Row>
				</Cell>
				<Cell id="20007" type="Cell">
					<Name>Cell 7</Name>
					<Room>10004</Room>
					<Column>3</Column>
					<Row>2</Row>
				</Cell>
			</Cells>
			<Partitions>
				<Partition id="20010" type="Partition">
					<Name>Conference Rm A/Conference Rm B</Name>
					<Cell1>20002</Cell1>
					<Cell2>20003</Cell2>
					<PartitionControls>
						<PartitionControl>
							<Device>30001</Device>
						</PartitionControl>
					</PartitionControls>
				</Partition>
				<Partition id="20011" type="Partition">
					<Name>Conference Rm B/Huddle Rm B</Name>
					<Cell1>20005</Cell1>
					<Cell2>20006</Cell2>
					<PartitionControls>
						<PartitionControl>
							<Device>30002</Device>
						</PartitionControl>
					</PartitionControls>
				</Partition>
				<Partition id="20012" type="Partition">
					<Name>Huddle Rm B/Huddle Rm A</Name>
					<Cell1>20006</Cell1>
					<Cell2>20007</Cell2>
					<PartitionControls>
						<PartitionControl>
							<Device>30003</Device>
						</PartitionControl>
					</PartitionControls>
				</Partition>
			</Partitions>
		</Partitioning>
		*/

		/// <summary>
		/// Gets the starting version for the input configuration.
		/// </summary>
		public override Version From { get { return new Version(4, 0); } }

		/// <summary>
		/// Gets the resulting version for the output configuration.
		/// </summary>
		public override Version To { get { return new Version(5, 0); } }

		/// <summary>
		/// Migrates the input xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override string Migrate(string xml)
		{
			XDocument document = XDocument.Parse(xml);
			XElement root = document.Root;
			if (root == null)
				throw new FormatException();

			XElement partitioning = root.Element("Partitioning");
			if (partitioning == null)
			{
				partitioning = new XElement("Partitioning");
				root.Add(partitioning);
			}

			// Put each room in a cell
			XElement rooms = root.Element("Rooms");
			int[] roomIds = GetRoomIds(rooms).ToArray();

			// Add cells to partitions

			throw new NotImplementedException();
		}

		private IEnumerable<int> GetRoomIds(XElement rooms)
		{
			throw new NotImplementedException();
		}
	}
}
