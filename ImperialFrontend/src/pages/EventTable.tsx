import React, { useEffect, useState } from "react";
import { DataGrid, GridColDef, GridActionsCellItem } from "@mui/x-data-grid";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";

interface Event {
  eventId: number;
  eventName: string;
  eventStart: string;
  eventEnd: string;
}

const EventTable: React.FC = () => {
  const [events, setEvents] = useState<Event[]>([]);
  const [editEvent, setEditEvent] = useState<Event | null>(null);
  const [deleteEvent, setDeleteEvent] = useState<Event | null>(null);
  const [openEdit, setOpenEdit] = useState(false);
  const [openDelete, setOpenDelete] = useState(false);
  const [openAdd, setOpenDone] = useState(false);
  const [newEvent, setNewEvent] = useState<Event>({
    eventId: 0,
    eventName: "",
    eventStart: "",
    eventEnd: "",
  });

  useEffect(() => {
    fetch("/api/events")
      .then((res) => res.json())
      .then((data) => setEvents(Array.isArray(data) ? data : data.value ?? []));
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
    await fetch(`/api/events/${editEvent.eventId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(editEvent),
    });
    setOpenEdit(false);
    setEditEvent(null);
    // Refresh
    fetch("/api/events")
      .then((res) => res.json())
      .then((data) => setEvents(Array.isArray(data) ? data : data.value ?? []));
  };

  const handleDeleteSubmit = async () => {
    if (!deleteEvent) return;
    await fetch(`/api/events/${deleteEvent.eventId}`, {
      method: "DELETE",
    });
    setOpenDelete(false);
    setDeleteEvent(null);
    // Refresh
    fetch("/api/events")
      .then((res) => res.json())
      .then((data) => setEvents(Array.isArray(data) ? data : data.value ?? []));
  };

  const columns: GridColDef[] = [
    { field: "eventId", headerName: "ID", width: 80 },
    { field: "eventName", headerName: "Name", width: 180 },
    { field: "eventStart", headerName: "Start", width: 160 },
    { field: "eventEnd", headerName: "End", width: 160 },
    {
      field: "actions",
      type: "actions",
      headerName: "Actions",
      width: 140,
      getActions: (params) => [
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

  const handleAddSubmit = async () => {
    await fetch("/api/events", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(newEvent),
    });
    setOpenDone(false);
    setNewEvent({ eventId: 0, eventName: "", eventStart: "", eventEnd: "" });
    // Refresh
    fetch("/api/events")
      .then((res) => res.json())
      .then((data) => setEvents(Array.isArray(data) ? data : data.value ?? []));
  };

  return (
    <div
      style={{
        height: 500,
        width: "100%",
        background: "#fff",
        borderRadius: 8,
        padding: 16,
      }}
    >
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <h3>All Events</h3>
        <Button
          variant="contained"
          color="primary"
          onClick={() => setOpenDone(true)}
        >
          Add Event
        </Button>
      </div>
      <DataGrid
        rows={events}
        columns={columns}
        getRowId={(row) => row.eventId}
        disableRowSelectionOnClick
      />
      {/* Add Dialog */}
      <Dialog open={openAdd} onClose={() => setOpenDone(false)}>
        <DialogContent>
          {/* <TextField
            label="Name"
            value={newEvent.eventName}
            onChange={(e) =>
              setNewEvent((ev) => ({ ...ev, eventName: e.target.value }))
            }
            fullWidth
            margin="normal"
          />
          <TextField
            label="Start"
            type="datetime-local"
            value={newEvent.eventStart}
            onChange={(e) =>
              setNewEvent((ev) => ({ ...ev, eventStart: e.target.value }))
            }
            fullWidth
            margin="normal"
          />
          <TextField
            label="End"
            type="datetime-local"
            value={newEvent.eventEnd}
            onChange={(e) =>
              setNewEvent((ev) => ({ ...ev, eventEnd: e.target.value }))
            }
            fullWidth
            margin="normal"
          /> */}

          <CustomForm title="Add an Event" apiEndpoint="/api/events">
            <CustomTextField id="eventName" label="Event Name" required />
            <CustomTextField id="eventStart" label="Start Date" required />
            <CustomTextField id="eventEnd" label="End Date" required />
          </CustomForm>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setOpenDone(false)}
            variant="contained"
            color="primary"
          >
            Done
          </Button>
        </DialogActions>
      </Dialog>
      {/* Edit Dialog */}
      <Dialog open={openEdit} onClose={() => setOpenEdit(false)}>
        <DialogTitle>Edit Event</DialogTitle>
        <DialogContent>
          <TextField
            label="Name"
            value={editEvent?.eventName ?? ""}
            onChange={(e) =>
              setEditEvent((ev) =>
                ev ? { ...ev, eventName: e.target.value } : ev
              )
            }
            fullWidth
            margin="normal"
          />
          <TextField
            label="Start"
            type="datetime-local"
            value={editEvent?.eventStart?.slice(0, 16) ?? ""}
            onChange={(e) =>
              setEditEvent((ev) =>
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
            onChange={(e) =>
              setEditEvent((ev) =>
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
          Are you sure you want to delete event "{deleteEvent?.eventName}"?
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
    </div>
  );
};

export default EventTable;
