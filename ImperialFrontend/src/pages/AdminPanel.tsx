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
import { GenericGuildMemberList } from "./GenericGuildMemberList";

const AdminPanel: React.FC = () => {
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);
  const [guildMembersOpen, setGuildMembersOpen] = useState(false);

  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] =
    useState<AlertColor>("success");

  const handleSnackbarClose = () => setSnackbarOpen(false);

  const handleSubmitSuccess = () => {
    setSnackbarMessage("Guild member added successfully!");
    setSnackbarSeverity("success");
    setSnackbarOpen(true);
    handleClose();
  };

  const handleSubmitError = (error: unknown) => {
    setSnackbarMessage("Failed to create guild member. Please try again.");
    setSnackbarSeverity("error");
    setSnackbarOpen(true);
  };

  return (
    <div style={{ padding: 32 }}>
      <h2>Admin Panel</h2>
      <Button
        variant="contained"
        color="primary"
        onClick={() => navigate("/events")}
      >
        Go to Event Table
      </Button>
      {/* ---------- Button that opens the dialog ---------- */}
      <Button variant="contained" onClick={handleOpen}>
        Add Guild Member
      </Button>
      {/* ---------- Dialog ---------- */}
      <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogContent dividers>
          <CustomForm
            title="Add a Guild Member"
            apiEndpoint="/api/guildmembers"
            onSubmitSuccess={handleSubmitSuccess}
            onSubmitError={handleSubmitError}
          >
            <CustomTextField id="discordTag" label="Discord Tag" required />
            <CustomTextField id="mainUsername" label="Main Username" required />
            <CustomTextField id="uuid" label="Minecraft UUID" required />

            <CustomDropdown
              idColumn="rankId"
              displayColumn="rankName"
              label="Rank"
              apiEndpoint="/api/ranks"
            />
            <CustomDateOnlySelector id="joinDate" label="Join Date" />
          </CustomForm>
        </DialogContent>
      </Dialog>

      <Button
        variant="contained"
        color="secondary"
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
          <GenericGuildMemberList apiEndpoint="/api/guildmembers" />
        </DialogContent>
      </Dialog>

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

export default AdminPanel;
