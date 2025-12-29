import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import Typography from "@mui/material/Typography";
import { GridColDef } from "@mui/x-data-grid";
import { DataGrid } from "@mui/x-data-grid/DataGrid";
import axios from "axios";
import { ReactElement, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

interface GenericAdminPageProps {
  apiEndpoint: string;
  createForm: ReactElement;
  rowId: string;
}

interface GuildMember {
  guildMemberId: number;
  mainUsername: string;
  minecraftUsername: string;
  rankName?: string;
  joinDate?: string;
  wynncraftRank?: string;
  uuid?: string;
}

export const GenericAdminPage = ({
  apiEndpoint,
  createForm,
  rowId,
}: GenericAdminPageProps) => {
  const [members, setMembers] = useState<GuildMember[]>([]);
  const [open, setOpen] = useState(false);
  useEffect(() => {
    axios.get(apiEndpoint).then((res) => {
      setMembers(res.data);
    });
  }, []);

  const columns: GridColDef[] =
    members.length !== 0
      ? Object.keys(members[0]).map((key) => ({
          field: key,
          headerName: key
            .replace(/([A-Z])/g, " $1")
            .replace(/^./, (c) => c.toUpperCase())
            .trim(),
          width: 150,
        }))
      : [];

  const el = (
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
      <Box sx={{ width: "100%", maxWidth: 1100 }}>
        <Button onClick={() => setOpen(true)}>test</Button>
        <Dialog
          open={open}
          onClose={() => setOpen(false)}
          maxWidth="sm"
          fullWidth
        >
          <DialogContent dividers>{createForm}</DialogContent>
        </Dialog>
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
          getRowId={(row) => row[rowId]}
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
      </Box>
    </Box>
  );
  return el;
};
