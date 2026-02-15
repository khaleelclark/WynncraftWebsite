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
import { useNavigate } from "react-router-dom";
import Grid from "@mui/material/Grid";
import { Header } from "./Header";
import Paper from "@mui/material/Paper";
import { useApiErrorSnackbar } from "./ApiErrorSnackbar";

interface LeaderboardEntry {
  id: number;
  minecraftUsername: string;
  uuid?: string;
  warsCompleted: number;
  hoursPlayed?: number | null;
  raidsCompleted: number;
  lastSynced?: string | null;
}

const Leaderboard: React.FC = () => {
  const navigate = useNavigate();

  const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
  const [startDateTime, setStartDateTime] = useState<Dayjs | null>(null);
  const [endDateTime, setEndDateTime] = useState<Dayjs | null>(null);
  const { handleError, SnackbarElement } = useApiErrorSnackbar();

  // Sentinel ids: -1 = all time, -2 = custom range.
  const [selectedEvent, setSelectedEvent] = useState<any>({
    id: -1,
    name: "All Time",
    eventStart: "2000-01-01T00:00:00",
    eventEnd: "2100-01-01T23:59:00",
  });

  const columns: GridColDef[] = [
    {
      field: "minecraftUsername",
      headerName: "Player",
      minWidth: 240,
      flex: 1.4,
      align: "center",
      headerAlign: "center",
      sortable: false,
      renderCell: (params: any) => (
        <span
          style={{
            display: "flex",
            alignItems: "center",
            width: "100%",
            cursor: "pointer",
          }}
          // Click through to the member profile.
          onClick={() => navigate(`/profile/${params.row.id}`)}
        >
          <img
            src={`https://mc-heads.net/avatar/${params.row.uuid}/45/`}
            alt="Skin"
            style={{
              width: 45,
              height: 45,
              marginRight: 10,
              verticalAlign: "middle",
              borderRadius: 6,
            }}
          />
          <span>{params.row.minecraftUsername}</span>
        </span>
      ),
    },
    {
      field: "raidsCompleted",
      headerName: "Raids",
      minWidth: 150,
      flex: 0.6,
      align: "center",
      headerAlign: "center",
      type: "number",
    },
    {
      field: "hoursPlayed",
      headerName: "Hours Played",
      minWidth: 180,
      flex: 0.7,
      align: "center",
      headerAlign: "center",
      type: "number",
      renderCell: (params: any) => {
        const hp = params.row.hoursPlayed;
        return hp != null && hp >= 0 ? hp : "Private";
      },
    },
    {
      field: "warsCompleted",
      headerName: "Wars",
      minWidth: 150,
      flex: 0.6,
      align: "center",
      headerAlign: "center",
      type: "number",
    },
    {
      field: "lastSynced",
      headerName: "Last Updated",
      minWidth: 200,
      flex: 0.9,
      align: "center",
      headerAlign: "center",
      renderCell: (params: any) =>
        params.value
          ? new Date(params.value as string).toLocaleString(undefined, {
              dateStyle: "short",
              timeStyle: "short",
            })
          : "—",
    },
  ];

  const getLeaderboard = () => {
    // Avoid requests until both bounds are set.
    if (!startDateTime || !endDateTime) return;

    axios
      .get("/api/guildmembers/leaderboard", {
        params: {
          startDate: startDateTime.toISOString(),
          endDate: endDateTime.toISOString(),
        },
      })
      .then(res => {
        const arr = res.data?.value ?? res.data ?? [];
        setEntries(arr);
      })
      .catch(handleError);
  };

  useEffect(() => {
    if (selectedEvent?.id === -2) {
      // Custom
      setStartDateTime(null);
      setEndDateTime(null);
      setEntries([]);
      return;
    }

    // Pre-fill date range for standard events.
    if (selectedEvent?.eventStart && selectedEvent?.eventEnd) {
      setStartDateTime(dayjs(selectedEvent.eventStart));
      setEndDateTime(dayjs(selectedEvent.eventEnd));
    }
  }, [selectedEvent]);

  useEffect(() => {
    // Fetch once date range changes.
    if (startDateTime && endDateTime) getLeaderboard();
  }, [startDateTime, endDateTime]);

  const leaderboardTitle =
    selectedEvent?.id === -1
      ? "All Time Leaderboard"
      : selectedEvent?.id === -2
      ? "Custom Leaderboard"
      : `${selectedEvent?.name} Leaderboard`;

  const pickerSx = {
    mb: 3,
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
      backgroundColor: "background.paper",
      borderRadius: 1.5,
    },
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Header />
      {SnackbarElement}
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
          <EventSelector onSelect={setSelectedEvent} />

          {!selectedEvent || selectedEvent?.id === -1 ? (
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
                {leaderboardTitle}
              </Typography>

              <Typography variant="body2" color="text.secondary">
                {entries.length} total
              </Typography>
            </Box>

            <Box sx={{ mt: 0.5 }}>
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
                    sortModel: [{ field: "raidsCompleted", sort: "desc" }],
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
          </Paper>
        </Box>
      </Box>
    </LocalizationProvider>
  );
};

export default Leaderboard;
