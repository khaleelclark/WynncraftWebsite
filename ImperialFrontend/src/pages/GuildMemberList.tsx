import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { DataGrid, GridColDef } from "@mui/x-data-grid";

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
  const [search, setSearch] = useState("");

  useEffect(() => {
    fetch("/api/guildmembers")
      .then((res) => res.json())
      .then((data) => {
        let arr = [];
        if (Array.isArray(data)) {
          arr = data;
        } else if (Array.isArray(data.value)) {
          arr = data.value;
        }
        // Map backend keys to frontend snake_case keys
        const keyMap: Record<string, string> = {
          GuildMemberId: "guildMemberId",
          MainUsername: "main_username",
          MinecraftUsername: "minecraft_username",
          RankName: "rank_name",
          JoinDate: "join_date",
          WynncraftRank: "wynncraft_rank",
          DiscordTag: "discord_tag",
          Uuid: "uuid",
          Games: "games",
          Medals: "medals",
        };
        const mappedArr = arr.map((item: any) => {
          const mapped: any = {};
          Object.keys(item).forEach((key) => {
            mapped[keyMap[key] || key] = item[key];
          });
          return mapped;
        });
        console.log("Mapped members:", mappedArr);
        setMembers(mappedArr);
      });
  }, []);

  const navigate = useNavigate();
  const handleProfileClick = (id: number) => {
    navigate(`/profile/${id}`);
  };

  const columns: GridColDef[] = [
    {
      field: "main_username",
      headerName: "Main Username",
      width: 220,
      renderCell: (params: any) => (
        <span style={{ display: "flex", alignItems: "center" }}>
          {params.row.uuid && (
            <img
              src={`https://crafatar.com/avatars/${params.row.uuid}?size=32&overlay`}
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
    {
      field: "join_date",
      headerName: "Join Date",
      width: 120,
      valueGetter: (params: any) => {
        console.log(params);
        return params ? String(params).slice(0, 10) : "";
      },
    },
    { field: "uuid", headerName: "UUID", width: 250 },
    { field: "wynncraft_rank", headerName: "Wynncraft Rank", width: 150 },
    {
      field: "games",
      headerName: "Games",
      width: 200,
      valueGetter: (params: any) =>
        params && params.row && Array.isArray(params.row.games)
          ? params.row.games.join(", ")
          : "",
    },
    {
      field: "medals",
      headerName: "Medals",
      width: 200,
      valueGetter: (params: any) =>
        params && params.row && Array.isArray(params.row.medals)
          ? params.row.medals.join(", ")
          : "",
    },
    {
      field: "profile",
      headerName: "Profile",
      width: 120,
      sortable: false,
      renderCell: (params: any) => (
        <button
          style={{
            background: "#bc511c",
            color: "#efdddb",
            border: "none",
            borderRadius: 4,
            padding: "4px 12px",
            cursor: "pointer",
          }}
          onClick={() => handleProfileClick(params.row.guild_member_id)}
        >
          View
        </button>
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
      <h2 style={{ color: "#efdddb", marginBottom: 16 }}>Guild Members</h2>
      <DataGrid
        rows={members}
        columns={columns}
        getRowId={(row) => row.guild_member_id}
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
  );
};

export default GuildMemberList;
