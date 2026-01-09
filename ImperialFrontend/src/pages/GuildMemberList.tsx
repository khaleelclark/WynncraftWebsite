import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import axios from "axios";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Paper from "@mui/material/Paper";
import { Header } from "./Header";

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
    axios.get("/api/guildmembers/public").then(res => setMembers(res.data));
  }, []);

  const columns: GridColDef[] = [
    {
      field: "id",
      headerName: "Id",
      minWidth: 75,
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
        <Box
          onClick={() => navigate(`/profile/${params.row.id}`)}
          sx={{
            display: "flex",
            alignItems: "center",
            cursor: "pointer",
            gap: 1,
            "&:hover": {
              color: "text.secondary",
            },
          }}
        >
          <Box
            component="img"
            src={`https://mc-heads.net/avatar/${params.row.uuid}/45/`}
            alt="Skin"
            sx={{
              width: 45,
              height: 45,
              borderRadius: 0.5,
            }}
          />

          <Box component="span">{params.row.name}</Box>
        </Box>
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
      minWidth: 125,
      flex: 0.7,
      align: "center",
      headerAlign: "center",
      sortable: false,
      renderCell: (params: any) => (
        <Button
          variant="contained"
          color="primary"
          size="small"
          onClick={() => navigate(`/profile/${params.row.id}`)}
          sx={{
            borderRadius: 1,
            px: 2,
          }}
        >
          View
        </Button>
      ),
    },
  ];

  return (
    <>
      <Header />
      <Box
        sx={{
          p: 3,
          bgcolor: "background.default",
          color: "text.primary",
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
          <Paper
            elevation={2}
            sx={{
              overflow: "hidden",
              borderRadius: 2,
            }}
          >
            <Box
              sx={{
                px: 3,
                py: 2,
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
              }}
            >
              <Typography
                variant="h5"
                sx={{
                  fontWeight: 800,
                  letterSpacing: 0.3,
                }}
              >
                Members
              </Typography>

              <Typography variant="body2" color="text.secondary">
                {members.length} total
              </Typography>
            </Box>
            <DataGrid
              rows={members}
              columns={columns}
              getRowId={row => row.id}
              getRowHeight={() => "auto"}
              columnHeaderHeight={70}
              pageSizeOptions={[20, 50, 100]}
              initialState={{
                pagination: { paginationModel: { pageSize: 20, page: 0 } },
              }}
              disableRowSelectionOnClick
              sx={{
                fontSize: "1rem",
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
          </Paper>
        </Box>
      </Box>
    </>
  );
};

export default GuildMemberList;
