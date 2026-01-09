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
    if (!axios.isAxiosError(err)) return "Something unexpected happened.";

    const data = err.response?.data as any;

    // Prefer explicit backend message
    if (typeof data?.message === "string" && data.message.trim())
      return data.message;

    // Otherwise map backend error codes
    if (typeof data?.error === "string") {
      switch (data.error) {
        case "AuthProviderUnavailable":
          return "Login is currently unavailable. Please try again later.";
      }
    }

    if (!err.response) {
      return navigator.onLine
        ? "Can’t reach the server right now."
        : "You appear to be offline.";
    }

    switch (err.response.status) {
      case 504:
        return "Unable to reach server. Please try again later.";
      case 503:
        return "Service unavailable. Please try again later.";
      case 500:
        return "Server error. Please try again later.";
      case 401:
        return "Your session expired. Please sign in again.";
      case 403:
        return "You don’t have permission to do this.";
      case 404:
        return "That resource wasn’t found.";
      default:
        return "Request failed.";
    }
  }

  const show = (msg: string, sev: Severity = "info") => {
    setMessage(msg);
    setSeverity(sev);
    setOpen(true);
  };

  const handleError = (
    err: unknown,
    opts?: { prefix?: string; fallback?: string }
  ) => {
    const base = getErrorMessage(err);
    const msg = opts?.prefix
      ? `${opts.prefix}: ${base}`
      : opts?.fallback ?? base;

    show(msg, "error");
  };

  const handleSuccess = (msg: string) => show(msg, "success");
  const handleInfo = (msg: string) => show(msg, "info");

  const SnackbarElement = (
    <Snackbar
      open={open}
      autoHideDuration={6000}
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

  return { show, handleError, handleSuccess, handleInfo, SnackbarElement };
}
