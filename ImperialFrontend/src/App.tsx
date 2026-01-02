import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import {
  ThemeProvider,
  CssBaseline,
  Button,
  Box
} from "@mui/material";
import Menu from '@mui/material/Menu';
import CircularProgress from '@mui/material/CircularProgress';
import MenuItem from '@mui/material/MenuItem';
import theme from "./theme";
import AdminPanel from "./pages/AdminPanel";
import GuildMemberList from "./pages/GuildMemberList";
import Profile from "./pages/Profile";
import Leaderboard from "./pages/Leaderboard";
import AddEvent from "./pages/AddEvent";
import EventTablePage from "./pages/EventTablePage";
import FormTest from "./pages/FormTest";

import { useUser } from "./auth/UserContext";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import React from "react";
import Snackbar from "@mui/material/Snackbar";

const App: React.FC = () => {
  const { user, isLoading, signinRedirect, signoutRedirect, signoutLocal } =
    useUser();
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const [snackbar, setSnackbar] = React.useState<{
    open: boolean;
    message: string;
  }>({ open: false, message: "" });

  React.useEffect(() => {
    const handleSessionExpired = () => {
      signoutLocal();
      setSnackbar({
        open: true,
        message: "Your session has expired. Please log in again.",
      });
    };

    window.addEventListener("sessionExpired", handleSessionExpired);

    return () => {
      window.removeEventListener("sessionExpired", handleSessionExpired);
    };
  }, [signoutLocal]);

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleSnackbarClose = (
    _event: React.SyntheticEvent | Event,
    reason?: string
  ) => {
    if (reason === "clickaway") {
      return;
    }
    setSnackbar((prev) => ({ ...prev, open: false }));
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleLocalLogout = () => {
    signoutLocal();
    handleClose();
  };

  const handleFullLogout = () => {
    signoutRedirect();
    handleClose();
  };

  const el =
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={handleSnackbarClose}
        message={snackbar.message}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      />
      <BrowserRouter>
        <Box
          component="nav"
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            padding: 2,
            borderBottom: "2px solid #bc511c",
            background: "#82172e",
            color: "#efdddb",
            marginBottom: 3,
          }}
        >
          <Box>
            <Link
              to="/"
              style={{
                color: "#efdddb",
                marginRight: 16,
                textDecoration: "none",
                fontWeight: 600,
              }}
            >
              Guild Members
            </Link>
            <Link
              to="/leaderboard"
              style={{
                color: "#efdddb",
                textDecoration: "none",
                fontWeight: 600,
                marginRight: 16,
              }}
            >
              Leaderboard
            </Link>
            {user && (
              <Link
                to="/admin"
                style={{
                  color: "#efdddb",
                  textDecoration: "none",
                  fontWeight: 600,
                  marginRight: 16,
                }}
              >
                Admin Panel
              </Link>
            )}
            <Link
              to="/form"
              style={{
                color: "#efdddb",
                textDecoration: "none",
                fontWeight: 600,
              }}
            >
              Custom Form
            </Link>
          </Box>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              minWidth: 150,
            }}
          >
            {isLoading ? (
              <CircularProgress size={24} color="secondary" />
            ) : user ? (
              <div>
                <Button
                  id="logout-button"
                  aria-controls={open ? "logout-menu" : undefined}
                  aria-haspopup="true"
                  aria-expanded={open ? "true" : undefined}
                  onClick={handleClick}
                  variant="contained"
                  color="secondary"
                >
                  Welcome, {user.profile.nickname}
                </Button>
                <Menu
                  id="logout-menu"
                  anchorEl={anchorEl}
                  open={open}
                  onClose={handleClose}
                >
                  <MenuItem onClick={handleLocalLogout}>Logout</MenuItem>
                  <MenuItem onClick={handleFullLogout}>
                    Logout from Authentik
                  </MenuItem>
                </Menu>
              </div>
            ) : (
              <Button
                variant="contained"
                color="secondary"
                onClick={() => signinRedirect()}
              >
                Admin Login
              </Button>
            )}
          </Box>
        </Box>
        <Routes>
          <Route path="/" element={<GuildMemberList />} />

          <Route path="/profile/:id" element={<Profile />} />
          <Route path="/leaderboard" element={<Leaderboard />} />
          <Route path="/add-event" element={<AddEvent />} />
          <Route
            path="/admin"
            element={
              <ProtectedRoute>
                <AdminPanel />
              </ProtectedRoute>
            }
          />
          <Route path="/form" element={<FormTest />} />
          <Route path="/events" element={<EventTablePage />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>;

  return el;
};

export default App;
