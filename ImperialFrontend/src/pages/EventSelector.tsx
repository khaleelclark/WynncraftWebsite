import Box from "@mui/material/Box";
import MenuItem from "@mui/material/MenuItem";
import TextField from "@mui/material/TextField";
import React, { useEffect, useState } from "react";

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

  useEffect(() => {
    fetch("/api/events")
      .then(res => res.json())
      .then(data => {
        setEvents([CUSTOM_EVENT, ALL_TIME_EVENT, ...data]);
      });
  }, []);

  useEffect(() => {
    const selected = events.find(e => e.id === selectedId) || ALL_TIME_EVENT;
    onSelect(selected);
  }, [selectedId, events, onSelect]);

  return (
    <Box
      sx={{
        my: 2,
        display: "flex",
        justifyContent: "center",
      }}
    >
      <TextField
        select
        label="Date Range"
        value={selectedId}
        onChange={e => setSelectedId(Number(e.target.value))}
        sx={{
          width: "min(520px, 100%)",
          "& .MuiInputBase-root": {
            backgroundColor: "#511220",
            color: "#ffffff",
          },
          "& .MuiInputBase-input": {
            color: "#ffffff",
          },
          "& .MuiInputLabel-root": {
            color: "#ffffff",
          },
          "& .MuiInputLabel-root.Mui-focused": {
            color: "#ffffff",
          },
          "& .MuiOutlinedInput-notchedOutline": {
            borderColor: "#bc511c",
          },
          "&:hover .MuiOutlinedInput-notchedOutline": {
            borderColor: "#efdddb",
          },
          "& .MuiOutlinedInput-root.Mui-focused .MuiOutlinedInput-notchedOutline":
            {
              borderColor: "#efdddb",
            },
          "& .MuiSvgIcon-root": {
            color: "#ffffff",
          },

          // dropdown menu styling
          "& .MuiMenu-paper": {
            backgroundColor: "#220c0e",
          },
        }}
      >
        {events.map(ev => (
          <MenuItem key={ev.id} value={ev.id}>
            {ev.name}
          </MenuItem>
        ))}
      </TextField>
    </Box>
  );
};

export default EventSelector;
