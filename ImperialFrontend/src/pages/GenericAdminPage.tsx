import DialogActions from "@mui/material/DialogActions";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import IconButton from "@mui/material/IconButton";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import Typography from "@mui/material/Typography";
import { GridColDef } from "@mui/x-data-grid";
import { DataGrid } from "@mui/x-data-grid/DataGrid";
import axios from "axios";
import { ReactElement, useEffect, useState } from "react";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import CircularProgress from "@mui/material/CircularProgress";

interface GenericAdminPageProps {
  apiGetEndpoint: string;
  apiDeleteEndpoint: string;
  createForm: ReactElement;
  rowId: string;
  rowName: string;
}

export const GenericAdminPage = ({
  apiGetEndpoint,
  apiDeleteEndpoint,
  createForm,
  rowId,
  rowName,
}: GenericAdminPageProps) => {
  const [members, setMembers] = useState<any[]>([]);
  const [openCreationDialog, setOpenCreationDialog] = useState(false);
  const [openDeletionDialog, setOpenDeletionDialog] = useState(false);
  const [idToDelete, setIdToDelete] = useState(-1);

  useEffect(() => {
    axios.get(apiGetEndpoint).then((res) => {
      setMembers(res.data);
    });
  }, []);

  const columns: GridColDef[] =
    members.length !== 0
      ? [
          ...Object.keys(members[0]).map((key) => ({
            field: key,
            headerName: key
              .replace(/([A-Z])/g, " $1")
              .replace(/^./, (c) => c.toUpperCase())
              .trim(),
            width: 150,
          })),
          {
            field: "actions",
            headerName: "Actions",
            width: 100,
            renderCell: (params: any) => (
              <>
                <IconButton sx={{ color: "#FFFFFF" }}>
                  <EditIcon />
                </IconButton>
                <IconButton
                  sx={{ color: "#FFFFFF" }}
                  onClick={() => {
                    setIdToDelete(params.row[rowId]);
                    setOpenDeletionDialog(true);
                  }}
                >
                  <DeleteIcon />
                </IconButton>
              </>
            ),
          },
        ]
      : [];

  const selectedUser = members.find((u) => u[rowId] === idToDelete);

  const el = (
    <>
      {members ? (
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
          <Dialog
            open={openDeletionDialog}
            onClose={() => setOpenDeletionDialog(false)}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
          >
            <DialogTitle id="alert-dialog-title">
              Deletion Confirmation
            </DialogTitle>
            <DialogContent>
              <DialogContentText id="alert-dialog-description">
                {`Are you sure you want to delete Id #${idToDelete}
              ${selectedUser ? selectedUser[rowName] : ""}?`}
              </DialogContentText>
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setOpenDeletionDialog(false)}>
                Cancel
              </Button>
              <Button
                onClick={() => {
                  axios
                    .delete(`${apiDeleteEndpoint}/${idToDelete}`)
                    .then(() => {
                      setMembers((prev) =>
                        prev.filter((r) => r[rowId] !== idToDelete)
                      );
                      setIdToDelete(-1);
                      setOpenDeletionDialog(false);
                    })
                    .catch((error) => {
                      console.log(error);
                      //error dialog message
                    });
                }}
                autoFocus
              >
                Delete
              </Button>
            </DialogActions>
          </Dialog>
          <Box sx={{ width: "100%", maxWidth: 1100 }}>
            <Button onClick={() => setOpenCreationDialog(true)}>test</Button>
            <Dialog
              open={openCreationDialog}
              onClose={() => setOpenCreationDialog(false)}
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
      ) : (
        <Box
          sx={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            height: "80vh",
            width: "100%",
          }}
        >
          {/* <CircularProgress /> */}
        </Box>
      )}
    </>
  );
  return el;
};
