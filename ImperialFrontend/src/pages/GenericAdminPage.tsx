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
import { adminApi } from "../api";
import { ReactElement, useEffect, useState } from "react";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import CircularProgress from "@mui/material/CircularProgress";
import React from "react";
import { CustomFormProps } from "./CustomForm";
import CloseIcon from "@mui/icons-material/Close";
import Tooltip from "@mui/material/Tooltip";

interface GenericAdminPageProps {
  apiGetEndpoint: string;
  apiDeleteEndpoint: string;
  createForm: ReactElement<CustomFormProps>;
  label: string;
}

export const GenericAdminPage = ({
  apiGetEndpoint,
  apiDeleteEndpoint,
  createForm,
  label,
}: GenericAdminPageProps) => {
  const [members, setMembers] = useState<any[]>([]);
  const [openCreationDialog, setOpenCreationDialog] = useState(false);
  const [openUpdateDialog, setOpenUpdateDialog] = useState(false);
  const [openDeletionDialog, setOpenDeletionDialog] = useState(false);
  const [idToDelete, setIdToDelete] = useState(-1);
  const [autofillData, setAutofillData] = useState({});

  useEffect(() => {
    adminApi.get(apiGetEndpoint).then(res => {
      setMembers(res.data);
    });
  }, []);

  const columns: GridColDef[] =
    members.length !== 0
      ? [
          ...Object.keys(members[0]).map(key => ({
            field: key,
            headerName: key
              .replace(/([A-Z])/g, " $1")
              .replace(/^./, c => c.toUpperCase())
              .trim(),
            minWidth: 150,
            flex: 1,

            // Custom render logic for the cell
            valueGetter: (value: any) => {
              if (Array.isArray(value)) {
                // If it's an array, join the 'name' properties
                return value.map(item => item?.name ?? "").join(" | ");
              } else if (value && typeof value === "object") {
                // If it's an object, return its 'name'
                return value.name ?? "";
              } else {
                // Otherwise, return the raw value
                return value ?? "";
              }
            },
          })),
          {
            field: "actions",
            headerName: "Actions",
            width: 120,
            renderCell: (params: any) => (
              <>
                <IconButton
                  sx={{ color: "#FFFFFF" }}
                  onClick={() => {
                    setAutofillData(params.row);
                    setOpenUpdateDialog(true);
                  }}
                >
                  <EditIcon />
                </IconButton>
                <IconButton
                  sx={{ color: "#FFFFFF" }}
                  onClick={() => {
                    setIdToDelete(params.row.id);
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

  const selectedUser = members.find(u => u.id === idToDelete);

  const addNewRecord = (newRecord: Object) => {
    setMembers(prev => [...prev, newRecord]);
    setOpenCreationDialog(false);
  };

  const updateRecord = (updatedRecord: any) => {
    setMembers(prev =>
      prev.map(item => (item.id === updatedRecord.id ? updatedRecord : item))
    );
    setOpenUpdateDialog(false);
  };

  const updatedCreateForm = React.cloneElement(createForm, {
    changeRecordsCallback: addNewRecord,
    isPost: true,
  });

  const updatedUpdateForm = React.cloneElement(createForm, {
    changeRecordsCallback: updateRecord,
    autofillData,
    isPost: false,
  });

  const blockBackdropAndEscClose =
    (setter: React.Dispatch<React.SetStateAction<boolean>>) =>
    (_event: object, reason?: "backdropClick" | "escapeKeyDown") => {
      if (reason === "backdropClick" || reason === "escapeKeyDown") return;
      setter(false);
    };

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
            onClose={blockBackdropAndEscClose(setOpenDeletionDialog)}
            disableEscapeKeyDown
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
          >
            <DialogTitle id="alert-dialog-title">
              Deletion Confirmation
            </DialogTitle>
            <DialogContent>
              <DialogContentText id="alert-dialog-description">
                {`Are you sure you want to delete Id #${idToDelete}
              ${selectedUser ? selectedUser.name : ""}?`}
              </DialogContentText>
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setOpenDeletionDialog(false)}>
                Cancel
              </Button>
              <Button
                onClick={() => {
                  adminApi
                    .delete(`${apiDeleteEndpoint}/${idToDelete}`)
                    .then(() => {
                      setMembers(prev => prev.filter(r => r.id !== idToDelete));
                      setIdToDelete(-1);
                      setOpenDeletionDialog(false);
                    })
                    .catch(error => {
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
          <Box sx={{ width: "100%" }}>
            <Button
              onClick={() => setOpenCreationDialog(true)}
              variant="contained"
            >
              Add new {label.replace("s", "")}
            </Button>
            <Dialog
              open={openCreationDialog}
              onClose={blockBackdropAndEscClose(setOpenCreationDialog)}
              disableEscapeKeyDown
              maxWidth="sm"
              fullWidth
            >
              <DialogContent sx={{ position: "relative", pt: 6 }}>
                <Tooltip title="Close window" arrow>
                  <IconButton
                    aria-label="close"
                    onClick={() => setOpenCreationDialog(false)}
                    sx={{
                      position: "absolute",
                      right: 8,
                      top: 8,
                      color: "#efdddb",
                      "&:hover": {
                        backgroundColor: "rgba(255,255,255,0.08)",
                      },
                    }}
                  >
                    <CloseIcon />
                  </IconButton>
                </Tooltip>

                {updatedCreateForm}
              </DialogContent>
            </Dialog>

            <Dialog
              open={openUpdateDialog}
              onClose={blockBackdropAndEscClose(setOpenUpdateDialog)}
              disableEscapeKeyDown
              maxWidth="sm"
              fullWidth
            >
              <DialogContent sx={{ position: "relative", pt: 6 }}>
                <Tooltip title="Close window" arrow>
                  <IconButton
                    aria-label="close"
                    onClick={() => setOpenUpdateDialog(false)}
                    sx={{
                      position: "absolute",
                      right: 8,
                      top: 8,
                      color: "#efdddb",
                      "&:hover": {
                        backgroundColor: "rgba(255,255,255,0.08)",
                      },
                    }}
                  >
                    <CloseIcon />
                  </IconButton>
                </Tooltip>

                {updatedUpdateForm}
              </DialogContent>
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
              {label}
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
          <CircularProgress />
        </Box>
      )}
    </>
  );
  return el;
};
