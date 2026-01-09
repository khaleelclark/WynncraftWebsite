import * as React from "react";
import axios from "axios";
import Snackbar from "@mui/material/Snackbar";
import Alert from "@mui/material/Alert";

export type Severity = "error" | "warning" | "info" | "success";

export function useApiErrorSnackbar() {
  const [open, setOpen] = React.useState(false);
  const [message, setMessage] = React.useState("");
  const [severity, setSeverity] = React.useState<Severity>("error");

  function getErrorMessage(err: unknown): string {
    if (!axios.isAxiosError(err)) {
      return "Something unexpected happened.";
    }

    if (!err.response) {
      return navigator.onLine
        ? "Can’t reach the server right now."
        : "You appear to be offline.";
    }

    switch (err.response.status) {
      case 401:
        return "Your session expired. Please sign in again.";
      case 403:
        return "You don’t have permission to do this.";
      case 404:
        return "That resource wasn’t found.";
      case 500:
        return "Server error. Please try again later.";
      case 503:
        return "Service unavailable. Please try again later.";
      default:
        return "Request failed.";
    }
  }

  const handleError = (err: unknown) => {
    setMessage(getErrorMessage(err));
    setSeverity("error");
    setOpen(prev => (prev ? prev : true));
  };

  const handleSuccess = (msg: string, success: Severity = "info") => {
    setMessage(msg);
    setSeverity(success);
    setOpen(true);
  };

  const SnackbarElement = (
    <Snackbar
      open={open}
      autoHideDuration={10000}
      onClose={() => setOpen(false)}
      anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
    >
      <Alert
        severity={severity}
        onClose={() => setOpen(false)}
        variant="filled"
      >
        {message}
      </Alert>
    </Snackbar>
  );

  return { handleError, handleSuccess, SnackbarElement };
}
