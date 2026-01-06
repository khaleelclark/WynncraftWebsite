import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import { ThemeProvider, CssBaseline, Button, Box } from "@mui/material";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import theme from "./theme";
import AdminPanel from "./pages/AdminPanel";
import GuildMemberList from "./pages/GuildMemberList";
import Profile from "./pages/Profile";
import Leaderboard from "./pages/Leaderboard";
import { useUser } from "./auth/UserContext";
import React, { useEffect, useState } from "react";
import Snackbar from "@mui/material/Snackbar";
import GuildRaidsBoard from "./pages/GuildRaidsBoard";
import axios from "axios";

const App: React.FC = () => {
  const { user } = useUser();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const [snackbar, setSnackbar] = useState<{
    open: boolean;
    message: string;
  }>({ open: false, message: "" });

  useEffect(() => {
    const handleSessionExpired = () => {
      setSnackbar({
        open: true,
        message: "Your session has expired. Please log in again.",
      });
    };

    window.addEventListener("sessionExpired", handleSessionExpired);

    return () => {
      window.removeEventListener("sessionExpired", handleSessionExpired);
    };
  }, []);

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
    setSnackbar(prev => ({ ...prev, open: false }));
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const el = (
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
            px: 3,
            py: 2,
            bgcolor: "primary.main",
            color: "primary.contrastText",
            borderBottom: theme => `2px solid ${theme.palette.secondary.main}`,
            flexShrink: 0,
          }}
        >
          {/* Left nav */}
          <Box
            sx={{
              display: "flex",
              gap: 2,
              alignItems: "center",
            }}
          >
            <Box
              component={Link}
              to="/"
              sx={{
                color: "inherit",
                textDecoration: "none",
                fontWeight: 700,
                "&:hover": { opacity: 0.9 },
              }}
            >
              Guild Members
            </Box>

            <Box
              component={Link}
              to="/leaderboard"
              sx={{
                color: "inherit",
                textDecoration: "none",
                fontWeight: 700,
                "&:hover": { opacity: 0.9 },
              }}
            >
              Leaderboard
            </Box>
            <Box
              component={Link}
              to="/completed-raids-board"
              sx={{
                color: "inherit",
                textDecoration: "none",
                fontWeight: 700,
                "&:hover": { opacity: 0.9 },
              }}
            >
              Completed Raids
            </Box>

            {user?.isAuthenticated && (
              <Box
                component={Link}
                to="/admin"
                sx={{
                  color: "inherit",
                  textDecoration: "none",
                  fontWeight: 700,
                  "&:hover": { opacity: 0.9 },
                }}
              >
                Admin Panel
              </Box>
            )}
          </Box>

          {/* Right actions */}
          <Box sx={{ display: "flex", alignItems: "center", minWidth: 180 }}>
            {user && user.claims ? (
              <>
                <Button
                  id="logout-button"
                  aria-controls={open ? "logout-menu" : undefined}
                  aria-haspopup="true"
                  aria-expanded={open ? "true" : undefined}
                  onClick={handleClick}
                  variant="contained"
                  color="secondary"
                >
                  Welcome,{" "}
                  {user.claims
                    ? user.claims.find(
                        (claim: any) => claim.type === "nickname"
                      ).value
                    : ""}
                </Button>

                <Menu
                  id="logout-menu"
                  anchorEl={anchorEl}
                  open={open}
                  onClose={handleClose}
                  slotProps={{
                    paper: {
                      sx: {
                        bgcolor: "background.paper",
                        color: "text.primary",
                        border: theme => `1px solid ${theme.palette.divider}`,
                      },
                    },
                  }}
                >
                  <MenuItem
                    onClick={() => (window.location.href = "/api/auth/logout")}
                  >
                    Logout
                  </MenuItem>
                  <MenuItem
                    onClick={() =>
                      (window.location.href = "/api/auth/logout-all")
                    }
                  >
                    Logout from Authentik
                  </MenuItem>
                  <MenuItem
                    onClick={async () => {
                      try {
                        const res = await axios.get("/api/auth/me");
                        console.log("User:", res.data);
                        alert(
                          `Logged in as: ${res.data.username ?? res.data.name}`
                        );
                      } catch (err: any) {
                        console.error(err);
                        alert("Not authenticated");
                      }
                    }}
                  >
                    Who am I
                  </MenuItem>
                </Menu>
              </>
            ) : (
              <Button
                variant="contained"
                color="secondary"
                onClick={() => {
                  window.location.href = "/api/auth/login";
                }}
              >
                Admin Login
              </Button>
            )}
          </Box>
        </Box>
        <Routes>
          <Route path="/completed-raids-board" element={<GuildRaidsBoard />} />
          <Route path="/" element={<GuildMemberList />} />
          <Route path="/profile/:id" element={<Profile />} />
          <Route path="/leaderboard" element={<Leaderboard />} />
          <Route path="/admin" element={<AdminPanel />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );

  return el;
};

export default App;
