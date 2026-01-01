// src/components/EventTable.tsx
import React, { useEffect, useState } from "react";
import { DataGrid, GridColDef, GridActionsCellItem } from "@mui/x-data-grid";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import Alert, { AlertColor } from "@mui/material/Alert";
import Snackbar from "@mui/material/Snackbar";

import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";
import { CustomTimeAndDateSelector } from "./CustomTimeAndDateSelector";

interface Event {
  id: number;
  name: string;
  eventStart: string;
  eventEnd: string;
}

const EventTable: React.FC = () => {
  const [events, setEvents] = useState<Event[]>([]);
  const [editEvent, setEditEvent] = useState<Event | null>(null);
  const [deleteEvent, setDeleteEvent] = useState<Event | null>(null);

  const [openEdit, setOpenEdit] = useState(false);
  const [openDelete, setOpenDelete] = useState(false);
  const [openAdd, setOpenAdd] = useState(false);

  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] =
    useState<AlertColor>("success");

  const handleSnackbarClose = () => setSnackbarOpen(false);

  const showSnackbar = (message: string, severity: AlertColor) => {
    setSnackbarMessage(message);
    setSnackbarSeverity(severity);
    setSnackbarOpen(true);
  };

  const loadEvents = () => {
    fetch("/api/events")
      .then(res => res.json())
      .then(data => setEvents(Array.isArray(data) ? data : data.value ?? []));
  };

  useEffect(() => {
    loadEvents();
  }, []);

  const handleEdit = (event: Event) => {
    setEditEvent(event);
    setOpenEdit(true);
  };

  const handleDelete = (event: Event) => {
    setDeleteEvent(event);
    setOpenDelete(true);
  };

  const handleEditSubmit = async () => {
    if (!editEvent) return;
    await fetch(`/api/events/${editEvent.id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(editEvent),
    });
    setOpenEdit(false);
    setEditEvent(null);
    loadEvents();
    showSnackbar("Event updated successfully!", "success");
  };

  const handleDeleteSubmit = async () => {
    if (!deleteEvent) return;
    await fetch(`/api/events/${deleteEvent.id}`, {
      method: "DELETE",
    });
    setOpenDelete(false);
    setDeleteEvent(null);
    loadEvents();
    showSnackbar("Event deleted.", "success");
  };

  const handleAddSuccess = () => {
    setOpenAdd(false);
    loadEvents();
    showSnackbar("Event created successfully!", "success");
  };

  const handleAddError = (error: unknown) => {
    console.error(error);
    showSnackbar("Failed to create event. Please try again.", "error");
  };

  const columns: GridColDef[] = [
    { field: "id", headerName: "ID", width: 80 },
    { field: "name", headerName: "Name", width: 180 },
    { field: "eventStart", headerName: "Start", width: 160 },
    { field: "eventEnd", headerName: "End", width: 160 },
    {
      field: "actions",
      type: "actions",
      headerName: "Actions",
      width: 140,
      getActions: params => [
        <GridActionsCellItem
          label="Edit"
          showInMenu
          onClick={() => handleEdit(params.row as Event)}
        />,
        <GridActionsCellItem
          label="Delete"
          showInMenu
          onClick={() => handleDelete(params.row as Event)}
        />,
      ],
    },
  ];

  return (
    <div
      style={{
        height: "100%",
        width: "100%",
        background: "#250404ff",
        borderRadius: 8,
        padding: 16,
      }}
    >
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 8,
        }}
      >
        <h3>All Events</h3>
        <Button
          variant="contained"
          color="primary"
          onClick={() => setOpenAdd(true)}
        >
          Add Event
        </Button>
      </div>

      <DataGrid
        rows={events}
        columns={columns}
        getRowId={row => row.id}
        disableRowSelectionOnClick
      />

      {/* 🔹 Add Event Dialog using CustomForm + pickers */}
      <Dialog
        open={openAdd}
        onClose={() => setOpenAdd(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogContent dividers>
          <CustomForm
            title="Add an Event"
            apiEndpoint="/api/events"
            onSubmitSuccess={handleAddSuccess}
            onSubmitError={handleAddError}
          >
            <CustomTextField id="name" label="Event Name" required />
            <CustomTimeAndDateSelector
              id="eventStart"
              label="Start Date & Time"
            />
            <CustomTimeAndDateSelector id="eventEnd" label="End Date & Time" />
          </CustomForm>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAdd(false)}>Cancel</Button>
        </DialogActions>
      </Dialog>

      {/* Edit Dialog (unchanged except snackbar) */}
      <Dialog open={openEdit} onClose={() => setOpenEdit(false)}>
        <DialogTitle>Edit Event</DialogTitle>
        <DialogContent>
          <TextField
            label="Name"
            value={editEvent?.name ?? ""}
            onChange={e =>
              setEditEvent(ev => (ev ? { ...ev, name: e.target.value } : ev))
            }
            fullWidth
            margin="normal"
          />
          <TextField
            label="Start"
            type="datetime-local"
            value={editEvent?.eventStart?.slice(0, 16) ?? ""}
            onChange={e =>
              setEditEvent(ev =>
                ev ? { ...ev, eventStart: e.target.value } : ev
              )
            }
            fullWidth
            margin="normal"
          />
          <TextField
            label="End"
            type="datetime-local"
            value={editEvent?.eventEnd?.slice(0, 16) ?? ""}
            onChange={e =>
              setEditEvent(ev =>
                ev ? { ...ev, eventEnd: e.target.value } : ev
              )
            }
            fullWidth
            margin="normal"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEdit(false)}>Cancel</Button>
          <Button
            onClick={handleEditSubmit}
            variant="contained"
            color="primary"
          >
            Save
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Dialog */}
      <Dialog open={openDelete} onClose={() => setOpenDelete(false)}>
        <DialogTitle>Delete Event</DialogTitle>
        <DialogContent>
          Are you sure you want to delete event "{deleteEvent?.name}"?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDelete(false)}>Cancel</Button>
          <Button
            onClick={handleDeleteSubmit}
            variant="contained"
            color="error"
          >
            Delete
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar shared by all actions */}
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={4000}
        onClose={handleSnackbarClose}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={handleSnackbarClose}
          severity={snackbarSeverity}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </div>
  );
};

export default EventTable;
