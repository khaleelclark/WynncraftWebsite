import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import Button from "@mui/material/Button";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";
import { CustomDropdown } from "./CustomDropdown";
import { CustomDateOnlySelector } from "./CustomDateOnlySelector";
import Alert, { AlertColor } from "@mui/material/Alert";
import Snackbar from "@mui/material/Snackbar";
import { GenericAdminPage } from "./GenericAdminPage";
import { CustomTimeAndDateSelector } from "./CustomTimeAndDateSelector";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Paper from "@mui/material/Paper";
import Grid from "@mui/material/Grid";
import GroupIcon from "@mui/icons-material/Group";
import EventIcon from "@mui/icons-material/Event";
import SportsEsportsIcon from "@mui/icons-material/SportsEsports";
import MilitaryTechIcon from "@mui/icons-material/MilitaryTech";
import WorkspacePremiumIcon from "@mui/icons-material/WorkspacePremium";

const AdminPanel: React.FC = () => {
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);
  const [guildMembersOpen, setGuildMembersOpen] = useState(false);
  const [eventsOpen, setEventsOpen] = useState(false);
  const [gamesOpen, setGamesOpen] = useState(false);
  const [medalsOpen, setMedalsOpen] = useState(false);
  const [ranksOpen, setRanksOpen] = useState(false);
  const [raidsOpen, setRaidsOpen] = useState(false);

  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] =
    useState<AlertColor>("success");

  const handleSnackbarClose = () => setSnackbarOpen(false);

  const handleSubmitSuccess = () => {
    // add dymanic message confirmations
    setSnackbarMessage("Entity added successfully!");
    setSnackbarSeverity("success");
    setSnackbarOpen(true);
    setOpen(false);
  };

  const handleSubmitError = (error: unknown) => {
    setSnackbarMessage("Failed to create new entity. Please try again.");
    setSnackbarSeverity("error");
    setSnackbarOpen(true);
  };

  const createGuildMemberForm = (
    <CustomForm
      title="Add a Guild Member"
      apiEndpoint="/api/guildmembers"
      onSubmitSuccess={handleSubmitSuccess}
      onSubmitError={handleSubmitError}
    >
      <CustomTextField id="discordTag" label="Discord Tag" required />
      <CustomTextField id="name" label="Main Username" required />
      <CustomTextField id="uuid" label="Minecraft UUID" required />
      <CustomDropdown id="rank" label="Rank" apiEndpoint="/api/ranks" />
      <CustomDropdown
        id="medals"
        label="Medals"
        apiEndpoint="/api/medals"
        multiple={true}
      />
      <CustomDropdown
        id="games"
        label="Games"
        apiEndpoint="/api/games"
        multiple={true}
      />
      <CustomDateOnlySelector id="joinDate" label="Join Date" />
    </CustomForm>
  );

  const createEventForm = (
    <CustomForm
      title="Add an Event"
      apiEndpoint="/api/events"
      onSubmitSuccess={handleSubmitSuccess}
      onSubmitError={handleSubmitError}
    >
      <CustomTextField id="name" label="Event Name" required />
      <CustomTimeAndDateSelector id="eventStart" label="Start Date & Time" />
      <CustomTimeAndDateSelector id="eventEnd" label="End Date & Time" />
    </CustomForm>
  );

  const createGameForm = (
    <CustomForm
      title="Add a Game"
      apiEndpoint="/api/games"
      onSubmitSuccess={handleSubmitSuccess}
      onSubmitError={handleSubmitError}
    >
      <CustomTextField id="name" label="Game Name" required />
    </CustomForm>
  );

  const createMedalForm = (
    <CustomForm
      title="Add a Medal"
      apiEndpoint="/api/medals"
      onSubmitSuccess={handleSubmitSuccess}
      onSubmitError={handleSubmitError}
    >
      <CustomTextField id="name" label="Medal Name" required />
    </CustomForm>
  );

  const createRankForm = (
    <CustomForm
      title="Add a Rank"
      apiEndpoint="/api/ranks"
      onSubmitSuccess={handleSubmitSuccess}
      onSubmitError={handleSubmitError}
    >
      <CustomTextField id="name" label="Rank Name" required />
    </CustomForm>
  );

  return (
    <Box
      sx={{
        p: 3,
        display: "flex",
        justifyContent: "center",
      }}
    >
      <Box sx={{ width: "100%", maxWidth: 1200 }}>
        <Paper
          sx={{
            p: 4,
            maxWidth: "100%",
            bgcolor: "#501117ff",
            color: "#efdddb",
            borderRadius: 3,
            boxShadow: 6,
          }}
        >
          <Typography variant="h3" textAlign="center" gutterBottom>
            Admin Panel
          </Typography>
          <Typography
            variant="subtitle1"
            textAlign="center"
            sx={{ opacity: 0.8, mb: 4 }}
          >
            Manage your guild data, events, and rankings in one place.
          </Typography>

          {/* --- TILE GRID --- */}
          <Grid
            container
            spacing={3}
            justifyContent="center" // --- center the whole grid
            alignItems="stretch"
            sx={{
              mt: 1,
            }}
          >
            {/* Guild Members */}
            <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }} display="flex">
              <Paper
                onClick={() => setGuildMembersOpen(true)}
                sx={{
                  p: 3,
                  height: "100%",
                  borderRadius: 3,
                  bgcolor: "#82172e",
                  cursor: "pointer",
                  display: "flex",
                  flexDirection: "column",
                  transition: "transform 0.15s ease, box-shadow 0.15s ease",
                  boxShadow: 3,
                  "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: 8,
                  },
                }}
              >
                <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
                  <Box
                    sx={{
                      mr: 2,
                      p: 1.2,
                      borderRadius: "999px",
                      bgcolor: "rgba(0,0,0,0.25)",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <GroupIcon />
                  </Box>
                  <Typography variant="h5">Guild Members</Typography>
                </Box>
                <Typography variant="body2" sx={{ opacity: 0.85, flexGrow: 1 }}>
                  View, edit, and manage all guild members and their details.
                </Typography>

                <Button
                  variant="contained"
                  sx={{
                    mt: 2,
                    alignSelf: "flex-start",
                    bgcolor: "#efdddb",
                    color: "#501117ff",
                    "&:hover": {
                      bgcolor: "#f6e8e6",
                    },
                  }}
                >
                  Open
                </Button>
              </Paper>
            </Grid>

            {/* Events */}
            <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }} display="flex">
              <Paper
                onClick={() => setEventsOpen(true)}
                sx={{
                  p: 3,
                  height: "100%",
                  borderRadius: 3,
                  bgcolor: "#82172e",
                  cursor: "pointer",
                  display: "flex",
                  flexDirection: "column",
                  transition: "transform 0.15s ease, box-shadow 0.15s ease",
                  boxShadow: 3,
                  "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: 8,
                  },
                }}
              >
                <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
                  <Box
                    sx={{
                      mr: 2,
                      p: 1.2,
                      borderRadius: "999px",
                      bgcolor: "rgba(0,0,0,0.25)",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <EventIcon />
                  </Box>
                  <Typography variant="h5">Events</Typography>
                </Box>
                <Typography variant="body2" sx={{ opacity: 0.85, flexGrow: 1 }}>
                  Configure upcoming raids, wars, and guild activities.
                </Typography>

                <Button
                  variant="contained"
                  sx={{
                    mt: 2,
                    alignSelf: "flex-start",
                    bgcolor: "#efdddb",
                    color: "#501117ff",
                    "&:hover": {
                      bgcolor: "#f6e8e6",
                    },
                  }}
                >
                  Open
                </Button>
              </Paper>
            </Grid>

            {/* Games */}
            <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }} display="flex">
              <Paper
                onClick={() => setGamesOpen(true)}
                sx={{
                  p: 3,
                  height: "100%",
                  borderRadius: 3,
                  bgcolor: "#82172e",
                  cursor: "pointer",
                  display: "flex",
                  flexDirection: "column",
                  transition: "transform 0.15s ease, box-shadow 0.15s ease",
                  boxShadow: 3,
                  "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: 8,
                  },
                }}
              >
                <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
                  <Box
                    sx={{
                      mr: 2,
                      p: 1.2,
                      borderRadius: "999px",
                      bgcolor: "rgba(0,0,0,0.25)",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <SportsEsportsIcon />
                  </Box>
                  <Typography variant="h5">Games</Typography>
                </Box>
                <Typography variant="body2" sx={{ opacity: 0.85, flexGrow: 1 }}>
                  Manage supported games and related configurations.
                </Typography>

                <Button
                  variant="contained"
                  sx={{
                    mt: 2,
                    alignSelf: "flex-start",
                    bgcolor: "#efdddb",
                    color: "#501117ff",
                    "&:hover": {
                      bgcolor: "#f6e8e6",
                    },
                  }}
                >
                  Open
                </Button>
              </Paper>
            </Grid>

            {/* Medals */}
            <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }} display="flex">
              <Paper
                onClick={() => setMedalsOpen(true)}
                sx={{
                  p: 3,
                  height: "100%",
                  borderRadius: 3,
                  bgcolor: "#82172e",
                  cursor: "pointer",
                  display: "flex",
                  flexDirection: "column",
                  transition: "transform 0.15s ease, box-shadow 0.15s ease",
                  boxShadow: 3,
                  "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: 8,
                  },
                }}
              >
                <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
                  <Box
                    sx={{
                      mr: 2,
                      p: 1.2,
                      borderRadius: "999px",
                      bgcolor: "rgba(0,0,0,0.25)",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <MilitaryTechIcon />
                  </Box>
                  <Typography variant="h5">Medals</Typography>
                </Box>
                <Typography variant="body2" sx={{ opacity: 0.85, flexGrow: 1 }}>
                  Create and assign medals to recognize achievements.
                </Typography>

                <Button
                  variant="contained"
                  sx={{
                    mt: 2,
                    alignSelf: "flex-start",
                    bgcolor: "#efdddb",
                    color: "#501117ff",
                    "&:hover": {
                      bgcolor: "#f6e8e6",
                    },
                  }}
                >
                  Open
                </Button>
              </Paper>
            </Grid>

            {/* Ranks */}
            <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4, xl: 3 }} display="flex">
              <Paper
                onClick={() => setRanksOpen(true)}
                sx={{
                  p: 3,
                  height: "100%",
                  borderRadius: 3,
                  bgcolor: "#82172e",
                  cursor: "pointer",
                  display: "flex",
                  flexDirection: "column",
                  transition: "transform 0.15s ease, box-shadow 0.15s ease",
                  boxShadow: 3,
                  "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: 8,
                  },
                }}
              >
                <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
                  <Box
                    sx={{
                      mr: 2,
                      p: 1.2,
                      borderRadius: "999px",
                      bgcolor: "rgba(0,0,0,0.25)",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <WorkspacePremiumIcon />
                  </Box>
                  <Typography variant="h5">Ranks</Typography>
                </Box>
                <Typography variant="body2" sx={{ opacity: 0.85, flexGrow: 1 }}>
                  Define and edit guild ranks and permissions.
                </Typography>

                <Button
                  variant="contained"
                  sx={{
                    mt: 2,
                    alignSelf: "flex-start",
                    bgcolor: "#efdddb",
                    color: "#501117ff",
                    "&:hover": {
                      bgcolor: "#f6e8e6",
                    },
                  }}
                >
                  Open
                </Button>
              </Paper>
            </Grid>
          </Grid>

          {/* ----- Validation ------ */}
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
        </Paper>

        {/* ----- Dialogs ----- */}
        <Dialog
          open={guildMembersOpen}
          onClose={() => setGuildMembersOpen(false)}
          maxWidth="lg"
          fullWidth
        >
          <DialogContent dividers>
            <GenericAdminPage
              label="Guild Members"
              apiGetEndpoint="/api/guildmembers/admin"
              apiDeleteEndpoint="/api/guildmembers"
              createForm={createGuildMemberForm}
            />
          </DialogContent>
        </Dialog>

        <Dialog
          open={eventsOpen}
          onClose={() => setEventsOpen(false)}
          maxWidth="lg"
          fullWidth
        >
          <DialogContent dividers>
            <GenericAdminPage
              label="Events"
              apiGetEndpoint="/api/events"
              apiDeleteEndpoint="/api/events"
              createForm={createEventForm}
            />
          </DialogContent>
        </Dialog>

        <Dialog
          open={gamesOpen}
          onClose={() => setGamesOpen(false)}
          maxWidth="lg"
          fullWidth
        >
          <DialogContent dividers>
            <GenericAdminPage
              label="Games"
              apiGetEndpoint="/api/games"
              apiDeleteEndpoint="/api/games"
              createForm={createGameForm}
            />
          </DialogContent>
        </Dialog>

        <Dialog
          open={medalsOpen}
          onClose={() => setMedalsOpen(false)}
          maxWidth="lg"
          fullWidth
        >
          <DialogContent dividers>
            <GenericAdminPage
              label="Medals"
              apiGetEndpoint="/api/medals"
              apiDeleteEndpoint="/api/medals"
              createForm={createMedalForm}
            />
          </DialogContent>
        </Dialog>

        <Dialog
          open={ranksOpen}
          onClose={() => setRanksOpen(false)}
          maxWidth="lg"
          fullWidth
        >
          <DialogContent dividers>
            <GenericAdminPage
              label="Ranks"
              apiGetEndpoint="/api/ranks"
              apiDeleteEndpoint="/api/ranks"
              createForm={createRankForm}
            />
          </DialogContent>
        </Dialog>
      </Box>
    </Box>
  );
};

export default AdminPanel;
