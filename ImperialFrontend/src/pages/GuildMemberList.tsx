import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import axios from "axios";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

interface GuildMember {
  id: number;
  name: string;
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
    axios.get("/api/guildmembers").then(res => setMembers(res.data));
  }, []);

  const columns: GridColDef[] = [
    {
      field: "id",
      headerName: "Id",
      minWidth: 70,
      flex: 0.4,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "name",
      headerName: "Main Username",
      minWidth: 200,
      flex: 1.4,
      headerAlign: "center",
      renderCell: (params: any) => (
        <span
          style={{ display: "flex", alignItems: "center" }}
          onClick={() => navigate(`/profile/${params.row.id}`)}
        >
          {params.row.uuid && (
            <img
              src={`https://mc-heads.net/avatar/${params.row.uuid}/100/`}
              alt="Skin"
              style={{
                width: 40,
                height: 40,
                marginRight: 8,
                verticalAlign: "middle",
                borderRadius: 4,
              }}
            />
          )}
          <span>{params.row.name}</span>
        </span>
      ),
    },
    {
      field: "discordTag",
      headerName: "Discord Tag",
      minWidth: 180,
      flex: 0.7,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "minecraftUsername",
      headerName: "Minecraft Username",
      minWidth: 240,
      flex: 0.7,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "rankName",
      headerName: "Rank",
      minWidth: 150,
      flex: 0.7,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "wynncraftRank",
      headerName: "Wynncraft Rank",
      minWidth: 180,
      flex: 0.7,
      align: "center",
      headerAlign: "center",
      renderCell: (params: any) => params.value || "N/A",
    },
    {
      field: "profile",
      headerName: "Profile",
      minWidth: 120,
      flex: 0.7,
      align: "center",
      headerAlign: "center",
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
          onClick={() => navigate(`/profile/${params.row.id}`)}
        >
          View
        </Button>
      ),
    },
  ];

  return (
    <Box
      sx={{
        padding: 3,
        background: "#220c0e",
        width: "100%",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
      }}
    >
      <Box
        sx={{
          width: "100%",
          maxWidth: { xl: 1400, lg: 1200, md: "100%" },
        }}
      >
        <Typography
          sx={{
            color: "#efdddb",
            marginBottom: 3,
            fontWeight: 600,
            fontSize: 30,
            textAlign: "center",
          }}
        >
          Imperial Guild Members
        </Typography>
        <DataGrid
          rows={members}
          columns={columns}
          getRowId={row => row.id}
          rowHeight={60}
          columnHeaderHeight={60}
          pageSizeOptions={[20, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 20, page: 0 } },
          }}
          disableRowSelectionOnClick
          sx={{
            fontSize: "1rem",
            backgroundColor: "#6A001B",
            color: "#F7F2F5",
            border: "1px solid #7A1C69",
            "& .MuiDataGrid-virtualScroller": {
              backgroundColor: "#220c0e",
            },
            "& .MuiDataGrid-filler": {
              backgroundColor: "#220c0e",
            },
            "& .MuiDataGrid-scrollbarFiller": {
              backgroundColor: "#82172e",
            },
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
            "& .MuiDataGrid-cell": {
              display: "flex",
              alignItems: "center",
              py: 2,
            },
            "& .MuiDataGrid-columnHeaderTitle": {
              textAlign: "center",
              width: "100%",
              py: 2,
              px: 2,
            },
          }}
        />
      </Box>
    </Box>
  );
};

export default GuildMemberList;
