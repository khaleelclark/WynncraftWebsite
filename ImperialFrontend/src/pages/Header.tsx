import { ReactNode, useState } from "react";
import { useUser } from "../auth/UserContext";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import { ThemeProvider } from "@mui/material/styles";
import { useNavigate } from "react-router-dom";
import theme from "../theme";
import GroupIcon from "@mui/icons-material/Group";
import VerifiedUserIcon from "@mui/icons-material/VerifiedUser";
import LeaderboardIcon from "@mui/icons-material/Leaderboard";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import Typography from "@mui/material/Typography";
import IconButton from "@mui/material/IconButton";
import MenuIcon from "@mui/icons-material/Menu";
import Drawer from "@mui/material/Drawer";
import List from "@mui/material/List";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import Divider from "@mui/material/Divider";
import axios from "axios";
import { useApiErrorSnackbar } from "./ApiErrorSnackbar";

// Declare global variables defined by Rsbuild
declare const RSBUILD_PUBLIC_API_URL: string;

type HeaderButton = {
  text: string;
  link: string;
  icon?: ReactNode;
  requiresAuth?: boolean;
};

const headerButtons: HeaderButton[] = [
  { text: "Members", link: "/", icon: <GroupIcon /> },
  { text: "Leaderboard", link: "/leaderboard", icon: <LeaderboardIcon /> },
  { text: "Raids", link: "/completed-raids-board", icon: <VerifiedUserIcon /> },
  {
    text: "Admin",
    link: "/admin",
    icon: <AdminPanelSettingsIcon />,
    requiresAuth: true,
  },
];

export const Header = () => {
  const navigate = useNavigate();
  const { user } = useUser();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const { handleError, SnackbarElement } = useApiErrorSnackbar();
  // Public backend URL injected at build time (used for auth redirects).
  const BACKEND_URL = RSBUILD_PUBLIC_API_URL;

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => setAnchorEl(null);

  const nickname =
    user?.claims?.find((c: any) => c.type === "nickname")?.value ?? "User";

  // Hide auth-only destinations for anonymous users.
  const availableButtons = headerButtons.filter(
    button => !(button.requiresAuth && !user?.isAuthenticated)
  );

  const authControl = user?.claims ? (
    <>
      <Button
        id="logout-button"
        aria-controls={open ? "logout-menu" : undefined}
        aria-haspopup="true"
        aria-expanded={open ? "true" : undefined}
        onClick={handleClick}
        variant="contained"
        color="primary"
        sx={{ whiteSpace: "nowrap" }}
      >
        Welcome, {nickname}
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
              border: t => `1px solid ${t.palette.divider}`,
            },
          },
        }}
      >
        <MenuItem onClick={() => (window.location.href = "/api/auth/logout")}>
          Logout
        </MenuItem>
        <MenuItem
          onClick={() => (window.location.href = "/api/auth/logout-all")}
        >
          Logout from Authentik
        </MenuItem>
      </Menu>
    </>
  ) : (
    <>
      <Button
        variant="contained"
        color="primary"
        onClick={async () => {
          try {
            // Verify Auth provider health before redirecting to SSO.
            await axios.get("/api/auth/status");
            window.location.href = `${BACKEND_URL}/api/auth/login`;
          } catch (err) {
            handleError(err);
          }
        }}
      >
        Admin Login
      </Button>
    </>
  );

  const el = (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {SnackbarElement}
      <Box
        component="nav"
        sx={{
          display: "flex",
          alignItems: "center",
          px: 3,
          py: 1.5,
          bgcolor: "background.paper",
          color: "text.primary",
          borderBottom: t => `1px solid #B0540F`,
          boxShadow: 2,
          position: "sticky",
          top: 0,
          zIndex: 10,
          backdropFilter: "blur(8px)",
          gap: 2,
          mb: 3,
        }}
      >
        {/* Left: Logo */}
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1.25,
            cursor: "pointer",
            userSelect: "none",
            flex: "0 0 auto",
          }}
          onClick={() => navigate("/")}
        >
          <Box
            component="img"
            src="/imperial-logo.png"
            alt="Imperial logo"
            sx={{
              width: 34,
              height: 34,
              borderRadius: "8px",
              boxShadow: 1,
            }}
          />
          <Typography
            sx={{
              fontSize: 36,
              fontWeight: 700,
              letterSpacing: 0.5,
              lineHeight: 1,
              background: "#ffffffff",
              WebkitBackgroundClip: "text",
              WebkitTextFillColor: "transparent",
            }}
          >
            Imperial
          </Typography>
        </Box>

        <Box
          sx={{
            flex: "1 1 auto",
            display: { xs: "none", lg: "flex" },
            justifyContent: { md: "flex-end", lg: "center" },
          }}
        >
          <Box sx={{ display: "flex", gap: 2 }}>
            {availableButtons.map(button => (
              <Button
                key={button.link}
                startIcon={button.icon}
                onClick={() => navigate(button.link)}
              >
                {button.text}
              </Button>
            ))}
          </Box>
        </Box>

        <Box
          sx={{
            flex: "0 0 auto",
            display: { xs: "none", lg: "flex" },
            alignItems: "center",
            gap: 1,
          }}
        >
          {authControl}
        </Box>

        <Box
          sx={{
            flex: "0 0 auto",
            display: { xs: "flex", lg: "none" },
            ml: "auto",
          }}
        >
          <IconButton
            aria-label="open navigation menu"
            onClick={() => setDrawerOpen(true)}
            color="inherit"
            size="large"
          >
            <MenuIcon />
          </IconButton>
        </Box>

        <Drawer
          anchor="right"
          open={drawerOpen}
          onClose={() => setDrawerOpen(false)}
          sx={{ display: { xs: "block", lg: "none" } }}
          slotProps={{
            paper: {
              sx: {
                bgcolor: "background.paper",
                color: "text.primary",
                borderLeft: t => `1px solid ${t.palette.divider}`,
                width: 260,
              },
            },
          }}
        >
          <List sx={{ width: "100%" }}>
            {availableButtons.map(button => (
              <ListItemButton
                key={button.link}
                onClick={() => {
                  navigate(button.link);
                  setDrawerOpen(false);
                }}
              >
                {button.icon ? (
                  <ListItemIcon sx={{ minWidth: 36 }}>
                    {button.icon}
                  </ListItemIcon>
                ) : null}
                <ListItemText primary={button.text} />
              </ListItemButton>
            ))}
          </List>

          <Divider />

          <Box sx={{ p: 2 }}>{authControl}</Box>
        </Drawer>
      </Box>
    </ThemeProvider>
  );
  return el;
};
