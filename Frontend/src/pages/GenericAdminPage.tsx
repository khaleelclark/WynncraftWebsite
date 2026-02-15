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

import { ReactElement, useEffect, useState } from "react";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import CircularProgress from "@mui/material/CircularProgress";
import React from "react";
import { CustomFormProps } from "./CustomForm";
import CloseIcon from "@mui/icons-material/Close";
import Tooltip from "@mui/material/Tooltip";
import axios from "axios";
import Paper from "@mui/material/Paper";
import { useApiErrorSnackbar } from "./ApiErrorSnackbar";

interface GenericAdminPageProps {
  apiGetEndpoint: string;
  apiDeleteEndpoint: string;
  createForm: ReactElement<CustomFormProps>;
  label: string;
}
const dialogSlotProps = {
  paper: {
    sx: (theme: any) => ({
      bgcolor: "background.default",
      color: "text.primary",
      borderRadius: 3,
      border: `1px solid ${theme.palette.divider}`,
      boxShadow: "0 20px 60px rgba(0,0,0,0.6)",
    }),
  },
};

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
  const { handleError, handleSuccess, SnackbarElement } = useApiErrorSnackbar();

  const singular = (s: string) => s.replace(/s$/i, ""); // good enough for your labels
  const entity = singular(label); // "Guild Member", "Event", ...

  const actionVerbPast = (a: "create" | "update" | "delete") =>
    a === "create" ? "added" : a === "update" ? "updated" : "deleted";

  useEffect(() => {
    axios
      .get(apiGetEndpoint)
      .then(res => {
        setMembers(res.data);
      })
      .catch(handleError);
  }, []);

  // Heuristic date formatting based on field name.
  const formatDateTime = (iso: string) =>
    new Date(iso).toLocaleString(undefined, {
      dateStyle: "short",
      timeStyle: "short",
    });

  const formatDateOnly = (iso: string) =>
    new Date(iso).toLocaleDateString(undefined, {
      dateStyle: "short",
    });

  const isDateOnlyField = (key: string) => key === "joinDate";
  const isDateTimeField = (key: string) =>
    /(date|time|at|created|updated|synced|completed|start|end)/i.test(key) &&
    !isDateOnlyField(key);

  // Build grid columns from the first row shape, then append actions.
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

            valueGetter: (value: any) => {
              // Display formatting for dates, arrays, and nested objects.
              if (
                typeof value === "string" &&
                (isDateTimeField(key) || isDateOnlyField(key))
              ) {
                return isDateOnlyField(key)
                  ? formatDateOnly(value)
                  : formatDateTime(value);
              }

              if (Array.isArray(value)) {
                // If it's an array, join the 'name' properties
                return value.map(item => item?.name ?? "").join(", ");
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
                  sx={{
                    color: "text.secondary",
                    "&:hover": {
                      bgcolor: theme => `${theme.palette.primary.main}22`,
                      color: "text.primary",
                    },
                  }}
                  onClick={() => {
                    setAutofillData(params.row);
                    setOpenUpdateDialog(true);
                  }}
                >
                  <EditIcon />
                </IconButton>
                <IconButton
                  sx={{
                    color: "text.secondary",
                    "&:hover": {
                      bgcolor: theme => `${theme.palette.primary.main}22`,
                      color: "text.primary",
                    },
                  }}
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
      prev.map(item => (item.id === updatedRecord.id ? updatedRecord : item)),
    );
    setOpenUpdateDialog(false);
  };

  const entityName = singular(label);
  const updatedCreateForm = React.cloneElement(createForm, {
    changeRecordsCallback: addNewRecord,
    isPost: true,
    onSubmitSuccess: () => {
      setOpenCreationDialog(false);
      handleSuccess(`${entityName} added successfully.`);
    },
    onSubmitError: ({ error }: any) => {
      handleError(error, { prefix: `Failed to add ${entityName}` });
    },
  });

  const updatedUpdateForm = React.cloneElement(createForm, {
    changeRecordsCallback: updateRecord,
    autofillData,
    isPost: false,
    onSubmitSuccess: () => {
      setOpenUpdateDialog(false);
      handleSuccess(`${entityName} updated successfully.`);
    },
    onSubmitError: ({ error }: any) => {
      handleError(error, { prefix: `Failed to update ${entityName}` });
    },
  });

  const blockBackdropAndEscClose =
    (setter: React.Dispatch<React.SetStateAction<boolean>>) =>
    (_event: object, reason?: "backdropClick" | "escapeKeyDown") => {
      // Force explicit close so forms don't lose state on accidental clicks.
      if (reason === "backdropClick" || reason === "escapeKeyDown") return;
      setter(false);
    };

  const el = (
    <>
      {SnackbarElement}
      {members ? (
        <Box
          sx={{
            padding: 3,
            bgcolor: "background.default",
            color: "text.primary",
            borderRadius: 3,
            height: "100%",
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
              ${selectedUser && selectedUser.name ? selectedUser.name : ""}?`}
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
                      setMembers(prev => prev.filter(r => r.id !== idToDelete));
                      setIdToDelete(-1);
                      setOpenDeletionDialog(false);
                      handleSuccess(`${entityName} deleted successfully.`);
                    })
                    .catch(error => {
                      handleError(error, {
                        prefix: `Failed to delete ${entityName}`,
                      });
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
              slotProps={dialogSlotProps}
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
                      color: "text.secondary",
                      "&:hover": {
                        bgcolor: theme => `${theme.palette.primary.main}22`,
                        color: "text.primary",
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
              <DialogContent
                sx={{
                  position: "relative",
                  pt: 6,
                  bgcolor: "background.default",
                }}
              >
                <Tooltip title="Close window" arrow>
                  <IconButton
                    aria-label="close"
                    onClick={() => setOpenUpdateDialog(false)}
                    sx={{
                      position: "absolute",
                      right: 8,
                      top: 8,
                      color: "text.secondary",
                      "&:hover": {
                        bgcolor: theme => `${theme.palette.primary.main}22`,
                        color: "text.primary",
                      },
                    }}
                  >
                    <CloseIcon />
                  </IconButton>
                </Tooltip>

                {updatedUpdateForm}
              </DialogContent>
            </Dialog>

            <Paper
              elevation={2}
              sx={{
                mt: 3,
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
                  {label}
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
                columnHeaderHeight={60}
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
                    justifyContent: "center",
                    textAlign: "center",
                    py: 2,
                    whiteSpace: "normal",
                    lineHeight: "1.35",
                    wordBreak: "break-word",
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
