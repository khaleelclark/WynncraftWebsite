import React, { useEffect, useState } from "react";
import { DataGrid, GridColDef } from "@mui/x-data-grid";

interface LeaderboardEntry {
  minecraftUsername: string;
  playerSkin?: string;
  weekliesCompleted: number;
  warsCompleted: number;
  hoursPlayed: number;
  raidsCompleted: number;
}

const columns: GridColDef[] = [
  {
    field: "minecraftUsername",
    headerName: "Player",
    width: 220,
    renderCell: (params) => (
      <span style={{ display: "flex", alignItems: "center" }}>
        {params.row.playerSkin && (
          <img
            src={params.row.playerSkin}
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
        {params.row.minecraftUsername}
      </span>
    ),
  },
  {
    field: "weekliesCompleted",
    headerName: "Weeklies",
    width: 120,
    type: "number",
  },
  { field: "warsCompleted", headerName: "Wars", width: 120, type: "number" },
  { field: "hoursPlayed", headerName: "Hours", width: 120, type: "number" },
  { field: "raidsCompleted", headerName: "Raids", width: 120, type: "number" },
];

const Leaderboard: React.FC = () => {
  const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");

  const fetchLeaderboard = () => {
    if (!startDate || !endDate) return;
    fetch(
      `/api/guildmembers/leaderboard?startDate=${startDate}&endDate=${endDate}`
    )
      .then((res) => res.json())
      .then((data) => setEntries(data.value ?? data ?? []));
  };

  useEffect(() => {
    if (startDate && endDate) fetchLeaderboard();
    // eslint-disable-next-line
  }, [startDate, endDate]);

  return (
    <div
      style={{
        padding: 24,
        minHeight: 600,
        width: "100%",
        background: "#220c0e",
      }}
    >
      <h2 style={{ color: "#efdddb", marginBottom: 16 }}>Leaderboard</h2>
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
            background: "#511220",
            color: "#efdddb",
            border: "1px solid #bc511c",
            borderRadius: 4,
            padding: 4,
          }}
        />
      </label>
      <div style={{ marginTop: 24 }}>
        <DataGrid
          rows={entries}
          columns={columns}
          getRowId={(row) => row.minecraftUsername}
          pageSizeOptions={[20, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 20, page: 0 } },
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
