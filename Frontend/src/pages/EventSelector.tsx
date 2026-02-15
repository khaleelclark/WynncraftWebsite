import Box from "@mui/material/Box";
import MenuItem from "@mui/material/MenuItem";
import TextField from "@mui/material/TextField";
import axios from "axios";
import React, { useEffect, useState } from "react";
import { useApiErrorSnackbar } from "./ApiErrorSnackbar";

interface Event {
  id: number;
  name: string;
  eventStart: string;
  eventEnd: string;
}

interface Props {
  onSelect: (event: Event | null) => void;
}

const CUSTOM_EVENT: Event = {
  id: -2,
  name: "Custom",
  eventStart: "",
  eventEnd: "",
};

const ALL_TIME_EVENT: Event = {
  id: -1,
  name: "All Time",
  eventStart: "2000-01-01",
  eventEnd: "2100-01-01",
};

const EventSelector: React.FC<Props> = ({ onSelect }) => {
  const [events, setEvents] = useState<Event[]>([]);
  const [selectedId, setSelectedId] = useState<number>(-1);
  const { handleError, SnackbarElement } = useApiErrorSnackbar();

  useEffect(() => {
    // Load server-defined events and prepend sentinel options.
    axios
      .get("/api/events")
      .then(res => {
        const data = res.data?.value ?? res.data ?? [];
        setEvents([CUSTOM_EVENT, ALL_TIME_EVENT, ...data]);
      })
      .catch(err => {
        handleError(err);
        // Fall back to the sentinel options when the API is unavailable.
        setEvents([CUSTOM_EVENT, ALL_TIME_EVENT]);
      });
  }, []);

  useEffect(() => {
    // Notify parent when selection changes.
    const selected = events.find(e => e.id === selectedId) || ALL_TIME_EVENT;
    onSelect(selected);
  }, [selectedId, events, onSelect]);

  return (
    <Box
      sx={{
        my: 2,
        display: "flex",
        justifyContent: "center",
        mb: 3,
      }}
    >
      {SnackbarElement}
      <TextField
        select
        label="Date Range"
        value={selectedId}
        onChange={e => setSelectedId(Number(e.target.value))}
        sx={{
          width: "min(520px, 100%)",

          "& .MuiOutlinedInput-root": {
            backgroundColor: "background.paper",
            borderRadius: 1.5,
          },

          "& .MuiSvgIcon-root": {
            color: "text.secondary",
          },
          "& .MuiSelect-select": {
            color: "#ffffff",
          },
        }}
      >
        {events.map(ev => (
          <MenuItem
            key={ev.id}
            value={ev.id}
            sx={{
              "&.Mui-selected": {
                backgroundColor: theme => `${theme.palette.primary.main}33`,
              },
              "&.Mui-selected:hover": {
                backgroundColor: theme => `${theme.palette.primary.main}44`,
              },
            }}
          >
            {ev.name}
          </MenuItem>
        ))}
      </TextField>
    </Box>
  );
};

export default EventSelector;
