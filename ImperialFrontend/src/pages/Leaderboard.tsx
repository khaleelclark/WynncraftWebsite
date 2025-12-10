import React, { useEffect, useState } from "react";
import EventSelector from "./EventSelector";
import { DataGrid, GridColDef } from "@mui/x-data-grid";

interface LeaderboardEntry {
  minecraftUsername: string;
  uuid?: string;
  weekliesCompleted: number;
  warsCompleted: number;
  hoursPlayed: number;
  raidsCompleted: number;
}

import { useNavigate } from "react-router-dom";

const columns: GridColDef[] = [
  {
    field: "minecraftUsername",
    headerName: "Player",
    width: 220,
    renderCell: (params) => (
      <span style={{ display: "flex", alignItems: "center" }}>
        {params.row.uuid && (
          <img
            src={`https://mc-heads.net/avatar/${params.row.uuid}/100/nohelm`}
            alt="Skin"
            style={{
              width: 32,
              height: 32,
              marginRight: 8,
              verticalAlign: "middle",
              borderRadius: 4,
            }}
          />
        )}
        <span>{params.row.minecraftUsername}</span>
        <button
          style={{
            marginLeft: 12,
            background: "#bc511c",
            color: "#efdddb",
            border: "none",
            borderRadius: 4,
            padding: "4px 8px",
            cursor: "pointer",
            fontWeight: 600,
          }}
          onClick={() =>
            (window.location.href = `/profile/${params.row.guildMemberId}`)
          }
        >
          Profile
        </button>
      </span>
    ),
  },
  { field: "raidsCompleted", headerName: "Raids", width: 120, type: "number" },
  {
    field: "weekliesCompleted",
    headerName: "Weeklies",
    width: 120,
    type: "number",
  },
  { field: "warsCompleted", headerName: "Wars", width: 120, type: "number" },
  { field: "hoursPlayed", headerName: "Hours", width: 120, type: "number" },
  {
    field: "lastUpdated",
    headerName: "Last Updated",
    width: 180,
    type: "string",
  },
];

const Leaderboard: React.FC = () => {
  const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [startTime, setStartTime] = useState("00:00");
  const [endTime, setEndTime] = useState("23:59");
  const [selectedEvent, setSelectedEvent] = useState<any>({
    eventId: -1,
    eventName: "All Time",
    eventStart: "2000-01-01",
    eventEnd: "2100-01-01",
  });

  const fetchLeaderboard = () => {
    if (!startDate || !endDate) return;
    // Combine date and time for precise filtering
    const startDateTime = `${startDate}T${startTime}`;
    const endDateTime = `${endDate}T${endTime}`;
    fetch(
      `/api/guildmembers/leaderboard?startDate=${startDateTime}&endDate=${endDateTime}`
    )
      .then((res) => res.json())
      .then((data) => {
        const arr = data.value ?? data ?? [];
        // Sort by raidsCompleted descending
        arr.sort(
          (a: any, b: any) => (b.raidsCompleted ?? 0) - (a.raidsCompleted ?? 0)
        );
        setEntries(arr);
      });
  };

  useEffect(() => {
    if (selectedEvent?.eventId === -2) {
      // Custom: reset dates/times and clear table when switching to custom
      setStartDate("");
      setEndDate("");
      setStartTime("00:00");
      setEndTime("23:59");
      setEntries([]);
      return;
    }
    if (selectedEvent?.eventId === -1) {
      // All Time: set dates/times and fetch
      setStartDate("2000-01-01");
      setEndDate("2100-01-01");
      setStartTime("00:00");
      setEndTime("23:59");
      return;
    }
    if (selectedEvent?.eventStart && selectedEvent?.eventEnd) {
      // Specific event: set dates/times and fetch
      const start = selectedEvent.eventStart.slice(0, 10);
      const end = selectedEvent.eventEnd.slice(0, 10);
      let startT = "00:00";
      let endT = "23:59";
      // If eventStart/eventEnd have time, use it
      if (selectedEvent.eventStart.length > 10) {
        startT = selectedEvent.eventStart.slice(11, 16);
      }
      if (selectedEvent.eventEnd.length > 10) {
        endT = selectedEvent.eventEnd.slice(11, 16);
      }
      setStartDate(start);
      setEndDate(end);
      setStartTime(startT);
      setEndTime(endT);
      return;
    }
  }, [selectedEvent]);

  useEffect(() => {
    // Only fetch for custom when both dates and times are set
    if (
      selectedEvent?.eventId === -2 &&
      startDate &&
      endDate &&
      startTime &&
      endTime
    ) {
      fetchLeaderboard();
    } else if (selectedEvent?.eventId !== -2) {
      fetchLeaderboard();
    }
  }, [startDate, endDate, startTime, endTime]);

  return (
    <div
      style={{
        padding: 24,
        minHeight: 600,
        width: "100%",
        background: "#220c0e",
      }}
    >
      <h2 style={{ color: "#efdddb", marginBottom: 16 }}>
        {selectedEvent?.eventId === -1
          ? "All Time Leaderboard"
          : selectedEvent?.eventId === -2
          ? "Custom Leaderboard"
          : `${selectedEvent?.eventName} Leaderboard`}
      </h2>
      <EventSelector onSelect={setSelectedEvent} />
      {selectedEvent?.eventId === -1 ? (
        <div style={{ color: "#efdddb", marginBottom: 8 }}>
          Date Range: <span style={{ fontWeight: "bold" }}>All Time</span>
        </div>
      ) : (
        <>
          <label style={{ color: "#efdddb" }}>
            Start Date:
            <input
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              style={{
                marginLeft: 8,
                marginRight: 16,
                background: "#511220",
                color: "#efdddb",
                border: "1px solid #bc511c",
                borderRadius: 4,
                padding: 4,
              }}
              disabled={selectedEvent?.eventId !== -2}
            />
          </label>
          <label style={{ color: "#efdddb", marginLeft: 16 }}>
            Start Time:
            <input
              type="time"
              value={startTime}
              onChange={(e) => setStartTime(e.target.value)}
              style={{
                marginLeft: 8,
                marginRight: 16,
                background: "#511220",
                color: "#efdddb",
                border: "1px solid #bc511c",
                borderRadius: 4,
                padding: 4,
              }}
              disabled={selectedEvent?.eventId !== -2}
            />
          </label>
          <label style={{ color: "#efdddb", marginLeft: 16 }}>
            End Date:
            <input
              type="date"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              style={{
                marginLeft: 8,
                marginRight: 16,
                background: "#511220",
                color: "#efdddb",
                border: "1px solid #bc511c",
                borderRadius: 4,
                padding: 4,
              }}
              disabled={selectedEvent?.eventId !== -2}
            />
          </label>
          <label style={{ color: "#efdddb", marginLeft: 16 }}>
            End Time:
            <input
              type="time"
              value={endTime}
              onChange={(e) => setEndTime(e.target.value)}
              style={{
                marginLeft: 8,
                background: "#511220",
                color: "#efdddb",
                border: "1px solid #bc511c",
                borderRadius: 4,
                padding: 4,
              }}
              disabled={selectedEvent?.eventId !== -2}
            />
          </label>
        </>
      )}
      <div style={{ marginTop: 24 }}>
        <DataGrid
          rows={entries}
          columns={columns}
          getRowId={(row) => row.minecraftUsername}
          pageSizeOptions={[20, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 20, page: 0 } },
            sorting: {
              sortModel: [{ field: "raidsCompleted", sort: "desc" }],
            },
          }}
          disableRowSelectionOnClick
          autoHeight
          sx={{
            backgroundColor: "#511220",
            color: "#efdddb",
            border: "1px solid #82172e",
            [`.MuiDataGrid-columnHeaders`]: {
              backgroundColor: "#82172e",
              color: "#efdddb",
            },
            [`.MuiDataGrid-row`]: {
              "&:nth-of-type(even)": {
                backgroundColor: "#220c0e",
              },
              "&:nth-of-type(odd)": {
                backgroundColor: "#511220",
              },
            },
            [`.MuiDataGrid-footerContainer`]: {
              backgroundColor: "#82172e",
              color: "#efdddb",
            },
          }}
        />
      </div>
    </div>
  );
};

export default Leaderboard;
