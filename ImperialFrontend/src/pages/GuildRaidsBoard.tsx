import React, { useEffect, useState } from "react";
import EventSelector from "./EventSelector";
import { DataGrid, GridColDef } from "@mui/x-data-grid";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";
import dayjs, { Dayjs } from "dayjs";
import axios from "axios";
import Grid from "@mui/material/Grid";
import { Header } from "./Header";

interface RaidCompletedPublicRow {
  id: number;
  completedDate: string;
  raidName: string;
  guildMembers: string[];
}

const GuildRaidsBoard: React.FC = () => {
  const [entries, setEntries] = useState<RaidCompletedPublicRow[]>([]);
  const [startDateTime, setStartDateTime] = useState<Dayjs | null>(null);
  const [endDateTime, setEndDateTime] = useState<Dayjs | null>(null);

  const [selectedEvent, setSelectedEvent] = useState<any>({
    id: -1,
    name: "All Time",
    eventStart: "2000-01-01T00:00:00",
    eventEnd: "2100-01-01T23:59:00",
  });

  const columns: GridColDef[] = [
    {
      field: "raidName",
      headerName: "Raid",
      minWidth: 260,
      flex: 1.2,
      align: "center",
      headerAlign: "center",
    },
    {
      field: "completedDate",
      headerName: "Completed",
      minWidth: 220,
      flex: 0.9,
      align: "center",
      headerAlign: "center",
      renderCell: (params: any) => {
        const v = params.row.completedDate;
        return v
          ? new Date(v).toLocaleString(undefined, {
              dateStyle: "short",
              timeStyle: "short",
            })
          : "—";
      },
    },

    {
      field: "guildMembers",
      headerName: "Members",
      minWidth: 420,
      flex: 2,
      align: "left",
      headerAlign: "center",
      sortable: false,
      renderCell: (params: any) => {
        const members: string[] = params.row.guildMembers ?? [];
        return members.length ? members.join(", ") : "—";
      },
    },
  ];

  const getPublicRaids = () => {
    if (!startDateTime || !endDateTime) return;

    axios
      .get("/api/raidscompleted/public", {
        params: {
          startDate: startDateTime.toISOString(),
          endDate: endDateTime.toISOString(),
        },
      })
      .then(res => {
        const arr = res.data?.value ?? res.data ?? [];
        setEntries(arr);
      })
      .catch(err => console.error("Failed to fetch public raids:", err));
  };

  useEffect(() => {
    if (selectedEvent?.id === -2) {
      // Custom
      setStartDateTime(null);
      setEndDateTime(null);
      setEntries([]);
      return;
    }

    if (selectedEvent?.eventStart && selectedEvent?.eventEnd) {
      setStartDateTime(dayjs(selectedEvent.eventStart));
      setEndDateTime(dayjs(selectedEvent.eventEnd));
    }
  }, [selectedEvent]);

  useEffect(() => {
    if (startDateTime && endDateTime) getPublicRaids();
  }, [startDateTime, endDateTime]);

  const leaderboardTitle =
    selectedEvent?.id === -1
      ? "All Completed Guild Raids"
      : selectedEvent?.id === -2
      ? "Custom Leaderboard"
      : `${selectedEvent?.name} - Raid Completions`;

  const pickerSx = {
    "& .MuiOutlinedInput-input.Mui-disabled": {
      color: "#c3c3c3ff !important",
      WebkitTextFillColor: "#c3c3c3ff !important",
      opacity: 1,
    },
    "& .MuiSvgIcon-root": {
      color: "#efdddb",
    },
    "& .MuiInputLabel-root": {
      color: "#efdddb",
    },
    "& .MuiOutlinedInput-root": {
      backgroundColor: "#3C002F",
      borderRadius: 1.5,
    },
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Header />
      <Box
        sx={{
          p: 3,
          bgcolor: "background.default",
          color: "text.primary",
          width: "100%",
          minHeight: "100vh",
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
            {leaderboardTitle}
          </Typography>

          <EventSelector onSelect={setSelectedEvent} />

          {selectedEvent?.id === -1 ? (
            <Typography
              sx={{ my: 2, color: "#efdddb", textAlign: "center" }}
            ></Typography>
          ) : (
            <Box sx={{ my: 2 }}>
              <Grid container spacing={2} justifyContent="center">
                <Grid>
                  <DateTimePicker
                    label="Start"
                    value={startDateTime}
                    onChange={setStartDateTime}
                    disabled={selectedEvent?.id !== -2}
                    sx={{ ...pickerSx, width: "100%" }}
                  />
                </Grid>

                <Grid>
                  <DateTimePicker
                    label="End"
                    value={endDateTime}
                    onChange={setEndDateTime}
                    minDateTime={startDateTime ?? undefined}
                    disabled={selectedEvent?.id !== -2}
                    sx={{ ...pickerSx, width: "100%" }}
                  />
                </Grid>
              </Grid>
            </Box>
          )}

          <Box sx={{ mt: 3 }}>
            <DataGrid
              rows={entries}
              columns={columns}
              getRowId={row => row.id}
              getRowHeight={() => "auto"}
              columnHeaderHeight={70}
              pageSizeOptions={[20, 50, 100]}
              initialState={{
                pagination: { paginationModel: { pageSize: 20, page: 0 } },
                sorting: {
                  sortModel: [{ field: "completedDate", sort: "desc" }],
                },
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
          </Box>
        </Box>
      </Box>
    </LocalizationProvider>
  );
};

export default GuildRaidsBoard;
