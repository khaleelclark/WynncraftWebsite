import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import axios from "axios";
import Button from "@mui/material/Button";

interface GuildMember {
  guildMemberId: number;
  mainUsername: string;
  minecraftUsername: string;
  rankName?: string;
  joinDate?: string;
  wynncraftRank?: string;
  uuid?: string;
}

const GuildMemberList: React.FC = () => {
  const [members, setMembers] = useState<GuildMember[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    axios.get("/api/guildmembers").then((res) => setMembers(res.data));
  }, []);

  const columns: GridColDef[] = [
    {
      field: "guild_member_id",
      headerName: "ID",
      width: 50,
    },
    {
      field: "main_username",
      headerName: "Main Username",
      width: 220,
      renderCell: (params: any) => (
        <span
          style={{ display: "flex", alignItems: "center" }}
          onClick={() => navigate(`/profile/${params.row.guild_member_id}`)}
        >
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
          <span>{params.row.main_username}</span>
        </span>
      ),
    },
    { field: "discord_tag", headerName: "Discord Tag", width: 150 },
    {
      field: "minecraft_username",
      headerName: "Minecraft Username",
      width: 180,
    },
    { field: "rank_name", headerName: "Rank", width: 120 },
    { field: "wynncraft_rank", headerName: "Wynncraft Rank", width: 150 },
    // {
    //   field: "games",
    //   headerName: "Games",
    //   width: 200,
    //   valueGetter: (params: any) =>
    //     params && Array.isArray(params) ? params.join(", ") : "",
    // },
    // {
    //   field: "medals",
    //   headerName: "Medals",
    //   width: 200,
    //   valueGetter: (params: any) =>
    //     params && params.row && Array.isArray(params.row.medals)
    //       ? params.row.medals.join(", ")
    //       : "",
    // },
    //     {
    //   field: "join_date",
    //   headerName: "Join Date",
    //   width: 120,
    // },
    {
      field: "profile",
      headerName: "Profile",
      width: 120,
      sortable: false,
      renderCell: (params: any) => (
        <Button
          style={{
            background: "#6A001B",
            color: "#efdddb",
            border: "none",
            borderRadius: 4,
            padding: "4px 12px",
            cursor: "pointer",
          }}
          onClick={() => navigate(`/profile/${params.row.guild_member_id}`)}
        >
          View
        </Button>
      ),
    },
  ];

  // Use all members for the table

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
        Imperial Guild Members
      </h2>
      <DataGrid
        rows={members}
        columns={columns}
        getRowId={(row) => row.guild_member_id}
        pageSizeOptions={[20, 50, 100]}
        initialState={{
          pagination: { paginationModel: { pageSize: 20, page: 0 } },
        }}
        disableRowSelectionOnClick
        sx={{
          backgroundColor: "#6A001B",
          color: "#F7F2F5",
          border: "1px solid #7A1C69",
          [`.MuiDataGrid-columnHeaders`]: {
            backgroundColor: "#82172e",
            color: "#efdddb",
          },
          [`.MuiDataGrid-row`]: {
            "&:nth-of-type(even)": {
              backgroundColor: "#220c0e",
            },
            "&:nth-of-type(odd)": {
              backgroundColor: "#3C002F",
            },
          },
          [`.MuiDataGrid-footerContainer`]: {
            backgroundColor: "#7A1C69",
            color: "#efdddb",
          },
        }}
      />
    </div>
  );
};

export default GuildMemberList;
