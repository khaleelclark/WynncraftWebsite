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

type HeaderButton = {
  text: string;
  link: string;
  icon?: ReactNode;
  requiresAuth?: boolean;
};

const headerButtons: HeaderButton[] = [
  {
    text: "Guild Members",
    link: "/",
    icon: <GroupIcon />,
  },
  {
    text: "Leaderboard",
    link: "/leaderboard",
    icon: <LeaderboardIcon />,
  },
  {
    text: "Completed Raids",
    link: "/completed-raids-board",
    icon: <VerifiedUserIcon />,
  },
  {
    text: "Admin Panel",
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

  const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const nickname =
    user?.claims?.find((c: any) => c.type === "nickname")?.value ?? "User";

  let el = (
    <ThemeProvider theme={theme}>
      <CssBaseline />
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
        <Box sx={{ display: "flex", gap: 2, alignItems: "center" }}>
          <nav>
            {headerButtons.map(button => {
              if (button.requiresAuth && !user?.isAuthenticated) return null;

              return (
                <Button
                  key={button.link}
                  startIcon={button.icon}
                  onClick={() => navigate(button.link)}
                >
                  {button.text}
                </Button>
              );
            })}
          </nav>
        </Box>

        <Box sx={{ display: "flex", alignItems: "center", minWidth: 180 }}>
          {user?.claims ? (
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
              </Menu>
            </>
          ) : (
            <Button
              variant="contained"
              color="secondary"
              onClick={() => (window.location.href = "/api/auth/login")}
            >
              Admin Login
            </Button>
          )}
        </Box>
      </Box>
    </ThemeProvider>
  );
  return el;
};
