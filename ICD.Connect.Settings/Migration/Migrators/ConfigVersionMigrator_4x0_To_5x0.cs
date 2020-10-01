using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXmlLinq;
#else
using System.Xml.Linq;
#endif

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
					<CellA>20002</CellA>
					<CellB>20003</CellB>
					<PartitionControls>
						<PartitionControl>
							<Device>30001</Device>
						</PartitionControl>
					</PartitionControls>
				</Partition>
				<Partition id="20011" type="Partition">
					<Name>Conference Rm B/Huddle Rm B</Name>
					<CellA>20005</CellA>
					<CellB>20006</CellB>
					<PartitionControls>
						<PartitionControl>
							<Device>30002</Device>
						</PartitionControl>
					</PartitionControls>
				</Partition>
				<Partition id="20012" type="Partition">
					<Name>Huddle Rm B/Huddle Rm A</Name>
					<CellA>20006</CellA>
					<CellB>20007</CellB>
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
				return xml;

			XElement partitions = partitioning.Element("Partitions");
			if (partitions == null)
				return xml;

			// Get all of the existing partition elements
			XElement[] partitionElements = partitions.Elements("Partition").ToArray();
			if (partitionElements.Length == 0)
				return xml;

			// Known ids
			IcdHashSet<int> ids = ConfigUtils.GetIdsInDocument(xml).ToIcdHashSet();
			IcdHashSet<int> roomIds = GetPartitionRoomIds(partitionElements).ToIcdHashSet();

			// Build the cells
			XElement cells = new XElement("Cells");
			partitioning.AddFirst(cells);

			Dictionary<int, int> roomToCell = new Dictionary<int, int>();

			int cellIndex = 0;
			foreach (int roomId in roomIds.Order())
			{
				int cellId = IdUtils.GetNewId(ids, eSubsystem.Cells);

				XElement cell = new XElement("Cell");
				cell.Add(new XAttribute("id", cellId));
				cell.Add(new XAttribute("type", "Cell"));

				cell.Add(new XElement("Name", string.Format("Cell {0}", ++cellIndex)));
				cell.Add(new XElement("Room", roomId));

				cells.Add(cell);
				ids.Add(cellId);
				roomToCell.Add(roomId, cellId);
			}

			// Update the partitions to refer to cells instead of rooms
			foreach (XElement partition in partitionElements)
			{
				XElement rooms = partition.Element("Rooms");
				if (rooms == null)
				{
					partition.Remove();
					continue;
				}

				rooms.Remove();

				int[] partitionRoomIds = rooms.Elements("Room")
				                              .Select(e => int.Parse(e.Value))
				                              .Distinct()
				                              .ToArray();

				if (partitionRoomIds.Length < 2)
				{
					partition.Remove();
					continue;
				}

				int room1 = partitionRoomIds[0];
				int room2 = partitionRoomIds[1];

				XElement cellA = new XElement("CellA", roomToCell[room1]);
				XElement cellB = new XElement("CellB", roomToCell[room2]);

				partition.Add(cellA);
				partition.Add(cellB);
			}

			return document.ToString();
		}

		private static IEnumerable<int> GetPartitionRoomIds(IEnumerable<XElement> partitionElements)
		{
			if (partitionElements == null)
				throw new ArgumentNullException("partitionElements");

			return partitionElements.Select(element => element.Element("Rooms"))
			                        .Where(rooms => rooms != null)
			                        .SelectMany(rooms => rooms.Elements("Room"))
			                        .Select(room => int.Parse(room.Value));
		}
	}
}
