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
      <Box sx={{ width: "100%" }}>
        <Paper
          sx={{
            p: 3,
            maxWidth: "100%",
            bgcolor: "#501117ff",
            color: "#efdddb",
            borderRadius: 2,
            boxShadow: 4,
          }}
        >
          <Typography variant="h2" textAlign={"center"}>
            Admin Panel
          </Typography>

          {/* ----- Guild Members ----- */}
          <Button
            variant="contained"
            color="primary"
            onClick={() => setGuildMembersOpen(true)}
          >
            View Guild Members
          </Button>

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

          {/* ----- Events ----- */}

          <Button
            variant="contained"
            color="primary"
            onClick={() => setEventsOpen(true)}
          >
            View Events
          </Button>

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

          {/* ----- Games ----- */}

          <Button
            variant="contained"
            color="primary"
            onClick={() => setGamesOpen(true)}
          >
            View Games
          </Button>

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

          {/* ----- Medals ----- */}

          <Button
            variant="contained"
            color="primary"
            onClick={() => setMedalsOpen(true)}
          >
            View Medals
          </Button>

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

          {/* ----- Ranks ----- */}

          <Button
            variant="contained"
            color="primary"
            onClick={() => setRanksOpen(true)}
          >
            View Ranks
          </Button>

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
      </Box>
    </Box>
  );
};

export default AdminPanel;
